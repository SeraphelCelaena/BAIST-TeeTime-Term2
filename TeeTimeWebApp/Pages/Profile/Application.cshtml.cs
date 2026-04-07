using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = "Authenticated")]
public class MembershipApplicationModel : PageModel
{
	private readonly IConfiguration _configuration;

	public MembershipApplicationModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
