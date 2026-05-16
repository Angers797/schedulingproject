using System;
using System.Collections.Generic;

namespace C969_Project
{
	public class DTOs
	{
		public class Customer 
		{
			public int CustomerId { get; set; }
			public string Name { get; set; }
			public string Address { get; set; }
			public string Address2 { get; set; }
			public string City { get; set; }
			public int PostalCode { get; set; }
			public string Phone { get; set; }
			public string Country { get; set; }
		}

		public class Appointment
		{
			public int? AppointmentId { get; set; }
			public int CustomerId { get; set; }
			public int? UserId { get; set; }
			public string Title { get; set; }
			public string Description { get; set; }
			public string Location { get; set; }
			public string Contact { get; set; }
			public string Type { get; set; }
			public string Url { get; set; }
			public DateTime Start { get; set; }
			public DateTime End { get; set; }
		}

		public class NewCustomer
		{
			public string Name { get; set; }
			public string Address { get; set; }
			public string Address2 { get; set; }
			public string City { get; set; }
			public int PostalCode { get; set; }
			public string Phone { get; set; }
			public string Country { get; set; }
			public DateTime CreateDate { get; set; }
			public string CreatedBy { get; set; }
			public DateTime LastUpdate { get; set; }
			public string LastUpdateBy { get; set; }
		}
		
		public class UpdateCustomer
		{
			public int CustomerId { get; set; }
			public string Name { get; set; }
			public string Address { get; set; }
			public string Address2 { get; set; }
			public string City { get; set; }
			public int PostalCode { get; set; }
			public string Phone { get; set; }
			public string Country { get; set; }			
			public DateTime LastUpdate { get; set; }
			public string LastUpdateBy { get; set; }
		}
		
		public class NewUser
		{
			public string UserName { get; set; }
			public string Password { get; set; }
			public int Active { get; set; }
			public DateTime CreateDate { get; set; }
			public string CreatedBy { get; set; }
			public DateTime LastUpdate { get; set; }
			public string LastUpdateBy { get; set; }
		}

		public class NewAppointment
		{
			public int CustomerId { get; set; }
			public int? UserId { get; set; }
			public string Title { get; set; }
			public string Description { get; set; }
			public string Location { get; set; }
			public string Contact { get; set; }
			public string Type { get; set; }
			public string Url { get; set; }
			public DateTime Start { get; set; }
			public DateTime End { get; set; }
			public DateTime CreateDate { get; set; }
			public string CreatedBy { get; set; }
			public DateTime LastUpdate { get; set; }
			public string LastUpdateBy { get; set; }
		}

		public class UpdateAppointment
		{
			public int AppointmentId { get; set; }
			public int CustomerId { get; set; }
			public int? UserId { get; set; }
			public string Title { get; set; }
			public string Description { get; set; }
			public string Location { get; set; }
			public string Contact { get; set; }
			public string Type { get; set; }
			public string Url { get; set; }
			public DateTime Start { get; set; }
			public DateTime End { get; set; }			
			public DateTime LastUpdate { get; set; }
			public string LastUpdateBy { get; set; }
		}
	}
}
