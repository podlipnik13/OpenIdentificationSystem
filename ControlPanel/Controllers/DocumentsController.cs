using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlPanel.Controllers;

public class DocumentsController : Controller
{
    private readonly OISContext _context;

    public DocumentsController(OISContext context) {
        _context = context;
    }

    private Exception NotFound(string s){
        return new( $"{s} not found!");
    }
    private int PageSize = 5; 
    
    
    
    #region DocumentType
    
    [HttpGet]
    public async Task<IActionResult> Index(int? pageNumber, int? pageSize){
        return View(await PaginatedList<DocumentType>.CreateAsync(_context.DocumentTypes.AsNoTracking(), 
            pageNumber ?? 1, pageSize ?? PageSize));
    }

        
    [HttpGet]
    public async Task<IActionResult> DocumentTypeAddOrEdit(int? id){

        DocumentType documentType = id==null? new():await _context.DocumentTypes
            .FirstOrDefaultAsync(u => u.Id == id);

        if(documentType == null) return NotFound();
        
        return View(documentType);
     }
    
    
    [HttpPost]
    public async Task<IActionResult> DocumentTypeAddOrEdit(DocumentType res){

        DocumentType documentType = await _context.DocumentTypes
            .FirstOrDefaultAsync(u => u.Id == res.Id);
        
        try{
            if(ModelState.IsValid){
                if(documentType == null){
                    
                    documentType = new(){
                        Label = res.Label
                    };
                    
                    _context.DocumentTypes.Add(documentType);

                }else{
                    documentType.Label = res.Label;

                    _context.DocumentTypes.Update(documentType);
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }catch(Exception ex){
            ViewData["ErrMsg"] = ex.Message;
        }
        return View(res);
    }

    
    [HttpGet]
    public async Task<IActionResult> DocumentTypeDelete(int? id){

        if(id == null) return NotFound();
        
        DocumentType documentType = await _context.DocumentTypes
            .FindAsync(id);

        try {
        
            if (documentType == null) throw NotFound("DocumentType");

            _context.DocumentTypes.Remove(documentType);
            await _context.SaveChangesAsync();

        }catch (Exception ex) {
            ViewData["ErrMsg"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    #endregion



    #region Parameter

    [HttpGet]
    public async Task<IActionResult> Parameters(int? id, int? pageNumber, int? pageSize){
        
        ViewData["TypeId"] = id??0;
        
        return View(await PaginatedList<DocumentParameter>.CreateAsync(_context.DocumentParameters.AsNoTracking(), 
            pageNumber ?? 1, pageSize ?? PageSize));
    }

    [HttpGet]
    public async Task<IActionResult> ParameterAddOrEdit(int? id,int? typeid){

        DocumentParameter documentParameters = id==null? new(){DocumentTypeId = (int)typeid}:
            await _context.DocumentParameters.FirstOrDefaultAsync(u => u.Id == id);

        if(documentParameters == null) return NotFound();
        
        return View(documentParameters);
    }
    
    
    [HttpPost]
    public async Task<IActionResult> ParameterAddOrEdit(DocumentParameter res){

        DocumentParameter documentParameter = await _context.DocumentParameters
            .FirstOrDefaultAsync(u => u.Id == res.Id);
        
        try{
            if(ModelState.IsValid){
                if(documentParameter == null){
                    
                    documentParameter = new(){
                        DocumentTypeId = res.DocumentTypeId,
                        DataType = res.DataType,
                        isIdentifier = res.isIdentifier,
                        Label = res.Label
                    };
                    
                    _context.DocumentParameters.Add(documentParameter);

                }else{
                    documentParameter.DataType = res.DataType;
                    documentParameter.isIdentifier = res.isIdentifier;
                    documentParameter.Label = res.Label;
                    documentParameter.DocumentTypeId = res.DocumentTypeId;

                    _context.DocumentParameters.Update(documentParameter);
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Parameters");

        }catch(Exception ex){
            ViewData["ErrMsg"] = ex.Message;
        }
        return View(res);
    }

    
    [HttpGet]
    public async Task<IActionResult> ParameterDelete(int? id){

        if(id == null) return NotFound();
        
        DocumentParameter documentParameter = await _context.DocumentParameters
            .FindAsync(id);

        try {
        
            if (documentParameter == null) throw NotFound("Document parameter");

            _context.DocumentParameters.Remove(documentParameter);
            await _context.SaveChangesAsync();

        } catch (Exception ex) {
            ViewData["ErrMsg"] = ex.Message;
        }

        return RedirectToAction("Parameters");
    }

    #endregion

}