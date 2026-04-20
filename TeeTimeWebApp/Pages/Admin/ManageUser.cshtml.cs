using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

[Authorize(Policy = "AdminOnly")]
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
	public List<Warning> WarningList { get; set; } = new List<Warning>();
	public bool ShowWarningList { get; set; }

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

	[BindProperty]
	public string EmailDelete { get; set; } = string.Empty;

	public async Task<IActionResult> OnGet()
	{
		await GetAll();

		return Page();
	}

	public async Task<IActionResult> OnPostEditForm()
	{
		await GetAll();

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

			ViewData["Success"] = "User details updated successfully.";
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while editing the user: {ex.Message}";
		}

		return Page();
	}

	public async Task<IActionResult> OnPostDelete()
	{
		await GetAll();

		SqlConnection DeleteUserConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand DeleteUserCommand = new()
		{
			Connection = DeleteUserConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "DeleteUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = EmailDelete }
			}
		};

		try
		{
			using (DeleteUserConnection)
			{
				DeleteUserConnection.Open();

				using (DeleteUserCommand)
				{
					DeleteUserCommand.ExecuteNonQuery();
				}
			}

			ViewData["Success"] = "User deleted successfully.";
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while deleting the user: {ex.Message}";
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostViewWarnings()
	{
		await GetAll();
		await LoadWarningsForUser(WarningEmail);

		ShowWarningList = true;

		return Page();
	}

	public async Task<IActionResult> OnPostAddWarning()
	{
		await GetAll();

		SqlConnection AddWarningConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand AddWarningCommand = new()
		{
			Connection = AddWarningConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "AddWarningToUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = WarningEmail },
				new SqlParameter("@WarningMessage", SqlDbType.VarChar, 255) { Value = WarningReason },
				new SqlParameter("@WarningStartDate", SqlDbType.Date) { Value = DateOnly.FromDateTime(DateTime.Today) },
				new SqlParameter("@WarningEndDate", SqlDbType.Date) { Value = WarningEndDate }
			}
		};

		try
		{
			using (AddWarningConnection)
			{
				AddWarningConnection.Open();

				using (AddWarningCommand)
				{
					AddWarningCommand.ExecuteNonQuery();
				}
			}

			ViewData["Success"] = "Warning added successfully.";
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while adding the warning: {ex.Message}";
		}

		await LoadWarningsForUser(WarningEmail);
		ShowWarningList = true;

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
			ViewData["Error"] = $"An error occurred while retrieving the user list: {ex.Message}";
		}

		return Page();
	}

	public async Task<IActionResult> GetAll()
	{
		await GetEmail();
		await GetAllUsers();
		await GetRolesList();

		return Page();
	}

	private async Task LoadWarningsForUser(string email)
	{
		WarningList = new List<Warning>();

		if (string.IsNullOrWhiteSpace(email))
		{
			return;
		}

		SqlConnection GetWarningsConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand GetWarningsCommand = new()
		{
			Connection = GetWarningsConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "GetWarningsForUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = email }
			}
		};

		try
		{
			using (GetWarningsConnection)
			{
				GetWarningsConnection.Open();

				using (GetWarningsCommand)
				{
					using (SqlDataReader reader = GetWarningsCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							WarningList.Add(new Warning
							{
								Email = email,
								Id = reader.GetInt32(0),
								Message = reader.GetString(1),
								StartDate = DateOnly.FromDateTime(reader.GetDateTime(2)),
								EndDate = DateOnly.FromDateTime(reader.GetDateTime(3))
							});
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while retrieving warnings for the user: {ex.Message}";
		}
	}
}
