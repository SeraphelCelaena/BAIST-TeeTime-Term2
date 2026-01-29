using Microsoft.AspNetCore.Mvc.RazorPages;

public class BookModel : PageModel
{
	private readonly IConfiguration _configuration;

	public BookModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}
}
