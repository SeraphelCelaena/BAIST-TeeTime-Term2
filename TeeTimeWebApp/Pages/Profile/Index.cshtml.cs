using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Data;

[Authorize(Policy = "Authenticated")]
public class ProfileModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ProfileModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Email { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string Province { get; set; } = string.Empty;
	public string PostalCode { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;

	[BindProperty]
	public string EmailEdit { get; set; } = string.Empty;
	[BindProperty]
	public string EmailEditConfirm { get; set; } = string.Empty;
	[BindProperty]
	public string CurrentPasswordEdit { get; set; } = string.Empty;
	[BindProperty]
	public string NewPasswordEdit { get; set; } = string.Empty;
	[BindProperty]
	public string NewPasswordEditConfirm { get; set; } = string.Empty;
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

	public async Task<IActionResult> OnGet()
	{
		await GetUserDataFromClaims();
		await GetProfileProperties();

		return Page();
	}

	public async Task<IActionResult> OnPostChangeEmail()
	{
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangeName()
	{
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangePhoneNumber()
	{
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangeAddress()
	{
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangePassword()
	{
		return RedirectToPage();
	}

	public async Task<IActionResult> GetProfileProperties()
	{
		SqlConnection GetProfilePropertiesConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand GetProfilePropertiesCommand = new()
		{
			Connection = GetProfilePropertiesConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "GetUserInformation",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = User.FindFirstValue(ClaimTypes.Email) }
			}
		};

		try
		{
			using (GetProfilePropertiesConnection)
			{
				GetProfilePropertiesConnection.Open();
				using (GetProfilePropertiesCommand)
				{
					using (SqlDataReader GetProfileReader = GetProfilePropertiesCommand.ExecuteReader())
					{
						if (GetProfileReader.HasRows)
						{
							while (GetProfileReader.Read())
							{
								Email = GetProfileReader.GetString(0);
								FirstName = GetProfileReader.GetString(1);
								LastName = GetProfileReader.GetString(2);
								PhoneNumber = GetProfileReader.GetString(3);
								Address = GetProfileReader.GetString(4);
								City = GetProfileReader.GetString(5);
								Province = GetProfileReader.GetString(6);
								PostalCode = GetProfileReader.GetString(7);
								Role = GetProfileReader.GetString(8);

							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while retrieving profile information: {ex.Message}";
			return Page();
		}

		return Page();
	}

	public async Task<IActionResult> GetUserDataFromClaims()
	{
		var EmailClaim = User.FindFirstValue(ClaimTypes.Email);
		var FirstNameClaim = User.FindFirstValue(ClaimTypes.GivenName);
		var LastNameClaim = User.FindFirstValue(ClaimTypes.Surname);
		var RoleClaim = User.FindFirstValue(ClaimTypes.Role);

		if (RoleClaim == null || EmailClaim == null || FirstNameClaim == null || LastNameClaim == null)
		{
			return RedirectToPage("/Login");
		}
		else
		{
			Role = RoleClaim;
			Email = EmailClaim;
			FirstName = FirstNameClaim;
			LastName = LastNameClaim;
			return Page();
		}
	}
}
