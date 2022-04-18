namespace AppLibrary.Models;

public enum DataType : short {
    number,
    text,
    date
}

public class DocumentParameter {

    public int Id { get; set; }
    public string Label { get; set; }
    public DataType DataType { get; set; }
    public bool isIdentifier { get; set; }

    public int DocumentTypeId { get; set; } 
    public DocumentType DocumentType { get; set; }

}
