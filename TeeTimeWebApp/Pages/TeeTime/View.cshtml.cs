using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

public class ViewModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ViewModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Email { get; set; } = string.Empty;
	public List<TeeTime> TeeTimesList { get; set; } = new List<TeeTime>();

	public async Task<IActionResult> OnGet()
	{
		await GetEmail();

		await GetAllTeeTimes();

		return Page();
	}

	public async Task<IActionResult> GetEmail()
	{
		var EmailClaim = User.FindFirstValue(ClaimTypes.Email);

		if (EmailClaim == null)
		{
			return RedirectToPage("/Login");
		}
		else
		{
			Email = EmailClaim;
			return Page();
		}
	}

	public async Task<IActionResult> GetAllTeeTimes()
	{
		SqlConnection GetTeeTimesConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand GetTeeTimesCommand = new()
		{
			Connection = GetTeeTimesConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "GetAllTeeTimes"
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
									Date = DateOnly.FromDateTime(GetTeeTimesReader.GetDateTime(1)),
									StartTime = TimeOnly.FromTimeSpan(GetTeeTimesReader.GetTimeSpan(2)),
									Count = GetTeeTimesReader.GetInt32(3),
									ConfirmedCount = GetTeeTimesReader.GetInt32(4),
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
		}

		return Page();
	}
}
