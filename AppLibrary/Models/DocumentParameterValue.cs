namespace AppLibrary.Models;


public class DocumentParameterValue {
    public int Id { get; set; }
    public string ParameterValue { get; set; }


    public int DocumentParameterId { get; set; }
    public DocumentParameter DocumentParameter { get; set; } 

    public int DocumentId { get; set; }
    public Document Document { get; set; } 
}
