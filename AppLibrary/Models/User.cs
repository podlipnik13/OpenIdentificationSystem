namespace AppLibrary.Models;

public enum Status : short {
    Pending,
    Accepted,
    Declined
}

public enum UserGroup : short {
    Default,
    Inspectee,
    Inspector,
    Admin = 99
}

public class User {
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; }
    public UserGroup UserGroup { get; set; } = UserGroup.Default;
    public Status Status { get; set; } = Status.Pending;
    public List<InspectorAuthorizedDocumentType> InspectorAuthorizedDocumentTypes { get; set; } = new();
    public List<Document> Documents { get; set; } = new();

}
