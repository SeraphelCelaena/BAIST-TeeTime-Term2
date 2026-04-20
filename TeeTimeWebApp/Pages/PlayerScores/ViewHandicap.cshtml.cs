using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = "Authenticated")]
public class ViewHandicapModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ViewHandicapModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public async Task<IActionResult> OnGet()
	{
		return Page();
	}
}
