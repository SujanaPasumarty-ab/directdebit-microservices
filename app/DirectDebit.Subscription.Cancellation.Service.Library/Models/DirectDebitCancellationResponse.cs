namespace DirectDebit.Subscription.Cancellation.Service.Library.Models
{
	public class DirectDebitCancellationResponse
	{
        public bool Success { get; set; }
        public string Status { get; set; } // "OK", "Error"
        public string Message { get; set; }
    }
}
