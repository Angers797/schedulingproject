using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static C969_Project.DTOs;
using static C969_Project.DataService;

namespace C969_Project
{
	public static class Validate
	{
		private static readonly DateTime earliestAppointment = DateTime.MinValue.AddHours(9);
		private static readonly DateTime latestAppointment = DateTime.MinValue.AddHours(17);

		public static bool IsAnyNull(params object[] values) 
		{
			foreach (var value in values)
			{
				if (value == null || (value is string str && string.IsNullOrEmpty(str)))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsPhoneValid(string value)
		{
			bool IsValid = Regex.IsMatch(value, @"^[0-9-]*$");
			// Add phone validation logic here
			return IsValid;
		}

		public static string TrimString(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				return value.Trim();
			}
			return value;
		}
		
		public static bool IsApptTimeValid(Appointment appt, List<Appointment> existingAppointments, TimeZoneInfo localZone)
		{
			DateTime utcStart;
			DateTime utcEnd;

			utcStart = TimeZoneInfo.ConvertTimeToUtc(appt.Start, localZone);
			utcEnd = TimeZoneInfo.ConvertTimeToUtc(appt.End, localZone);

			if (utcStart >= utcEnd || utcStart.DayOfWeek != utcEnd.DayOfWeek)
			{
				MessageBox.Show("Appointment start time must be before end time and both times must be on the same day.");
				return false;
			}
			if (utcStart < earliestAppointment || utcStart > latestAppointment || utcEnd < earliestAppointment || utcEnd > latestAppointment)
			{
				MessageBox.Show("Appointment times must be within business hours (9 AM to 5 PM).");
				return false;
			}
			if (utcStart.DayOfWeek == DayOfWeek.Saturday || utcStart.DayOfWeek == DayOfWeek.Sunday)
			{
				MessageBox.Show("Appointments cannot be scheduled on weekends.");
				return false;
			}
			foreach (var existingAppt in existingAppointments)
			{
				if (existingAppt.AppointmentId != appt.AppointmentId && 
					((utcStart >= existingAppt.Start && utcStart < existingAppt.End) || 
					 (utcEnd > existingAppt.Start && utcEnd <= existingAppt.End) || 
					 (utcStart <= existingAppt.Start && utcEnd >= existingAppt.End)))
				{
					MessageBox.Show("Appointment times overlap with an existing appointment.");
					return false;
				}
			}
			return true;
		}	
	}
}
