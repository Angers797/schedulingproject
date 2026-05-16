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
			this.txt_userName = new System.Windows.Forms.TextBox();
			this.txt_password = new System.Windows.Forms.TextBox();
			this.lbl_username = new System.Windows.Forms.Label();
			this.lbl_password = new System.Windows.Forms.Label();
			this.lbl_login = new System.Windows.Forms.Label();
			this.lbl_userLocalZone = new System.Windows.Forms.Label();
			this.lbl_userRegionDisplay = new System.Windows.Forms.Label();
			this.rbtn_english = new System.Windows.Forms.RadioButton();
			this.rbtn_spanish = new System.Windows.Forms.RadioButton();
			this.btn_exit = new System.Windows.Forms.Button();
			this.btn_login = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txt_userName
			// 
			this.txt_userName.Location = new System.Drawing.Point(260, 169);
			this.txt_userName.Name = "txt_userName";
			this.txt_userName.Size = new System.Drawing.Size(100, 20);
			this.txt_userName.TabIndex = 0;
			// 
			// txt_password
			// 
			this.txt_password.Location = new System.Drawing.Point(260, 212);
			this.txt_password.Name = "txt_password";
			this.txt_password.PasswordChar = '*';
			this.txt_password.Size = new System.Drawing.Size(100, 20);
			this.txt_password.TabIndex = 1;
			this.txt_password.UseSystemPasswordChar = true;
			// 
			// lbl_username
			// 
			this.lbl_username.AutoSize = true;
			this.lbl_username.Location = new System.Drawing.Point(161, 175);
			this.lbl_username.Name = "lbl_username";
			this.lbl_username.Size = new System.Drawing.Size(55, 13);
			this.lbl_username.TabIndex = 2;
			this.lbl_username.Text = "Username";
			// 
			// lbl_password
			// 
			this.lbl_password.AutoSize = true;
			this.lbl_password.Location = new System.Drawing.Point(164, 218);
			this.lbl_password.Name = "lbl_password";
			this.lbl_password.Size = new System.Drawing.Size(53, 13);
			this.lbl_password.TabIndex = 3;
			this.lbl_password.Text = "Password";
			// 
			// lbl_login
			// 
			this.lbl_login.AutoSize = true;
			this.lbl_login.Location = new System.Drawing.Point(214, 123);
			this.lbl_login.Name = "lbl_login";
			this.lbl_login.Size = new System.Drawing.Size(33, 13);
			this.lbl_login.TabIndex = 4;
			this.lbl_login.Text = "Login";
			// 
			// lbl_userLocalZone
			// 
			this.lbl_userLocalZone.AutoSize = true;
			this.lbl_userLocalZone.Location = new System.Drawing.Point(375, 358);
			this.lbl_userLocalZone.Name = "lbl_userLocalZone";
			this.lbl_userLocalZone.Size = new System.Drawing.Size(35, 13);
			this.lbl_userLocalZone.TabIndex = 7;
			this.lbl_userLocalZone.Text = "label6";
			// 
			// lbl_userRegionDisplay
			// 
			this.lbl_userRegionDisplay.AutoSize = true;
			this.lbl_userRegionDisplay.Location = new System.Drawing.Point(375, 371);
			this.lbl_userRegionDisplay.Name = "lbl_userRegionDisplay";
			this.lbl_userRegionDisplay.Size = new System.Drawing.Size(35, 13);
			this.lbl_userRegionDisplay.TabIndex = 8;
			this.lbl_userRegionDisplay.Text = "label7";
			// 
			// rbtn_english
			// 
			this.rbtn_english.AutoSize = true;
			this.rbtn_english.Checked = true;
			this.rbtn_english.Location = new System.Drawing.Point(481, 13);
			this.rbtn_english.Name = "rbtn_english";
			this.rbtn_english.Size = new System.Drawing.Size(59, 17);
			this.rbtn_english.TabIndex = 9;
			this.rbtn_english.TabStop = true;
			this.rbtn_english.Text = "English";
			this.rbtn_english.UseVisualStyleBackColor = true;
			this.rbtn_english.CheckedChanged += new System.EventHandler(this.rbtn_english_CheckedChanged);
			// 
			// rbtn_spanish
			// 
			this.rbtn_spanish.AutoSize = true;
			this.rbtn_spanish.Location = new System.Drawing.Point(586, 13);
			this.rbtn_spanish.Name = "rbtn_spanish";
			this.rbtn_spanish.Size = new System.Drawing.Size(63, 17);
			this.rbtn_spanish.TabIndex = 10;
			this.rbtn_spanish.Text = "Spanish";
			this.rbtn_spanish.UseVisualStyleBackColor = true;
			this.rbtn_spanish.CheckedChanged += new System.EventHandler(this.rbtn_spanish_CheckedChanged);
			// 
			// btn_exit
			// 
			this.btn_exit.Location = new System.Drawing.Point(586, 405);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new System.Drawing.Size(75, 23);
			this.btn_exit.TabIndex = 11;
			this.btn_exit.Text = "Exit";
			this.btn_exit.UseVisualStyleBackColor = true;
			this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
			// 
			// btn_login
			// 
			this.btn_login.Location = new System.Drawing.Point(260, 253);
			this.btn_login.Name = "btn_login";
			this.btn_login.Size = new System.Drawing.Size(100, 23);
			this.btn_login.TabIndex = 12;
			this.btn_login.Text = "Login";
			this.btn_login.UseVisualStyleBackColor = true;
			this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
			// 
			// Login
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.btn_login);
			this.Controls.Add(this.btn_exit);
			this.Controls.Add(this.rbtn_spanish);
			this.Controls.Add(this.rbtn_english);
			this.Controls.Add(this.lbl_userRegionDisplay);
			this.Controls.Add(this.lbl_userLocalZone);
			this.Controls.Add(this.lbl_login);
			this.Controls.Add(this.lbl_password);
			this.Controls.Add(this.lbl_username);
			this.Controls.Add(this.txt_password);
			this.Controls.Add(this.txt_userName);
			this.Name = "Login";
			this.Text = "Form1";
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
		private System.Windows.Forms.Label lbl_userLocalZone;
		private System.Windows.Forms.Label lbl_userRegionDisplay;
		private System.Windows.Forms.RadioButton rbtn_english;
		private System.Windows.Forms.RadioButton rbtn_spanish;
		private System.Windows.Forms.Button btn_exit;
		private System.Windows.Forms.Button btn_login;
	}
}

