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

    public async Task<IActionResult> Index() {
        return View(await _context.DocumentTypes.ToListAsync());
    }

    #region Parameter

    [HttpGet]
    public IActionResult ParameterCreate() {
        return View();
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> ParameterCreate(
        [Bind("Label,DataType,isIdentifier")] DocumentParameter docParam, int id ) {
        try {
            if (ModelState.IsValid) {
                var documentParameter = new DocumentParameter {
                    Label = docParam.Label,
                    DataType = docParam.DataType,
                    isIdentifier = docParam.isIdentifier,
                    DocumentTypeId = id
                };

                _context.DocumentParameters.Add(documentParameter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        } catch (Exception ex) {
            // log exception
        }
        return View(NotFound());
    }

    [HttpGet]
    public async Task<IActionResult> ParameterEdit(int? id) {
        if (id is null)
            return NotFound();

        var docParam = await _context.DocumentParameters.FirstOrDefaultAsync(d => d.Id == id);

        return View(docParam);
    }

    [HttpPost]
    public async Task<IActionResult> ParameterEdit(int id) {

        var docParam = await _context.DocumentParameters
                .FirstOrDefaultAsync(d => d.Id == id);


        if (await TryUpdateModelAsync<DocumentParameter>( docParam, "",
                d => d.Label, 
                d => d.DataType, 
                d => d.isIdentifier)){
                
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
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> ParameterDelete(int id) {
        var docParam = await _context.DocumentParameters.FindAsync(id);

        if (docParam is null) return NotFound();

        try {
            _context.DocumentParameters.Remove(docParam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) {
            //Log the error
        }
        return View(NotFound());
    }
    #endregion

    #region Type
    
    [HttpGet]
    public IActionResult TypeCreate() {
        return View();
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> TypeCreate(
        [Bind("Label")] DocumentType docType) {
        
        try {
            if (ModelState.IsValid) {
                
                var documentType = new DocumentType {
                    Label = docType.Label               
                };

                _context.DocumentTypes.Add(docType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        } catch (Exception ex) {
            // log exception
        }
        return View(docType);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> TypeEdit(int? id) {
        
        if (id is null) return NotFound();

        var docType = await _context.DocumentTypes
            .Include(d => d.DocumentParameters)
            .FirstOrDefaultAsync(d => d.Id == id);

        return View(docType);
    }
    
    [HttpPost]
    public async Task<IActionResult> TypeEdit(int id){
        
        var docType = await _context.DocumentTypes
                .FirstOrDefaultAsync(d => d.Id == id);


        if (await TryUpdateModelAsync<DocumentType>( 
            docType, "", d => d.Label)){
                
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
    public async Task<IActionResult> TypeDelete(int id) {
        
        var docType = await _context.DocumentTypes.FindAsync(id);

        if (docType is null) return NotFound();

        try {
            _context.DocumentTypes.Remove(docType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) {
            //Log the error
        }
        return View(NotFound());
    }
    
    
    #endregion
    
}
