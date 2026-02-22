using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Data;

public class RequestStandingTeeTimeModel : PageModel
{
	private readonly IConfiguration _configuration;

	public RequestStandingTeeTimeModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Email { get; set; } = string.Empty;
	[BindProperty]
	public string Member2 { get; set; } = string.Empty;
	[BindProperty]
	public string Member3 { get; set; } = string.Empty;
	[BindProperty]
	public string Member4 { get; set; } = string.Empty;
	[BindProperty]
	public int DayOfWeek { get; set; }
	[BindProperty]
	public DateOnly StartDate { get; set; }
	[BindProperty]
	public DateOnly EndDate { get; set; }
	[BindProperty]
	public TimeOnly PreferredTime { get; set; }
	[BindProperty]
	public int NumberOfCarts { get; set; }

	public async Task<IActionResult> OnGet()
	{
		await GetEmail();

		return Page();
	}

	public async Task<IActionResult> OnPostSubmit()
	{
		if (Email == null || Member2 == null || Member3 == null || Member4 == null || PreferredTime == TimeOnly.MinValue || NumberOfCarts <= 0)
		{
			ViewData["Error"] = "Please fill in all fields.";
			return Page();
		}

		if (StartDate < DateOnly.FromDateTime(DateTime.Now) || EndDate < DateOnly.FromDateTime(DateTime.Now))
		{
			ViewData["Error"] = "Dates must be in the future.";
			return Page();
		}

		if (EndDate < StartDate)
		{
			ViewData["Error"] = "End date must be after start date.";
			return Page();
		}

		SqlConnection AddStandingTeeTimeConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand AddStandingTeeTimeCommand = new()
		{
			Connection = AddStandingTeeTimeConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "AddStandingTeeTime",
			Parameters =
			{
				new SqlParameter("@StakeholderEmail", SqlDbType.VarChar) { Value = Email },
				new SqlParameter("@DayOfWeek", SqlDbType.Int) { Value = DayOfWeek },
				new SqlParameter("@StartDate", SqlDbType.Date) { Value = StartDate.ToDateTime(TimeOnly.MinValue) },
				new SqlParameter("@EndDate", SqlDbType.Date) { Value = EndDate.ToDateTime(TimeOnly.MinValue) },
				new SqlParameter("@PreferredTime", SqlDbType.Time) { Value = PreferredTime },
				new SqlParameter("@NumberOfCarts", SqlDbType.Int) { Value = NumberOfCarts },
				new SqlParameter("@TeeTimeIDReturn", SqlDbType.Int) { Direction = ParameterDirection.Output }
			}
		};

		try
		{
			using (AddStandingTeeTimeConnection)
			{
				AddStandingTeeTimeConnection.Open();

				using (AddStandingTeeTimeCommand)
				{
					AddStandingTeeTimeCommand.ExecuteNonQuery();
					int StandingTeeTimeID = (int)AddStandingTeeTimeCommand.Parameters["@TeeTimeIDReturn"].Value;

					if (StandingTeeTimeID > 0)
					{
						SqlCommand AddStandingTeeTimeMembersCommand = new()
						{
							Connection = AddStandingTeeTimeConnection,
							CommandType = CommandType.StoredProcedure,
							CommandText = "AddStandingTeeTimeConfirmation",
							Parameters =
							{
								new SqlParameter("@StandingTeeTimeID", SqlDbType.Int) { Value = StandingTeeTimeID },
								new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = Email },
								new SqlParameter("@Date", SqlDbType.Date) { Value = StartDate.ToDateTime(TimeOnly.MinValue) },
								new SqlParameter("@Confirmed", SqlDbType.Bit) { Value = false }
							}
						};
						AddStandingTeeTimeMembersCommand.Parameters.Add(new SqlParameter("@StandingTeeTimeID", SqlDbType.Int) { Value = StandingTeeTimeID });

						using (AddStandingTeeTimeMembersCommand)
						{
							AddStandingTeeTimeMembersCommand.ExecuteNonQuery();
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while submitting the request: {ex.Message}";
		}

		return Page();
	}

	public async Task<IActionResult> GetEmail()
	{
		var Role = User.FindFirstValue(ClaimTypes.Role);
		if (Role != "Stakeholder")
		{
			return RedirectToPage("/Index");
		}

		var TempEmail = User.FindFirstValue(ClaimTypes.Email);
		if (TempEmail != null)
		{
			Email = TempEmail;
		}
		else
		{
			return RedirectToPage("/Login");
		}

		return Page();
	}
}
