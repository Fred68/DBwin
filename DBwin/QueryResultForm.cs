using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DBwin
	{
	public partial class QueryResultForm : Form
		{
		Risposta res;
		Impostazioni imp;
		DialogData.TipoDialogEnum _opDoppioClick;

		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		/// <param name="title"></param>
		public QueryResultForm(string title,  Impostazioni imp, DialogData.TipoDialogEnum opDoppioClick)
			{
			InitializeComponent();
			this.Text = title;
			this.imp = imp;
			this._opDoppioClick = opDoppioClick;
			}

		/// <summary>
		/// Riempie il dataGridView con righe e colonne della risposta
		/// </summary>
		/// <param name="_res"></param>
		public void SetResponse(Risposta _res)
			{
			res = _res;
			dataGridView1.Columns.Clear();
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
		private async void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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
		}
	}
