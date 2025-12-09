namespace DirectDebit.Subscription.Cancellation.Service.Library.BaseObjects
{
    /// <summary>
    /// Base pbject that is to be inherited by all DB related tables so that they have a Primary Key of type TKey
    /// </summary>
    /// <typeparam name="TKey">The struct property assigned to the primary key (id) of the table</typeparam>
    public class BaseObject<TKey>
        where TKey : struct
    {
        public TKey Id { get; set; }
    }
}
