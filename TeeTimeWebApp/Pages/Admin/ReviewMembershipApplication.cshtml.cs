using Microsoft.AspNetCore.Mvc.RazorPages;

public class ReviewMembershipApplicationModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ReviewMembershipApplicationModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
