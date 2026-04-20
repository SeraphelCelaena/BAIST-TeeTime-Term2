using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeeTimeWebApp.Functions;
using Microsoft.AspNetCore.Authorization;

[Authorize(Policy = "PayingMember")]
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
	public string StartTimeEdit { get; set; } = string.Empty;
	[BindProperty]
	public int CountEdit { get; set; }
	[BindProperty]
	public bool ConfirmedEdit { get; set; }
	[BindProperty]
	public int TeeTimeIDDelete { get; set; }
	[BindProperty]
	public int TeeTimeIDPopulate { get; set; }
	public List<SelectListItem> AvailableTeeTimes { get; set; } = new List<SelectListItem>();

	public async Task<IActionResult> OnGet()
	{
		await GetEmail();

		await GetAllTeeTimes();

		return Page();
	}

	public async Task<IActionResult> OnPostDelete()
	{
		await GetEmail();
		await GetAllTeeTimes();

		SqlConnection DeleteTeeTimeConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand DeleteTeeTimeCommand = new()
		{
			Connection = DeleteTeeTimeConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "DeleteTeeTime",
			Parameters =
			{
				new SqlParameter("@TeeTimeID", SqlDbType.Int) { Value = TeeTimeIDDelete }
			}
		};

		try
		{
			using (DeleteTeeTimeConnection)
			{
				DeleteTeeTimeConnection.Open();

				using (DeleteTeeTimeCommand)
				{
					DeleteTeeTimeCommand.ExecuteNonQuery();
				}
			}

			ViewData["Success"] = "Tee time deleted successfully.";
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while deleting the tee time: {ex.Message}";
			return Page();
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostEdit()
	{
		await GetEmail();
		await GetAllTeeTimes();

		SqlConnection EditTeeTimeConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		TimeOnly parsedStartTime = TimeOnly.Parse(StartTimeEdit);

		SqlCommand EditTeeTimeCommand = new()
		{
			Connection = EditTeeTimeConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "UpdateTeeTimeForUser",
			Parameters =
			{
				new SqlParameter("@TeeTimeID", SqlDbType.Int) { Value = TeeTimeIDEdit },
				new SqlParameter("@Email", SqlDbType.VarChar) { Value = Email },
				new SqlParameter("@Date", SqlDbType.Date) { Value = DateEdit },
				new SqlParameter("@StartTime", SqlDbType.Time) { Value = parsedStartTime },
				new SqlParameter("@Count", SqlDbType.Int) { Value = CountEdit },
				new SqlParameter("@Confirmed", SqlDbType.Bit) { Value = ConfirmedEdit }
			}
		};

		try
		{
			using (EditTeeTimeConnection)
			{
				EditTeeTimeConnection.Open();

				using (EditTeeTimeCommand)
				{
					EditTeeTimeCommand.ExecuteNonQuery();
				}
			}

			ViewData["Success"] = "Tee time updated successfully.";
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while editing the tee time: {ex.Message}";
			return Page();
		}

		return RedirectToPage();
	}

// I had AI help me with this help populate the edit form while keepign the client side from refreshing and losing the data
	public async Task<IActionResult> OnPostEditPopulate()
	{
		await GetEmail();
		await GetAllTeeTimes();

		// Find the tee time to edit
		var teeTimeToEdit = TeeTimesList.FirstOrDefault(t => t.TeeTimeID == TeeTimeIDPopulate);
		if (teeTimeToEdit != null)
		{
			TeeTimeIDEdit = teeTimeToEdit.TeeTimeID;
			DateEdit = teeTimeToEdit.Date;
			StartTimeEdit = teeTimeToEdit.StartTime.ToString("HH:mm");
			CountEdit = teeTimeToEdit.Count;
			ConfirmedEdit = teeTimeToEdit.Confirmed;

			AvailableTeeTimes = GetTeeTimes.GetAvailableTeeTimes(DateEdit, Role);

			// Ensure the current start time is in the list, even if not normally available
			if (!AvailableTeeTimes.Any(item => item.Value == StartTimeEdit))
			{
				AvailableTeeTimes.Add(new SelectListItem
				{
					Value = StartTimeEdit,
					Text = TimeOnly.Parse(StartTimeEdit).ToString("hh:mm")
				});
			}
		}
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
		}

		return Page();
	}
}
