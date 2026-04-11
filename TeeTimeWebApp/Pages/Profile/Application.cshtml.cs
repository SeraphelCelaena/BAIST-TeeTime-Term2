using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize(Policy = "Authenticated")]
public class MembershipApplicationModel : PageModel
{
	private readonly IConfiguration _configuration;

	public MembershipApplicationModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Role { get; set; } = string.Empty;

	public async Task<IActionResult> OnGet()
	{
		await GetUserFromClaims();

		return Page();
	}

	public async Task<IActionResult> GetUserFromClaims()
	{
		var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
		if (roleClaim != null)
		{
			Role = roleClaim.Value;
			return Page();
		}
		else
		{
			return RedirectToPage("/Error");
		}
	}
}
