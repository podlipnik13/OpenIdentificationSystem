using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlPanel.Controllers;
public class UsersController : Controller {

    private readonly OISContext _context;

    public UsersController(OISContext context) {
        _context = context;
    }


#region Users
    
    public async Task<IActionResult> Index() {
        return View(await _context.Users.ToListAsync());
    }

    [HttpGet]
    public IActionResult UserCreate() {
        return View();
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> UserCreate(
        [Bind("UserName,Documents,Email,Status,UserGroup")] User usr) {
        
        try {
            if (ModelState.IsValid) {
                
                var user = new User {
                    UserName = usr.UserName,
                    Documents = usr.Documents,
                    Email = usr.Email,
                    Status = usr.Status,
                    UserGroup = usr.UserGroup               
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        } catch (Exception ex) {
            // log exception
        }
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> UserEdit(int? id) {
        
        if (id is null) return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(d => d.Id == id);

        return View(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> UserEdit(int id){
        
        var user = await _context.Users
                .FirstOrDefaultAsync(d => d.Id == id);


        if (await TryUpdateModelAsync<User>( user, "", 
            u => u.UserName,
            u => u.UserGroup,
            u => u.Status,
            u => u.Email
            )){
                try {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }catch (Exception ex){
                    //log
                }      
        }
        return View(NotFound());
    }
    
    [HttpGet]
    public async Task<IActionResult> UserDelete(int id) {
        
        var user = await _context.Users.FindAsync(id);

        if (user is null) return NotFound();

        try {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) {
            ViewData["ErrorMessage"] = ex.Message;
            //log 
        }
        return View(NotFound());
    }

#endregion

#region User Documents

    [HttpGet]
    public async Task<IActionResult> UserDocuments(int? id) {
        
        if (id is null) return NotFound();

        var userDocs = await _context.Documents
            .Where(d => d.UserId == id)
            .Select(d => new DocumentView{
                Id = d.Id,
                UserId = d.UserId,
                DocumentName= d.DocumentType.Label,
                IssueDate = d.IssueDate,
                ValidThrough = d.ValidThrough,
                DocumentTypeId = d.DocumentTypeId,
                DocumentStatus = d.DocumentStatus
            })
            .ToListAsync();
        
        return View(userDocs);
    }

    [HttpGet]
    public async Task<IActionResult> UserDocumentEdit(int? id) {
        
        if (id is null) return NotFound();

        var userDocParams = await _context.DocumentParameterValues
            .Where(dp => dp.DocumentId == id)
            .Select(dp => new DocumentParameterValueView{
                Id = dp.Id,
                ParameterName = dp.DocumentParameter.Label,
                ParameterValue = dp.ParameterValue
            })
            .ToListAsync();

        return View(userDocParams);
    }

    [HttpGet]
    public IActionResult UserDocumentCreate() {
        return View();
    }

    /*
    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> UserDocumentCreate(
        [Bind("UserName,Documents,Email,Status,UserGroup")] User usr) {
        
        try {
            if (ModelState.IsValid) {
                
                var user = new User {
                    UserName = usr.UserName,
                    Documents = usr.Documents,
                    Email = usr.Email,
                    Status = usr.Status,
                    UserGroup = usr.UserGroup               
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        } catch (Exception ex) {
            // log exception
        }
        return View();
    }
    
    */

        
    [HttpGet]
    public async Task<IActionResult> ParameterValueEdit(int? id) {
        
        if (id is null) return NotFound();

        var userDocParam = await _context.DocumentParameterValues
            .Include(dp => dp.DocumentParameter)
            .FirstOrDefaultAsync(pv => pv.Id == id);
        


        return View(userDocParam);
    }

    [HttpPost]
    public async Task<IActionResult> ParameterValueEdit(int id) {
    
        var userDocParam = await _context.DocumentParameterValues
            .FirstOrDefaultAsync(pv => pv.Id == id);
        
        if (await TryUpdateModelAsync<DocumentParameterValue>(
                userDocParam, "", dp => dp.ParameterValue)){
                
                try {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }catch (Exception ex){
                    //log
                }      
        }
        return View(NotFound());
    }
    
#endregion

}