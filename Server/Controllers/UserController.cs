using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Server.Controllers;

[EnableCors]
[ApiController]
[Route("Api/[controller]")]
public class UserController : ControllerBase
{
    private readonly OISContext _context;
    public UserController(OISContext context) {
        _context = context;
    }
    
    [HttpGet("Register")]
    public async Task<ActionResult<Response>> Register([FromQuery] UserPayload userPayload) {

        Response response = new();

        User user = await _context.Users
           .FirstOrDefaultAsync(u => u.Email == userPayload.Email);

        try {
            if(user != null) throw new Exception("User already exists!");
            
            user = new(){
                UserName = userPayload.UserName,
                Email = userPayload.Email,
                Password  = Cryptography.HashThis(userPayload.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Status = ResponseStatus.OK;
            response.Message = $"Successfull registration!";
        }
        catch (Exception ex) {
            response.Status = ResponseStatus.Error;
            response.Message = ex.ToString();
        }
        
        return response;
    }

    [HttpGet("CheckUser")]
    public Response CheckUser([FromQuery] UserPayload res) {
        
        Response response = new();

        try{
            
            User user = _context.Users
                .FirstOrDefault(u => u.Email == res.Email && u.UserName == res.UserName);

            if(user == null) throw new Exception("User doesn't exist!");
            if(user.UserStatus == UserStatus.Declined) throw new Exception("User declined!");
            if(user.Password != Cryptography.HashThis(res.Password)) throw new Exception("Invalid credentials!");
            if(user.UserStatus != UserStatus.Accepted) throw new Exception("User not Accapted!");

            response.Message = "OK"; 
            response.Status = ResponseStatus.OK;

        } catch (Exception ex) {
            response.Data = null;
            response.Message = ex.Message.ToString();
            response.Status = ResponseStatus.Error;
        }
        return response;
    }

    [HttpGet("GetToken")]
    public Response GetToken([FromQuery] UserPayload res) {
        return Security.GetToken(_context,res);
    }

    [EnableCors(default)]
    [HttpGet("GetDocuments")]
    public async Task<ActionResult<Response>> GetDocuments(string token){
        Response response = new();

        var r = Security.Authenticate(token);
        if(r.Status != ResponseStatus.OK){
            response.Status = r.Status;
            response.Message = r.Message;
            return response;
        }
        try {
            int userId = (int)r.Data;
            
            List<Document> document = await _context.Documents
                .Where(d => d.UserId == userId)
                .ToListAsync();
            
            if(document == null) throw new Exception("No documents found!");

            response.Data = document;
            response.Status = ResponseStatus.OK;

        } catch (Exception ex){
            response.Status = ResponseStatus.Error;
            response.Message = ex.Message;
        }
        return response;
    }

}