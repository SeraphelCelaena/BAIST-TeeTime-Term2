using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace TeeTimeWebApp.Pages;

public class LoginModel : PageModel
{
	[BindProperty]
	public string Username { get; set; } = string.Empty;
	[BindProperty]
	public string Password { get; set; } = string.Empty;

    public void OnGet()
    {
    }

	public void OnPost()
	{
		SqlConnection SqlConnection = new()
		{
			ConnectionString = @"server=localhost;database=TeeTimeDB;"
		};
		SqlConnection.Open();
	}
}
