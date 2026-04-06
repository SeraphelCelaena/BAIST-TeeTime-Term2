using Microsoft.AspNetCore.Mvc.RazorPages;
public class MembershipApplicationModel : PageModel
{
	private readonly IConfiguration _configuration;

	public MembershipApplicationModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
