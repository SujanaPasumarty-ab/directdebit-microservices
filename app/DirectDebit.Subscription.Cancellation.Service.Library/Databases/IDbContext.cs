using DirectDebit.Subscription.Cancellation.Service.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Databases
{
	public interface IDbContext : IDbContextRestriction
	{
		public DbSet<Subscriptions> Subscriptions { get; set; }

		public DbSet<DirectDebitSettings> DirectDebitSettings { get; set; }
	}
}
