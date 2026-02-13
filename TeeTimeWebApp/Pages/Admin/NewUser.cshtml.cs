using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using System.Data;

public class NewUserModel : PageModel
{
	private readonly IConfiguration _configuration;

	public NewUserModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	[BindProperty]
	public string Email { get; set;} = string.Empty;
	[BindProperty]
	public string Password { get; set;} = string.Empty;
	[BindProperty]
	public string ConfirmPassword { get; set;} = string.Empty;
	[BindProperty]
	public string FirstName { get; set;} = string.Empty;
	[BindProperty]
	public string LastName { get; set;} = string.Empty;
	[BindProperty]
	public int PhoneNumber { get; set;}
	[BindProperty]
	public string Address { get; set;} = string.Empty;
	[BindProperty]
	public string City { get; set;} = string.Empty;
	[BindProperty]
	public string Province { get; set;} = string.Empty;
	[BindProperty]
	public string PostalCode { get; set;} = string.Empty;
	[BindProperty]
	public string Role { get; set;}

	public List<SelectListItem> RolesList { get; set;} = new List<SelectListItem>();

	public async Task<IActionResult> OnGet()
	{
		await GetRolesList();

		return Page();
	}

	public async Task<IActionResult> OnPostSubmit()
	{
		string RegexEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
		string RegexPostalCode = @"^[A-Z]\d[A-Z]\d[A-Z]\d$";

		await GetRolesList();

		if (Email == null || Password == null || FirstName == null || LastName == null || Address == null || City == null || Province == null || PostalCode == null)
		{
			ViewData["Error"] = "Please fill in all fields.";
			return Page();
		}

		if (!Regex.IsMatch(Email, RegexEmail))
		{
			ViewData["Error"] = "Please enter a valid email address.";
			return Page();
		}

		if (PhoneNumber == 0 || PhoneNumber.ToString().Length != 10)
		{
			ViewData["Error"] = "Please enter a valid phone number.";
			return Page();
		}

		if (!Regex.IsMatch(PostalCode, RegexPostalCode))
		{
			ViewData["Error"] = "Please enter a valid postal code.";
			return Page();
		}

		if (Password != ConfirmPassword)
		{
			ViewData["Error"] = "Passwords do not match.";
			return Page();
		}

		SqlConnection RegisterConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand RegisterCommand = new()
		{
			Connection = RegisterConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "RegisterUser",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar) { Value = Email },
				new SqlParameter("@Password", SqlDbType.VarChar) { Value = Password },
				new SqlParameter("@FirstName", SqlDbType.VarChar) { Value = FirstName },
				new SqlParameter("@LastName", SqlDbType.VarChar) { Value = LastName },
				new SqlParameter("@PhoneNumber", SqlDbType.VarChar) { Value = PhoneNumber },
				new SqlParameter("@Address", SqlDbType.VarChar) { Value = Address },
				new SqlParameter("@City", SqlDbType.VarChar) { Value = City },
				new SqlParameter("@Province", SqlDbType.VarChar) { Value = Province },
				new SqlParameter("@PostalCode", SqlDbType.VarChar) { Value = PostalCode },
				new SqlParameter("@RoleID", SqlDbType.Int) { Value = int.Parse(Role) }
			}
		};

		using (RegisterConnection)
		{
			RegisterConnection.Open();

			using (RegisterCommand)
			{
				RegisterCommand.ExecuteNonQuery();
			}
		}

		return Page();
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
}
