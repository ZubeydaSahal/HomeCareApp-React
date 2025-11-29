namespace HomeCareApp.Models
{
    public class ErrorViewModelDto
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}