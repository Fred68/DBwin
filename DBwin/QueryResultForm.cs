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

		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		/// <param name="title"></param>
		public QueryResultForm(string title,  Impostazioni imp)
			{
			InitializeComponent();
			this.Text = title;
			this.imp = imp;
			}
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

		private async void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
			{
			string cod, mod;
			if(GetCodice(e.RowIndex, out cod, out mod))
				{
				Dictionary<string,string> dct = await imp.Mf.EstraiDatiCodiceSingolo(cod, mod);

				StringBuilder sb = new StringBuilder();
				foreach(var item in dct)
					{
					sb.AppendLine($"[{item.Key}]={item.Value}");
					}
#if DEBUG
				MessageBox.Show($"Selezionato codice: {cod}{mod}\n{sb.ToString()}");
#endif
				DialogData dd = new DialogData(ref imp);
				int nsetcampi;
				dd.Set(dct, out nsetcampi);
				dd.TipoDialog = DialogData.TipoDialogEnum.Modifica;
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
