using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using System.Data;

[Authorize(Policy = "Authenticated")]
public class MembershipApplicationModel : PageModel
{
	private readonly IConfiguration _configuration;

	public MembershipApplicationModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string Role { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;

	[BindProperty]
	public string? AlternativePhoneNumber { get; set; } = string.Empty;
	[BindProperty]
	public string Address { get; set; } = string.Empty;
	[BindProperty]
	public string City { get; set; } = string.Empty;
	[BindProperty]
	public string Province { get; set; } = string.Empty;
	[BindProperty]
	public string PostalCode { get; set; } = string.Empty;
	[BindProperty]
	public DateOnly DateOfBirth { get; set; }
	[BindProperty]
	public string Occupation { get; set; } = string.Empty;
	[BindProperty]
	public string? CompanyName { get; set; } = string.Empty;
	[BindProperty]
	public string? CompanyAddress { get; set; } = string.Empty;
	[BindProperty]
	public string? CompanyPostalCode { get; set; } = string.Empty;
	[BindProperty]
	public string? CompanyPhoneNumber { get; set; } = string.Empty;

	private static object DbNullableString(string? value) =>
		string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;

	public async Task<IActionResult> OnGet()
	{
		await GetUserFromClaims();

		return Page();
	}

	public async Task<IActionResult> OnPostSubmitApplication()
	{
		await GetUserFromClaims();

		SqlConnection SubmitApplicationConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand SubmitApplicationCommand = new()
		{
			Connection = SubmitApplicationConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "AddMembershipApplication",
			Parameters =
			{
				new SqlParameter("@Email", SqlDbType.VarChar, 100) { Value = Email },
				new SqlParameter("@FirstName", SqlDbType.VarChar, 50) { Value = FirstName },
				new SqlParameter("@LastName", SqlDbType.VarChar, 50) { Value = LastName },
				new SqlParameter("@PhoneNumber", SqlDbType.VarChar, 10) { Value = PhoneNumber },
				new SqlParameter("@Alt_PhoneNumber", SqlDbType.VarChar, 10) { Value = DbNullableString(AlternativePhoneNumber) },
				new SqlParameter("@Address", SqlDbType.VarChar, 100) { Value = Address },
				new SqlParameter("@City", SqlDbType.VarChar, 50) { Value = City },
				new SqlParameter("@Province", SqlDbType.VarChar, 50) { Value = Province },
				new SqlParameter("@PostalCode", SqlDbType.VarChar, 6) { Value = PostalCode },
				new SqlParameter("@Occupation", SqlDbType.VarChar, 50) { Value = Occupation },
				new SqlParameter("@DateOfBirth", SqlDbType.Date) { Value = DateOfBirth },
				new SqlParameter("@CompanyName", SqlDbType.VarChar, 50) { Value = DbNullableString(CompanyName) },
				new SqlParameter("@CompanyAddress", SqlDbType.VarChar, 100) { Value = DbNullableString(CompanyAddress) },
				new SqlParameter("@CompanyPostalCode", SqlDbType.VarChar, 6) { Value = DbNullableString(CompanyPostalCode) },
				new SqlParameter("@CompanyPhoneNumber", SqlDbType.VarChar, 10) { Value = DbNullableString(CompanyPhoneNumber) }
			}
		};

		try
		{
			using (SubmitApplicationConnection)
			{
				SubmitApplicationConnection.Open();

				using (SubmitApplicationCommand)
				{
					SubmitApplicationCommand.ExecuteNonQuery();
				}
			}

			ViewData["Success"] = "Membership application submitted successfully.";
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while submitting your application: {ex.Message}";
		}

		return Page();
	}

	public async Task<IActionResult> GetUserFromClaims()
	{
		var EmailClaim = User.FindFirstValue(ClaimTypes.Email);
		var FirstNameClaim = User.FindFirstValue(ClaimTypes.GivenName);
		var LastNameClaim = User.FindFirstValue(ClaimTypes.Surname);
		var RoleClaim = User.FindFirstValue(ClaimTypes.Role);
		var PhoneNumberClaim = User.FindFirstValue(ClaimTypes.MobilePhone);

		if (RoleClaim == null || EmailClaim == null || FirstNameClaim == null || LastNameClaim == null || PhoneNumberClaim == null)
		{
			return RedirectToPage("/Login");
		}
		else
		{
			Role = RoleClaim;
			Email = EmailClaim;
			FirstName = FirstNameClaim;
			LastName = LastNameClaim;
			PhoneNumber = PhoneNumberClaim;
			return Page();
		}
	}
}
