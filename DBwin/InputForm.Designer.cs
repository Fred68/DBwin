namespace DBwin
	{
	partial class InputForm
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
			this.tb1 = new System.Windows.Forms.TextBox();
			this.lb1 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btAnnulla = new System.Windows.Forms.Button();
			this.btHelp = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tb1
			// 
			this.tb1.Location = new System.Drawing.Point(137, 9);
			this.tb1.Name = "tb1";
			this.tb1.Size = new System.Drawing.Size(337, 20);
			this.tb1.TabIndex = 14;
			// 
			// lb1
			// 
			this.lb1.AutoSize = true;
			this.lb1.Location = new System.Drawing.Point(12, 12);
			this.lb1.Name = "lb1";
			this.lb1.Size = new System.Drawing.Size(119, 13);
			this.lb1.TabIndex = 12;
			this.lb1.Text = "XXXXXXXXXXXXXXXX";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(497, 7);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 15;
			this.btOK.Text = "Ok";
			this.btOK.UseVisualStyleBackColor = true;
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btAnnulla
			// 
			this.btAnnulla.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btAnnulla.Location = new System.Drawing.Point(497, 37);
			this.btAnnulla.Name = "btAnnulla";
			this.btAnnulla.Size = new System.Drawing.Size(75, 23);
			this.btAnnulla.TabIndex = 16;
			this.btAnnulla.Text = "Annulla";
			this.btAnnulla.UseVisualStyleBackColor = true;
			this.btAnnulla.Click += new System.EventHandler(this.Annulla_Click);
			// 
			// btHelp
			// 
			this.btHelp.Location = new System.Drawing.Point(359, 37);
			this.btHelp.Name = "btHelp";
			this.btHelp.Size = new System.Drawing.Size(132, 23);
			this.btHelp.TabIndex = 17;
			this.btHelp.Text = "?";
			this.btHelp.UseVisualStyleBackColor = true;
			this.btHelp.Click += new System.EventHandler(this.btHelp_Click);
			// 
			// InputForm
			// 
			this.AcceptButton = this.btOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btAnnulla;
			this.ClientSize = new System.Drawing.Size(590, 88);
			this.Controls.Add(this.btHelp);
			this.Controls.Add(this.btAnnulla);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.tb1);
			this.Controls.Add(this.lb1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "InputForm";
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion
		private System.Windows.Forms.TextBox tb1;
		private System.Windows.Forms.Label lb1;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btAnnulla;
		private System.Windows.Forms.Button btHelp;
		}
	}