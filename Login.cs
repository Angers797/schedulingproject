using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Text.Json;

namespace C969_Project
{
	public partial class Login : Form
	{
		TimeZoneInfo localZone;
		public Login()
		{
			InitializeComponent();
		}

		private void Main_Load(object sender, EventArgs e)
		{
			populateDatabase();
			localZone = TimeZoneInfo.Local;
			RegionInfo userRegion = RegionInfo.CurrentRegion;
			string localZoneName = localZone.DisplayName;
			string countryName = userRegion.DisplayName;	

			lbl_userLocalZone.Text = $"Local time zone: {localZoneName}";			
			lbl_userRegionDisplay.Text = $"Region: {userRegion.EnglishName}";			
		}

		private void populateDatabase()
		{
			try
			{
				DataService dataService = new DataService("server = localhost; user = sqlUser; database = client_schedule; port = 3306; password = Passw0rd!;AllowUserVariables = True;allowMultiQueries=true");
				int id = dataService.addTestUser();
				dataService.populateDatabase(id);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error populating database: {ex.Message}");
			}
		}

		private void rbtn_english_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void rbtn_spanish_CheckedChanged(object sender, EventArgs e)
		{
					
		}

		private void btn_exit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_login_Click(object sender, EventArgs e)
		{
			DataService dataService = new DataService("server=localhost;user=sqlUser;database=client_schedule;port=3306;password=Passw0rd!;AllowUserVariables=True;AllowMultipleQueries=true");
			//Add Validation for username and password fields
			string user = txt_userName.Text;
			string pass = txt_password.Text;
			int userId = dataService.AuthenticateUser(user, pass);
			//Then
			if (userId == -1)
			{
				MessageBox.Show("Invalid username or password. Please try again.");
				return;
			}
			else
			{
				Main main = new Main(localZone, userId);
				main.Show();
				this.Hide();
			}	
		}
	}
}
