using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ControlPanel.Controllers;
public class UsersController : Controller {

    private readonly OISContext _context;

    public UsersController(OISContext context) {
        _context = context;
    }

    private Exception NotFound(string s){
        return new( $"{s} not found!");
    }
    private int PageSize = 5; 
        
    #region User
    
    [HttpGet]
    public async Task<IActionResult> Index(int? pageNumber, int? pageSize){
        return View(await PaginatedList<User>.CreateAsync(_context.Users.AsNoTracking(), 
            pageNumber ?? 1, // return the value of pageNumber if it has value, or return 1 if pageNumber is null. 
            pageSize ?? PageSize));
    }    

    [HttpGet]
    public async Task<IActionResult> UserAddOrEdit(int? id){

        User user = id==null? new():await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if(user == null) return NotFound();
        
        return View(user);
     }
    
    
    [HttpPost]
    public async Task<IActionResult> UserAddOrEdit(User res){

        User user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == res.Id);
        
        try{
            if(ModelState.IsValid){
                if(user == null){
                    
                    user = new(){
                        UserName = res.UserName,
                        Email = res.Email,
                        UserStatus = res.UserStatus,
                        UserGroup = res.UserGroup
                    };
                    
                    _context.Users.Add(user);

                }else{
                    
                    user.UserName = res.UserName;
                    user.Email = res.Email;
                    user.UserStatus = res.UserStatus;
                    user.UserGroup = res.UserGroup;

                    _context.Users.Update(user);
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
    public async Task<IActionResult> UserDelete(int? id){

        if(id == null) return NotFound();
        
        User user = await _context.Users
            .FindAsync(id);

        try {
        
            if (user == null) throw NotFound("User");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

        }catch (Exception ex) {
            ViewData["ErrMsg"] = ex.Message;
        }

        return RedirectToAction("Index");
    }

    #endregion

    #region Documents

    [HttpGet]
    public async Task<IActionResult> Documents(int? id, int? pageNumber, int? pageSize) {
        
        if (id is null) return NotFound();
        
        
        ViewData["UserId"] = id;
        ViewData["UserName"] = _context.Users.FirstOrDefault(u => u.Id == id).UserName;
        
        var documentView = await PaginatedList<DocumentView>.CreateAsync(_context.Documents 
            .Where(d => d.UserId == id)
            .Select(d => new DocumentView {
                Id = d.Id,
                DocumentName = d.DocumentType.Label,
                IssueDate = d.IssueDate,
                ValidThrough = d.ValidThrough,
                DocumentTypeId = d.DocumentTypeId,
                DocumentStatus = d.DocumentStatus
            }).AsNoTracking(),
            pageNumber ?? 1, 
            pageSize ?? PageSize
        );

        return View(documentView);
    }

    [HttpGet]
    public async Task<IActionResult> DocumentAddOrEdit(int? id, int? userId){

        Document document = id.HasValue? await _context.Documents
            .FirstOrDefaultAsync(u => u.Id == id): new(){
                UserId = (int)userId};

        if(document == null) return NotFound();
      
        ViewData["DocumentTypesList"] = new SelectList(await _context.DocumentTypes.ToListAsync(),"Id","Label", document.DocumentTypeId);
        
        return View(document);
     }
    
    
    [HttpPost]
    public async Task<IActionResult> DocumentAddOrEdit(Document res){

        Document document = await _context.Documents
            .FirstOrDefaultAsync(u => u.Id == res.Id);
        
        try{
            if(ModelState.IsValid){
                if(document == null){
                    
                    document = new(){
                        UserId = res.UserId,
                        DocumentTypeId = res.DocumentTypeId,
                        DocumentStatus = res.DocumentStatus,
                        IssueDate = DateTime.SpecifyKind(res.IssueDate, DateTimeKind.Utc),
                        ValidThrough = DateTime.SpecifyKind(res.ValidThrough, DateTimeKind.Utc),
                    };
                    
                    _context.Documents.Add(document);
                }else{
                    
                    document.DocumentTypeId = res.DocumentTypeId;
                    document.DocumentStatus = res.DocumentStatus;
                    document.IssueDate = DateTime.SpecifyKind(res.IssueDate, DateTimeKind.Utc);
                    document.ValidThrough = DateTime.SpecifyKind(res.ValidThrough, DateTimeKind.Utc);

                    _context.Documents.Update(document);
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Documents", new { id = document.UserId });

        }catch(Exception ex){
            ViewData["ErrMsg"] = ex.Message;
        }
        
        ViewData["DocumentTypesList"] = new SelectList(await _context.DocumentTypes.ToListAsync(),"Id","Label", document.DocumentTypeId);
        return View(res);
    }

    [HttpGet]
    public async Task<IActionResult> DocumentDelete(int? id){

        if(id == null) return NotFound();
        
        Document document = await _context.Documents
            .FindAsync(id);

        try {
        
            if (document == null) throw NotFound("Document");

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

        }catch (Exception ex) {
            ViewData["ErrMsg"] = ex.Message;
        }

        return RedirectToAction("Documents", new { id = document.UserId });
    }
  
    #endregion

    #region Parameters

    [HttpGet]
    public async Task<IActionResult> Parameters(int? id, int? pageNumber, int? pageSize) {
        
        if (id is null) return NotFound();
        
        var typeid = _context.Documents.Where(d => d.Id == id)
        .Select(dt => dt.DocumentTypeId).First();

        var docparams = _context.DocumentParameters
            .Where(dp => dp.DocumentTypeId == typeid).ToArray();
            var documentView = await _context.DocumentParameterValues
            .Where(dp => dp.DocumentId == id)
            .Select(dp => new DocumentParameterValueView{
                Id = dp.Id,
                ParameterName = dp.DocumentParameter.Label,
                ParameterValue = dp.ParameterValue
            })
            .ToListAsync();

        return View(documentView);
    }

    [HttpGet]
    public async Task<IActionResult> ParameterEdit(int? id, int? userId){

        Document document = id.HasValue? await _context.Documents
            .FirstOrDefaultAsync(u => u.Id == id): new(){
                UserId = (int)userId};

        if(document == null) return NotFound();
      
        ViewData["DocumentTypesList"] = new SelectList(await _context.DocumentTypes.ToListAsync(),"Id","Label", document.DocumentTypeId);
        
        return View(document);
     }
    
    
    [HttpPost]
    public async Task<IActionResult> ParameterEdit(Document res){

        Document document = await _context.Documents
            .FirstOrDefaultAsync(u => u.Id == res.Id);
        
        try{
            if(ModelState.IsValid){
                if(document == null){
                    
                    document = new(){
                        UserId = res.UserId,
                        DocumentTypeId = res.DocumentTypeId,
                        DocumentStatus = res.DocumentStatus,
                        IssueDate = DateTime.SpecifyKind(res.IssueDate, DateTimeKind.Utc),
                        ValidThrough = DateTime.SpecifyKind(res.ValidThrough, DateTimeKind.Utc),
                    };
                    
                    _context.Documents.Add(document);

                }else{
                    
                    document.DocumentTypeId = res.DocumentTypeId;
                    document.DocumentStatus = res.DocumentStatus;
                    document.IssueDate = DateTime.SpecifyKind(res.IssueDate, DateTimeKind.Utc);
                    document.ValidThrough = DateTime.SpecifyKind(res.ValidThrough, DateTimeKind.Utc);

                    _context.Documents.Update(document);
                }
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Documents", new { id = document.UserId });

        }catch(Exception ex){
            ViewData["ErrMsg"] = ex.Message;
        }
        
        ViewData["DocumentTypesList"] = new SelectList(await _context.DocumentTypes.ToListAsync(),"Id","Label", document.DocumentTypeId);
        return View(res);
    }

    #endregion

}