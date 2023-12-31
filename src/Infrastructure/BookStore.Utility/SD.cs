﻿namespace BookStore.Utility
{
	public static class SD
	{
		public const string Role_User_Individual = "Individual";
		public const string Role_User_Company = "Company";
		public const string Role_User_Admin = "Admin";
		public const string Role_User_Employee = "Employee";

		public static string StatusPending = "Pending";
		public static string StatusApprove = "Approved";
		public static string StatusInProgress = "Processing";
		public static string StatusShipped = "Shipped";
		public static string StatusCancelled = "Cancelled";
		public static string StatusRefunded = "Refunded";

		public static string PaymentStatusPending = "Pending";
		public static string PaymentStatusApproved = "Approved";
		public static string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
		public static string PaymentStatusRejected = "Rejected";
	}
}