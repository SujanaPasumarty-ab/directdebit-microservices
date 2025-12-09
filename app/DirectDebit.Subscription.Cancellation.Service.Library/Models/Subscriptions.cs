using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DirectDebit.Subscription.Cancellation.Service.Library.Models
{
	public class Subscriptions : BaseObjects.BaseObject<int>
	{
		public int SubscriptionNum { get; set; }

		public int PayerId { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }

		public string PaymentMethod { get; set; }

		[MaxLength(50)]
		public string CustomerId { get; set; }

		[Required]
		[Column(TypeName = "Decimal(10,2)")]
		public decimal CurrentCreditBalance { get; set; }

		[Required]
		[Column(TypeName = "Decimal(10,2)")]
		public decimal OverdueAmount { get; set; }

		[Required]
		[Column(TypeName = "Decimal(10,2)")]
		public decimal CurrentPenalty { get; set; }

		public DateTime? NextCollectionDate { get; set; }

		public DateTime? LastCollectionDate { get; set; }

		[Required]
		public string Status { get; set; }

		public DateTime? ActualEndDate { get; set; }

		[Column("SubscribingPracticeId")]
		[Required]
		public int PracticeId { get; set; }

		[Required]
		public DateTime CreatedDate { get; set; }

		[Required]
		public string CreatedBy { get; set; }

		[Required]
		public string LastUpdatedBy { get; set; }

		[Required]
		public DateTime LastUpdateDate { get; set; }

		[Required]
		public short SubsTermMonths { get; set; }

		[Required]
		public short Max_Subs_Products { get; set; }

		[Column("ProgramCode")]
		[Required]
		[MaxLength(10)]
		public string ProgramCodeId { get; set; }

		[Required]
		[Column(TypeName = "Decimal(10,2)")]
		public decimal RegularAmount { get; set; }

		[Required]
		[MaxLength(10)]
		public string PaymentType { get; set; }

		public decimal? InitialPayment { get; set; }

		public int? WearerId { get; set; }

		public int? ReplacesSubscriptionNum { get; set; }

		public DateTime? RenewalStartDate { get; set; }

		public DateTime? RenewalEndDate { get; set; }

		public DateTime? InArrearsDate { get; set; }

		public DateTime? InDefaultDate { get; set; }

		public decimal? StartingTotal { get; set; }

		public decimal? TotalPurchaseCost { get; set; }

		public bool IsPaymentPlan { get; set; }

	}
}
