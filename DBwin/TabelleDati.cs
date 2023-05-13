using System;
using System.Collections.Generic;
using System.Linq;

namespace DBwin
	{
	public class TabelleDati
		{
		public struct NomeFlag
			{
			public string nome;
			public string flag;
			public int nCampo;
			public bool bReadOnly;
			public NomeFlag(string nam, string flg, int nc, bool ro)
				{
				nome = nam;
				flag = flg;
				nCampo = nc;
				bReadOnly = ro;
				}
			}

		public static int numTipi => Enum.GetNames(typeof(Tipi)).Length - 1;
		public enum Tipi { assieme = 0, schema, particolare, commerciale, nessuno };

		/// <summary>
		/// Nomi delle tabelle del database da caricare, basato su:
		/// # separatore
		/// Tipo di input: L listbox, T textbox, C checkbox
		/// 1011 Flag per visualizzarlo nel form per Assiemi, schemi, particolari, commerciali
		/// </summary>
		string[] tabNames, tabFlags;
		
		/// <summary>
		/// Item speciali, all'inizio delle tabelle
		/// </summary>
		readonly string[] tabSpecialItems = new string[] { "<Ignora>", "<Vuoto>" };

		/// <summary>
		/// Nomi dei campi semplici del database
		/// </summary>
		string[] txtNames, txtFlags;
		int[] txtNumC, tabNumC;
		bool[] txtReadOnly, tabReadOnly;        // Campi txt se readonly

		/// <summary>
		/// Corrispondenza nome tabella database e indice della tabella
		/// </summary>
		Dictionary<string, int> indici;

		/// <summary>
		/// Tabelle con i contenuti delle tabelle, letti dal database, ed i rispettivi ID.
		/// </summary>
		string[][,] tabelle;

		/// <summary>
		/// Offset indici tabelle se aggiunti item speciali
		/// </summary>
		int offsetSpecialItems;

		public int OffsetSpecialItems => offsetSpecialItems;

		/// <summary>
		/// COSTRUTTORE della classe
		/// </summary>
		public TabelleDati(List<string> cfg)
			{
			ReadConfig(cfg);
			indici = new Dictionary<string, int>();
			int len = tabNames.Length;
			for (int i = 0; i < len; i++)
				{
				indici.Add(tabNames[i], i);
				}
			tabelle = new string[len][,];
			offsetSpecialItems = tabSpecialItems.Length;
			}

		/// <summary>
		/// Legge la configurazione del database e crea le tabelle dei dati
		/// </summary>
		/// <param name="cfgFile"></param>
		/// <returns></returns>
		bool ReadConfig(List<string> cfgFile)
			{
			bool ok = true;
			List<string> cfg;
			if (cfgFile == null)
				{
				#warning Disabilitare linee. Segnalare errore
				cfg = new List<string>(new string[]
					{
					"codice			#T	1111	@1	*	",
					"modifica		#T	1111	@2	*	",
					"descrizione	#T	1110	@3		",
					"materiali		#L	0010	@5		",
					"prodotti		#L	0001	@7		",
					"costruttori	#L	0001	@6		",
					"modello		#T	0001	@8		",
					"dettagli		#T	0001	@9		"
					});
				}
			else
				{
				cfg = cfgFile;
				}
			List<string> tab = new List<string>();
			List<string> txt = new List<string>();
			List<string> tabf = new List<string>();
			List<string> txtf = new List<string>();
			List<int> tabNc = new List<int>();
			List<int> txtNc = new List<int>();
			List<bool> txtRo = new List<bool>();
			List<bool> tabRo = new List<bool>();

			foreach (string s in cfg)
				{
				string nam, typ, flg, nc;
				int nci = -1;
				bool ronly = false;
				nam = typ = flg = string.Empty;
				int indx1 = s.IndexOf('#');
				int indx2 = s.IndexOf('@');
				if ((indx1 != -1) && (indx2 != -1))
					{
					nc = s.Substring(indx2 + 1);            // Il numero campo è la parte dopo @
					nam = s.Substring(0, indx1);			// Il nome è nella parte prima di #
					string s2 = s.Substring(indx1 + 1, indx2 - indx1 - 1);      // La parte compresa tra # e @
					if (s2.Length > TabelleDati.numTipi)    // Se contiene almeno # ed i caratteri del flag
						{
						typ = s.Substring(indx1 + 1, 1);
						flg = s.Substring(indx1 + 2, TabelleDati.numTipi).PadRight(TabelleDati.numTipi, '0');
						}
					if (nc.Contains('*'))
						{
						ronly = true;
						nc = nc.Replace("*", "");
						}
					else
						{
						ronly = false;
						}
					if (!int.TryParse(nc, out nci))
						{
						nci = -1;
						}
					//x.Append(nam + "[#]" + s2 + "[@]" + nc + " :typ=" + typ + " :flg=" + flg + " :nci=" + nci.ToString() + "\n");
					}
				switch (typ)
					{
					case "L":
						tab.Add(nam);
						tabf.Add(flg);
						tabNc.Add(nci);
						tabRo.Add(ronly);
						break;
					case "T":
						txt.Add(nam);
						txtf.Add(flg);
						txtNc.Add(nci);
						txtRo.Add(ronly);
						break;
					default:
						break;
					}
				}

			tabNames = tab.ToArray();
			txtNames = txt.ToArray();
			tabFlags = tabf.ToArray();
			txtFlags = txtf.ToArray();
			txtNumC = txtNc.ToArray();
			tabNumC = tabNc.ToArray();
			txtReadOnly = txtRo.ToArray();
			tabReadOnly = tabRo.ToArray();

#warning In futuro usare array di struct o class

			tab.Clear(); txt.Clear(); tabf.Clear(); txtf.Clear(); tabNc.Clear(); txtNc.Clear();
			return ok;
			}

		/// <summary>
		/// Tabella dal nome (null se manca)
		/// </summary>
		/// <param name="nome">nome</param>
		/// <returns></returns>
		public string[,] GetTabella(string nome)
			{
			string[,] res = null;
			int itab;
			bool ifound = indici.TryGetValue(nome, out itab);
			if (ifound)
				{
				res = tabelle[itab];
				}
			return res;
			}

		/// <summary>
		/// Indice dal nome (-1 se manca)
		/// </summary>
		/// <param name="nome">nome</param>
		/// <returns></returns>
		public int GetIndice(string nome)
			{
			int itab = -1;
			int i;
			bool ifound = indici.TryGetValue(nome, out i);
			if (ifound)
				{
				itab = i;
				}
			return itab;
			}
		public string GetValoreTabella(string nome, int selezione)
			{
			string s = string.Empty;
			string[,] t = GetTabella(nome);
			if (t != null)
				{
				try
					{
					s = t[selezione, 1];
					}
				catch/* (Exception e)*/
					{ }
				}
			return s;
			}

		/// <summary>
		/// Riempie le tabelle
		/// Leggendole dalla risposta del database
		/// </summary>
		/// <param name="nome">nome della tabella</param>
		/// <param name="m">Risposta del databese</param>
		/// <returns></returns>
		public bool RiempiTabella(string nome, Risposta m)
			{
			bool ok = false;
			int itab;
			bool ifound = indici.TryGetValue(nome, out itab);
			if (ifound)
				{
				if ((m.righe > 0) && (m.colonne > 0))
					{
					tabelle[itab] = new string[m.righe + offsetSpecialItems, m.colonne];
					int iss = -offsetSpecialItems;
					foreach (string s in SpecialItems())
						{
						tabelle[itab][iss + offsetSpecialItems, 0] = iss.ToString();
						tabelle[itab][iss + offsetSpecialItems, 1] = s;
						iss++;
						}
					for (int r = 0; r < m.righe; r++)
						for (int c = 0; c < m.colonne; c++)
							tabelle[itab][r + offsetSpecialItems, c] = m.valori[r][c];
					ok = true;
					}
				else
					{
					tabelle[itab] = null;
					}
				}
			return ok;
			}
	
		/// <summary>
		/// Enumeratore, restituisce i nomi delle tabelle
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NomeFlag> NomiTabelle()
			{
			for (int i = 0; i < tabNames.Length; i++)
				{
				yield return new NomeFlag(tabNames[i], tabFlags[i], tabNumC[i], tabReadOnly[i]);
				}
			}

		/// <summary>
		/// Enumeratore, restituisce i nomi dei campi
		/// </summary>
		/// <returns></returns>
		public IEnumerable<NomeFlag> NomiCampi()
			{
			for (int i = 0; i < txtNames.Length; i++)
				{
				yield return new NomeFlag(txtNames[i], txtFlags[i], txtNumC[i], txtReadOnly[i]);
				}
			}

		/// <summary>
		/// Enumeratore, restituisce gli item speciali (vuoto, ignora...)
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> SpecialItems()
			{
			for (int i = 0; i < tabSpecialItems.Length; i++)
				yield return tabSpecialItems[i];
			}
		}
	}
