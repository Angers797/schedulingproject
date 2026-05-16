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
	}
}
