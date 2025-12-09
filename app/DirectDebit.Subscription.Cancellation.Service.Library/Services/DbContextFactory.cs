using DirectDebit.Subscription.Cancellation.Service.Library.Enums;
using DirectDebit.Subscription.Cancellation.Service.Library.Structs;
using Microsoft.Extensions.Configuration;
using DirectDebit.Subscription.Cancellation.Service.Library.Interfaces;
using DirectDebit.Subscription.Cancellation.Service.Library.Databases;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Services
{
	public class DbContextFactory : IDbContextFactory
	{
		private readonly IConfiguration _configuration;

		public DbContextFactory(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Based off of the incoming DB Instance, it creates an active context for DB interfacing
		/// </summary>
		/// <param name="dbInstance">The required DB Context instance guid you need in order to make a call on</param>
		/// <returns></returns>
		public object CreateDatabaseContext(Guid dbInstance)
			=> CreateDatabaseContext(DbContextList.GetInstanceEnum(dbInstance));

		private object CreateDatabaseContext(DbContextEnum dbContextEnum)
		{
			throw new NotImplementedException();
		}

		// Resolve by GUID
		public DbContext GetDbContext(Guid dbInstance)
		{
			var dbEnum = DbContextList.GetInstanceEnum(dbInstance);
			return GetDbContext(dbEnum);
		}

		// Resolve by Enum
		public DbContext GetDbContext(DbContextEnum dbEnum)
		{
			var dbGuid = DbContextList.GetInstanceGuid(dbEnum);
			var friendlyName = DbContextList.GetInstanceFriendlyName(dbEnum);

			// Get connection string from secrets/configuration
			var connStr = _configuration.GetConnectionString(friendlyName);
			if (string.IsNullOrWhiteSpace(connStr))
				throw new InvalidOperationException($"Connection string for '{friendlyName}' not found.");

			//Return DbContext struct
			return new DbContext(dbGuid, dbEnum, friendlyName);
		}

		public object CreateDbContext(DbContextEnum databaseType)
		{
			throw new NotImplementedException();
		}

		public object CreateDbContext(Guid dbInstance)
		{
			throw new NotImplementedException();
		}

		public TInterface CreateStronglyTypedDatabaseContext<TInterface>(DbContextEnum databaseType) where TInterface : IDbContextRestriction
		{
			throw new NotImplementedException();
		}

		public TInterface CreateStronglyTypedDatabaseContext<TInterface>(Guid dbInstance) where TInterface : IDbContextRestriction
		{
			throw new NotImplementedException();
		}
	}
}