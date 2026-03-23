namespace TeeTimeWebApp.Models
{
	public class User
	{
		public string Email { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string PhoneNumber { get; set; }
		public string Address { get; set; } = string.Empty;
		public string City { get; set; } = string.Empty;
		public string Province { get; set; } = string.Empty;
		public string PostalCode { get; set; } = string.Empty;
		public int Role { get; set; }
		public List<Warning> Warnings { get; set; } = new List<Warning>();
	}
}
