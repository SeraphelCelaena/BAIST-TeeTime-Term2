using Microsoft.AspNetCore.Mvc.RazorPages;

public class ManageUserModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ManageUserModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
