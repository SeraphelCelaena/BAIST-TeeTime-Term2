using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;
using TeeTimeWebApp.Models;

public class ManageUserModel : PageModel
{
	private readonly IConfiguration _configuration;

	public ManageUserModel(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public List<User> UserList { get; set; } = new List<User>();

	public async Task<IActionResult> OnGet()
	{
		await GetAllUsers();

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
					using (SqlDataReader GetUserListReader = await GetUserListCommand.ExecuteReaderAsync())
					{
						while (await GetUserListReader.ReadAsync())
						{
							UserList.Add(new User
							{
								Email = GetUserListReader.GetString(0),
								FirstName = GetUserListReader.GetString(1),
								LastName = GetUserListReader.GetString(2),
								PhoneNumber = GetUserListReader.GetInt32(3),
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
