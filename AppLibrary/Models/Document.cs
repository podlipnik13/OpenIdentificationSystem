using System.ComponentModel.DataAnnotations.Schema;

namespace AppLibrary.Models;

public enum DocumentStatus : short {
    Pending,
    Accepted,
    Declined
}


public partial class Document {

    public int Id { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime ValidThrough { get; set; } = DateTime.UtcNow;
    public DocumentStatus DocumentStatus { get; set; } = DocumentStatus.Pending;

    public int DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public List<DocumentParameterValue> DocumentParameterValues { get; set; } = new();

    [NotMapped]
    public List<DocumentParameter> DocumentParameters { get; set; } = new();

}
