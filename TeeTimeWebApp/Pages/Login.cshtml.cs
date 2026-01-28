using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

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
			ConnectionString = @"Data Source=localhost;Initial Catalog=TeeTimeDB;Integrated Security=True;Trust Server Certificate=True"
		};

		SqlCommand AttemptLoginCommand = new()
		{
			Connection = SqlConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "LoginUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = Username },
				new SqlParameter("@Password", SqlDbType.VarChar, 100) { Value = Password}
			}
		};

		using (SqlConnection)
		{
			SqlConnection.Open();

			using (AttemptLoginCommand)
			{
				SqlDataReader LoginDataReader = AttemptLoginCommand.ExecuteReader();

				if (LoginDataReader.HasRows)
				{

				}
			}
		}
	}
}
