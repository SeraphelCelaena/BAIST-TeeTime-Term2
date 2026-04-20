namespace TeeTimeWebApp.Models
{
	public class GolfScore
	{
		public int ID { get; set; }
		public string Email { get; set; } = string.Empty;
		public DateOnly DatePlayed { get; set; }
		public string CourseName { get; set; } = string.Empty;
		public decimal CourseRating { get; set; }
		public int SlopeRating { get; set; }
		public int Score1 { get; set; }
		public int Score2 { get; set; }
		public int Score3 { get; set; }
		public int Score4 { get; set; }
		public int Score5 { get; set; }
		public int Score6 { get; set; }
		public int Score7 { get; set; }
		public int Score8 { get; set; }
		public int Score9 { get; set; }
		public int Score10 { get; set; }
		public int Score11 { get; set; }
		public int Score12 { get; set; }
		public int Score13 { get; set; }
		public int Score14 { get; set; }
		public int Score15 { get; set; }
		public int Score16 { get; set; }
		public int Score17 { get; set; }
		public int Score18 { get; set; }
		public int TotalScore => Score1 + Score2 + Score3 + Score4 + Score5 + Score6 + Score7 + Score8 + Score9 + Score10 + Score11 + Score12 + Score13 + Score14 + Score15 + Score16 + Score17 + Score18;
	}
}
