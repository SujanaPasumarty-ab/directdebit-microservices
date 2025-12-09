using DirectDebit.Subscription.Cancellation.Service.Library.Models;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Interfaces
{
	public interface IDirectDebitSubscriptionCancellation
	{
		/// <summary>
		/// Asynchronously processes direct debit subscription cancellations for a specified database instance.
		/// </summary>
		/// <param name="dbInstance">The unique identifier of the database instance to target for cancellation operations.</param>
		/// <returns>A task that represents the asynchronous operation, containing a status or result message upon completion.</returns>
		Task<DirectDebitCancellationResponse> ProcessDirectDebitSubscriptionCancellationsAsync(Guid dbInstance);
	}
}