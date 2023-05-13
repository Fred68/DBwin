using System;
using System.Collections.Generic;
using System.Text;

namespace DBwin
	{
	public class Risposta
		{
		public static readonly string SEP_Msg = "<#>MSG<#>";
		public static readonly string SEP_Err = "<#>ERR<#>";
		public static readonly string SEP_Dat = "<#>DAT<#>";
		public static readonly string SEP_Sts = "<#>STS<#>";
		public static readonly string SEP_Usr = "<#>USR<#>";
		public static readonly string SEP_righe = "\n";
		public static readonly string SEP_colonne = ";";
		public static readonly string TOK_righe = "Righe=";
		public static readonly string TOK_colonne = "Colonne=";
		public enum STAT { disconnesso = 0, lettura = 1, scrittura = 2, errore = -1 };
		public string full { get; private set; }        // Stringa con la risposta completa (da pagina PHP)
		public string msgDB { get; private set; }       // Messaggi relativi al database
		public string errDB { get; private set; }       // Errori relativi al database
		public string msg { get; set; }                 // Messaggi relativi allo script php o al programma C#
		public STAT sts { get; private set; }           // Stato della connessione (connesso o disconnesso)
		public string usrDB { get; private set; }       // Nome dell'utente connesso
		public string datDB { get; set; }               // Stringa con la risposta ad una query, dalla pagina php
		public int righe, colonne;						// Righe e colonne della query
		public string[] titoli;                         // Titoli dei campi
		public string[] linee;                          // Le linee separate
		public string[][] valori;						// I risultati della query, separati

		public bool hasQuery { get; private set; }		// Se c'è una query disponibile
		
		
		/// <summary>
		/// Ctor vuoto
		/// </summary>
		public Risposta()
			{
			msg = msgDB = errDB = datDB = usrDB = full = string.Empty;
			sts = STAT.disconnesso;
			righe = colonne = 0;
			hasQuery = false;
			}

		/// <summary>
		/// Risposta vuota
		/// </summary>
		public bool isEmpty
			{get {return full.Length == 0;}}

		/// <summary>
		/// Testo completo della risposta
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			{
			return $"{full}";
			}

		/// <summary>
		/// Messaggi della risposta
		/// </summary>
		/// <returns></returns>
		public string MessagesToString()
			{
			StringBuilder m = new StringBuilder();
			if (msg.Length > 0) m.Append(msg);
			if (msgDB.Length > 0) m.Append(msgDB);
			if (errDB.Length > 0) m.Append(errDB);
			string tmp = m.ToString();
			return m.ToString();
			}

		/// <summary>
		/// Estre i dati dalla risposta
		/// </summary>
		/// <param name="res"></param>
		public void SetRisposta(string res)
			{
			string msg = res;
			string x = GetStringBetweenTokens(msg, SEP_Sts);
			switch (x)
				{
				case "1":
					sts = STAT.lettura;
					break;
				case "2":
					sts = STAT.scrittura;
					break;
				case "-1":
				default:
					sts = STAT.disconnesso;
					break;
				}
			msgDB = GetStringBetweenTokens(msg, SEP_Msg);
			errDB = GetStringBetweenTokens(msg, SEP_Err);
			datDB = GetStringBetweenTokens(msg, SEP_Dat);
			usrDB = GetStringBetweenTokens(msg, SEP_Usr);
			full = res;
			hasQuery = ReadQuery();
			}

		/// <summary>
		/// Legge i dati e li raccoglie in tabella
		/// I dati sono in datDB, la risposta in valori[][], le intestazioni in titoli[]
		/// </summary>
		/// <returns></returns>
		protected bool ReadQuery()
			{
			bool ok = false;
			if (datDB.Length > 0)
				{
				try
					{
					linee = datDB.Split(new string[] { SEP_righe }, StringSplitOptions.RemoveEmptyEntries);
					int r = linee[0].IndexOf(TOK_righe) + TOK_righe.Length;
					int c = linee[1].IndexOf(TOK_colonne) + TOK_colonne.Length;
					string rr = linee[0].Substring(r);
					string cc = linee[1].Substring(c);
					titoli = linee[2].Split(new string[] { SEP_colonne }, StringSplitOptions.RemoveEmptyEntries);
					if (int.TryParse(rr, out righe) && int.TryParse(cc, out colonne))
						{
						if (titoli.Length == colonne)
							{
							valori = new string[righe][];
							bool btmp = true;
							for (int ir = 0; ir < righe; ir++)
								{
								string[] tmp = linee[ir + 3].Split(new string[] { SEP_colonne }, StringSplitOptions.None);
								if (tmp.Length != colonne)
									{
									msg += $"I numeri elementi e colonne non corrispondono per la riga {ir}\n";
									btmp = false;
									break;
									}
								else
									{
									valori[ir] = tmp;
									}
								}
							ok = btmp;
							}
						else
							{
							msg += "I numeri di titoli e colonne non corrispondono\n";
							}
						}
					else
						{
						msg += "Errore di parsing del numero di righe o di colonne\n";
						}
					}
				catch
					{ ok = false; }
				}
			return ok;
			}

		/// <summary>
		/// Estrae una stringa tra due token
		/// </summary>
		/// <param name="res"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		string GetStringBetweenTokens(string res, string token)
			{
			string x = string.Empty;
			int ini = res.IndexOf(token);
			int fin = res.LastIndexOf(token);
			if ((ini > -1) && (fin > -1))
				{
				try
					{
					x = res.Substring(ini + token.Length, fin - ini - token.Length);
					}
				catch
					{
					x = string.Empty;
					}
				}
			return x;
			}

		/// <summary>
		/// Crea un dizionario con i valori che ha per chiavi i titoli
		/// </summary>
		/// <param name="riga"></param>
		/// <returns></returns>
		public Dictionary<string, string> GetValues(int riga)
			{
			Dictionary<string, string> coppie = new Dictionary<string, string>();
			if (hasQuery)
				{
				if ((riga >= 0) && (riga < valori.GetLength(0)))
					{
					for (int i = 0; i < titoli.Length; i++)
						{
						coppie.Add(titoli[i], valori[riga][i]);
						}
					}
				}
			return coppie;
			}

		public  Tuple<string,string,string> EstraiCodice(Impostazioni imp, int riga = 0)
			{
			string cod = string.Empty;
			string mod = string.Empty;
			string des = string.Empty;
			bool trovatoCod, trovatoMod, trovatoDes;
			trovatoCod = trovatoMod = trovatoDes = false;

			if(righe > riga)
				{
				for(int c = 0; c < colonne; c++)
					{
					if(titoli[c] == imp.Config.CampoCodice)
						{
						cod = valori[riga][c];
						trovatoCod = true;
						}
					if(titoli[c] == imp.Config.CampoModifica)
						{
						mod = valori[riga][c];
						trovatoMod = true;
						}
					if(titoli[c] == imp.Config.CampoDescrizione)
						{
						des = valori[riga][c];
						trovatoDes = true;
						}
					}
				if( !trovatoCod || !trovatoMod || !trovatoDes )
					{
					cod = mod = des = string.Empty;
					}
				}
			return new Tuple<string,string,string>(cod, mod, des);
			}
		}
	}
