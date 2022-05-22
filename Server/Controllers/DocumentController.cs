using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly OISContext _context;

    public DocumentController(OISContext context) {
        _context = context;
    }
    
    [HttpGet("GetTypes")]
    public async Task<ActionResult<Response>> GetTypes(string token){        
        
        Response response = new();

        var r = Security.Authenticate(token);
        if(r.Status != ResponseStatus.OK){
            response.Status = r.Status;
            response.Message = r.Message;
            return response;
        }
        try {
            response.Data = await _context.DocumentTypes.ToListAsync() as List<DocumentType>;

        } catch (Exception ex){
            response.Status = ResponseStatus.Error;
            response.Message = ex.ToString();
        }
        return response;
    }

    [HttpGet("GetTypeParameters")]
    public async Task<ActionResult<Response>> GetTypeParameters([FromQuery]int? id, string token){        
         
        Response response = new();
        
        var r = Security.Authenticate(token);
        if(r.Status != ResponseStatus.OK){
            response.Status = r.Status;
            response.Message = r.Message;
            return response;
        }
        
        if(!id.HasValue){
            response.Status = ResponseStatus.Error;
            response.Message = $"Coudn't find document type with id:{id}";
            return response;
        }

        var documentParameters = await _context.DocumentParameters
            .Where(dp => dp.DocumentTypeId == id).ToListAsync();

        if(documentParameters.Any()){
            response.Data = documentParameters;
            return response;
        }    
        
        response.Status = ResponseStatus.Error;
        response.Message = $"Coudn't find any parameters for document type with id:{id}";
        return response;
    }
    
    [HttpPut("Submit")]
    public async Task<ActionResult<Response>> Submit([FromBody]DocumentPayload res, string token) {

        Response response = new();

        var r = Security.Authenticate(token);
        
        if(r.Status != ResponseStatus.OK){
            response.Status = r.Status;
            response.Message = r.Message;
            return response;
        }
        
        try {
            
            int userId = (int)r.Data;

            Document document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == res.Id);
           
            if(document == null) {
                
                document = new(){
                    IssueDate = res.IssueDate,
                    ValidThrough = res.ValidThrough,
                    DocumentTypeId = res.DocumentTypeId,
                    UserId = userId
                };
                
            }else{
                if(userId != document.UserId) throw new Exception("Access denied!");
                
                document.IssueDate = res.IssueDate;
                document.ValidThrough = res.ValidThrough;
                document.DocumentStatus = DocumentStatus.Pending;
                
                foreach(var i in document.DocumentParameterValues){
                    _context.DocumentParameterValues.Remove(i);
                }
            }   
            
            foreach(var i in res.ParameterValues) {
                var documentparam = await _context.DocumentParameters
                    .FirstOrDefaultAsync(d => 
                        d.DocumentTypeId == document.DocumentTypeId && d.Id == i.DocumentParameterId);
                
                string ErrorMsg = $"Invalid parameter value! ParameterId: {i.DocumentParameterId}, Value: {i.Value}";
                
                if(documentparam == null) throw new Exception(ErrorMsg);
                
                switch (documentparam.DataType) {
                    case DataType.number_float:
                        decimal trydec;
                        if(!decimal.TryParse(i.Value, out trydec)) throw new Exception(ErrorMsg);
                        break;
                    case DataType.number_int:
                        int tryint;
                        if(!int.TryParse(i.Value, out tryint)) throw new Exception(ErrorMsg);
                        break;
                    case DataType.date:
                        DateTime trydate;
                        if(!DateTime.TryParse(i.Value, out trydate)) throw new Exception(ErrorMsg);
                        break;
                }
                   
                var documentParameterValue = new DocumentParameterValue(){
                    ParameterValue = i.Value,
                    DocumentParameterId = i.DocumentParameterId
                };
                document.DocumentParameterValues.Add(documentParameterValue);
            };   

            response.Data = document;

            _context.Documents.Add(document);
            _context.SaveChanges();
            
            response.Status = ResponseStatus.OK;
            response.Message = "Document successfully " + res.Id is null? "added":"updated";
        
        }catch (Exception ex) {
            response.Status = ResponseStatus.Error;
            response.Message = $"Coudn't add or updtade Document" + ex;
        }

        return response;
    }

    [HttpPut("GenerateQR")]
    public async Task<ActionResult<Response>> GenerateQR([FromBody]DocumentPayload res, string token){

        Response response = new();

        var r = Security.Authenticate(token);
        if(r.Status != ResponseStatus.OK){
            response.Status = r.Status;
            response.Message = r.Message;
            return response;
        }
        try {
            int userId = (int)r.Data;
            
            Document document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == res.Id);

            if(document == null) throw new Exception("Document not found!");
            if(document.DocumentStatus != DocumentStatus.Accepted) throw new Exception($"Document status: {document.DocumentStatus}");
            if(document.UserId != userId) throw new Exception("Do this irl :PotFriend: to proceed!");

            Inspection inspection = new(){
                UserId = userId,
                DocumentId  = document.Id,
                IsValid = false,
                ValidatonStartTime = DateTime.UtcNow
            };
            
            _context.Inspections.Add(inspection);
            _context.SaveChanges();

            response.Data = Cryptography.StringToBase64($"{inspection.Id}.{userId}.{document.Id}");

        } catch (Exception ex){
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
        }
        return response;
    }

    [HttpPut("InspectQR")]
    public async Task<ActionResult<Response>> InspectQR(string qrStr, string token){

        Response response = new(){};

        var r = Security.Authenticate(token);
        if(r.Status != ResponseStatus.OK){
            response.Status = r.Status;
            response.Message = r.Message;
            return response;
        }
        
        Inspection inspection = new();
        
        try {
            int InspectorId = (int)r.Data; 
            var qrData = Array.ConvertAll( 
                Cryptography.Base64ToString(qrStr).Split("."), s => int.Parse(s)); //[id,userid,docid]
            
            inspection = await _context.Inspections
                .FirstOrDefaultAsync(i => i.Id == qrData[0]);

            if(inspection == null) throw new Exception("No such document");
            if(inspection.UserId != qrData[1]) throw new Exception("Unkown person!");

            inspection.InspectorId = InspectorId;
            var document = _context.Documents.FirstOrDefault(d => d.Id == qrData[2]);

            if(!_context.InspectorAuthorizedDocumentTypes
                .Any(iadt => document.DocumentTypeId == iadt.DocumentTypeId && iadt.UserId == InspectorId)){
                    response.Message = "You are unauthorized to check this type of document";
            }else{
                if(document.DocumentStatus != DocumentStatus.Accepted){
                    response.Message = $"Document status: {document.DocumentStatus}";
                }else{
                    if(DateTime.UtcNow < inspection.ValidatonStartTime.AddMinutes(10)){
                        response.Message = "Validation window has expired";
                    }else{
                        inspection.IsValid = true;
                        response.Message = "Document OK";
                        response.Status = ResponseStatus.OK;
                    } 
                }
            }
            
            _context.Update(inspection);
            _context.SaveChanges();
    
        } catch (Exception ex){
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
        }
        
        return response;
    }
}
  
 /*
 var x = from d in _context.Documents
     join iadt in _context.InspectorAuthorizedDocumentTypes on d.DocumentTypeId equals iadt.DocumentTypeId
     where d.Id == qrData[2] && d.DocumentTypeId == iadt.DocumentTypeId && iadt.UserId == InspectorId
     select new { Documents = d, InspectorAuthorizedDocumentTypes = iadt };
 */


