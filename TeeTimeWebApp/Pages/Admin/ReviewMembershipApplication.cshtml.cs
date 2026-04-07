using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

[Authorize(Policy = "AdminOnly")]
public class ReviewMembershipApplicationModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ReviewMembershipApplicationModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
