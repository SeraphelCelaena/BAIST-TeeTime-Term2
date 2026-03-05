using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

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
									Date = DateOnly.FromDateTime(GetTeeTimesReader.GetDateTime(1)),
									StartTime = TimeOnly.FromTimeSpan(GetTeeTimesReader.GetTimeSpan(2)),
									Confirmed = GetTeeTimesReader.GetBoolean(3)
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
}
