namespace DirectDebit.Subscription.Cancellation.Service.Library.Models
{
	public class DirectDebitSettings : BaseObjects.BaseObject<int>
	{
		public string SettingName { get; set; }

		public string SettingValue { get; set; }

	}
}
