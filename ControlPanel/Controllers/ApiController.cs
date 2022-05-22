using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ControlPanel.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiController : Controller {

    private readonly OISContext _context;

    public ApiController(OISContext context) {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Document>> GetDocument(int id) {
        
        var document = await _context.Documents.FindAsync(id);
        if (document == null) {
            return NotFound();
        }
        return document;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Document>>> GetDocuments(){
        return await _context.Documents.ToListAsync();
    }

}