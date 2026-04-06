using Microsoft.AspNetCore.Mvc.RazorPages;

public class ProfileModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ProfileModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
