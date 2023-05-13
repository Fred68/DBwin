using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace DBwin
	{

	
	public class DialogData
		{

		public enum TipoDialogEnum {Nuovo, Modifica, Ricerca};
		public enum DialogDataResult {Annulla, Scrivi, Elimina, Cerca};

		Dictionary<string, string> campi;
		Dictionary<string, int> selezioni;
		Impostazioni.TipoCodice tipo;
		Impostazioni _imp;

		TipoDialogEnum _tipoDialog;
		bool _canWrite;
		DialogDataResult _ddResult;

		/// <summary>
		/// Impostazioni (sola lettura)
		/// </summary>
		public Impostazioni Impostazioni => _imp;

		/// <summary>
		/// DialogDataResult
		/// </summary>
		public DialogDataResult DdResult
			{
			get {return _ddResult;}
			set {
				if(_tipoDialog == TipoDialogEnum.Ricerca && value != DialogDataResult.Annulla)
					{
					_ddResult = DialogDataResult.Cerca;
					}
				else
					{
					_ddResult = value;
					}
				}
			}
		

		/// <summary>
		/// Tipo di dato
		/// </summary>
		public Impostazioni.TipoCodice Tipo
			{
			get => tipo;
			set => tipo = value;
			}

		/// <summary>
		/// Tipo di dialog richiesta
		/// </summary>
		public TipoDialogEnum TipoDialog
			{
			get => _tipoDialog;
			set => _tipoDialog = value;
			}
		/// <summary>
		/// La dialog richiesta ha abilitati i controlli di scrittura o modifica
		/// </summary>
		public bool CanWrite
			{
			get => _canWrite;
			set => _canWrite = value;
			}

		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		/// <param name="imp"></param>
		public DialogData(ref Impostazioni imp)
			{
			_imp = imp;
			campi = new Dictionary<string, string>();
			selezioni = new Dictionary<string, int>();
			_canWrite = false;

			foreach (DatiCampo dc in _imp.CampiTesto())
				{
				campi.Add(dc.query, string.Empty);
				}
			foreach (DatiCampo dc in _imp.CampiLista())
				{
				selezioni.Add(dc.query, -1);
				}
			}

		/// <summary>
		/// Imposta il contenuto di un campo (numero)
		/// </summary>
		/// <param name="nome"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		public bool Set(string nome, int n)
			{
			bool ok = false;
			if (selezioni.ContainsKey(nome))
				{
				selezioni[nome] = n;
				ok = true;
				}
			return ok;
			}
		
		/// <summary>
		/// Imposta il contenuto di un campo (stringa)
		/// </summary>
		/// <param name="nome"></param>
		/// <param name="s"></param>
		/// <returns></returns>
		public bool Set(string nome, string s)
			{
			bool ok = false;
			if (campi.ContainsKey(nome))
				{
				campi[nome] = s;
				ok = true;
				}
			return ok;
			}

		/// <summary>
		/// Estrae l'indice della linea selezionata del campo
		/// </summary>
		/// <param name="nome">nome del campo</param>
		/// <returns></returns>
		public int GetSelection(string nome)
			{
			int i = -1;
			if (selezioni.ContainsKey(nome))
				{
				i = selezioni[nome];
				}
			return i;
			}

		/// <summary>
		/// Estrae il contenuto del campo
		/// </summary>
		/// <param name="nome">nome del campo</param>
		/// <returns></returns>
		public string GetText(string nome)
			{
			string s = string.Empty;
			if (campi.ContainsKey(nome))
				{
				s = campi[nome];
				}
			return s;
			}

		/// <summary>
		/// Estrae il testo della linea selezionata del campo
		/// </summary>
		/// <param name="nome"></param>
		/// <returns></returns>
		public string GetSelectedText(string nome)
			{
			string s = string.Empty;
			int i = GetSelection(nome);
			if (i > -1)
				{
				s = _imp.DatoListaDaNomeCampo(nome, i);
				}
			return s;
			}

		/// <summary>
		/// Imposta i campi presenti in base a quelli letti dal dizionario (proveniente da una query)
		/// </summary>
		/// <param name="dict">Dizionario <strig,string></param>
		/// <param name="nset">Numero di campi impostati</param>
		/// <returns>true se trovati tutti i campi di dict</returns>
		public bool Set(Dictionary<string, string> dict, out int nset)
			{
			bool ok = false;
			nset = 0;

			try
				{
				foreach (DatiCampo dc in this.Impostazioni.Campi())		// Percorre i dati dei campi standard
					{
					if (dc.isOk)										// Se il campo è valido
						{
						switch (dc.tipo)
							{
							case Impostazioni.TipoInput.lista:			// Se il campo è una lista...
								{
								int indexLista = -1;
								if(dict.ContainsKey(dc.query))			// Cerca nel dizionario ricevuto dalla query...			
									{
									if(dict[dc.query].Length > 0)		// Se il campo non è vuoto
										{
										string [,] tmp = _imp.Lista(dc.tabella);	// Ottiene la tabella dal nome dc.tabella 
										if(tmp.GetLength(1) != 2)					// Verifica ceh abbia 2 colonne	
											{
											throw new Exception($"Tabella {dc.tabella} di dimensioni errate");
											}
										for(int r=0; r<tmp.GetLength(0); r++)		// Percorre le righe e cerca...
											{
											if(tmp[r,1] == dict[dc.query])			// ...il contenuto della query
												{
#if DEBUG
												MessageBox.Show($"Trovato indice {tmp[r,0]} della riga {tmp[r,1]}");
#endif
												// Non usa l'ID della tabella: if(!int.TryParse(tmp[r,0], out indexLista)) {throw new Exception($"E,,,");											
												// Usa la riga: le tabelle arrivano già ordinate (order by...) dalla query
												indexLista = r;
												break;
												}
											}
										}
									if(indexLista != -1)
										{
										Set(dc.query, indexLista);
										}
									}
								}
								break;
							case Impostazioni.TipoInput.testo:			// Se il campo è di testo...
								{
								if(dict.ContainsKey(dc.query))			// Cerca nel dizionario ricevuto dalla query...
									{
									if(dict[dc.query].Length > 0)		// Se il campo non è vuoto
										{
										if(dc.query == _imp.Config.CampoTipo)	// COM PAR ASM SCH
											{
											string id = dict[dc.query];
											if(id == _imp.Config.IdParticolare)
												{
												this.tipo = Impostazioni.TipoCodice.particolare;
												}
											else if(id == _imp.Config.IdSchema)
												{
												this.tipo = Impostazioni.TipoCodice.schema;
												}
											else if(id == _imp.Config.IdAssieme)
												{
												this.tipo = Impostazioni.TipoCodice.assieme;
												}
											else if(id == _imp.Config.IdCommerciale)
												{
												this.tipo = Impostazioni.TipoCodice.commerciale;
												}
#if DEBUG
											MessageBox.Show($"{this.tipo}!");
#endif
											}

										Set(dc.query, dict[dc.query]);	// Imposta il testo
										}
									}
								}
								break;
							default:
								{
								throw new Exception("dc.tipo è un TipoInput non gestito");
								}
							}
						}
					}
				}
			catch (Exception e)
				{
				ok = false;
				MessageBox.Show(e.ToString());
				}

			return ok;
			}

		/// <summary>
		/// Override di string ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($"Tipo {_tipoDialog.ToString()} {(_canWrite ? "con" : "senza" )} permesso di scrittura.");
			sb.AppendLine($"Restituito {DdResult.ToString()}.\n");
			foreach(var x in campi)
				{
				sb.AppendLine($"[{x.Key}]= {x.Value}");
				}
			foreach(var x in selezioni)
				{
				string s = string.Empty;
				int v = x.Value;
				if (v > -1)
					{
					s = _imp.DatoListaDaNomeCampo(x.Key, v);
					}
				sb.AppendLine($"[{x.Key}]= {s}");
				}
			return sb.ToString();
			} 

		/// <summary>
		/// Converte i dati in un array di stringhe con i parametri da inserire
		/// </summary>
		/// <returns></returns>
		public string[] ToParamsArray()
			{
			string[] p = null;
			p = new string[_imp.SizeParamsArray];
			for (int i = 0; i < p.Length; i++)
				p[i] = string.Empty;

			foreach (DatiCampo dc in _imp.Campi())
				{
				if (dc.isOk)
					{
					string txt = string.Empty;
					switch (dc.tipo)
						{
						case Impostazioni.TipoInput.lista:
							{
							txt = GetSelectedText(dc.query);
							}
						break;
						case Impostazioni.TipoInput.testo:
							{
							txt = GetText(dc.query);
							}
						break;
						}
					if ((dc.par >= 0) && (dc.par < p.Length))
						p[dc.par] = txt;
					}
				}
			return p;
			}
		}
	}
