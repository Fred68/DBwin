namespace DBwin
	{
	partial class MainForm
		{
		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Pulire le risorse in uso.
		/// </summary>
		/// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
		protected override void Dispose(bool disposing)
			{
			if (disposing && (components != null))
				{
				components.Dispose();
				}
			base.Dispose(disposing);
			}

		#region Codice generato da Progettazione Windows Form

		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
			{
			this.components = new System.ComponentModel.Container();
			this.rtbMessages = new System.Windows.Forms.RichTextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.connessioneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mLogin = new System.Windows.Forms.ToolStripMenuItem();
			this.mloginAutomatico = new System.Windows.Forms.ToolStripMenuItem();
			this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.memorizzaDatiLoginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mCheck = new System.Windows.Forms.ToolStripMenuItem();
			this.mScrittura = new System.Windows.Forms.ToolStripMenuItem();
			this.mLogout = new System.Windows.Forms.ToolStripMenuItem();
			this.mChangePassword = new System.Windows.Forms.ToolStripMenuItem();
			this.mChangeAccessPassword = new System.Windows.Forms.ToolStripMenuItem();
			this.mChangeWritePassword = new System.Windows.Forms.ToolStripMenuItem();
			this.esciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lOGINPippoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.visualizzaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnVedi = new System.Windows.Forms.ToolStripMenuItem();
			this.mnListeTest = new System.Windows.Forms.ToolStripMenuItem();
			this.mnQuery = new System.Windows.Forms.ToolStripMenuItem();
			this.dirtyTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.costruttoriDirtyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dIRTYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.dIRTYToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.modificaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nuovoCodiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mnModificaCodice = new System.Windows.Forms.ToolStripMenuItem();
			this.eliminaCodiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tsStato = new System.Windows.Forms.ToolStripStatusLabel();
			this.timerRefresh = new System.Windows.Forms.Timer(this.components);
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// rtbMessages
			// 
			this.rtbMessages.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.rtbMessages.Location = new System.Drawing.Point(0, 116);
			this.rtbMessages.MaxLength = 20;
			this.rtbMessages.Name = "rtbMessages";
			this.rtbMessages.Size = new System.Drawing.Size(367, 139);
			this.rtbMessages.TabIndex = 8;
			this.rtbMessages.TabStop = false;
			this.rtbMessages.Text = "";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connessioneToolStripMenuItem,
            this.visualizzaToolStripMenuItem,
            this.modificaToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(367, 24);
			this.menuStrip1.TabIndex = 11;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// connessioneToolStripMenuItem
			// 
			this.connessioneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mLogin,
            this.mloginAutomatico,
            this.mCheck,
            this.mScrittura,
            this.mLogout,
            this.mChangePassword,
            this.lOGINPippoToolStripMenuItem,
            this.esciToolStripMenuItem});
			this.connessioneToolStripMenuItem.Name = "connessioneToolStripMenuItem";
			this.connessioneToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
			this.connessioneToolStripMenuItem.Text = "Connessione";
			// 
			// mLogin
			// 
			this.mLogin.Name = "mLogin";
			this.mLogin.Size = new System.Drawing.Size(180, 22);
			this.mLogin.Text = "Login";
			this.mLogin.Click += new System.EventHandler(this.Login_Click);
			// 
			// mloginAutomatico
			// 
			this.mloginAutomatico.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.memorizzaDatiLoginToolStripMenuItem});
			this.mloginAutomatico.Name = "mloginAutomatico";
			this.mloginAutomatico.Size = new System.Drawing.Size(180, 22);
			this.mloginAutomatico.Text = "Login automatico";
			// 
			// loginToolStripMenuItem
			// 
			this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
			this.loginToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.loginToolStripMenuItem.Text = "Login !";
			this.loginToolStripMenuItem.Click += new System.EventHandler(this.LoginRapido_Click);
			// 
			// memorizzaDatiLoginToolStripMenuItem
			// 
			this.memorizzaDatiLoginToolStripMenuItem.Name = "memorizzaDatiLoginToolStripMenuItem";
			this.memorizzaDatiLoginToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.memorizzaDatiLoginToolStripMenuItem.Text = "Memorizza dati login";
			this.memorizzaDatiLoginToolStripMenuItem.Click += new System.EventHandler(this.MemorizzaDatiLogin_Click);
			// 
			// mCheck
			// 
			this.mCheck.Name = "mCheck";
			this.mCheck.Size = new System.Drawing.Size(180, 22);
			this.mCheck.Text = "Check";
			this.mCheck.Click += new System.EventHandler(this.Check_Click);
			// 
			// mScrittura
			// 
			this.mScrittura.Name = "mScrittura";
			this.mScrittura.Size = new System.Drawing.Size(180, 22);
			this.mScrittura.Text = "Abilita scrittura";
			this.mScrittura.Click += new System.EventHandler(this.AbilitaScrittura_Click);
			// 
			// mLogout
			// 
			this.mLogout.Name = "mLogout";
			this.mLogout.Size = new System.Drawing.Size(180, 22);
			this.mLogout.Text = "Logout";
			this.mLogout.Click += new System.EventHandler(this.Logout_Click);
			// 
			// mChangePassword
			// 
			this.mChangePassword.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mChangeAccessPassword,
            this.mChangeWritePassword});
			this.mChangePassword.Name = "mChangePassword";
			this.mChangePassword.Size = new System.Drawing.Size(180, 22);
			this.mChangePassword.Text = "Cambia password";
			// 
			// mChangeAccessPassword
			// 
			this.mChangeAccessPassword.Name = "mChangeAccessPassword";
			this.mChangeAccessPassword.Size = new System.Drawing.Size(183, 22);
			this.mChangeAccessPassword.Text = "Password di accesso";
			this.mChangeAccessPassword.Click += new System.EventHandler(this.ChangeAccessPassword_Click);
			// 
			// mChangeWritePassword
			// 
			this.mChangeWritePassword.Name = "mChangeWritePassword";
			this.mChangeWritePassword.Size = new System.Drawing.Size(183, 22);
			this.mChangeWritePassword.Text = "Password di scrittura";
			this.mChangeWritePassword.Click += new System.EventHandler(this.ChangeWritePassword_Click);
			// 
			// esciToolStripMenuItem
			// 
			this.esciToolStripMenuItem.Name = "esciToolStripMenuItem";
			this.esciToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.esciToolStripMenuItem.Text = "Esci";
			this.esciToolStripMenuItem.Click += new System.EventHandler(this.EsciToolStripMenuItem_Click);
			// 
			// lOGINPippoToolStripMenuItem
			// 
			this.lOGINPippoToolStripMenuItem.ForeColor = System.Drawing.Color.Maroon;
			this.lOGINPippoToolStripMenuItem.Name = "lOGINPippoToolStripMenuItem";
			this.lOGINPippoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.lOGINPippoToolStripMenuItem.Text = "LOGIN pippo";
			this.lOGINPippoToolStripMenuItem.Click += new System.EventHandler(this.LOGINPippoToolStripMenuItem_Click);
			// 
			// visualizzaToolStripMenuItem
			// 
			this.visualizzaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnVedi,
            this.mnListeTest,
            this.mnQuery,
            this.dirtyTestToolStripMenuItem});
			this.visualizzaToolStripMenuItem.Name = "visualizzaToolStripMenuItem";
			this.visualizzaToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
			this.visualizzaToolStripMenuItem.Text = "Visualizza";
			// 
			// mnVedi
			// 
			this.mnVedi.Name = "mnVedi";
			this.mnVedi.Size = new System.Drawing.Size(144, 22);
			this.mnVedi.Text = "Vedi";
			this.mnVedi.Click += new System.EventHandler(this.Vedi_Click);
			// 
			// mnListeTest
			// 
			this.mnListeTest.Name = "mnListeTest";
			this.mnListeTest.Size = new System.Drawing.Size(144, 22);
			this.mnListeTest.Text = "Liste [debug]";
			this.mnListeTest.Click += new System.EventHandler(this.ListeTest_Click);
			// 
			// mnQuery
			// 
			this.mnQuery.Name = "mnQuery";
			this.mnQuery.Size = new System.Drawing.Size(144, 22);
			this.mnQuery.Text = "Query";
			this.mnQuery.Click += new System.EventHandler(this.Query_Click);
			// 
			// dirtyTestToolStripMenuItem
			// 
			this.dirtyTestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.costruttoriDirtyToolStripMenuItem,
            this.dIRTYToolStripMenuItem,
            this.dIRTYToolStripMenuItem1});
			this.dirtyTestToolStripMenuItem.Name = "dirtyTestToolStripMenuItem";
			this.dirtyTestToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.dirtyTestToolStripMenuItem.Text = "Dirty [debug]";
			// 
			// costruttoriDirtyToolStripMenuItem
			// 
			this.costruttoriDirtyToolStripMenuItem.Name = "costruttoriDirtyToolStripMenuItem";
			this.costruttoriDirtyToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.costruttoriDirtyToolStripMenuItem.Text = "Costruttori dirty";
			this.costruttoriDirtyToolStripMenuItem.Click += new System.EventHandler(this.costruttoriDirtyToolStripMenuItem_Click);
			// 
			// dIRTYToolStripMenuItem
			// 
			this.dIRTYToolStripMenuItem.Name = "dIRTYToolStripMenuItem";
			this.dIRTYToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.dIRTYToolStripMenuItem.Text = "Materiali dirty";
			this.dIRTYToolStripMenuItem.Click += new System.EventHandler(this.materialiDirtyToolStripMenuItem_Click);
			// 
			// dIRTYToolStripMenuItem1
			// 
			this.dIRTYToolStripMenuItem1.Name = "dIRTYToolStripMenuItem1";
			this.dIRTYToolStripMenuItem1.Size = new System.Drawing.Size(158, 22);
			this.dIRTYToolStripMenuItem1.Text = "Prodotti dirty";
			this.dIRTYToolStripMenuItem1.Click += new System.EventHandler(this.prodottiiDirtyToolStripMenuItem_Click);
			// 
			// modificaToolStripMenuItem
			// 
			this.modificaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nuovoCodiceToolStripMenuItem,
            this.mnModificaCodice,
            this.eliminaCodiceToolStripMenuItem});
			this.modificaToolStripMenuItem.Name = "modificaToolStripMenuItem";
			this.modificaToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
			this.modificaToolStripMenuItem.Text = "Modifica";
			// 
			// nuovoCodiceToolStripMenuItem
			// 
			this.nuovoCodiceToolStripMenuItem.Name = "nuovoCodiceToolStripMenuItem";
			this.nuovoCodiceToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.nuovoCodiceToolStripMenuItem.Text = "Nuovo codice";
			this.nuovoCodiceToolStripMenuItem.Click += new System.EventHandler(this.nuovoCodice_Click);
			// 
			// mnModificaCodice
			// 
			this.mnModificaCodice.Name = "mnModificaCodice";
			this.mnModificaCodice.Size = new System.Drawing.Size(204, 22);
			this.mnModificaCodice.Text = "Modifica codice [debug]";
			this.mnModificaCodice.Click += new System.EventHandler(this.vediCodiceSingolo_Click);
			// 
			// eliminaCodiceToolStripMenuItem
			// 
			this.eliminaCodiceToolStripMenuItem.Name = "eliminaCodiceToolStripMenuItem";
			this.eliminaCodiceToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
			this.eliminaCodiceToolStripMenuItem.Text = "Elimina codice";
			this.eliminaCodiceToolStripMenuItem.Click += new System.EventHandler(this.eliminaCodiceToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.About_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStato});
			this.statusStrip1.Location = new System.Drawing.Point(0, 94);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(367, 22);
			this.statusStrip1.TabIndex = 12;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tsStato
			// 
			this.tsStato.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tsStato.Name = "tsStato";
			this.tsStato.Size = new System.Drawing.Size(40, 17);
			this.tsStato.Text = "Stato";
			// 
			// timerRefresh
			// 
			this.timerRefresh.Tick += new System.EventHandler(this.onRefresh);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(367, 255);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.rtbMessages);
			this.Controls.Add(this.menuStrip1);
			this.Name = "MainForm";
			this.Text = "Database disegni";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion
		private System.Windows.Forms.RichTextBox rtbMessages;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem connessioneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mLogin;
		private System.Windows.Forms.ToolStripMenuItem mCheck;
		private System.Windows.Forms.ToolStripMenuItem mLogout;
		private System.Windows.Forms.ToolStripMenuItem mChangePassword;
		private System.Windows.Forms.ToolStripMenuItem visualizzaToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnVedi;
		private System.Windows.Forms.ToolStripMenuItem modificaToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mChangeAccessPassword;
		private System.Windows.Forms.ToolStripMenuItem mChangeWritePassword;
		private System.Windows.Forms.ToolStripMenuItem esciToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mScrittura;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel tsStato;
		private System.Windows.Forms.Timer timerRefresh;
		private System.Windows.Forms.ToolStripMenuItem mloginAutomatico;
		private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem memorizzaDatiLoginToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnListeTest;
		private System.Windows.Forms.ToolStripMenuItem mnQuery;
		private System.Windows.Forms.ToolStripMenuItem lOGINPippoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nuovoCodiceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dirtyTestToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem costruttoriDirtyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dIRTYToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem dIRTYToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem eliminaCodiceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnModificaCodice;
		}
	}

