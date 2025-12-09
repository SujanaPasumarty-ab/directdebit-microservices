using DirectDebit.Subscription.Cancellation.Service.Library.Databases;
using DirectDebit.Subscription.Cancellation.Service.Library.Enums;
using DirectDebit.Subscription.Cancellation.Service.Library.Structs;
using System;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Interfaces
{
    /// <summary>
    /// Resolves a DbContext struct based on a GUID or DbContextEnum.
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// Gets a DbContext struct for the specified database GUID.
        /// </summary>
        /// <param name="contextId">The GUID of the database context.</param>
        /// <returns>The resolved DbContext struct.</returns>
        DbContext GetDbContext(Guid contextId);

        /// <summary>
        /// Gets a DbContext struct for the specified DbContextEnum.
        /// </summary>
        /// <param name="dbEnum">The database context enum.</param>
        /// <returns>The resolved DbContext struct.</returns>
        DbContext GetDbContext(DbContextEnum dbEnum);

		/// <summary>
		/// Based off of the incoming DB Instance, it creates an active context for DB interfacing
		/// </summary>
		/// <param name="databaseType">The required DB Context instance you need in order to make a call on</param>
		/// <returns></returns>
		public object CreateDbContext(DbContextEnum databaseType);

		/// <summary>
		/// Based off of the incoming DB Instance, it creates an active context for DB interfacing
		/// </summary>
		/// <param name="dbInstance">The required DB Context instance guid you need in order to make a call on</param>
		/// <returns></returns>
		public object CreateDbContext(Guid dbInstance);

		/// <summary>
		/// Explicitly defines the return type of the context so that you have acess to required tables
		/// </summary>
		/// <typeparam name="TInterface">The interface type that you want back</typeparam>
		/// <param name="databaseType">The required DB Context instance you need in order to make a call on</param>
		/// <returns></returns>
		public TInterface CreateStronglyTypedDatabaseContext<TInterface>(DbContextEnum databaseType)
			where TInterface : IDbContextRestriction;

		//
		// Summary:
		//     Explicitly defines the return type of the context so that you have acess to required
		//     tables
		//
		// Parameters:
		//   dbInstance:
		//     The required DB Context instance guid you need in order to make a call on
		//
		// Type parameters:
		//   TInterface:
		//     The interface type that you want back
		TInterface CreateStronglyTypedDatabaseContext<TInterface>(Guid dbInstance) where TInterface : IDbContextRestriction;
	}
}
