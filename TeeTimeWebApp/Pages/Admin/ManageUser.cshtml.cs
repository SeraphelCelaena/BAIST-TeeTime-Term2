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

	[BindProperty]
	public string EmailEdit { get; set; } = string.Empty;
	[BindProperty]
	public string FirstNameEdit { get; set; } = string.Empty;
	[BindProperty]
	public string LastNameEdit { get; set; } = string.Empty;
	[BindProperty]
	public string PhoneNumberEdit { get; set; } = string.Empty;
	[BindProperty]
	public string AddressEdit { get; set; } = string.Empty;
	[BindProperty]
	public string CityEdit { get; set; } = string.Empty;
	[BindProperty]
	public string ProvinceEdit { get; set; } = string.Empty;
	[BindProperty]
	public string PostalCodeEdit { get; set; } = string.Empty;
	[BindProperty]
	public int RoleEdit { get; set; }

	[BindProperty]
	public string WarningEmail { get; set; } = string.Empty;
	[BindProperty]
	public string WarningReason { get; set; } = string.Empty;
	[BindProperty]
	public DateOnly WarningEndDate { get; set; }

	public async Task<IActionResult> OnGet()
	{
		await GetEmail();

		await GetAllUsers();

		await GetRolesList();

		return Page();
	}

	public async Task<IActionResult> OnPostEditForm()
	{
		await GetEmail();
		await GetAllUsers();
		await GetRolesList();

		SqlConnection EditUserConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand EditUserCommand = new()
		{
			Connection = EditUserConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "EditUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = EmailEdit },
				new SqlParameter("@FirstName", SqlDbType.VarChar, 50) { Value = FirstNameEdit },
				new SqlParameter("@LastName", SqlDbType.VarChar, 50) { Value = LastNameEdit },
				new SqlParameter("@PhoneNumber", SqlDbType.VarChar, 20) { Value = PhoneNumberEdit },
				new SqlParameter("@Address", SqlDbType.VarChar, 200) { Value = AddressEdit },
				new SqlParameter("@City", SqlDbType.VarChar, 50) { Value = CityEdit },
				new SqlParameter("@Province", SqlDbType.VarChar, 50) { Value = ProvinceEdit },
				new SqlParameter("@PostalCode", SqlDbType.VarChar, 10) { Value = PostalCodeEdit },
				new SqlParameter("@RoleID", SqlDbType.Int) { Value = RoleEdit }
			}
		};

		try
		{
			using (EditUserConnection)
			{
				EditUserConnection.Open();

				using (EditUserCommand)
				{
					EditUserCommand.ExecuteNonQuery();
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

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
