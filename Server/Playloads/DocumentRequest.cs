namespace Server.Payloads;

public class DocumentPayload {
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ValidThrough { get; set; }
    public int DocumentTypeId { get; set; }
    public List<ParameterValue> ParameterValues { get; set; }
}

public class ParameterValue {    
    public int Id { get; set; }
    public string Value { get; set; }
    public int DocumentParameterId { get; set; }
    public int DocumentId { get; set; }
}


