using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = "Authenticated")]
public class ProfileModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ProfileModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
