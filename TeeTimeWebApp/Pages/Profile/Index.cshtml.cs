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
		if (EmailEdit != EmailEditConfirm)
		{
			ViewData["Error"] = "New email and confirmation do not match.";
			return Page();
		}

		SqlConnection ChangeEmailConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand ChangeEmailCommand = new()
		{
			Connection = ChangeEmailConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "UpdateEmail",
			Parameters =
			{
				new SqlParameter("@CurrentEmail", SqlDbType.VarChar, 100) { Value = User.FindFirstValue(ClaimTypes.Email) },
				new SqlParameter("@NewEmail", SqlDbType.VarChar, 100) { Value = EmailEdit }
			}
		};

		try
		{
			using (ChangeEmailConnection)
			{
				ChangeEmailConnection.Open();
				using (ChangeEmailCommand)
				{
					ChangeEmailCommand.ExecuteNonQuery();
				}
			}

			var identity = User.Identity as ClaimsIdentity;
			if (identity != null)
			{
				var emailClaim = identity.FindFirst(ClaimTypes.Email);
				if (emailClaim != null)
				{
					identity.RemoveClaim(emailClaim);
					identity.AddClaim(new Claim(ClaimTypes.Email, EmailEdit));
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while changing email: {ex.Message}";
			return Page();
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangeName()
	{
		SqlConnection ChangeNameConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand ChangeNameCommand = new()
		{
			Connection = ChangeNameConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "UpdateName",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = User.FindFirstValue(ClaimTypes.Email) },
				new SqlParameter("@NewFirstName", SqlDbType.VarChar, 50) { Value = FirstNameEdit },
				new SqlParameter("@NewLastName", SqlDbType.VarChar, 50) { Value = LastNameEdit }
			}
		};

		try
		{
			using (ChangeNameConnection)
			{
				ChangeNameConnection.Open();
				using (ChangeNameCommand)
				{
					ChangeNameCommand.ExecuteNonQuery();
				}
			}

			var identity = User.Identity as ClaimsIdentity;
			if (identity != null)
			{
				var firstNameClaim = identity.FindFirst(ClaimTypes.GivenName);
				var lastNameClaim = identity.FindFirst(ClaimTypes.Surname);

				if (firstNameClaim != null && lastNameClaim != null)
				{
					identity.RemoveClaim(firstNameClaim);
					identity.RemoveClaim(lastNameClaim);
					identity.AddClaim(new Claim(ClaimTypes.GivenName, FirstNameEdit));
					identity.AddClaim(new Claim(ClaimTypes.Surname, LastNameEdit));
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while changing name: {ex.Message}";
			return Page();
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangePhoneNumber()
	{
		SqlConnection ChangePhoneNumberConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand ChangePhoneNumberCommand = new()
		{
			Connection = ChangePhoneNumberConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "UpdatePhoneNumber",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = User.FindFirstValue(ClaimTypes.Email) },
				new SqlParameter("@NewPhoneNumber", SqlDbType.VarChar, 10) { Value = PhoneNumberEdit }
			}
		};

		try
		{
			using (ChangePhoneNumberConnection)
			{
				ChangePhoneNumberConnection.Open();
				using (ChangePhoneNumberCommand)
				{
					ChangePhoneNumberCommand.ExecuteNonQuery();
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while changing phone number: {ex.Message}";
			return Page();
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangeAddress()
	{
		SqlConnection ChangeAddressConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand ChangeAddressCommand = new()
		{
			Connection = ChangeAddressConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "UpdateAddress",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = User.FindFirstValue(ClaimTypes.Email) },
				new SqlParameter("@NewAddress", SqlDbType.VarChar, 200) { Value = AddressEdit },
				new SqlParameter("@NewCity", SqlDbType.VarChar, 100) { Value = CityEdit },
				new SqlParameter("@NewProvince", SqlDbType.VarChar, 100) { Value = ProvinceEdit },
				new SqlParameter("@NewPostalCode", SqlDbType.VarChar, 6) { Value = PostalCodeEdit }
			}
		};

		try
		{
			using (ChangeAddressConnection)
			{
				ChangeAddressConnection.Open();
				using (ChangeAddressCommand)
				{
					ChangeAddressCommand.ExecuteNonQuery();
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while changing address: {ex.Message}";
			return Page();
		}

		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostChangePassword()
	{
		SqlConnection ChangePasswordConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand ChangePasswordCommand = new()
		{
			Connection = ChangePasswordConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "UpdatePassword",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = User.FindFirstValue(ClaimTypes.Email) },
				new SqlParameter("@CurrentPassword", SqlDbType.VarChar, 255) { Value = CurrentPasswordEdit },
				new SqlParameter("@NewPassword", SqlDbType.VarChar, 255) { Value = NewPasswordEdit }
			}
		};

		try
		{
			using (ChangePasswordConnection)
			{
				ChangePasswordConnection.Open();
				using (ChangePasswordCommand)
				{
					ChangePasswordCommand.ExecuteNonQuery();
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while changing password: {ex.Message}";
			return Page();
		}

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
