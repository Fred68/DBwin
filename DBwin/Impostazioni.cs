using System;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Fred68.CfgReader;

namespace DBwin
	{

	/// <summary>
	/// Informazioni per ogni tipo di dato
	/// </summary>
	public class Impostazioni
		{
		
		MainForm mf;
		public MainForm Mf { get { return mf; } }

		public dynamic Config;

		/// <summary>
		/// Tipi di campi di input
		/// </summary>
		public enum TipoInput { lista, testo, nessuno };

		/// <summary>
		/// Tipi di codice
		/// </summary>
		public enum TipoCodice { assieme = 0, schema, particolare, commerciale, nessuno };

		/// <summary>
		/// Numero di tipi di codice
		/// public static int nTipi { get { return nTp; } set { nTp = value; } }
		/// public static int nTipi { get => nTp; set => nTp = value; }
		/// </summary>
		public static int nTipi => nTp;
		/// <summary>
		/// Array con i nomi dei tipi di codice
		/// </summary>
		public static string[] nomiTipi => sNTC;

		static int nTp;
		public static string[] sNTC;

		/// <summary>
		/// I dati di configurazione
		/// </summary>
		List<DatiCampo> datiCfg;

		/// <summary>
		/// Dizionario con i nomi delle liste
		/// </summary>
		Dictionary<string, string[,]> dicListe;

		public bool isOk => ok;
		bool ok;

		/// <summary>
		/// Numero più alto dei parametri
		/// </summary>
		int parMax;

		/// <summary>
		/// Dimensione dell'array dei parametri del POST al programma PHP del database
		/// </summary>
		public int SizeParamsArray => parMax + 1;

		/// <summary>
		/// COSTRUTTORE statico
		/// </summary>
		static Impostazioni()
			{
			nTp = Enum.GetNames(typeof(TipoCodice)).Length /*- 1*/;
			sNTC = new string[nTp];
			Array.Copy(Enum.GetNames(typeof(TipoCodice)), 0, sNTC, 0, nTp);
			}

		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		public Impostazioni(MainForm mf)
			{
			
			FileInfo fileinfo;
			List<string> cfgFileContent;

			ok = true;
			this.mf = mf;
			fileinfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
			string configfile = fileinfo.DirectoryName + "\\" + Properties.Settings.Default.ConfigFile;

			// Legge la configurazione del database
			string errorMessage;
			if (!LeggiConfigFile(configfile, out cfgFileContent, out errorMessage))
				{
				ok = false;
				MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			if (!LeggiConfigurazione(cfgFileContent, out datiCfg, out errorMessage))
				{
				ok = false;
				MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			cfgFileContent.Clear();
			dicListe = new Dictionary<string, string[,]>();

			// Legge le variabili del file di configurazione
			Config = new CfgReader();
			try
				{
				Config.ReadConfiguration(configfile);
				}
			catch(Exception ex)
				{
				MessageBox.Show($"Errore {ex}");
				}
#if DEBUG
			MessageBox.Show($"{Config.ToString()}\n{Config.DumpEntries()}");
#endif
			}

		/// <summary>
		/// Legge il file di configurazione
		/// </summary>
		/// <param name="cfgFile">Nome completo del file di configurazione</param>
		/// <param name="cfgList">Lista per le righe del file</param>
		/// <returns>false se errore</returns>
		bool LeggiConfigFile(string cfgFile, out List<string> cfgList, out string msgErr)
			{
			bool ok = false;
			cfgList = new List<string>();
			msgErr = string.Empty;
			if (File.Exists(cfgFile))
				{
				System.IO.StreamReader file = null;
				try
					{
					bool finish = false;									// Finita lettura della sezione
					string line;
					file = new System.IO.StreamReader(cfgFile);
					while (((line = file.ReadLine()) != null) && (!finish))
						{
						bool isFirstSection = false;						// Sezione iniziale
						
						line = line.Trim();
						if (line.Length > 0)
							{
							bool iSCommentLine = line.StartsWith("#");		// Commento
							bool isSectionLine = false;
							
							int iIni = line.IndexOf('[');					// Indicatore di sezione
							int iFin = line.IndexOf(']');
							if( (iIni != -1) && (iFin != -1) && (iFin>iIni))
								{
								isSectionLine = true;
								isFirstSection = true;
								}

							if( (line.Contains("[.]") && isFirstSection) )	// Fine della prima sezione
								{
								finish = true;
								}

							if (!iSCommentLine && !isSectionLine)           // Elimina le linee di commento
								{
								if (line.Contains("#"))                    // Elimina i commenti a fine linea
									line = line.Substring(0, line.IndexOf("#"));
								line = line.Replace("\t", " ");             // Sostituisce le tabulazioni con spazi
								while (line.Contains("  "))                 // Elimina tutti gli spazi multipli
									{
									line = line.Replace("  ", " ");
									}
								cfgList.Add(line);
								}
							}
						}
					ok = true;
					}
				catch (Exception e)
					{
					msgErr = $"Fallita lettura del file: {cfgFile}.\n{e.ToString()}";
					}
				if (file != null)
					{
					file.Close();
					}
				}
			else
				{
				msgErr = $"File di configurazione: {cfgFile}\nnon trovato.";
				}
			return ok;
			}

		/// <summary>
		/// Legge le linee e imposta la configurazione
		/// Le linee sono ordinate in base a DatiCampo IComparable<DatiCampo>
		/// </summary>
		/// <param name="cfgList">Lista delle linee</param>
		/// <param name="dati">Lista dei dati dei campi</param>
		/// <param name="msgErr">Messaggio di errore</param>
		/// <returns></returns>
		bool LeggiConfigurazione(List<string> cfgList, out List<DatiCampo> dati, out string msgErr)
			{
			bool ok = true;
			msgErr = string.Empty;
			dati = new List<DatiCampo>();
			int il = 1;
			foreach (string line in cfgList)
				{
				try
					{
					string[] c = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					if (c.Length == DatiCampo.nParam)
						{
						DatiCampo d = new DatiCampo(c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7]);
						if (d.isOk)
							{
							dati.Add(d);
							}
						else
							{
							msgErr = $"Errore di lettura della linea {il} della configurazione";
							ok = false;
							break;
							}
						}
					else
						{
						msgErr = $"Numero errato di argomenti nella linea {il} della configurazione";
						ok = false;
						break;
						}
					}
				catch
					{
					msgErr = $"Errore nella separazione degli argomenti nella linea {il} della configurazione";
					ok = false;
					break;
					}
				il++;
				}
			dati.Sort();

			parMax = -1;
			foreach (DatiCampo dc in dati)
				{
				if (dc.par >= parMax)
					parMax = dc.par;
				}

			return ok;
			}

		/// <summary>
		/// Enumera tutti i campi delle tabelle
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DatiCampo> CampiLista()
			{
			foreach (DatiCampo dc in datiCfg)
				{
				if ((dc.isOk) && (dc.tipo == TipoInput.lista))
					yield return dc;
				}
			}

		/// <summary>
		/// Enumera tutti i campi testo
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DatiCampo> CampiTesto()
			{
			foreach (DatiCampo dc in datiCfg)
				{
				if ((dc.isOk) && (dc.tipo == TipoInput.testo))
					yield return dc;
				}
			}

		/// <summary>
		/// Enumera tutti i campi
		/// </summary>
		/// <returns></returns>
		public IEnumerable<DatiCampo> Campi()
			{
			foreach (DatiCampo dc in datiCfg)
				{
				if (dc.isOk)
					yield return dc;
				}
			}

		/// <summary>
		/// Riempie la lista 'nomeLista', leggendola dalla risposta del server
		/// </summary>
		/// <param name="nomeLista">nome della lista</param>
		/// <param name="m">risposta dell'interrogazione al database</param>
		/// <returns>true se ok</returns>
		public bool RiempiLista(string nomeLista, Risposta m)
			{
			bool ok = false;
			bool changes = true;
			if ((m.righe == 1) && (m.colonne == 1))
				{
				if(m.valori[0][0] == "NO CHANGE")
					{
					changes = false;
					}
				}
			if ((m.righe > 0) && (m.colonne > 0) && changes)
				{
				string[,] tabella = new string[m.righe, m.colonne];
				for (int r = 0; r < m.righe; r++)
					for (int c = 0; c < m.colonne; c++)
						tabella[r, c] = m.valori[r][c];
				dicListe[nomeLista] = tabella;
				ok = true;
				}
			return ok;
			}

		/// <summary>
		/// Restituisce la lista 'nomeLista'
		/// </summary>
		/// <param name="nomeLista">nome della lista</param>
		/// <returns>La lista string[,]</returns>
		public string[,] Lista(string nomeLista)
			{
			string[,] res = null;
			if (dicListe.ContainsKey(nomeLista))
				{
				res = dicListe[nomeLista];
				}
			return res;
			}

		/// <summary>
		/// Estrae il testo alla riga 'indice' della lista associata al campo 'nomeCampo'
		/// </summary>
		/// <param name="nomeCampo">Nome del campo (non della lista)</param>
		/// <param name="indice">indice della selezione</param>
		/// <returns></returns>
		public string DatoListaDaNomeCampo(string nomeCampo, int indice)
			{
			string s = string.Empty;

			DatiCampo dcS = null;
			foreach (DatiCampo dc in CampiLista())
				{
				if (dc.query == nomeCampo)
					dcS = dc;
				}
			string[,] l = Lista(dcS.tabella);
			if (l != null)
				{
				int rows = l.GetLength(0);
				int cols = l.GetLength(1);
				if ((cols > 1) && (indice >= 0) && (indice < rows))
					s = l[indice, 1];
				}
			return s;
			}

		/// <summary>
		/// Verifica se un campo è visibile con uno soltanto (o almeno uno) dei tipi di codice
		/// </summary>
		/// <param name="dc"></param>
		/// <param name="tc">TipoCodice (assieme, particolare...) o nessuno se in almeno uno</param>
		/// <returns></returns>
		public bool Visibile(DatiCampo dc, TipoCodice tc = TipoCodice.nessuno)
			{
			bool vis = false;
			int i;
			switch (tc)
				{
				case TipoCodice.assieme:
				case TipoCodice.commerciale:
				case TipoCodice.particolare:
				case TipoCodice.schema:
					{
					i = (int)tc;
					vis = dc.flag[i];
					}
				break;
				case TipoCodice.nessuno:
					{
					for (i = 0; i < DatiCampo.nFlag; i++)
						{
						vis = vis || dc.flag[i];
						}
					}
				break;
				}
			return vis;
			}

		}
	}
