using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static C969_Project.DTOs;
using static C969_Project.Validate;

namespace C969_Project
{
	public partial class Main : Form
	{		
		private enum Tab
		{
			Overview,
			Customers,
			Appointments
		}

		DataService dataService;
		int selectedCustomerId;
		int selectedAppointmentId;
		TimeZoneInfo localZone;	
		int loggedInUser;
		

		public Main(TimeZoneInfo localZone, int userId)
		{
			InitializeComponent();
			this.localZone = localZone;
			this.loggedInUser = userId;
			dataService = new DataService("server=localhost;user=sqlUser;database=client_schedule;port=3306;password=Passw0rd!;AllowUserVariables=True");
			fillApptsByDay(DateTime.Today, dgv_upcoming);
			SetupDateTimePickers();
		}

		private void SetupDateTimePickers()
		{
			dtp_timeStart.Format = DateTimePickerFormat.Time;			
			dtp_timeEnd.Format = DateTimePickerFormat.Time;			
		}

		private void btn_exit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			if(selectedCustomerId >= 0)
			{
				MessageBox.Show("Existing customer selected. Please use Update or clear the fields before adding a new customer.");
				return;
			}

			if (IsAnyNull(txt_name.Text, txt_address.Text, txt_city.Text, txt_phone.Text, txt_country.Text))
			{
				MessageBox.Show("Please fill out all fields before adding a customer.");
				return;
			}
			if(!IsPhoneValid(txt_phone.Text))
			{
				MessageBox.Show("Please enter a valid phone number.");
				return;
			}

			DTOs.Customer newCustomer = new DTOs.Customer
			{
				Name = TrimString(txt_name.Text),
				Address = TrimString(txt_address.Text),
				Address2 = TrimString(txt_addressTwo.Text),
				City = TrimString(txt_city.Text),
				PostalCode = TrimString(txt_zipCode.Text),
				Phone = TrimString(txt_phone.Text),
				Country = TrimString(txt_country.Text)
			};
			//Hit API to add customer to database here
			if(dataService.AddNewCustomer(newCustomer, loggedInUser))
			{
				//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
				MessageBox.Show($"Customer {newCustomer.Name} added successfully!");
				ClearFields(Tab.Customers);
			}
			else
			{
				MessageBox.Show("Failed to add customer. Please try again.");
			}
		}
		private void ClearFields(Tab tab)
		{
			if (tab == Tab.Customers)
			{
				txt_name.Clear();
				txt_address.Clear();
				txt_addressTwo.Clear();
				txt_city.Clear();
				txt_zipCode.Clear();
				txt_phone.Clear();
				txt_country.Clear();
			}
			else if (tab == Tab.Appointments)
			{
				txt_apptNameView.Clear();
				txt_apptTitle.Clear();
				txt_apptDescription.Clear();
				txt_apptLocation.Clear();
				txt_apptContact.Clear();
				txt_apptType.Clear();
				txt_apptUrl.Clear();
				dtp_dateStart.Value = DateTime.Today;
				dtp_timeStart.Value = DateTime.Now;
				dtp_dateEnd.Value = DateTime.Today;
				dtp_timeEnd.Value = DateTime.Now;
			}
			
			selectedCustomerId = -1;
			selectedAppointmentId = -1;
		}

		private void tab_main_SelectedIndexChanged(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine($"[DEBUG] Tab changed. SelectedTab is: {tab_main.SelectedTab?.Name}");

			if (tab_main.SelectedTab == null) return;

			if (tab_main.SelectedTab == tab_customers)
			{
				System.Diagnostics.Debug.WriteLine($"[DEBUG] Switched to Customers tab.");
				selectedAppointmentId = -1;
				selectedCustomerId = -1;

				if (dgv_customers == null)
				{
					System.Diagnostics.Debug.WriteLine("[DEBUG] ERROR: dgv_customers is null!");
					MessageBox.Show("Error: dgv_customers control not found.");
					return;
				}

				fillCustomers(dgv_customers);
			}
			else if (tab_main.SelectedTab == tab_appointments)
			{
				System.Diagnostics.Debug.WriteLine($"[DEBUG] Switched to Appointments tab.");
				selectedCustomerId = -1;
				selectedAppointmentId = -1;

				if (dgv_apptAppointments != null) fillApptsByDay(DateTime.Today, dgv_apptAppointments);
				if (dgv_apptCustomers != null) fillCustomers(dgv_apptCustomers);
			}
			else if (tab_main.SelectedTab == tab_overview)
			{
				System.Diagnostics.Debug.WriteLine($"[DEBUG] Switched to Overview tab.");
				selectedAppointmentId = -1;
				selectedCustomerId = -1;

				if (dgv_upcoming != null) fillApptsByDay(DateTime.Today, dgv_upcoming);
			}
		}


		private void fillApptsByDay(DateTime date, DataGridView view)
		{
			DataGridView fillDgv = view;
			List<Appointment> upcomingAppointments = new List<Appointment>();
			upcomingAppointments = dataService.GetAppointmentsByDate(date);
			fillDgv.Rows.Clear();
			foreach (Appointment appointment in upcomingAppointments)
			{
				fillDgv.Rows.Add(
					appointment.AppointmentId, 
					appointment.Title, 
					appointment.Description, 
					appointment.Location, 
					appointment.Contact, 
					appointment.Type, 
					appointment.Url, 
					ConvertToLocalTime(appointment.Start),
					ConvertToLocalTime(appointment.End));
			}			
		}

		private DateTime ConvertToLocalTime(DateTime utcDateTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, localZone);
		}

		private void fillCustomers(DataGridView view)
		{
			System.Diagnostics.Debug.WriteLine($"[DEBUG] fillCustomers called for {view.Name}");

			if (view == null)
			{
				System.Diagnostics.Debug.WriteLine("[DEBUG] DataGridView is null. Cannot fill customers.");
				return;
			}

			System.Diagnostics.Debug.WriteLine("[DEBUG] Attempting to fetch data...");
			
			List<Customer> customers = dataService.GetAllCustomers();

			if (customers == null)
			{
				System.Diagnostics.Debug.WriteLine("[DEBUG] No customers retrieved. Check data service.");
				return;
			}

			view.Rows.Clear();
			foreach (Customer customer in customers)
			{
				view.Rows.Add(
					customer.CustomerId,
					customer.Name,
					customer.Address,
					customer.Address2,
					customer.City,
					customer.PostalCode,
					customer.Phone,
					customer.Country);
			}
		}

		private void fillApptByPerson(int id, bool isCustomer)
		{			
			List<Appointment> appointments = new List<Appointment>();			
			appointments = dataService.GetAppointmentsByPerson(id, isCustomer);
			dgv_apptAppointments.Rows.Clear();
			foreach (Appointment appointment in appointments)
			{
				dgv_apptAppointments.Rows.Add(					
					appointment.AppointmentId,
					appointment.Title,
					appointment.Description,
					appointment.Location,
					appointment.Contact,
					appointment.Type,
					appointment.Url,
					ConvertToLocalTime(appointment.Start),
					ConvertToLocalTime(appointment.End));
			}
		}

		private void btn_clear_Click(object sender, EventArgs e)
		{
			ClearFields(Tab.Customers);
		}

		private void btn_update_Click(object sender, EventArgs e)
		{
			if (selectedCustomerId < 0)
			{
				MessageBox.Show("No customer selected. Please select a customer to update.");
				return;
			}
			DialogResult result = MessageBox.Show("Are you sure you want to update the user " + txt_name.Text + "?", "Update Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{	
				if (IsAnyNull(txt_name.Text, txt_address.Text, txt_city.Text, txt_phone.Text, txt_country.Text))
				{
					MessageBox.Show("Please fill out all fields before adding an appointment.");
					return;
				}
				if (!IsPhoneValid(txt_phone.Text))
				{
					MessageBox.Show("Please enter a valid phone number.");
					return;
				}

				DTOs.Customer updateCustomer = new DTOs.Customer
				{
					CustomerId = selectedCustomerId,
					Name = TrimString(txt_name.Text),
					Address = TrimString(txt_address.Text),
					Address2 = TrimString(txt_addressTwo.Text),
					City = TrimString(txt_city.Text),
					PostalCode = TrimString(txt_zipCode.Text),
					Phone = TrimString(txt_phone.Text),
					Country = TrimString(txt_country.Text)
				};
				//Hit API to add customer to database here
				//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
				MessageBox.Show($"Customer {updateCustomer.Name} added successfully!");
				ClearFields(Tab.Customers);
				//Hit API to update customer in database here
				//If good response, show success message. If not, show error message.
				MessageBox.Show($"Customer {txt_name.Text} updated successfully!");
			}
		}

		private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
		{

		}

		private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
		{
			fillApptsByDay(monthCalendar1.SelectionStart, dgv_upcoming);
		}

		private void dgv_apptCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0) 
			{
				DataGridViewRow row = dgv_apptCustomers.Rows[e.RowIndex];
				int customerId = Convert.ToInt32(row.Cells["CustomerId"].Value);
				fillApptByPerson(customerId, true);
			}			
			//Hit API to get appointments for selected customer from database here
			//If good response, fill dgv_apptAppointments with appointments for selected customer. If not, show error message and leave dgv_apptAppointments blank.
			//Make sure to handle case where customer has no appointments and show appropriate message.
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			if (selectedCustomerId < 0)
			{
				MessageBox.Show("No customer selected. Please select a customer to delete.");
				return;
			}
			DialogResult result = MessageBox.Show("Are you sure you want to delete the user " + txt_name.Text + "?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{				
				DTOs.Customer deleteCustomer = new DTOs.Customer
				{
					CustomerId = selectedCustomerId,
					Name = TrimString(txt_name.Text)
					
				};
				//Hit API to add customer to database here
				dataService.DeleteCustomer(deleteCustomer.CustomerId);
				//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
				MessageBox.Show($"Customer {deleteCustomer.Name} deleted successfully!");
				ClearFields(Tab.Customers);	
				fillCustomers(dgv_customers);
			}
		}

		private void btn_appt_add_Click(object sender, EventArgs e)
		{
			DateTime start;
			DateTime end;

			if (selectedAppointmentId >= 0)
			{
				MessageBox.Show("Existing appointment selected. Please use Update or clear the fields before adding an appointment.");
				return;
			}

			if (IsAnyNull(txt_apptTitle.Text, txt_apptDescription.Text, txt_apptLocation.Text, txt_apptContact.Text, txt_apptType.Text, txt_apptUrl.Text))
			{
				MessageBox.Show("Please fill out all fields before adding an appointment.");
				return;
			}			

			start = dtp_dateStart.Value.Date + dtp_timeStart.Value.TimeOfDay;
			end = dtp_dateEnd.Value.Date + dtp_timeEnd.Value.TimeOfDay;	

			DTOs.NewAppointment newAppointment = new DTOs.NewAppointment
			{
				CustomerId = selectedCustomerId,
				UserId = loggedInUser,
				Title = TrimString(txt_apptTitle.Text),
				Description = TrimString(txt_apptDescription.Text),
				Location = TrimString(txt_apptLocation.Text),
				Contact = TrimString(txt_apptContact.Text),
				Type = TrimString(txt_apptType.Text),
				Url = TrimString(txt_apptUrl.Text),
				Start = start,
				End = end
			};
			List<Appointment> existingAppointments = dataService.GetAppointmentsByPerson((int)newAppointment.UserId, false);

			if (!IsApptTimeValid(newAppointment, existingAppointments, TimeZoneInfo.Local))
			{
				MessageBox.Show("Invalid appointment time.");
				return;
			}
			//Hit API to add appointment to database here
			if (dataService.AddAppointment(newAppointment))
			{
				//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
				MessageBox.Show($"Appointment {newAppointment.Title} added successfully!");
				ClearFields(Tab.Appointments);
			}
			else
			{
				MessageBox.Show("Failed to add appointment. Please try again.");
			}
		}

		private void btn_appt_update_Click(object sender, EventArgs e)
		{
			DateTime start;
			DateTime end;

			if (selectedAppointmentId < 0)
			{
				MessageBox.Show("No appointment selected. Please select an appointment to update.");
				return;
			}

			DialogResult result = MessageBox.Show("Are you sure you want to update the appointment " + txt_apptTitle.Text + "?", "Update Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{
				if (IsAnyNull(txt_apptTitle.Text, txt_apptDescription.Text, txt_apptLocation.Text, txt_apptContact.Text, txt_apptType.Text, txt_apptUrl.Text))
				{
					MessageBox.Show("Please fill out all fields before updating an appointment.");
					return;
				}				

				start = dtp_dateStart.Value.Date + dtp_timeStart.Value.TimeOfDay;
				end = dtp_dateEnd.Value.Date + dtp_timeEnd.Value.TimeOfDay;

				DTOs.UpdateAppointment updateAppointment = new DTOs.UpdateAppointment
				{
					CustomerId = selectedCustomerId,
					UserId = loggedInUser,
					Title = TrimString(txt_apptTitle.Text),
					Description = TrimString(txt_apptDescription.Text),
					Location = TrimString(txt_apptLocation.Text),
					Contact = TrimString(txt_apptContact.Text),
					Type = TrimString(txt_apptType.Text),
					Url = TrimString(txt_apptUrl.Text),
					Start = start,
					End = end
				};
				List<Appointment> existingAppointments = dataService.GetAppointmentsByPerson((int)updateAppointment.UserId, false);
				if (!IsApptTimeValid(updateAppointment, existingAppointments, TimeZoneInfo.Local))
				{
					MessageBox.Show("Invalid appointment time.");
					return;
				}


				//Hit API to add appointment to database here
				if (dataService.UpdateAppointment(updateAppointment))
				{
					//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
					MessageBox.Show($"Appointment {updateAppointment.Title} updated successfully!");
					ClearFields(Tab.Appointments);
				}
				else
				{
					MessageBox.Show("Failed to update appointment. Please try again.");
				}
			}
		}

		private void btn_apptClearForm_Click(object sender, EventArgs e)
		{
			ClearFields(Tab.Appointments);
		}

		private void btn_appt_delete_Click(object sender, EventArgs e)
		{
			if (selectedAppointmentId < 0)
			{
				MessageBox.Show("No appointment selected. Please select an appointment to delete.");
				return;
			}
			DialogResult result = MessageBox.Show("Are you sure you want to delete the appointment " + txt_apptTitle.Text + "?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{
				DTOs.Appointment deleteAppointment = new DTOs.Appointment
				{
					AppointmentId = selectedAppointmentId,
					Title = TrimString(txt_apptTitle.Text)

				};
				//Hit API to add appointment to database here
				dataService.DeleteAppointment((int)deleteAppointment.AppointmentId);
				//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
				MessageBox.Show($"Appointment {deleteAppointment.Title} deleted successfully!");
				ClearFields(Tab.Appointments);
			}
		}

		private void dgv_customers_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				DataGridViewRow row = dgv_customers.Rows[e.RowIndex];
				int customerId = Convert.ToInt32(row.Cells["colId"].Value);
			}
		}

		private void dgv_customers_Click(object sender, EventArgs e)
		{
			if (dgv_customers.CurrentRow != null)
			{
				DataGridViewRow row = dgv_customers.CurrentRow;
				selectedCustomerId = Convert.ToInt32(row.Cells["colId"].Value);
				txt_name.Text = row.Cells["colName"].Value.ToString();
				txt_address.Text = row.Cells["colAddress"].Value.ToString();
				txt_addressTwo.Text = row.Cells["colAddress2"].Value.ToString();
				txt_city.Text = row.Cells["colCity"].Value.ToString();
				txt_zipCode.Text = row.Cells["colPostalCode"].Value.ToString();
				txt_phone.Text = row.Cells["colPhone"].Value.ToString();
				txt_country.Text = row.Cells["colCountry"].Value.ToString();
			}
		}

		private void dgv_apptCustomers_Click(object sender, EventArgs e)
		{
			if (dgv_apptCustomers.CurrentRow != null)
			{
				DataGridViewRow row = dgv_apptCustomers.CurrentRow;
				int customerId = Convert.ToInt32(row.Cells["colApptTabCustId"].Value);
				selectedCustomerId = customerId;
				fillApptByPerson(customerId, true);
				txt_apptNameView.Text = row.Cells["colApptTabCustName"].Value.ToString();
			}
		}
	}	
}


