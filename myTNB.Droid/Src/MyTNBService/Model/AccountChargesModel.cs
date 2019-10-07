﻿using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.MyTNBService.Model
{
	public class AccountChargesModel
	{
		public List<AccountChargeModel> AccountChargeList { get; set; }
		public List<MandatoryChargesPopupModel> MandatoryChargesPopUpList { get; set; }
	}

	public class AccountChargeModel
	{
		public string ContractAccount { get; set; }
		public double CurrentCharges { get; set; }
		public double OutstandingCharges { get; set; }
		public double AmountDue { get; set; }
		public string DueDate { get; set; }
		public string BillDate { get; set; }
		public string IncrementREDueDateByDays { get; set; }
		public bool IsCleared { get; set; }
		public bool IsPaidExtra { get; set; }
		public bool IsNeedPay { get; set; }
		public MandatoryChargeModel MandatoryCharges { get; set; }
	}

	public class MandatoryChargeModel
	{
		public double TotalAmount { get; set; }
		public List<ChargeModel> ChargeModelList { get; set; }
	}

	public class ChargeModel
	{
		public string Key { get; set; }
		public string Title { get; set; }
		public double Amount { get; set; }
	}

	public class MandatoryChargesPopupModel
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string CTA { get; set; }
		public string Type { get; set; }
	}
}
