using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ManageUserModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ManageUserModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Role { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;

	public List<User> UserList { get; set; } = new List<User>();
	public List<SelectListItem> RolesList { get; set; } = new List<SelectListItem>();

	public async Task<IActionResult> OnGet()
	{
		await GetEmail();

		await GetAllUsers();

		await GetRolesList();

		return Page();
	}

	public async Task<IActionResult> GetEmail()
	{
		var RoleClaim = User.FindFirstValue(ClaimTypes.Role);
		var EmailClaim = User.FindFirstValue(ClaimTypes.Email);

		if (EmailClaim == null || RoleClaim == null)
		{
			return RedirectToPage("/Login");
		}
		else
		{
			Email = EmailClaim;
			Role = RoleClaim;
			return Page();
		}
	}

	public async Task<IActionResult> GetRolesList()
	{
		SqlConnection GetRolesConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand GetRolesCommand = new()
		{
			Connection = GetRolesConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "GetAllRoles"
		};

		using (GetRolesConnection)
		{
			GetRolesConnection.Open();

			using (GetRolesCommand)
			{
				SqlDataReader reader = GetRolesCommand.ExecuteReader();

				while (reader.Read())
				{
					RolesList.Add(new SelectListItem
					{
						Value = reader["RoleID"].ToString(),
						Text = reader["RoleName"].ToString()
					});
				}
			}
		}

		return Page();
	}

	public async Task<IActionResult> GetAllUsers()
	{
		SqlConnection GetUserListConnection = new SqlConnection
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand GetUserListCommand = new SqlCommand
		{
			Connection = GetUserListConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "GetAllUsers"
		};

		try
		{
			using (GetUserListConnection)
			{
				GetUserListConnection.Open();

				using (GetUserListCommand)
				{
					using (SqlDataReader GetUserListReader = GetUserListCommand.ExecuteReader())
					{
						while (GetUserListReader.Read())
						{
							UserList.Add(new User
							{
								Email = GetUserListReader.GetString(0),
								FirstName = GetUserListReader.GetString(1),
								LastName = GetUserListReader.GetString(2),
								PhoneNumber = GetUserListReader.GetString(3),
								Address = GetUserListReader.GetString(4),
								City = GetUserListReader.GetString(5),
								Province = GetUserListReader.GetString(6),
								PostalCode = GetUserListReader.GetString(7),
								Role = GetUserListReader.GetInt32(8)
							});
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

		return Page();
	}
}
