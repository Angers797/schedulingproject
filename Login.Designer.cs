namespace C969_Project
{
	partial class Login
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
			this.txt_userName = new System.Windows.Forms.TextBox();
			this.txt_password = new System.Windows.Forms.TextBox();
			this.lbl_username = new System.Windows.Forms.Label();
			this.lbl_password = new System.Windows.Forms.Label();
			this.lbl_login = new System.Windows.Forms.Label();
			this.rbtn_english = new System.Windows.Forms.RadioButton();
			this.rbtn_spanish = new System.Windows.Forms.RadioButton();
			this.btn_exit = new System.Windows.Forms.Button();
			this.btn_login = new System.Windows.Forms.Button();
			this.lbl_userLocalZone = new System.Windows.Forms.Label();
			this.lbl_userRegionDisplay = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txt_userName
			// 
			resources.ApplyResources(this.txt_userName, "txt_userName");
			this.txt_userName.Name = "txt_userName";
			// 
			// txt_password
			// 
			resources.ApplyResources(this.txt_password, "txt_password");
			this.txt_password.Name = "txt_password";
			this.txt_password.UseSystemPasswordChar = true;
			// 
			// lbl_username
			// 
			resources.ApplyResources(this.lbl_username, "lbl_username");
			this.lbl_username.Name = "lbl_username";
			// 
			// lbl_password
			// 
			resources.ApplyResources(this.lbl_password, "lbl_password");
			this.lbl_password.Name = "lbl_password";
			// 
			// lbl_login
			// 
			resources.ApplyResources(this.lbl_login, "lbl_login");
			this.lbl_login.Name = "lbl_login";
			// 
			// rbtn_english
			// 
			resources.ApplyResources(this.rbtn_english, "rbtn_english");
			this.rbtn_english.Checked = true;
			this.rbtn_english.Name = "rbtn_english";
			this.rbtn_english.TabStop = true;
			this.rbtn_english.UseVisualStyleBackColor = true;
			this.rbtn_english.CheckedChanged += new System.EventHandler(this.rbtn_english_CheckedChanged);
			// 
			// rbtn_spanish
			// 
			resources.ApplyResources(this.rbtn_spanish, "rbtn_spanish");
			this.rbtn_spanish.Name = "rbtn_spanish";
			this.rbtn_spanish.UseVisualStyleBackColor = true;
			this.rbtn_spanish.CheckedChanged += new System.EventHandler(this.rbtn_spanish_CheckedChanged);
			// 
			// btn_exit
			// 
			resources.ApplyResources(this.btn_exit, "btn_exit");
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.UseVisualStyleBackColor = true;
			this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
			// 
			// btn_login
			// 
			resources.ApplyResources(this.btn_login, "btn_login");
			this.btn_login.Name = "btn_login";
			this.btn_login.UseVisualStyleBackColor = true;
			this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
			// 
			// lbl_userLocalZone
			// 
			resources.ApplyResources(this.lbl_userLocalZone, "lbl_userLocalZone");
			this.lbl_userLocalZone.Name = "lbl_userLocalZone";

			// 
			// lbl_userRegionDisplay
			// 
			resources.ApplyResources(this.lbl_userRegionDisplay, "lbl_userRegionDisplay");
			this.lbl_userRegionDisplay.Name = "lbl_userRegionDisplay";

			// 
			// Login
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lbl_userRegionDisplay);
			this.Controls.Add(this.lbl_userLocalZone);
			this.Controls.Add(this.btn_login);
			this.Controls.Add(this.btn_exit);
			this.Controls.Add(this.rbtn_spanish);
			this.Controls.Add(this.rbtn_english);
			this.Controls.Add(this.lbl_login);
			this.Controls.Add(this.lbl_password);
			this.Controls.Add(this.lbl_username);
			this.Controls.Add(this.txt_password);
			this.Controls.Add(this.txt_userName);
			this.Name = "Login";
			this.Load += new System.EventHandler(this.Main_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txt_userName;
		private System.Windows.Forms.TextBox txt_password;
		private System.Windows.Forms.Label lbl_username;
		private System.Windows.Forms.Label lbl_password;
		private System.Windows.Forms.Label lbl_login;
		private System.Windows.Forms.RadioButton rbtn_english;
		private System.Windows.Forms.RadioButton rbtn_spanish;
		private System.Windows.Forms.Button btn_exit;
		private System.Windows.Forms.Button btn_login;
		private System.Windows.Forms.Label lbl_userLocalZone;
		private System.Windows.Forms.Label lbl_userRegionDisplay;
	}
}

