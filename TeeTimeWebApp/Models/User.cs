namespace TeeTimeWebApp.Models
{
	public class User
	{
		public string Email { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public int PhoneNumber { get; set; }
		public string Role { get; set; } = string.Empty;
	}
}
