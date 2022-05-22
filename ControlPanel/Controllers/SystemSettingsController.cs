using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ControlPanel.Controllers;


public class SystemSettingsController : Controller {

    private readonly OISContext _context;

    public SystemSettingsController(OISContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> Index() {
        
        return View( await _context.SystemParameters.ToListAsync());
    }

    [HttpGet]
    public async Task<ActionResult> SystemParameterEdit(int? id) {
        
        if(id is null) return BadRequest();

        var systemParameter = await _context.SystemParameters.FirstOrDefaultAsync(sp => sp.Id == id);

        if(systemParameter is null) return NotFound();
        
        return View(systemParameter); 
    }

    [HttpPost]
    public async Task<ActionResult> SystemParameterEdit(SystemParameter res) {
        
        if(res is null) return BadRequest();

        var systemParameter = await _context.SystemParameters.FirstOrDefaultAsync(sp => sp.Id == res.Id);

        try{
            if(!ModelState.IsValid) throw new Exception("Invalid Model state!");
            if(systemParameter == null) throw new Exception("There is no such entity as:" + res);

            systemParameter.Property = res.Property;
            systemParameter.Value = res.Value;

            _context.SystemParameters.Update(systemParameter);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }catch(Exception ex){
            ViewData["ErrMsg"] = ex.Message;
        }
        return View(res);
    }

}