using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

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
		return Page();
	}
}
