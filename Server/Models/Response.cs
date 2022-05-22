namespace Server.Models;

public enum ResponseStatus{
    OK = 0,
    Error = 1,
    TimedOut = 3,

}

public class Response { 
    public string Message { get; set; } = "";
    public ResponseStatus Status { get; set; } = ResponseStatus.Error;
    public Object Data { get; set; }

}

public class JsonToken { 
    public string Token { get; set; }
    public DateTimeOffset Expires { get; set; }

}


