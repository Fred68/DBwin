namespace DBwin
	{
	partial class QueryFilterForm
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
			if (disposing && (components != null)) {
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
			this.lbType = new System.Windows.Forms.ListBox();
			this.CONTROL = new System.Windows.Forms.TextBox();
			this.LABEL = new System.Windows.Forms.Label();
			this.btOk = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btElimina = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbType
			// 
			this.lbType.FormattingEnabled = true;
			this.lbType.Location = new System.Drawing.Point(401, 67);
			this.lbType.Name = "lbType";
			this.lbType.Size = new System.Drawing.Size(142, 69);
			this.lbType.TabIndex = 23;
			this.lbType.SelectedIndexChanged += new System.EventHandler(this.lbType_SelectedIndexChanged);
			// 
			// CONTROL
			// 
			this.CONTROL.Location = new System.Drawing.Point(134, 67);
			this.CONTROL.Name = "CONTROL";
			this.CONTROL.Size = new System.Drawing.Size(203, 20);
			this.CONTROL.TabIndex = 22;
			// 
			// LABEL
			// 
			this.LABEL.AutoSize = true;
			this.LABEL.Location = new System.Drawing.Point(33, 70);
			this.LABEL.Name = "LABEL";
			this.LABEL.Size = new System.Drawing.Size(40, 13);
			this.LABEL.TabIndex = 21;
			this.LABEL.Text = "Codice";
			// 
			// btOk
			// 
			this.btOk.Location = new System.Drawing.Point(474, 255);
			this.btOk.Name = "btOk";
			this.btOk.Size = new System.Drawing.Size(75, 23);
			this.btOk.TabIndex = 20;
			this.btOk.Text = "Scrivi";
			this.btOk.UseVisualStyleBackColor = true;
			this.btOk.Click += new System.EventHandler(this.btOk_Click);
			// 
			// btCancel
			// 
			this.btCancel.Location = new System.Drawing.Point(474, 313);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 24;
			this.btCancel.Text = "Chiudi";
			this.btCancel.UseVisualStyleBackColor = true;
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btElimina
			// 
			this.btElimina.Location = new System.Drawing.Point(474, 284);
			this.btElimina.Name = "btElimina";
			this.btElimina.Size = new System.Drawing.Size(75, 23);
			this.btElimina.TabIndex = 25;
			this.btElimina.Text = "Elimina";
			this.btElimina.UseVisualStyleBackColor = true;
			this.btElimina.Click += new System.EventHandler(this.btElimina_Click);
			// 
			// QueryFilterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(561, 362);
			this.Controls.Add(this.btElimina);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.lbType);
			this.Controls.Add(this.CONTROL);
			this.Controls.Add(this.LABEL);
			this.Controls.Add(this.btOk);
			this.Name = "QueryFilterForm";
			this.Text = "-";
			this.Load += new System.EventHandler(this.QueryFilterFormNew_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.ListBox lbType;
		private System.Windows.Forms.TextBox CONTROL;
		private System.Windows.Forms.Label LABEL;
		private System.Windows.Forms.Button btOk;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btElimina;
		}
	}