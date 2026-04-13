using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;

[Authorize(Policy = "AdminOnly")]
public class ReviewMembershipApplicationModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ReviewMembershipApplicationModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public List<MembershipApplication> MembershipApplicationsList { get; set; } = new();

	[BindProperty]
	public int MembershipApplicationID { get; set; }

	public async Task<IActionResult> OnGet()
	{
		await GetAllApplications();

		return Page();
	}

	public async Task<IActionResult> OnPostApprove()
	{
		await UpdateApplicationStatus("Approved");

		return Page();
	}

	public async Task<IActionResult> OnPostReject()
	{
		await UpdateApplicationStatus("Rejected");

		return Page();
	}

	public async Task<IActionResult> UpdateApplicationStatus(string newStatus)
	{
		SqlConnection UpdateStatusConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand UpdateStatusCommand = new()
		{
			Connection = UpdateStatusConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "UpdateMembershipApplicationStatus",
			Parameters =
			{
				new SqlParameter("@ApplicationID", SqlDbType.Int) { Value = MembershipApplicationID },
				new SqlParameter("@NewStatus", SqlDbType.NVarChar, 20) { Value = newStatus }
			}
		};

		try
		{
			using (UpdateStatusConnection)
			{
				UpdateStatusConnection.Open();
				using (UpdateStatusCommand)
				{
					UpdateStatusCommand.ExecuteNonQuery();
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while updating application status: {ex.Message}";
		}

		return Page();
	}

	public async Task<IActionResult> GetAllApplications()
	{
		SqlConnection GetAllApplicationsConnection = new()
		{
			ConnectionString = _configuration.GetConnectionString("DefaultConnection")
		};

		SqlCommand GetAllApplicationsCommand = new()
		{
			Connection = GetAllApplicationsConnection,
			CommandType = CommandType.StoredProcedure,
			CommandText = "GetAllMembershipApplications"
		};

		try
		{
			using (GetAllApplicationsConnection)
			{
				GetAllApplicationsConnection.Open();
				using (GetAllApplicationsCommand)
				{
					using (SqlDataReader ApplicationReader = GetAllApplicationsCommand.ExecuteReader())
					{
						if (ApplicationReader.HasRows)
						{
							while (ApplicationReader.Read())
							{
								MembershipApplicationsList.Add(new MembershipApplication
								{
									MembershipApplicationID = ApplicationReader.GetInt32(0),
									Email = ApplicationReader.GetString(1),
									FirstName = ApplicationReader.GetString(2),
									LastName = ApplicationReader.GetString(3),
									Address = ApplicationReader.GetString(4),
									City = ApplicationReader.GetString(5),
									Province = ApplicationReader.GetString(6),
									PostalCode = ApplicationReader.GetString(7),
									PhoneNumber = ApplicationReader.GetString(8),
									Alt_PhoneNumber = GetNullableString(ApplicationReader, 9) ?? string.Empty,
									DateOfBirth = DateOnly.FromDateTime(ApplicationReader.GetDateTime(10)),
									Occupation = ApplicationReader.GetString(11),
									CompanyName = GetNullableString(ApplicationReader, 12) ?? string.Empty,
									CompanyAddress = GetNullableString(ApplicationReader, 13) ?? string.Empty,
									CompanyPostalCode = GetNullableString(ApplicationReader, 14) ?? string.Empty,
									CompanyPhoneNumber = GetNullableString(ApplicationReader, 15) ?? string.Empty,
									DateApplied = ApplicationReader.GetDateTime(16),
									Status = ApplicationReader.GetString(17)
								});
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ViewData["Error"] = $"An error occurred while retrieving membership applications: {ex.Message}";
		}

		return Page();
	}

	private static string? GetNullableString(SqlDataReader reader, int ordinal)
	{
		return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
	}
}
