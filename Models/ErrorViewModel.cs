namespace Databas.Models;

// Model for error view information
public class ErrorViewModel {
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
