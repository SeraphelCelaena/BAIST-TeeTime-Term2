using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Policy = "Authenticated")]
public class RecordScoreModel : PageModel
{
	private readonly IConfiguration _configuration;

	public RecordScoreModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

}
