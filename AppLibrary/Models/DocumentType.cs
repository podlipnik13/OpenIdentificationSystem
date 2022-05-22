namespace AppLibrary.Models;


public interface IDocumentType {
    public int Id { get; set; }
    public string Label { get; set; }
}

public class DocumentType {
    public int Id { get; set; }
    public string Label { get; set; }
    public List<DocumentParameter> DocumentParameters { get; set; }  = new();
    public List<Document> Documents { get; set; } = new();

}


