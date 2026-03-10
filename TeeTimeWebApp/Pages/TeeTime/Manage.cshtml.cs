using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ManageModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ManageModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Role { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public List<TeeTime> TeeTimesList { get; set; } = new List<TeeTime>();
	[BindProperty]
	public int TeeTimeIDEdit { get; set; }
	[BindProperty]
	public DateOnly DateEdit { get; set; }
	[BindProperty]
	public TimeOnly StartTimeEdit { get; set; }
	[BindProperty]
	public int CountEdit { get; set; }
	[BindProperty]
	public bool ConfirmedEdit { get; set; }
	[BindProperty]
	public int TeeTimeIDDelete { get; set; }

	public async Task<IActionResult> OnGet()
	{
		await GetEmail();

		SqlConnection GetTeeTimesConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand GetTeeTimesCommand = new()
		{
			Connection = GetTeeTimesConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "GetTeeTimesForUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar) { Value = Email }
			}
		};

		try
		{
			using (GetTeeTimesConnection)
			{
				GetTeeTimesConnection.Open();

				using (GetTeeTimesCommand)
				{
					using (SqlDataReader GetTeeTimesReader = GetTeeTimesCommand.ExecuteReader())
					{
						if (GetTeeTimesReader.HasRows)
						{
							while (GetTeeTimesReader.Read())
							{
								TeeTimesList.Add(new TeeTime
								{
									TeeTimeID = GetTeeTimesReader.GetInt32(0),
									Email = GetTeeTimesReader.GetString(1),
									Date = DateOnly.FromDateTime(GetTeeTimesReader.GetDateTime(2)),
									StartTime = TimeOnly.FromTimeSpan(GetTeeTimesReader.GetTimeSpan(3)),
									Count = GetTeeTimesReader.GetInt32(4),
									Confirmed = GetTeeTimesReader.GetBoolean(5)
								});
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while checking tee times: {ex.Message}";
			return Page();
		}

		return Page();
	}

	public async Task<IActionResult> OnPostDelete()
	{
		await GetEmail();

		

		return Page();
	}
	public async Task<IActionResult> GetEmail()
	{
		var RoleClaim = User.FindFirstValue(ClaimTypes.Role);
		var EmailClaim = User.FindFirstValue(ClaimTypes.Email);

		if (EmailClaim == null || RoleClaim == null)
		{
			return RedirectToPage("/Login");
		}
		else
		{
			Email = EmailClaim;
			Role = RoleClaim;
			return Page();
		}
	}
}
