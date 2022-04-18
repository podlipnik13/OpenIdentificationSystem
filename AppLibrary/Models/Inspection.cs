namespace AppLibrary.Models;

public class Inspection {
    public int Id { get; set; }
    public bool IsValid { get; set; }
    public DateTime ValidatonStartTime { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int? InspectorId { get; set; }
    public User Inspector { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; }

}

