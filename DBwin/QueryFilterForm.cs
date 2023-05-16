using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;				// Per Take

namespace DBwin
	{
	public partial class QueryFilterForm : Form
		{
		/// <summary>
		/// Dialog data a cui è associato il form
		/// </summary>
		DialogData dd;
		/// <summary>
		/// Impostazioni
		/// </summary>
		Impostazioni imp;
		/// <summary>
		/// Dizionario con il nome del campo restituito dal database ed il controllo
		/// </summary>
		Dictionary<string, Control> ctrls;
		/// <summary>
		/// Delegate per controllo e correzione del contenuto della Dialog data
		/// </summary>
		DelegateCheckDialogData _dlgCheck;

		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		/// <param name="d">Dati della dialog</param>
		/// <param name="i">Impostazioni</param>
		public QueryFilterForm(ref DialogData d, Impostazioni imp, DelegateCheckDialogData dlgCheck)
			{
			InitializeComponent();
			dd = d;
			this.imp = imp;
			ctrls = new Dictionary<string, Control>();
			this.Text = d.TipoDialog.ToString();
			_dlgCheck = dlgCheck;
			}

		/// <summary>
		/// Abilita i controlli solo se se ha il permesso di scrittura o se è una query
		/// Disabilita se il comando della dialog è Elimina 
		/// </summary>
		/// <returns></returns>
		private bool CrtlsEnabled()
			{
			bool enabled;
			enabled =	(dd.CanWrite || dd.TipoDialog == DialogData.TipoDialogEnum.Ricerca)
						&&
						(dd.TipoDialog != DialogData.TipoDialogEnum.Elimina);
			return enabled;
			}

		/// <summary>
		/// Compone i controlli della dialog
		/// </summary>
		/// <returns></returns>
		bool SetupDialog()
			{
			bool ok = false;
			
			// Riempie la listbox con i tipi. Se il tipo di dialog è di ricerca, usa anche il tipo 'nessuno'
			int numele = Impostazioni.nomiTipi.Length - ((dd.TipoDialog == DialogData.TipoDialogEnum.Ricerca) ? 0 : 1 );
			lbType.Items.AddRange(Impostazioni.nomiTipi.Take(numele).ToArray());

			// Ricava la posizione dei controlli della prima riga
			Control ctrlLab = FindControlByName("LABEL");
			Control ctrlInp = FindControlByName("CONTROL");
			if ((ctrlLab == null) || (ctrlInp == null))
				{
				return ok;
				}
			Point pLab = ctrlLab.Location;
			Point pInp = ctrlInp.Location;
			Size szInp = ctrlInp.Size;
			int stepy = (int)(ctrlInp.Size.Height * imp.Config.step);
			ctrlLab.Visible = ctrlInp.Visible = false;

			// Crea i nuovi controlli
			ok = true;
			int n = 0;
			try
				{
				foreach (DatiCampo dc in dd.Impostazioni.Campi())
					{
					if (dc.isOk)
						{
						if (dd.Impostazioni.Visibile(dc))
							{
							Label l = new Label();
							l.Text = dc.label;
							l.Location = new Point(pLab.X, pLab.Y + n * stepy);
							this.Controls.Add(l);

							switch (dc.tipo)
								{
								case Impostazioni.TipoInput.lista:
									{
									ComboBoxMod cb = new ComboBoxMod();
									cb.Location = new Point(pInp.X, pInp.Y + n * stepy);
									cb.Size = new Size(szInp.Width, szInp.Height);
									cb.Name = dc.query;
									cb.DropDownStyle = ComboBoxStyle.DropDown;
									cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
									cb.Sorted = false;				// MySql usa ORDER BY, già ordinati.

									// Abilita o disabilita il controllo 
									cb.Enabled = CrtlsEnabled();

							#warning Prima di un inserimento o consultazione, interrogare le liste (refresh con il Check)


									string[,] tabella = dd.Impostazioni.Lista(dc.tabella);
									if (tabella != null)
										{
										int rows = tabella.GetLength(0);
										int cols = tabella.GetLength(1);
										for (int i = 0; i < rows; i++)
											{
											cb.Items.Add(tabella[i, 1]);
											}
										}
									ctrls.Add(dc.query, cb);			// Aggiunge il controllo al dizionario
									this.Controls.Add(cb);
									}
								break;
								case Impostazioni.TipoInput.testo:
									{
									TextBoxMod tb = new TextBoxMod();
									tb.Location = new Point(pInp.X, pInp.Y + n * stepy);
									tb.Size = new Size(szInp.Width, szInp.Height);
									tb.Name = dc.query;
									
									// Abilita o disabilita il controllo 
									tb.Enabled = CrtlsEnabled();

									ctrls.Add(dc.query, tb);	// Aggiunge il controllo al dizionario
									this.Controls.Add(tb);
									}
								break;
								default:
									{
									throw new Exception("dc.tipo è un TipoInput non gestito");
									}
								}
							n++;		// Incrementa contatore dei controlli creati
							}
						}
					}	// Fine del foreach()

				#warning Se form di modifica di un commerciale, il campo descrizione deve essere disabilitato alle modifiche.

				// Imposta il nome del pulsante btOK
				switch(dd.TipoDialog)
					{
					case DialogData.TipoDialogEnum.Modifica:
					case DialogData.TipoDialogEnum.Elimina:		// con Elimina il pulsante viene poi disabilitato
						btOk.Text = "Aggiorna";
						break;
					case DialogData.TipoDialogEnum.Ricerca:
						btOk.Text = "Cerca...";
						break;
					case DialogData.TipoDialogEnum.Nuovo:
						btOk.Text = "Inserisci";
						break;
					}

				// Blocca la scelta del tipo (particolare, assieme...) per la modifica di un codice esistente.
				if((dd.TipoDialog == DialogData.TipoDialogEnum.Modifica) || (dd.TipoDialog == DialogData.TipoDialogEnum.Elimina))
					{
					lbType.Enabled = false;
					}

				// Abilita i pulsanti che richiedono la scrittura solo se se ne ha il permesso 
				btElimina.Enabled = dd.CanWrite;	
				btOk.Enabled = dd.CanWrite || dd.TipoDialog == DialogData.TipoDialogEnum.Ricerca;

				btOk.Enabled = CrtlsEnabled();
				}
			catch (Exception e)
				{
				ok = false;
				MessageBox.Show(e.ToString());
				}
			return ok;
			}

		/// <summary>
		/// Copia i dati dall'oggetto DialogData nel form
		/// </summary>
		/// <returns></returns>
		bool FillDialog()
			{
			bool ok = true;

			foreach (DatiCampo dc in dd.Impostazioni.Campi())
				{
				if (dc.isOk)
					{
					if (ctrls.ContainsKey(dc.query))
						{
						switch (dc.tipo)
							{
							case Impostazioni.TipoInput.lista:
								{
								ComboBoxMod cb = (ComboBoxMod)ctrls[dc.query];
								int sel = dd.GetSelezione(dc.query);
								if ((sel >= 0) && (sel < cb.Items.Count))
									{
									bool cbEnabled = cb.Enabled;
									cb.Enabled = true;
									cb.SelectedIndex = dd.GetSelezione(dc.query);
									cb.Enabled = cbEnabled;
									}
								}
							break;
							case Impostazioni.TipoInput.testo:
								{
								TextBoxMod tb = (TextBoxMod)ctrls[dc.query];
								bool tbEnabled = tb.Enabled;
								tb.Enabled = true;
								tb.Text = dd.GetTesto(dc.query);
								tb.Enabled = tbEnabled;
								}
							break;
							}
						}
					}
				}

			switch(dd.Tipo)
				{
				case Impostazioni.TipoCodice.assieme:
				case Impostazioni.TipoCodice.particolare:
				case Impostazioni.TipoCodice.commerciale:
				case Impostazioni.TipoCodice.schema:
					{
					lbType.SelectedIndex = (int)dd.Tipo;
					}
					break;
				default:
					{
					lbType.SelectedIndex = 0;
					}
					break;
				}
			return ok;
			}

		/// <summary>
		/// Copia i dati dal form all'oggetto DialogData
		/// </summary>
		/// <returns></returns>		
		bool ReadDialogData()
			{
			bool ok = true;
			foreach (DatiCampo dc in dd.Impostazioni.Campi())
				{
				if (dc.isOk)
					{
					if (ctrls.ContainsKey(dc.query))
						{
						switch (dc.tipo)
							{
							case Impostazioni.TipoInput.lista:
								{
								ComboBox cb = (ComboBox)ctrls[dc.query];
								if(cb.SelectedIndex != -1)
									{
									dd.SetSelezione(dc.query, cb.SelectedIndex);	// Memorizza l'indice (elemento esistente)...
									}
								else
									{
									dd.SetSelezione(dc.query, cb.Text);				// ...oppure il testo (elemento non ancora registrato)
									}
								}
							break;
							case Impostazioni.TipoInput.testo:
								{
								TextBox tb = (TextBox)ctrls[dc.query];
								dd.SetTesto(dc.query, tb.Text);
								}
							break;
							}
						}
					}
				}
			foreach (Impostazioni.TipoCodice t in Enum.GetValues(typeof(Impostazioni.TipoCodice)))
				{
				if ((int)t == lbType.SelectedIndex)
					{
					dd.Tipo = t;
					}
				}
			return ok;
			}

		/// <summary>
		/// Controlla e corregge i dati dell'oggetto DialogData
		/// </summary>
		/// <returns>true se dati corretti, false se ci sono errori</returns>
		bool CheckDialog(bool correggi)
			{
			bool ok = false;
			if(_dlgCheck != null)
				ok = _dlgCheck(ref dd, correggi);
			return ok;
			}
		/// <summary>
		/// Mostra e nasconde i controlli di input in base alla selezione della list box (assieme, particolare...)
		/// </summary>
		/// <param name="indice"></param>
		public void VisibilitaControlli(int indice)
			{
			foreach (DatiCampo dc in dd.Impostazioni.Campi())
				{
				if (dc.isOk)
					{
					bool vis = dd.Impostazioni.Visibile(dc, (Impostazioni.TipoCodice)indice);
					if (ctrls.ContainsKey(dc.query))
						{
						ctrls[dc.query].Visible = vis;
						}
					}
				}
			}

		/// <summary>
		/// Trova un controllo in base al nome
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		Control FindControlByName(string name)
			{
			Control cb = null;
			foreach (Control c in this.Controls)
				{
				if (c.Name == name)
					{
					cb = c;
					break;
					}
				}
			return cb;
			}

		/// <summary>
		/// Handler del caricamento del Form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void QueryFilterFormNew_Load(object sender, EventArgs e)
			{
			if (!SetupDialog())
				{
				MessageBox.Show("Errore in SetupDialog()");
				this.Close();
				}
			if (!FillDialog())
				{
				MessageBox.Show("Errore in FillDialog()");
				this.Close();
				}
			// lbType.SelectedIndex = 0; // Genera evento SelectedIndexChanged. Non serve chiamare VisibilitaControlli()
			// lbType.SelectedIndex viene già chiamato in FillDialog();
			}

		/// <summary>
		/// Handler del cambio selezione
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lbType_SelectedIndexChanged(object sender, EventArgs e)
			{
			VisibilitaControlli(lbType.SelectedIndex);
			}

		/// <summary>
		/// Conferma chiusura con pulsante btOK, controlla e corregge i dati
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btOk_Click(object sender, EventArgs e)
			{
			ReadDialogData();
			if(!CheckDialog(false))
				{
				if(MessageBox.Show("Dati incompleti o errati. Correggere ?","Correzione", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
					CheckDialog(true);
					FillDialog();
					}
				else
					{
					DialogResult = DialogResult.Cancel;
					dd.DdResult = DialogData.DialogDataResult.Annulla;
					Close();
					}
				}
			else
				{
				DialogResult = DialogResult.OK;
				dd.DdResult = DialogData.DialogDataResult.Scrivi;
				Close();
				}
			}

		/// <summary>
		/// Confermata chiusura con Cancel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btCancel_Click(object sender, EventArgs e)
			{
			DialogResult = DialogResult.Cancel;
			dd.DdResult = DialogData.DialogDataResult.Annulla;
			Close();
			}

		/// <summary>
		/// Confermata chiusura con Elimina
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btElimina_Click(object sender, EventArgs e)
			{
			DialogResult = DialogResult.OK;
			dd.DdResult = DialogData.DialogDataResult.Elimina;
			Close();
			}
		}
	}
