using DirectDebit.Subscription.Cancellation.Service.Library.Databases;
using DirectDebit.Subscription.Cancellation.Service.Library.Enums;
using DirectDebit.Subscription.Cancellation.Service.Library.Models;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Structs
{
	public class DbContext
	{
		public Guid DbGuid { get; }

		public DbContextEnum DbInstance { get; }

		public string FriendlyName { get; }
		public Microsoft.EntityFrameworkCore.DbSet<Subscriptions> Subscriptions { get; set; }
		public Microsoft.EntityFrameworkCore.DbSet<DirectDebitSettings> DirectDebitSettings { get; set; }

		public DbContext(Guid dbGuid, DbContextEnum dbInstance, string friendlyName)
		{
			DbGuid = dbGuid;
			DbInstance = dbInstance;
			FriendlyName = friendlyName;
		}

		public string GetUserName()
		{
			throw new NotImplementedException();
		}
	}
}