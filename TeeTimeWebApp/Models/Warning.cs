namespace TeeTimeWebApp.Models
{
	public class Warning
	{
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public DateOnly StartDate { get; set; }
		public DateOnly EndDate { get; set; }
	}
}
