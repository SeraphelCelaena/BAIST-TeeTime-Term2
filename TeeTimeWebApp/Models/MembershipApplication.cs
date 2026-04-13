namespace TeeTimeWebApp.Models
{
	public class MembershipApplication
	{
		public int MembershipApplicationID { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string Province { get; set; } = string.Empty;
		public string PostalCode { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string Alt_PhoneNumber { get; set; } = string.Empty;
		public DateOnly DateOfBirth { get; set; }
		public string Occupation { get; set; } = string.Empty;
		public string CompanyName { get; set; } = string.Empty;
		public string CompanyAddress { get; set; } = string.Empty;
		public string CompanyPostalCode { get; set; } = string.Empty;
		public string CompanyPhoneNumber { get; set; } = string.Empty;
		public DateTime DateApplied { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
