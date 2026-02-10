using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;
using TeeTimeWebApp.Functions;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

public class BookModel : PageModel
{
	private readonly IConfiguration _configuration;

	public BookModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Message { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;

	[BindProperty]
	public DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
	[BindProperty]
	public TimeOnly SelectedTime { get; set; } = TimeOnly.MinValue;
	public List<UsedTeeTime> UsedTeeTimes { get; set; } = new List<UsedTeeTime>();
	public bool ValidDate { get; set; } = false;

	public List<SelectListItem> AvailableTeeTimes { get; set; } = new List<SelectListItem>();

	public async Task<IActionResult> OnGet()
	{
		var RoleClaim = User.FindFirstValue(ClaimTypes.Role);
		if (RoleClaim == null)
		{
			return RedirectToPage("/Login");
		}
		else
		{
			Role = RoleClaim;
			return Page();
		}
	}

	public async Task<IActionResult> OnPostVerifyDate()
	{
		if (SelectedDate == default || SelectedDate < DateOnly.FromDateTime(DateTime.Now))
		{
			ValidDate = false;
			Message = "Please select a valid date.";
			return Page();
		}

		if (SelectedDate > DateOnly.FromDateTime(DateTime.Now.AddDays(14)))
		{
			ValidDate = false;
			Message = "Please select a date within the next 2 weeks.";
			return Page();
		}

		var RoleClaim = User.FindFirstValue(ClaimTypes.Role);
		if (RoleClaim == null)
		{
			return RedirectToPage("/Login");
		}
		else
		{
			Role = RoleClaim;
		}

		SqlConnection SqlConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand VerifyDateCommand = new()
		{
			Connection = SqlConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "CheckTeeTimeOnDate",
			Parameters =
			{
				new SqlParameter("@Date", SqlDbType.Date) { Value = SelectedDate.ToDateTime(TimeOnly.MinValue) }
			}
		};

		using (SqlConnection)
		{
			SqlConnection.Open();

			using (VerifyDateCommand)
			{
				using (SqlDataReader VerifyDateDataReader = VerifyDateCommand.ExecuteReader())
				{
					if (VerifyDateDataReader.HasRows)
					{
						while (VerifyDateDataReader.Read())
						{
							UsedTeeTimes.Add(new UsedTeeTime
							{
								TeeTime = TimeOnly.FromTimeSpan(VerifyDateDataReader.GetTimeSpan(1)),
								Count = VerifyDateDataReader.GetInt32(2)
							});
						}
					}
				}
			}
		}

		AvailableTeeTimes = GetTeeTimes.GetAvailableTeeTimes(SelectedDate, Role);

		ValidDate = true;
		return Page();
	}

	public async Task<IActionResult> OnPostCancel()
	{
		ValidDate = false;
		Message = "OnCancel";
		UsedTeeTimes = new List<UsedTeeTime>();
		AvailableTeeTimes = new List<SelectListItem>();

		return Page();
	}

	public async Task<IActionResult> OnPostBook()
	{
		return Page();
	}
}
