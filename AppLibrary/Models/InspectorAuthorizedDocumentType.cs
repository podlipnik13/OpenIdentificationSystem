namespace AppLibrary.Models;

public class InspectorAuthorizedDocumentType {
    public int Id { get; set; }

    public int DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}
