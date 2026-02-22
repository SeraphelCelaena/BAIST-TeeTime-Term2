using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TeeTimeWebApp.Pages;

public class LoginModel : PageModel
{
	private readonly IConfiguration _configuration;

	public LoginModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Message { get; set; } = string.Empty;
	[BindProperty]
	public string Email { get; set; } = string.Empty;
	[BindProperty]
	public string Password { get; set; } = string.Empty;

	public async Task<IActionResult> OnPostAsync()
	{
		if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
		{
			Message = "Please enter both email and password.";
			return Page();
		}

		SqlConnection SqlConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand AttemptLoginCommand = new()
		{
			Connection = SqlConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "LoginUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = Email },
				new SqlParameter("@Password", SqlDbType.VarChar, 100) { Value = Password}
			}
		};

		try
		{
			using (SqlConnection)
			{
				SqlConnection.Open();

				using (AttemptLoginCommand)
				{
					using (SqlDataReader LoginDataReader = AttemptLoginCommand.ExecuteReader())
					{

						if (LoginDataReader.HasRows)
						{
							while (LoginDataReader.Read())
							{
								var claimsList = new List<Claim>
								{
									new Claim(ClaimTypes.Email, Email),
									new Claim(ClaimTypes.GivenName, LoginDataReader["FirstName"].ToString()!),
									new Claim(ClaimTypes.Surname, LoginDataReader["LastName"].ToString()!),
									new Claim(ClaimTypes.MobilePhone, LoginDataReader["PhoneNumber"].ToString()!),
									new Claim(ClaimTypes.Role, LoginDataReader["RoleName"].ToString()!)
								};

								var authProperties = new AuthenticationProperties
								{
									IsPersistent = true,
									ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
								};

								var userIdentity = new ClaimsIdentity(claimsList, CookieAuthenticationDefaults.AuthenticationScheme);
								var userPrincipal = new ClaimsPrincipal(userIdentity);

								await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authProperties);

								return RedirectToPage("/Index");
							}
						}

					}
				}
			}
		}
		catch (Exception ex)
		{
			Message = $"An error occurred during login: {ex.Message}";
		}

		return Page();
	}
}
