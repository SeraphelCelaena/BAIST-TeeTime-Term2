namespace TeeTimeWebApp.Models
{
	public class TeeTime
	{
		public int TeeTimeID { get; set; }
		public string Email { get; set; } = string.Empty;
		public DateOnly Date { get; set; }
		public TimeOnly StartTime { get; set; }
		public int Count { get; set; }
		public int ConfirmedCount { get; set; }
		public bool Confirmed { get; set; }
	}
}
