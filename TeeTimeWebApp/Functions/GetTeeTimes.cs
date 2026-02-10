using TeeTimeWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

// Stole the functionality from my previous term's project
namespace TeeTimeWebApp.Functions
{
	public class GetTeeTimes
	{
		public static List<SelectListItem> GetAvailableTeeTimes(DateOnly date, string Role)
		{
			List<SelectListItem> availableTeeTimes = new List<SelectListItem>();
			TimeOnly startTime = new TimeOnly(7, 0);
			bool addSeven = true;
			bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

			for (int i = 0; i < 12 * 60;)
			{
				TimeOnly time = startTime.AddMinutes(i);
				if (Role == "Gold" ||
				Role == "Shareholder" ||
				Role == "Admin" ||

				(Role == "Silver" && (!isWeekend &&
				(time < new TimeOnly(15, 0) || time > new TimeOnly(17, 30)) ||
				(isWeekend && time > new TimeOnly(11, 0)))) ||

				(Role == "Bronze" && (!isWeekend &&
				(time < new TimeOnly(15, 0) || time > new TimeOnly(18, 0)) ||
				(isWeekend && time > new TimeOnly(13, 0)))))
				{
					availableTeeTimes.Add(new SelectListItem
					{
						Value = time.ToString("HH:mm"),
						Text = time.ToString("hh:mm")
					});
				}

				if (addSeven)
				{
					i += 7;
				}
				else
				{
					i += 8;
				}
				addSeven = !addSeven;
			}

			return availableTeeTimes;
		}
	}
}
