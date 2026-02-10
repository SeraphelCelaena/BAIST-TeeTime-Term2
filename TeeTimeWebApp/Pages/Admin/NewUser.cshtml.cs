using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
	public int Role { get; set;}

	public List<SelectListItem> RolesList { get; set;} = new List<SelectListItem>();

	public async Task<IActionResult> OnGet()
	{
		return Page();
	}
}
