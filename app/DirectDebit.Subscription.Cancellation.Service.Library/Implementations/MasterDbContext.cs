using DirectDebit.Subscription.Cancellation.Service.Library.Databases;
using DirectDebit.Subscription.Cancellation.Service.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Implementations
{
	public class MasterDbContext<TContext> : DbContext, IDbContext
	where TContext : DbContext
	{
		public DbSet<Subscriptions> Subscriptions { get; set; }
		public DbSet<DirectDebitSettings> DirectDebitSettings { get; set; }

		public MasterDbContext(DbContextOptions<TContext> options) : base(options) { }

		public string GetUserName()
		{
			throw new NotImplementedException();
		}
	}
}
