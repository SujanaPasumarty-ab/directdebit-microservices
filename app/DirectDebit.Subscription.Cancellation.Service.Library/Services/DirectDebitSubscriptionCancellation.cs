using DirectDebit.Subscription.Cancellation.Service.Library.Application;
using DirectDebit.Subscription.Cancellation.Service.Library.Databases;
using DirectDebit.Subscription.Cancellation.Service.Library.Interfaces;
using DirectDebit.Subscription.Cancellation.Service.Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml;

namespace DirectDebit.Subscription.Cancellation.Service.ServiceDefaults.Services
{
    public class DirectDebitSubscriptionCancellation : IDirectDebitSubscriptionCancellation
    {
        /// <summary>
        /// The logger instance for logging errors and information.
        /// </summary>
        private readonly ILogger<DirectDebitSubscriptionCancellation> _logger;

        /// <summary>
        /// The database context factory for creating database contexts.
        /// </summary>
        private readonly IDbContextFactory _dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectDebitSubscriptionCancellation"/> class.
        /// </summary>
        /// <param name="logger">The logger instance to be used for logging.</param>
        public DirectDebitSubscriptionCancellation(ILogger<DirectDebitSubscriptionCancellation> logger,
            IDbContextFactory dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Asynchronously processes direct debit subscription cancellations for a specified database instance.
        /// </summary>
        /// <param name="dbInstance">The unique identifier of the database instance to target for cancellation operations.</param>
        public async Task<DirectDebitCancellationResponse> ProcessDirectDebitSubscriptionCancellationsAsync(Guid dbInstance)
        {
            _logger.LogInformation($"Starting ProcessDirectDebitSubscriptionCancellationsAsync for dbInstance: {dbInstance}");

            try
            {
                var context = _dbContextFactory.CreateStronglyTypedDatabaseContext<IDbContext>(dbInstance);

                DateTime dtCheck = DateTime.Now.AddDays(-7);

                var sdg = new SmartDebitGateway(context);

                int currentRow = 0;
                int processedCancellations = 0;

                // Get all cancelled subs with a payment method that have been updated in the last 7 days
                var cancelledSubs = await context.Subscriptions
                    .Where(s => (s.Status == "C" || s.Status == "E" || s.Status == "G" || s.Status == "W")
                                && !string.IsNullOrEmpty(s.PaymentMethod)
                                && s.LastUpdateDate > dtCheck)
                    .AsNoTracking()
                    .ToListAsync();

                var activePaymentReferences = await context.Subscriptions
                    .Where(s => (s.Status == "A" || s.Status == "R" || s.Status == "O" || s.Status == "D" || s.Status == "V")
                                && !string.IsNullOrEmpty(s.PaymentMethod))
                    .Select(x => x.PaymentMethod)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var sub in cancelledSubs)
                {
                    currentRow++;

                    // Will use this to track failures before the end of the cancellations list.
                    // Not all cancellations will log and there can be skips. 
                    _logger.LogInformation("Processing cancellation: {currentRow} of {cancelledSubs.Count()}", currentRow, cancelledSubs.Count());

                    if (activePaymentReferences.Contains(sub.PaymentMethod))
                    {
                        _logger.LogInformation($"DD {sub.PaymentMethod} exists on another active subscription so we cannot suspend it. Current Status: " + sub.Status);
                        continue;
                    }

                    var response = sdg.GetResponse("api/ddi/variable/" + sub.PaymentMethod + "/update", new SmartDebitGateway.variable_ddi() { reference_number = sub.PaymentMethod }.xml);

                    XmlNode? cancelledOn = null;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        cancelledOn = sdg.ContentXml.SelectNodes("variable_ddi")[0]["cancelled_on"];
                    }
                    else
                    {
                        _logger.LogInformation($"BadRequest for paymethod: {sub.PaymentMethod}. Current Status: {sub.Status}. SmartDebit Response: {response.StatusCode}.");
                        continue;
                    }

                    if (cancelledOn != null)
                    {
                        _logger.LogInformation($"DD {sub.PaymentMethod} has already been cancelled so no need to process. Current Status: " + sub.Status);
                        Console.WriteLine($"DD {sub.PaymentMethod} has already been cancelled so no need to process. Current Status: " + sub.Status);
                        continue;
                    }

                    if (sub.Status == "C" || sub.Status == "E")
                    {
                        try
                        {
                            response = sdg.GetResponse("api/ddi/variable/" + sub.PaymentMethod + "/cancel", new XmlDocument());
                            _logger.LogInformation($"DD {sub.PaymentMethod} has been cancelled successfully. Current Status: {sub.Status}");
                            Console.WriteLine($"DD {sub.PaymentMethod} has been cancelled successfully. Current Status: {sub.Status}");

                            processedCancellations++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation($"Error cancelling DD for Subscription: {sub.SubscriptionNum} and Reference: {sub.PaymentMethod} - {ex.Message}");
                            Console.WriteLine($"Error cancelling DD for Subscription: {sub.SubscriptionNum} and Reference: {sub.PaymentMethod} - {ex.Message}");
                            continue;
                        }
                    }
                    else
                    {
                        if (response.IsSuccessful && DateTime.ParseExact(sdg.ContentXml.SelectNodes("variable_ddi")[0]["start_date"].InnerText, "yyyy-MM-ddT00:00:00Z", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") != DateTime.MaxValue.ToString("yyyy-MM-dd"))
                        {
                            response = sdg.GetResponse("api/ddi/variable/" + sub.PaymentMethod + "/update", new SmartDebitGateway.variable_ddi()
                            {
                                start_date = DateTime.MaxValue.ToString("yyyy-MM-dd"),
                                end_date = DateTime.MaxValue.ToString("yyyy-MM-dd")
                            }.xml);

                            _logger.LogInformation("DD {PaymentMethod} has been postponed to the max date. Current Status: {Status}", sub.PaymentMethod, sub.Status);
                        }
                    }
                }

                _logger.LogInformation($"Finished processing cancelled and ended subscriptions. {processedCancellations} of {currentRow} subscriptions have been cancelled.");

                return new DirectDebitCancellationResponse
                {
                    Success = true,
                    Status = "OK",
                    Message = $"Finished processing cancelled and ended subscriptions. {processedCancellations} of {currentRow} subscriptions have been cancelled."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during cancellation processing.");

                return new DirectDebitCancellationResponse
                {
                    Success = false,
                    Status = "Error",
                    Message = $"Error occurred during cancellation processing. Exception: {ex.Message}"
                };

            }
        }
    }

}