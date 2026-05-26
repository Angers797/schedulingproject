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
using System.IO;

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
		TimeSpan appointmentAlertThreshold = TimeSpan.FromMinutes(15);	
		string dbString = "";	

		public Main(TimeZoneInfo localZone, int userId, string dbString)
		{
			InitializeComponent();
			this.localZone = localZone;
			loggedInUser = userId;
			this.dbString = dbString;	
			dataService = new DataService(dbString);
			fillApptsByDay(DateTime.Today, dgv_upcoming);
			SetupDateTimePickers();
			AppointmentAlert();
			LoginHistory();
		}

		private void LoginHistory()
		{
			string logFilePath = "Login_History.txt";
			using (StreamWriter writer = new StreamWriter(logFilePath, true))
			{
				writer.WriteLine($"{DateTime.UtcNow}: User {loggedInUser} logged in.");
			}
		}

		//Still needs to add a string to Strings.resx
		private void AppointmentAlert()
		{
			List<Appointment> upcomingAppointments = dataService.GetAppointmentsByUser((int)loggedInUser);
			DateTime now = DateTime.UtcNow;
			foreach (Appointment appointment in upcomingAppointments)
			{
				DateTime localStart = ConvertToLocalTime(appointment.Start);
				if (localStart > now && localStart - now <= appointmentAlertThreshold)
				{
					MessageBox.Show(string.Format(Strings.AppointmentAlert, appointment.Title, localStart));
				}
			}
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
				MessageBox.Show(Strings.ExistingCustomerSelected);
				return;
			}

			if (IsAnyNull(txt_name.Text, txt_address.Text, txt_city.Text, txt_phone.Text, txt_country.Text))
			{
				MessageBox.Show(Strings.FillOutAllFields);
				return;
			}
			if(!IsPhoneValid(txt_phone.Text))
			{
				MessageBox.Show(Strings.InvalidPhoneNumber);
				return;
			}

			DTOs.NewCustomer newCustomer = new DTOs.NewCustomer
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
				MessageBox.Show(string.Format(Strings.SuccessCustomerAdded));
				ClearFields(Tab.Customers);
			}
			else
			{
				MessageBox.Show(Strings.FailedToAdd);
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
				MessageBox.Show(Strings.SelectCustomer);
				return;
			}
			DialogResult result = MessageBox.Show(Strings.ConfirmUpdate + txt_name.Text + "?", Strings.UpdateConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{	
				if (IsAnyNull(txt_name.Text, txt_address.Text, txt_city.Text, txt_phone.Text, txt_country.Text))
				{
					MessageBox.Show(Strings.FillOutAllFields);
					return;
				}
				if (!IsPhoneValid(txt_phone.Text))
				{
					MessageBox.Show(Strings.InvalidPhoneNumber);
					return;
				}

				DTOs.UpdateCustomer updateCustomer = new DTOs.UpdateCustomer
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
				if (dataService.UpdateCustomer(updateCustomer, loggedInUser))
				{
					ClearFields(Tab.Customers);					
					MessageBox.Show(Strings.SuccessCustomerUpdated);
				}	
			}
		}

		private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
		{

		}

		private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
		{
			fillApptsByDay(monthCalendar1.SelectionStart, dgv_upcoming);
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			if (selectedCustomerId < 0)
			{
				MessageBox.Show(Strings.SelectCustomer);
				return;
			}
			DialogResult result = MessageBox.Show(Strings.ConfirmDelete + txt_name.Text + "?", Strings.DeleteConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
				MessageBox.Show(Strings.SuccessCustomerDeleted);
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
				MessageBox.Show(Strings.ExistingAppointmentSelected);
				return;
			}

			if (IsAnyNull(txt_apptTitle.Text, txt_apptDescription.Text, txt_apptLocation.Text, txt_apptContact.Text, txt_apptType.Text, txt_apptUrl.Text))
			{
				MessageBox.Show(Strings.FillOutAllFields);
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
				MessageBox.Show(Strings.InvalidAppointmentTime);
				return;
			}
			//Hit API to add appointment to database here
			if (dataService.AddAppointment(newAppointment))
			{
				//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
				MessageBox.Show(Strings.SuccessAppointmentAdded);
				ClearFields(Tab.Appointments);
			}
			else
			{
				MessageBox.Show(Strings.FailedToAdd);
			}
		}

		private void btn_appt_update_Click(object sender, EventArgs e)
		{
			DateTime start;
			DateTime end;

			if (selectedAppointmentId < 0)
			{
				MessageBox.Show(Strings.SelectAppointment);
				return;
			}

			DialogResult result = MessageBox.Show(Strings.ConfirmUpdate + txt_apptTitle.Text + "?", Strings.UpdateConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes)
			{
				if (IsAnyNull(txt_apptTitle.Text, txt_apptDescription.Text, txt_apptLocation.Text, txt_apptContact.Text, txt_apptType.Text, txt_apptUrl.Text))
				{
					MessageBox.Show(Strings.FillOutAllFields);
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
					MessageBox.Show(Strings.InvalidAppointmentTime);
					return;
				}


				//Hit API to add appointment to database here
				if (dataService.UpdateAppointment(updateAppointment))
				{
					//If good response, show success message and clear fields. If not, show error message and keep fields populated for user to try again.
					MessageBox.Show(Strings.SuccessAppointmentUpdated);
					ClearFields(Tab.Appointments);
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
				MessageBox.Show(Strings.SelectAppointment);
				return;
			}
			DialogResult result = MessageBox.Show(Strings.ConfirmDelete + txt_apptTitle.Text + "?", Strings.DeleteConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
				MessageBox.Show(Strings.SuccessAppointmentDeleted);
				ClearFields(Tab.Appointments);
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
				//Capture the row
				DataGridViewRow row = dgv_apptCustomers.CurrentRow;
				//Set a temp variable for customer ID
				int customerId = Convert.ToInt32(row.Cells["colApptTabCustId"].Value);
				//Set the global var so we know we have a customer selected
				selectedCustomerId = customerId;
				//Grab appointments for that customer
				fillApptByPerson(customerId, true);
				//Ensure fields are cleared
				ClearFields(Tab.Appointments);
				//Set the Customer Name field on the appointment tab so user knows they have a selection
				txt_apptNameView.Text = row.Cells["colApptTabCustName"].Value.ToString();
				//Ensure selected appointment is cleared
				selectedAppointmentId = -1;
			}
		}

		private void dgv_apptAppointments_Click(object sender, EventArgs e)
		{
			if (dgv_apptAppointments.CurrentRow != null)
			{
				DataGridViewRow row = dgv_apptAppointments.CurrentRow;
				selectedAppointmentId = Convert.ToInt32(row.Cells["colApptTabApptId"].Value);
				txt_apptNameView.Text = row.Cells["colName"].Value.ToString();
				txt_apptTitle.Text = row.Cells["colApptTitle"].Value.ToString();
				txt_apptDescription.Text = row.Cells["colApptDescription"].Value.ToString();
				txt_apptLocation.Text = row.Cells["colApptLocation"].Value.ToString();
				txt_apptContact.Text = row.Cells["colApptContact"].Value.ToString();
				txt_apptType.Text = row.Cells["colApptType"].Value.ToString();
				txt_apptUrl.Text = row.Cells["colApptUrl"].Value.ToString();
				dtp_dateStart.Value = Convert.ToDateTime(row.Cells["colApptStartDate"].Value);
				dtp_timeStart.Value = Convert.ToDateTime(row.Cells["colApptStartTime"].Value);
				dtp_dateEnd.Value = Convert.ToDateTime(row.Cells["colApptEndDate"].Value);
				dtp_timeEnd.Value = Convert.ToDateTime(row.Cells["colApptEndTime"].Value);				
			}
		}
	}	
}


