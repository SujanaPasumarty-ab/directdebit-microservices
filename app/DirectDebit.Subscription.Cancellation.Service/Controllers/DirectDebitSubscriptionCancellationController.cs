using DirectDebit.Subscription.Cancellation.Service.Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DirectDebit.Subscription.Cancellation.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectDebitSubscriptionCancellationController : ControllerBase
    {
        /// <summary>
        /// Provides logging capabilities for the DirectDebitSubscriptionCancellationController using the specified logger implementation.
        /// </summary>
        private readonly ILogger<DirectDebitSubscriptionCancellationController> _logger;

        /// <summary>
        /// The service responsible for handling direct debit subscription cancellations.
        /// </summary>
        private readonly IDirectDebitSubscriptionCancellation _service;

        /// <summary>
        /// Initializes a new instance of the DirectDebitSubscriptionCancellationController class with the specified
        /// logger and subscription cancellation service.
        /// </summary>
        /// <param name="logger">The logger used to record diagnostic and operational information for the controller.</param>
        /// <param name="service">The service that provides operations for direct debit subscription cancellation.</param>
        public DirectDebitSubscriptionCancellationController(ILogger<DirectDebitSubscriptionCancellationController> logger, IDirectDebitSubscriptionCancellation service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Processes a request to cancel a direct debit subscription for the specified database instance.
        /// </summary>
        /// <remarks>This method logs the outcome of the cancellation attempt and returns an appropriate
        /// HTTP status code based on the result. Ensure that the provided database instance identifier is valid and
        /// corresponds to an existing subscription.</remarks>
        /// <param name="dbInstance">The unique identifier of the database instance for which the direct debit subscription cancellation is to be
        /// processed.</param>
        /// <returns>An IActionResult indicating the outcome of the cancellation request. Returns an HTTP 200 response with a
        /// success message if the cancellation is successful; otherwise, returns an HTTP 500 response with an error
        /// message.</returns>
        [HttpPost("ProcessDirectDebitSubscriptionCancellation/{dbInstance}")]
        public async Task<IActionResult> DirectDebitSubscriptionCancellationAsync(Guid dbInstance)
        {
            _logger.LogInformation("Received request to cancel direct debit subscription.");

            var result = await _service.ProcessDirectDebitSubscriptionCancellationsAsync(dbInstance);

            if (result.Success == true)
            {
                _logger.LogInformation(result.Message);
                return Ok(new { message = result.Message });
            }
            else
            {
                _logger.LogError("Failed to cancel direct debit subscription.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to cancel subscription." });
            }
        }
    }
}