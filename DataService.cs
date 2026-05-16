using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static C969_Project.DTOs;
using System.IO;
using System.Windows.Forms;

namespace C969_Project
{
	public class DataService
	{
		//Get on the wgu virtual machine thing for the connectionstring
		string connectionString;
		public DataService(string dbstring)
		{
			connectionString = dbstring;
		}

		public int AuthenticateUser(string username, string password)
		{
			try
			{
				using (var _dbConnection = new MySqlConnection(connectionString))
				using (var command = new MySqlCommand("SELECT userId FROM user WHERE userName = @username AND password = @password", _dbConnection))
				{
					command.Parameters.AddWithValue("@username", username);
					command.Parameters.AddWithValue("@password", password);

					_dbConnection.Open();
					var result = command.ExecuteScalar();

					if(result == null || result == DBNull.Value)
					{
						MessageBox.Show("Invalid username or password.");
						return -1; // Indicate authentication failure with a special value
					}
					return Convert.ToInt32(result);
				}
			}
			catch (MySqlException ex)
			{
				MessageBox.Show("Database error: " + ex.Message);
				return -1; // Indicate an error with a special value
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred: " + ex.Message);
				return -1; // Indicate an error with a special value
			}
		}

		//Send either a login or logout
		public  bool UserActiveFlag(bool active, int userId)
		{
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("UPDATE user SET active = @active WHERE userId = @userId", _dbConnection))
			{
				command.Parameters.AddWithValue("@active", active);
				command.Parameters.AddWithValue("@userId", userId);
				_dbConnection.Open();
				var result =  command.ExecuteNonQuery();
				return result > 0;
			}
		}

		//incomplete, put in try/catch and return bool for success/failure, also need to add userId for createdBy and lastUpdateBy fields in the database
		public bool AddNewCustomer(Customer customer, int userId)
		{
			var countryId = 0;
			var cityId = 0;
			var addressId = 0;

			using (var _dbConnection = new MySqlConnection(connectionString))
			{
				_dbConnection.Open();
				using (var transaction = _dbConnection.BeginTransaction())
				{
					using (var cmdCountry = new MySqlCommand(@"
						INSERT INTO country (country, createDate, createdBy, lastUpdate, lastUpdateBy)
						SELECT @country, @createDate,@createdBy, @lastUpdate, @lastUpdateBy
						FROM DUAL
						WHERE NOT EXISTS (SELECT 1 FROM country WHERE country = @country);
						SELECT countryId FROM country WHERE country = @country)", _dbConnection, transaction))
					{
						cmdCountry.Parameters.AddWithValue("@country", customer.Country);
						cmdCountry.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
						cmdCountry.Parameters.AddWithValue("@createdBy", userId);
						cmdCountry.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
						cmdCountry.Parameters.AddWithValue("@lastUpdateBy", userId);
						countryId = Convert.ToInt32(cmdCountry.ExecuteScalar());
					}
					using (var cmdCity = new MySqlCommand(@"
						INSERT INTO city (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy)
						SELECT @city, @countryId, @createDate, @createdBy, @lastUpdate, @lastUpdateBy
						FROM DUAL
						WHERE NOT EXISTS (SELECT 1 FROM city WHERE city = @city);
						SELECT cityId FROM city WHERE city = @city", _dbConnection, transaction))
					{
						cmdCity.Parameters.AddWithValue("@city", customer.City);
						cmdCity.Parameters.AddWithValue("@countryId", countryId);
						cmdCity.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
						cmdCity.Parameters.AddWithValue("@createdBy", userId);
						cmdCity.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
						cmdCity.Parameters.AddWithValue("@lastUpdateBy", userId);
						cityId = Convert.ToInt32(cmdCity.ExecuteScalar());
					}						
					using (var cmdAddress = new MySqlCommand(@"
						INSERT INTO address (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy)
						VALUES (@address, @address2, @cityId, @postalCode, @phone, @createDate, @createdBy, @lastUpdate, @lastUpdateBy);
						SELECT LAST_INSERT_ID()", _dbConnection, transaction))
					{
						cmdAddress.Parameters.AddWithValue("@address", customer.Address);
						cmdAddress.Parameters.AddWithValue("@address2", customer.Address2);
						cmdAddress.Parameters.AddWithValue("@cityId", cityId);
						cmdAddress.Parameters.AddWithValue("@postalCode", customer.PostalCode);
						cmdAddress.Parameters.AddWithValue("@phone", customer.Phone);
						cmdAddress.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
						cmdAddress.Parameters.AddWithValue("@createdBy", userId);
						cmdAddress.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
						cmdAddress.Parameters.AddWithValue("@lastUpdateBy", userId);
						addressId = Convert.ToInt32(cmdAddress.ExecuteScalar());
					}
					using (var cmdCustomer = new MySqlCommand("INSERT INTO customer (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy)" +
						"VALUES (@customerName, @addressId, @active, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)", _dbConnection, transaction))
					{
						cmdCustomer.Parameters.AddWithValue("@customerName", customer.Name);
						cmdCustomer.Parameters.AddWithValue("@addressId", addressId);
						cmdCustomer.Parameters.AddWithValue("@active", 1);
						cmdCustomer.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
						cmdCustomer.Parameters.AddWithValue("@createdBy", userId);
						cmdCustomer.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
						cmdCustomer.Parameters.AddWithValue("@lastUpdateBy", userId);
						cmdCustomer.ExecuteNonQuery();
					}
				}
				return true;
			}
		}

		public bool UpdateCustomer(Customer customer, int userId)
		{
			var countryId = 0;
			var cityId = 0;

			using (var _dbConnection = new MySqlConnection(connectionString))
			{
				_dbConnection.Open();
				using (var transaction = _dbConnection.BeginTransaction())
				{
					using (var cmdCountry = new MySqlCommand("INSERT INTO country (country, createdBy)" +
						"SELECT @country, @createdBy" +
						"FROM DUAL" +
						"WHERE NOT EXISTS (SELECT 1 FROM country WHERE country = @country);" +
						"SELECT countryId FROM country WHERE country = @country;", _dbConnection, transaction))
					{
						cmdCountry.Parameters.AddWithValue("@country", customer.Country);
						cmdCountry.Parameters.AddWithValue("@createdBy", userId);
						countryId = Convert.ToInt32(cmdCountry.ExecuteScalar());
					}
					using (var cmdCity = new MySqlCommand(@"
						INSERT INTO city (city, countryId, createdBy) 
						SELECT @city, @countryId, @createdBy 
						FROM DUAL 
						WHERE NOT EXISTS (SELECT 1 FROM city WHERE city = @city);
						SELECT cityId FROM city WHERE city = @city;", _dbConnection, transaction))
					{
						cmdCity.Parameters.AddWithValue("@city", customer.City);
						cmdCity.Parameters.AddWithValue("@countryId", countryId);
						cmdCity.Parameters.AddWithValue("@createdBy", userId);

						cityId = Convert.ToInt32(cmdCity.ExecuteScalar());

					}
					using (var cmdUpdate = new MySqlCommand(@"
						UPDATE customer c
						INNER JOIN address a ON c.addressId = a.addressId
						SET
							a.address = @address,
							a.address2 = @address2,
							a.cityId = @cityId,
							a.postalCode = @postalCode,	
							a.phone = @phone,
							a.lastUpdateBy = @lastUpdateBy,
							c.customerName = @customerName,
							c.lastUpdateBy = @lastUpdateBy
						WHERE c.customerId = @customerId", _dbConnection, transaction))
					{
						cmdUpdate.Parameters.AddWithValue("@customerName", customer.Name);
						cmdUpdate.Parameters.AddWithValue("@address", customer.Address);
						cmdUpdate.Parameters.AddWithValue("@address2", customer.Address2);
						cmdUpdate.Parameters.AddWithValue("@cityId", cityId);
						cmdUpdate.Parameters.AddWithValue("@postalCode", customer.PostalCode);
						cmdUpdate.Parameters.AddWithValue("@phone", customer.Phone);
						cmdUpdate.Parameters.AddWithValue("@lastUpdateBy", userId);
						cmdUpdate.Parameters.AddWithValue("@customerId", customer.CustomerId);
						cmdUpdate.ExecuteNonQuery();
					}
				}
				return true;
			}
		}

		public bool DeleteCustomer(int customerId)
		{
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("DELETE customer, address" +
				"FROM customer" +
				"INNER JOIN address ON  customer.addressId = address.addressId" +
				"WHERE customer.customerId = @customerId", _dbConnection))
			{
				command.Parameters.AddWithValue("@customerId", 1); // Placeholder, should be replaced with actual customer ID
				_dbConnection.Open();
				var result = command.ExecuteNonQuery();
				return result > 0;
			}
		}

		public bool AddAppointment(Appointment appointment)
		{

			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand(@"
				INSERT INTO appointment (customerId, userId, title, description, location, contact, url, start, end, createdBy) 
				VALUES (@customerId, @userId, @title, @description, @location, @contact, @url, @start, @end)", _dbConnection))
			{
				command.Parameters.AddWithValue("@customerId", appointment.CustomerId);
				command.Parameters.AddWithValue("@userId", appointment.UserId);
				command.Parameters.AddWithValue("@title", appointment.Title);
				command.Parameters.AddWithValue("@description", appointment.Description);
				command.Parameters.AddWithValue("@location", appointment.Location);
				command.Parameters.AddWithValue("@contact", appointment.Contact);
				command.Parameters.AddWithValue("@url", appointment.Url);
				command.Parameters.AddWithValue("@start", appointment.Start);
				command.Parameters.AddWithValue("@end", appointment.End);
				command.Parameters.AddWithValue("@createdBy", appointment.UserId);
				_dbConnection.Open();
				var result = command.ExecuteNonQuery();
				return result > 0;
			}
		}

		public bool UpdateAppointment(Appointment appointment)
		{

			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand(@"
				UPDATE appointment
				SET customerId = @customerId,
					userId = @userId,
					title = @title,
					description = @description,
					location = @location,
					contact = @contact,
					url = @url,
					start = @start,
					end = @end,
					lastUpdateBy = @userId
				WHERE appointmentId = @appointmentId", _dbConnection))
			{
				command.Parameters.AddWithValue("@customerId", appointment.CustomerId);
				command.Parameters.AddWithValue("@userId", appointment.UserId);
				command.Parameters.AddWithValue("@title", appointment.Title);
				command.Parameters.AddWithValue("@description", appointment.Description);
				command.Parameters.AddWithValue("@location", appointment.Location);
				command.Parameters.AddWithValue("@contact", appointment.Contact);
				command.Parameters.AddWithValue("@url", appointment.Url);
				command.Parameters.AddWithValue("@start", appointment.Start);
				command.Parameters.AddWithValue("@end", appointment.End);
				command.Parameters.AddWithValue("@lastUpdateBy", appointment.UserId);
				_dbConnection.Open();
				var result = command.ExecuteNonQuery();
				return result > 0;
			}
		}
		public bool DeleteAppointment(int appointmentId)
		{
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("DELETE FROM appointment WHERE appointmentId = @appointmentId", _dbConnection))
			{
				command.Parameters.AddWithValue("@appointmentId", appointmentId);
				_dbConnection.Open();
				var result = command.ExecuteNonQuery();
				return result > 0;
			}
		}

		public List<Appointment> GetAppointmentsByDate(DateTime date)
		{
			var appointments = new List<Appointment>();

			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("SELECT * " +
				"FROM appointment WHERE DATE(start) = @date", _dbConnection))
			{
				command.Parameters.AddWithValue("@date", date);
				_dbConnection.Open();
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						//DB Returns apptId(0), customerId(1), userId(2), title(3), description(4), location(5), contact(6),
						//type(7), url(8), start(9), end(10), createDate(11), createdBy(12, lastUpdate(13), lastUpdateBy(14)
						appointments.Add(new Appointment
						{
							AppointmentId = reader.GetInt32(0),
							CustomerId = reader.GetInt32(1),
							UserId = reader.GetInt32(2),
							Title = reader.IsDBNull(3) ? null : reader.GetString("title"),
							Description = reader.IsDBNull(4) ? null : reader.GetString("description"),
							Location = reader.IsDBNull(5) ? null : reader.GetString("location"),
							Contact = reader.IsDBNull(6) ? null : reader.GetString("contact"),
							Type = reader.IsDBNull(7) ? null : reader.GetString("type"),
							Url = reader.IsDBNull(8) ? null : reader.GetString("url"),
							Start = reader.GetDateTime(9),
							End = reader.GetDateTime(10)
						});
					}
				}
			}
			return appointments;
		}

		//Gets appointments by either user or customer, depending on the ID passed in, flag isCustomer in the request
		public List<Appointment> GetAppointmentsByPerson(int personId, bool isCustomer)
		{
			var appointments = new List<Appointment>();
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("SELECT * " +
				"FROM appointment WHERE " + (isCustomer ? "customerId" : "userId") + " = @personId", _dbConnection))
			{
				command.Parameters.AddWithValue("@personId", personId);
				_dbConnection.Open();
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						appointments.Add(new Appointment
						{
							AppointmentId = reader.GetInt32(0),
							CustomerId = reader.GetInt32(1),
							UserId = reader.GetInt32(2),
							Title = reader.IsDBNull(3) ? null : reader.GetString("title"),
							Description = reader.IsDBNull(4) ? null : reader.GetString("description"),
							Location = reader.IsDBNull(5) ? null : reader.GetString("location"),
							Contact = reader.IsDBNull(6) ? null : reader.GetString("contact"),
							Type = reader.IsDBNull(7) ? null : reader.GetString("type"),
							Url = reader.IsDBNull(8) ? null : reader.GetString("url"),
							Start = reader.GetDateTime(9),
							End = reader.GetDateTime(10)
						});
					}
				}
			}
			return appointments;
		}

		public List<Appointment> GetAppointmentsByMonth(int month, int year)
		{
			var appointments = new List<Appointment>();
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("SELECT * " +
				"FROM appointment WHERE MONTH(start) = @month AND YEAR(start) = @year", _dbConnection))
			{
				command.Parameters.AddWithValue("@month", month);
				command.Parameters.AddWithValue("@year", year);
				_dbConnection.Open();
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						appointments.Add(new Appointment
						{
							AppointmentId = reader.GetInt32(0),
							CustomerId = reader.GetInt32(1),
							UserId = reader.GetInt32(2),
							Title = reader.IsDBNull(3) ? null : reader.GetString("title"),
							Description = reader.IsDBNull(4) ? null : reader.GetString("description"),
							Location = reader.IsDBNull(5) ? null : reader.GetString("location"),
							Contact = reader.IsDBNull(6) ? null : reader.GetString("contact"),
							Type = reader.IsDBNull(7) ? null : reader.GetString("type"),
							Url = reader.IsDBNull(8) ? null : reader.GetString("url"),
							Start = reader.GetDateTime(9),
							End = reader.GetDateTime(10)
						});
					}
				}
			}
			return appointments;
		}
		public List<Customer> GetAllCustomers()
		{
			var customers = new List<Customer>();
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("SELECT c.customerId, c.customerName, a.address, a.address2, ci.city, co.country, a.postalCode, a.phone " +
				"FROM customer c " +
				"INNER JOIN address a ON c.addressId = a.addressId " +
				"INNER JOIN city ci ON a.cityId = ci.cityId " +
				"INNER JOIN country co ON ci.countryId = co.countryId", _dbConnection))
			{
				_dbConnection.Open();
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						customers.Add(new Customer
						{
							Name = reader.GetString("customerName"),
							Address = reader.GetString("address"),
							Address2 = reader.IsDBNull(reader.GetOrdinal("address2")) ? null : reader.GetString("address2"),
							City = reader.GetString("city"),
							Country = reader.GetString("country"),
							PostalCode = reader.GetString("postalCode"),
							Phone = reader.GetString("phone")
						});
					}
				}
			}
			return customers;
		}

		public Customer GetCustomerById(int customerId)
		{
			Customer customer = null;
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("SELECT c.customerId, c.customerName, a.address, a.address2, ci.city, co.country, a.postalCode, a.phone " +
				"FROM customer c " +
				"INNER JOIN address a ON c.addressId = a.addressId " +
				"INNER JOIN city ci ON a.cityId = ci.cityId " +
				"INNER JOIN country co ON ci.countryId = co.countryId " +
				"WHERE c.customerId = @customerId", _dbConnection))
			{
				command.Parameters.AddWithValue("@customerId", customerId);
				_dbConnection.Open();
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						customer = new Customer
						{
							Name = reader.GetString("customerName"),
							Address = reader.GetString("address"),
							Address2 = reader.IsDBNull(reader.GetOrdinal("address2")) ? null : reader.GetString("address2"),
							City = reader.GetString("city"),
							Country = reader.GetString("country"),
							PostalCode = reader.GetString("postalCode"),
							Phone = reader.GetString("phone")
						};
					}
				}
			}
			return customer;
		}

		public int addTestUser()
		{
			using (var _dbConnection = new MySqlConnection(connectionString))
			using (var command = new MySqlCommand("INSERT INTO user (userName, password, active, createDate, createdBy, lastUpdate, lastUpdateBy) VALUES ('test', 'test', 1, @createDate, 'system', @lastUpdate, @lastUpdateBy); SELECT LAST_INSERT_ID();", _dbConnection))
			{
				command.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
				command.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
				command.Parameters.AddWithValue("@lastUpdateBy", "system");
				_dbConnection.Open();
				var result = command.ExecuteScalar();
				return Convert.ToInt32(result);
			}
		}

		public void populateDatabase(int userId)
		{
			//Create a test user first.
			//This method is for testing purposes only. It will populate the database with some dummy data to work with when testing the application.
			//In a production environment, this method would not be used and would likely be removed from the codebase entirely.
			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json", "customers.json"));	
			var customers = JsonSerializer.Deserialize<List<DTOs.Customer>>(json);
			foreach (var customer in customers)
			{
				AddNewCustomer(customer, userId);
			}
		}
	}
}
