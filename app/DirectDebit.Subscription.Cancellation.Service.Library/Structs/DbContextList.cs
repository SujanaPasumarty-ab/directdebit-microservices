using DirectDebit.Subscription.Cancellation.Service.Library.Enums;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Structs
{
	public static class DbContextList
	{
		private static List<DbContext> _dbInstances;

		static DbContextList()
		{
			_dbInstances = new List<DbContext>
			{
				new DbContext(Guid.Parse("28eb63bf-af2c-44a7-ae25-4df689eaa028"), DbContextEnum.VisionExpress, "Vision Express"),
				new DbContext(Guid.Empty, DbContextEnum.Undefined, "Undefined")
			};
		}

		/// <summary>
		/// Gets the <see cref="DbContextEnum"/> for a specified database GUID.
		/// </summary>
		/// <param name="dbGuid">The GUID of the database to get the enum for.</param>
		/// <returns>The <see cref="DbContextEnum"/> for the specified database GUID.</returns>
		public static DbContextEnum GetInstanceEnum(Guid dbGuid)
		{
			try
			{
				return _dbInstances.Where(x => x.DbGuid == dbGuid).First().DbInstance;
			}
			catch (Exception)
			{
				return DbContextEnum.Undefined;
			}
		}

		/// <summary>
		/// Gets the GUID for a specified <see cref="DbContextEnum"/>.
		/// </summary>
		/// <param name="dbEnum">The database context enum to get the GUID for.</param>
		/// <returns>The GUID for the specified database context enum.</returns>
		public static Guid GetInstanceGuid(DbContextEnum dbEnum)
		{
			return _dbInstances.Where(x => x.DbInstance == dbEnum).First().DbGuid;
		}

		/// <summary>
		/// Gets the friendly name for a specified database GUID.
		/// </summary>
		/// <param name="dbGuid">The GUID of the database to get the friendly name for.</param>
		/// <returns>The friendly name for the specified database GUID.</returns>
		public static string GetInstanceFriendlyName(Guid dbGuid)
			=> GetInstanceFriendlyName(GetInstanceEnum(dbGuid));

		/// <summary>
		/// Gets the friendly name for a specified <see cref="DbContextEnum"/>.
		/// </summary>
		/// <param name="dbEnum">The database context enum to get the friendly name for.</param>
		/// <returns>The friendly name for the specified database context enum.</returns>
		public static string GetInstanceFriendlyName(DbContextEnum dbEnum)
		{
			try
			{
				return _dbInstances.Where(x => x.DbInstance == dbEnum).First().FriendlyName;
			}
			catch (Exception)
			{
				return _dbInstances.Where(x => x.DbInstance == DbContextEnum.Undefined).First().FriendlyName;
			}
		}
	}
}