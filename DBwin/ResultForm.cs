using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace DBwin
	{
	public partial class ResultForm : Form
		{
		Risposta res;
		Impostazioni imp;
		DialogData.TipoDialogEnum _opDoppioClick;
		DialogData ddq;

		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		/// <param name="title"></param>
		public ResultForm(string title, Impostazioni imp, DialogData dDquery, DialogData.TipoDialogEnum opDoppioClick)
			{
			InitializeComponent();
			this.Text = title;
			this.imp = imp;
			this._opDoppioClick = opDoppioClick;
			this.ddq = dDquery;
			}

		/// <summary>
		/// Riempie il dataGridView con righe e colonne della risposta
		/// </summary>
		/// <param name="_res"></param>
		public void SetResponse(Risposta _res)
			{
			res = _res;
			dataGridView1.Columns.Clear();
			#warning Forse superfluo dataGridView.Colums.Clear() e dataGridView.Rows.Clear()
			dataGridView1.Rows.Clear();
			dataGridView1.ColumnCount = res.colonne;
			for (int c = 0; c < res.colonne; c++)
				{
				dataGridView1.Columns[c].Name = res.titoli[c];
				}
			for (int r = 0; r < res.righe; r++)
				{
				dataGridView1.Rows.Add(res.valori[r]);
				}
			Invalidate();
			}

		/// <summary>
		/// Handler del doppio click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
			{
			string cod, mod;
			if(GetCodice(e.RowIndex, out cod, out mod))
				{
				DialogData dd = await imp.Mf.DialogDataDaCodiceSingolo(cod, mod);
				dd.TipoDialog = _opDoppioClick;
				dd.CanWrite = imp.Mf.CanWrite;

				if(imp.Mf.QueryDialog(ref dd))		// In DoubleClick (analogo a modifica codice singolo)...
					{
					imp.Mf.ApplicaComandoDialogData(dd);
					}
				}
			}

		public string RowToString(int row)
			{
			StringBuilder sb = new StringBuilder();
			sb.Append($"Row index: {row}\n");
			if( (row >= 0) && (row < dataGridView1.Rows.Count) )
				{
				for(int col=0; col<dataGridView1.Columns.Count; col++)
					{
					sb.Append(dataGridView1.Columns[col].Name + ": " + dataGridView1.Rows[row].Cells[col].Value.ToString() + Environment.NewLine);
					}
				}
			return sb.ToString();
			}

		public string GetValue(int row, string column)
			{
			string s = string.Empty;
			if( (row >= 0) && (row < dataGridView1.Rows.Count) )
				{
				for(int col=0; col<dataGridView1.Columns.Count; col++)
					{
					if(dataGridView1.Columns[col].Name == column)
						{
						s = dataGridView1.Rows[row].Cells[col].Value.ToString();
						break;
						}
					}
				}
			return s;
			}
	
		/// <summary>
		/// Estrae codice e modifica di una riga della tabella dei risultati
		/// </summary>
		/// <param name="row">riga</param>
		/// <param name="cod">codice (ref) </param>
		/// <param name="mod">modifica (ref) </param>
		/// <returns></returns>
		public bool GetCodice(int row, out string cod, out string mod)
			{
			bool ok = false;
			cod = GetValue(row, imp.Config.CampoCodice);
			mod = GetValue(row, imp.Config.CampoModifica);
			if(cod.Length>0)
				{
				ok = true;
				}
			return ok;
			}

		/// <summary>
		/// Esegue una ricerca per codice, asincrona, usando i dati della DialogData memorizzata
		/// </summary>
		/// <returns>Numero di righe trovate</returns>
		public async Task<int> Cerca()
			{
			int righe = 0;								// Azzera il risultato
			res = null;
			switch(ddq.TipoRicerca)						// Ricerca per codice o completa (su tutti i campi non vuoti)
				{
				case DialogData.TipoRicercaEnum.PerCodice:
					{
					res = await imp.Mf.CercaPerCodice(ddq.GetTesto(imp.Config.CampoCodice), ddq.GetTesto(imp.Config.CampoModifica));
					}
					break;
				default:
					MessageBox.Show("Ricerca completa mancate, ancora da scrivere !");
					break;
				}
			if(res != null)
				{	
				if( !res.isEmpty)
					{
					if(res.righe > 0)
						{
						SetResponse(res);				// Imposta il contenuto del form
						righe = res.righe;
						Show();
						}
					}
				}
			return righe;
			}

		private void ResultForm_Load(object sender, EventArgs e)
			{
			int h = btRefresh.Height + 8;
			dataGridView1.ScrollBars = ScrollBars.Both;					// Non funziona
			dataGridView1.Location = new System.Drawing.Point(0, h);
			dataGridView1.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height-h);
			dataGridView1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
			}

		/// <summary>
		/// Ripete l'ultima ricerca memorizzata con il Form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void btRefresh_Click(object sender, EventArgs e)
			{
			await Cerca();		// Ripete l'ultima ricerca, leggendo i valori aggiornati nel database
			}
		}
	}
