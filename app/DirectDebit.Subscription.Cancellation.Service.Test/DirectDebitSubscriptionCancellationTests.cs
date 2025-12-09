using DirectDebit.Subscription.Cancellation.Service.Library.Databases;
using DirectDebit.Subscription.Cancellation.Service.Library.Interfaces;
using DirectDebit.Subscription.Cancellation.Service.Library.Models;
using DirectDebit.Subscription.Cancellation.Service.Library.Structs;
using DirectDebit.Subscription.Cancellation.Service.ServiceDefaults.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Linq.Expressions;

namespace DirectDebit.Subscription.Cancellation.Service.Test
{
    public class DirectDebitSubscriptionCancellationTests
    {
        private static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Add async support for EF Core
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));
			TestAsyncQueryProvider<T> testAsyncQueryProvider = new(queryable.Provider);
			mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns((IQueryProvider)testAsyncQueryProvider);

            return mockSet;
        }

        // Helper classes for async query support
        internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
            public object Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(expression);
            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));

			TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
			{
				throw new NotImplementedException();
			}
		}
        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }

        [Fact]
        public async Task ProcessDirectDebitSubscriptionCancellationsAsync_ReturnsSuccessResponse_WhenNoCancelledSubscriptions()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DirectDebitSubscriptionCancellation>>();
            var dbContextFactoryMock = new Mock<IDbContextFactory>();
            var dbContextMock = new Mock<IDbContext>();

            // Setup dbContextFactory to return dbContextMock
            dbContextFactoryMock
                .Setup(f => f.CreateStronglyTypedDatabaseContext<IDbContext>(It.IsAny<Guid>()))
                .Returns(dbContextMock.Object);

            // Setup dbContextMock to return empty subscriptions using a mocked DbSet
            var mockDbSet = CreateMockDbSet(new List<Subscriptions>());
            dbContextMock.Setup(c => c.Subscriptions).Returns(mockDbSet.Object);

            var service = new DirectDebitSubscriptionCancellation(loggerMock.Object, dbContextFactoryMock.Object);

            // Act
            var result = await service.ProcessDirectDebitSubscriptionCancellationsAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("OK", result.Status);
        }

        [Fact]
        public async Task ProcessDirectDebitSubscriptionCancellationsAsync_ReturnsSuccessResponse_WhenSubscriptionsCancelled()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DirectDebitSubscriptionCancellation>>();
            var dbContextFactoryMock = new Mock<IDbContextFactory>();
            var dbContextMock = new Mock<IDbContext>();

            // Create a cancelled subscription
            var cancelledSubscription = new Subscriptions
            {
                Status = "C",
                PaymentMethod = "PM123",
                LastUpdateDate = DateTime.Now
            };

            // Setup dbContext to return the cancelled subscription
            var mockDbSet = CreateMockDbSet(new List<Subscriptions> { cancelledSubscription });
            dbContextMock.Setup(c => c.Subscriptions).Returns(mockDbSet.Object);

            // Setup dbContextFactory to return dbContextMock
            dbContextFactoryMock
                .Setup(f => f.CreateStronglyTypedDatabaseContext<IDbContext>(It.IsAny<Guid>()))
                .Returns(dbContextMock.Object);

            // Mock SmartDebitGateway using Moq.Protected or a wrapper if possible
            // For simplicity, assume SmartDebitGateway.GetResponse returns a successful response
            // You may need to refactor SmartDebitGateway for better testability (e.g., inject as interface)
            var service = new DirectDebitSubscriptionCancellation(loggerMock.Object, dbContextFactoryMock.Object);

            // Act
            var result = await service.ProcessDirectDebitSubscriptionCancellationsAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("OK", result.Status);
            Assert.Contains("cancelled", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ProcessDirectDebitSubscriptionCancellationsAsync_ReturnsErrorResponse_WhenContextNotFound()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DirectDebitSubscriptionCancellation>>();
            var dbContextFactoryMock = new Mock<IDbContextFactory>();

            // Setup dbContextFactory to return null (context not found)
            dbContextFactoryMock
                .Setup(f => f.CreateStronglyTypedDatabaseContext<IDbContext>(It.IsAny<Guid>()))
                .Returns((IDbContext)null);

            var service = new DirectDebitSubscriptionCancellation(loggerMock.Object, dbContextFactoryMock.Object);

            // Act
            var result = await service.ProcessDirectDebitSubscriptionCancellationsAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Error", result.Status);
            Assert.Contains("Object reference", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ProcessDirectDebitSubscriptionCancellationsAsync_ContinuesProcessing_WhenExceptionThrownDuringCancellation()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DirectDebitSubscriptionCancellation>>();
            var dbContextFactoryMock = new Mock<IDbContextFactory>();
            var dbContextMock = new Mock<IDbContext>();

            // Create a cancelled subscription
            var cancelledSubscription = new Subscriptions
            {
                Status = "C",
                PaymentMethod = "PM123",
                LastUpdateDate = DateTime.Now
            };

            // Setup dbContext to return the cancelled subscription
            var mockDbSet = CreateMockDbSet(new List<Subscriptions> { cancelledSubscription });
            dbContextMock.Setup(c => c.Subscriptions).Returns(mockDbSet.Object);

            // Setup dbContext to return required DirectDebitSettings
            var settings = new List<DirectDebitSettings>
            {
                new DirectDebitSettings { SettingName = "URL", SettingValue = "http://localhost" },
                new DirectDebitSettings { SettingName = "USERNAME", SettingValue = "user" },
                new DirectDebitSettings { SettingName = "PASSWORD", SettingValue = "pass" },
                new DirectDebitSettings { SettingName = "SUID", SettingValue = "suid" }
            };
            var mockSettingsDbSet = CreateMockDbSet(settings);
            dbContextMock.Setup(c => c.DirectDebitSettings).Returns(mockSettingsDbSet.Object);

            // Setup dbContextFactory to return dbContextMock
            dbContextFactoryMock
                .Setup(f => f.CreateStronglyTypedDatabaseContext<IDbContext>(It.IsAny<Guid>()))
                .Returns(dbContextMock.Object);

            // Act
            var service = new DirectDebitSubscriptionCancellation(loggerMock.Object, dbContextFactoryMock.Object);
            var result = await service.ProcessDirectDebitSubscriptionCancellationsAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Error", result.Status);
            Assert.Contains("cancelled", result.Message, StringComparison.OrdinalIgnoreCase);
            // Optionally, verify logger was called with error
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error cancelling DD")),
                    It.IsAny<Exception>(),
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce
            );
        }
    }
}
