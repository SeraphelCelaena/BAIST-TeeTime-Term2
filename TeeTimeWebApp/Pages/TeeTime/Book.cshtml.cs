using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

public class BookModel : PageModel
{
	private readonly IConfiguration _configuration;

	public BookModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	[BindProperty]
	public DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}
