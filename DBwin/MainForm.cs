#define RESPONSE_DEBUG
#undef RESPONSE_DEBUG


//////// v	Leggere ed importare in strutture dati le tabelle materiali, costruttori, prodotti.
//////// v	Scrivere nuova Form, analoga a LoginForm, per inserire più dati, leggere i valori dalle tabelle importate, verificare i valori.
#warning .  Assicurarsi che possano essere aperti nuovi Form multipli. Ogni Form deve avere le sue strutture dati private.
//////// x	Valutare se il progetto può essere WPF invece che Windows Form. Ma perderebbe la compatibilità con Se31: lasciare ivariato.
//////// v	Scrivere i Form per l'inserimento di nuovi codici (di vario tipo).
//////// v	Usare menù a discesa con i tipi (disegno, schema, commerciale...) per selezionare i controlli Form oppure usare Form differenti.
////////	Studiare un form generico per le query, basate su condizioni multiple. 
//////// x	Creare un account su altervista e provare il database online. Non possible, non accetta le stored procedure
//////// v	ATTENZIONE ! Dopo ogni comando (check stat o altro comando che imposta $this->sts = 1) viene visualizzato lo stato di utente connesso in lettura, invece che abilitata scrittura.
//////// v	Aggiungere gestore click su tabelle, per copiare il codice (ed usarlo per operazioni successive).
//////// v	Verificare funzione per leggere tutti i dati di un codice e salvarli in un oggetto
#warning	Aggiungere, al Form di inserimento codice, la possibilità di usare l'oggetto con i dati di un codice.
////////	Usare listbox/combo: devono diventare textbox editabili ma anche con lista e restituire una stringa.
//////// v	Impostare i flag 'dirty' in u_connessi separatamente per liste, per ogni utente. Dopo caricamento, resettare il flag.
#warning	Alla richiesta di lista, aggiungere flag reinvia comunque. Il programma PHP conosce l'ID dell'utente.
//////// v	Nella risposta di una lista, includere l'opzione: no change.
#warning	Permettere l'invio di una risposta da php in più chiamate, almeno per le liste. Quindi includere richiesta lista + indice.
#warning	Oppure salvare l'indice in variabile di sessione PHP ed incudere un flag 'more...' nella risposta. Il programma C#, in tal caso, eseguire un'altra http_request ed unirla ai dati della precedente.
#warning	Eventuale coda FIFO per i comandi (ma poi come trattare le risposte ?)
////////	Usare una classe Log per i messaggi da inviare alla rich text box.
////////	I messaggi ricevuti dalla classe Log devono avere un timestamp.
#warning	Prima della versione finale, allungare il ritardo nel file php: var $pwdDelay = ... da 500 a 3000 (contro attacchi).
////////	Disabilitare i controlli senza cambiare i colori di sfondo e primo piano (derivare nuova classe)
#warning	Correggere il messagggio di log: 'Trovato 1 elemento' anche se il contenuto è 'errore'

/*
Dati per inserimento codici:
	
	TIPO		Assieme		Schema		Particolare		Commerciale	
VALORI			
codice				v			v			v				v
modifica			v			v			v				v
descrizione			v			v			v				
utente				v			v			v				v
materiali[]									v
costruttori[]												v
prodotti[]													v
modello														v
dettagli													v


Dati per inserimento prodotti, materiali, costruttori (in futuro da estendere con altri dati)
descrizione		v
add				v (da aggiungere o da aggiornare)
*/



using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Fred68.CfgReader;

namespace DBwin
	{

	delegate void DelegateMsg(string msg);			// Delegate per scrivere i messaggi di Log nella main form
	public delegate bool DelegateCheckDialogData(ref DialogData dd, bool correggi);	// Delegate per controllare e correggere il contenuto di DialogData
	public enum DirtyEnum {materiali = 1, prodotti, costruttori};

	public partial class MainForm : Form
		{	

		FileInfo fileinfo;
		string configfile;
		List<string> config = null;
		static Connessione conn = null;
		bool startOk = false;
		Encryption enc = null;			// Classe per crittografia aes
		TabelleDati td = null;
		Impostazioni imp = null;
		Log log = null;					// Classe per il Log
		int refreshInterval;			// Intervallo di refresh (impostato in base al file di configurazione)

		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		public MainForm()
			{
			startOk = true;
			InitializeComponent();

			fileinfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
			configfile = fileinfo.DirectoryName + "\\" + Properties.Settings.Default.ConfigFile;
			imp = new Impostazioni(this);
			enc = new Encryption();
			conn = new Connessione(imp.Config.url, imp.Config.maxRowResponse);
			log = new Log(ScriviMessaggio);		// Delegate della funzione per scrivere il messaggio nella finestra

			if(!(imp.isOk))
				{
				startOk = false;
				}

			if ((conn == null) || (enc == null))
				{
				startOk = false;
				MessageBox.Show("Fallita allocazione memoria", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			RefreshInterval = imp.Config.minRefreshInterval;		// Chiamare RefreshInterval dopo aver creato la Connessione
			mCheck.Enabled = true;
			UpdateLayout();
			}

		/// <summary>
		/// Intervallo di refresh
		/// </summary>
		public int RefreshInterval
			{
			get
				{
				return refreshInterval;
				}
			set
				{
				refreshInterval = (value > imp.Config.minRefreshInterval) ? value : imp.Config.minRefreshInterval;
				timerRefresh.Interval = refreshInterval * 1000;
				if((conn.connected == Risposta.STAT.lettura)||(conn.connected == Risposta.STAT.scrittura))
					{
					log.ScriviLog($"Refresh = {refreshInterval} s.\n");
					}
				}
			}

		/// <summary>
		/// Permesso di scrittura della connessione attuale
		/// </summary>
		public bool CanWrite
			{
			get {return (conn.connected == Risposta.STAT.scrittura); }
			}
		private void onRefresh(object sender, EventArgs e)
			{
			Check();    // Controlla connessione e azzera il timer sul server
			Liste();    // Ricarica le liste
			}
		private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
			{
			await Logout();     // Attende il logout
			if (conn != null)   // Verifica per sicurezza (se la chiamata non fosse asincrona, senza await)
				{
				if ((int)conn.connected > 0)
					{
					e.Cancel = true;
					MessageBox.Show("Chiudere la connessione.");
					}
				}
			}
		private void MainForm_Load(object sender, EventArgs e)
			{
			if (startOk)
				{
				td = new TabelleDati(config);
				if (td == null)
					{
					startOk = false;
					MessageBox.Show("Fallita allocazione tabelle", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			if (!startOk)
				Close();

			#if !DEBUG
			dirtyTestToolStripMenuItem.Enabled = dirtyTestToolStripMenuItem.Visible =false;
			mnListeTest.Enabled = mnListeTest.Visible = false;
			mnModificaCodice.Enabled = mnModificaCodice.Visible = false; 
			#endif

			}

		/***************************************/
		/// <summary>
		/// Funzione, passata come delegate, per scrivere un messaggio di Log
		/// </summary>
		/// <param name="msg"></param>
		public void ScriviMessaggio(string msg)
			{
			string[] lines = msg.Split('\n');
			foreach(string s in lines)
				{
				if(s.Length > 0) rtbMessages.AppendText(DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "\t\t" + s + "\n");
				}
			}
		/***************************************/

		/***************************************/
		/// <summary>
		/// Funzione, passata come delegate, per controllare e correggere il contenuto di un oggetto dialog data
		/// </summary>
		/// <param name="dd"></param>
		/// <returns></returns>
		public bool CheckDialogData(ref DialogData dd, bool correggi)
			{
			bool ok = true;

			// Corregge o completa i dati in base al tipo (assieme, particolare...)
			switch(dd.Tipo)
				{
				case Impostazioni.TipoCodice.assieme:
					{}
					break;
				case Impostazioni.TipoCodice.particolare:
					{}
					break;
				case Impostazioni.TipoCodice.schema:
					{}
					break;
				case Impostazioni.TipoCodice.commerciale:
					{
					// Le descrizioni commerciali vengono composte da MySQL combinando gli altri campi
					#if false
					string des = dd.GetText(imp.Config.CampoDescrizione);
					string desC =	dd.GetSelectedText(imp.Config.CampoProdotto) + ' ' +
									(dd.GetSelectedText(imp.Config.CampoCostruttore)).ToUpper() + ' ' + 
									dd.GetText(imp.Config.CampoModello);
					if(des != desC) 
						{
						if(correggi)
							{
							dd.Set(imp.Config.CampoDescrizione, desC);
							}
						else
							{
							ok = false;
							}
						}
					#endif
					}
					break;
				}

			// Corregge le lunghezze superiori al massimo consentito (solo per i campi testo)
			foreach(DatiCampo dc in imp.CampiTesto())
				{
				string txt = dd.GetTesto(dc.query);
				if(txt.Length > dc.lmax)
					{
					txt = txt.Substring(0,dc.lmax);
					if(correggi)
						{
						dd.SetTesto(dc.query, txt);
						}
						else
						{
						ok = false;
						}
					}
				}

			#warning Correggere anche il testo delle selezioni (con indice == -1): nuovo testo

			return ok;
			}
		/***************************************/


		#region OnClick() dei menù
		private void Login_Click(object sender, EventArgs e)
			{
			LoginWithForm();
			}
		private void Check_Click(object sender, EventArgs e)
			{
			Check();    // Controlla connessione e azzera il timer sul server
			Liste();    // Ricarica le liste
			}
		private async void Logout_Click(object sender, EventArgs e)
			{
			await Logout();
			}
		private void Vedi_Click(object sender, EventArgs e)
			{
			Vedi(DialogData.TipoDialogEnum.Modifica);
			}
		private void ChangeAccessPassword_Click(object sender, EventArgs e)
			{
			ChangePassword_N(1);
			}
		private void ChangeWritePassword_Click(object sender, EventArgs e)
			{
			ChangePassword_N(2);
			}
		private async void EsciToolStripMenuItem_Click(object sender, EventArgs e)
			{
			await Logout();
			this.Close();
			}
		private void AbilitaScrittura_Click(object sender, EventArgs e)
			{
			AbilitaScrittura();
			}
		private void LoginRapido_Click(object sender, EventArgs e)
			{
			QuickLogin();
			}
		private void MemorizzaDatiLogin_Click(object sender, EventArgs e)
			{
			MemorizzaDatiLogin();
			}
		private void ListeTest_Click(object sender, EventArgs e)
			{
			Liste();    // Ricarica le liste
			}
		private void About_Click(object sender, EventArgs e)
			{
			MessageBox.Show(Versione());
			}
		private void Query_Click(object sender, EventArgs e)
			{
			Query();
			}
		private void vediCodiceSingolo_Click(object sender, EventArgs e)
			{
			#if DEBUG
			ModificaCodiceSingolo();
			#endif
			}
		private void materialiDirtyToolStripMenuItem_Click(object sender, EventArgs e)
			{
			Dirty(DirtyEnum.materiali);
			}
		private void prodottiiDirtyToolStripMenuItem_Click(object sender, EventArgs e)
			{
			Dirty(DirtyEnum.prodotti);
			}
		private void eliminaCodiceToolStripMenuItem_Click(object sender, EventArgs e)
			{
			Vedi(DialogData.TipoDialogEnum.Elimina);
			}	
		private void nuovoCodice_Click(object sender, EventArgs e)
			{
			InsertNewCode();
			}	
		#endregion



		#region Connessione e utente

		/// <summary>
		/// Esegue controllo e refresh
		/// </summary>
		private async void Check()
			{
			if (conn != null)
				{
				Risposta res = await conn.Check();
#if RESPONSE_DEBUG
				MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
				log.ScriviLog(res.MessagesToString());
				}
			UpdateLayout();
			}

		/// <summary>
		/// Esegue il logout (async)
		/// </summary>
		/// <returns></returns>
		private async Task Logout()
			{
			if (conn != null)
				{
				Risposta res = await conn.Logout();
#if RESPONSE_DEBUG
				MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
				log.ScriviLog(res.MessagesToString());
				UpdateLayout();
				}
			}

		/// <summary>
		/// Esegue il login, inserendo utente e password
		/// </summary>
		private void LoginWithForm()
			{
			string[] par;
			InputForm lf = new InputForm("Connessione", new string[] { "@Utente", "Password di accesso" }, out par, "Login");
			if (lf.ShowDialog() == DialogResult.OK)
				{
				Login(par[0], par[1]);
				}
			}

		/// <summary>
		/// Esegue il login rapido
		/// </summary>
		private void QuickLogin()
			{
			string usr = enc.Decrypt(Properties.Settings.Default.QuickLogUsr, Build());
			string pwd = enc.Decrypt(Properties.Settings.Default.QuickLogPwd, Build());
			Login(usr, pwd);
			}

		/// <summary>
		/// Esegue il login (async) e legge il timeout restituito dal programma PHP
		/// </summary>
		/// <param name="user"></param>
		/// <param name="password"></param>
		private async void Login(string user, string password)
			{
			Risposta res = await conn.Login(user, password);    // Esegue il login
#if RESPONSE_DEBUG
			MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
			log.ScriviLog(res.MessagesToString());
			int timeout;
			if(int.TryParse(res.datDB, out timeout))			// Legge il timeout del programma PHP
				{
				RefreshInterval = timeout - imp.Config.timeoutRefreshTolerance;	// Refresh poco prima del timeout
				}
				
			Liste();    // Carica le liste
			UpdateLayout();
			}

		/// <summary>
		/// Memorizza i dati per login rapido
		/// </summary>
		private void MemorizzaDatiLogin()
			{
			string[] par;
			InputForm lf = new InputForm("Memorizza dati per login rapido", new string[] { "@Utente", "Password di accesso" }, out par, "Memorizza");
			if (lf.ShowDialog() == DialogResult.OK)
				{
				try
					{
					Properties.Settings.Default.QuickLogUsr = enc.Encrypt(par[0], Build());
					Properties.Settings.Default.QuickLogPwd = enc.Encrypt(par[1], Build());
					Properties.Settings.Default.Save();
					}
				catch/* (InvalidCastException e)*/
					{
					MessageBox.Show("Errore nella lettura dei dati");
					}
				}
			}

		/// <summary>
		/// Abilita la scrittura nel database
		/// </summary>
		private async void AbilitaScrittura()
			{
			if (conn != null)
				{
				string[] par;
				Risposta res;
				switch (conn.connected)
					{
					case Risposta.STAT.lettura:     // Abilita, se possibile
						InputForm lf = new InputForm("Accesso in scrittura", new string[] { "Password di scrittura" }, out par, "Abilita");
						if (lf.ShowDialog() == DialogResult.OK)
							{
							res = await conn.WriteEnable(true, par[0]);
#if RESPONSE_DEBUG
							MessageBox.Show($"{res.ToString()}", $"{url}");
#endif
							log.ScriviLog(res.MessagesToString());
							UpdateLayout();
							}
						break;
					case Risposta.STAT.scrittura:   // Disabilita, se possibile
						res = await conn.WriteEnable(false, "");
#if RESPONSE_DEBUG
						MessageBox.Show($"{res.ToString()}", $"{url}");
#endif
						log.ScriviLog(res.MessagesToString());
						UpdateLayout();
						break;
					default:                        // Non fa nulla
						break;
					}
				}
			}

		/// <summary>
		/// Cambia la password
		/// </summary>
		/// <param name="np"></param>
		private async void ChangePassword_N(int np)
			{
			string[] par;
			string titolo = "";
			switch (np)
				{
				case 1:
					titolo = "Modifica della password di accesso";
					break;
				case 2:
					titolo = "Modifica della password di scrittura";
					break;
				default:
					titolo = "???";
					break;
				}
			InputForm lf = new InputForm(titolo, new string[] { "@Utente", "Password di accesso", "Nuova password", "Nuova password" }, out par, "Cambia");
			if (lf.ShowDialog() == DialogResult.OK)
				{
				if (par[2] == par[3])
					{
					Risposta res = await conn.ChangePwd(par[0], par[1], par[2], np);
#if RESPONSE_DEBUG
					MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
					log.ScriviLog(res.MessagesToString());
					}
				else
					MessageBox.Show("Le nuove password non corrispondono");
				}
			UpdateLayout();
			return;
			}

		#endregion



		#region Inserimento e modifica

		/// <summary>
		/// Apre la dialog di query (in ricerca)
		/// </summary>
		private void Query()
			{
			DialogData dd = new DialogData(imp);
			dd.TipoDialog = DialogData.TipoDialogEnum.Ricerca;
			dd.CanWrite = false;
			if(QueryDialog(ref dd))			// Query...
				{
				ApplicaComandoDialogData(dd);
				}
			}

		/// <summary>
		/// Crea ed inserisce un nuovo codice
		/// </summary>
		/// <param name="dd"></param>
		private void InsertNewCode()
			{
			DialogData dd = new DialogData(imp);
			dd.TipoDialog = DialogData.TipoDialogEnum.Nuovo;
			dd.CanWrite = this.CanWrite;

			if(QueryDialog(ref dd))			// Nuovo codice...
				{
				ApplicaComandoDialogData(dd);
				}
			}

#if DEBUG
		/// <summary>
		/// Inserisce un nuovo codice con il contenuto della dialog
		/// </summary>
		/// <param name="dd"></param>
		private async void InsertCodeNew()
			{
			DialogData dd = new DialogData(imp);
			dd.TipoDialog = DialogData.TipoDialogEnum.Nuovo;
			
			if (conn != null)
				{
				Risposta res;
				int n;
				string[] p = InsertDialogwithPostArray(ref dd);
				if (p != null)
					{
					res = await conn.Conta(p[1], p[2]);
					bool bInsert = true;
					int.TryParse(res.datDB, out n);
					if (n > 0)
						{
						if (MessageBox.Show("Codice già presente. Aggiornare con i nuovi dati ?", "ATTENZIONE: codice esistente", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
							{
							bInsert = false;
							}
						}
					else
						{
						if (MessageBox.Show("Proseguire con l'inserimento del nuovo codice ?", "Nuovo codice", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
							{
							bInsert = false;
							}
						}

					if (bInsert)
						{
						res = await conn.InserisciNew(dd.Tipo, p);
						}
					}
				}
			}



		/// <summary>
		/// Apre una dialog di ricerca per codice e modifica ed elimina il codice.
		/// </summary>
		private async void EliminaCodice()
			{
			Risposta res;
			Tuple<string,string,string> codice = new Tuple<string,string,string>(string.Empty, string.Empty, string.Empty);
			bool ok = false;

			string[] codmod = ChiediCodicePerRicerca();					// Richiede codice e modifica, per eliminazione
			res = await CercaPerCodice(codmod[0], codmod[1]);			// Esegue ricerca

			switch(res.righe)
				{
				case 0:
					{
					//log.ScriviLog($"Nessun codice trovato");
					}
					break;
				case 1:
					{
					codice =  res.EstraiCodice(imp);
					ok = true;
					}
					break;
				default:
					{
					//log.ScriviLog($"Trovati {res.righe} codici");
					}
					break;
				}

			if(ok)
				{
				if(MessageBox.Show($"Proseguire con l'eliminazione del:\n{codice.Item1}{codice.Item2} {codice.Item3}\ndall'archivio?","Eliminazione codice",MessageBoxButtons.OKCancel)!=DialogResult.OK)
					{
					ok = false;
					MessageBox.Show($"Annullata eliminazione del:\n{codice.Item1}{codice.Item2} {codice.Item3}.");
					}
				}

			if(ok)
				{
				res = await conn.Elimina(codice.Item1, codice.Item2);
				//log.ScriviLog(res.MessagesToString());
				}
			
			UpdateLayout();

			}

		/// <summary>
		/// Prepara l'array di parametri, chiama la dialog e lo riempie.
		/// Usa solo i parametri da 1 in poi.
		/// Se dialog chiusa con Cancel: p <- null;
		/// </summary>
		/// <param name="dd"></param>
		/// <returns>array dei parametri da 1 in poi</returns>
		private string[] InsertDialogwithPostArray(ref DialogData dd)
			{
			string[] p = null;
			if (QueryDialog(ref dd))		// In: InsertDialogwithPostArray...
				{
				p = dd.ToParamsArray();
				}
			return p;
			}
		
		private async void ModificaCodiceSingolo()
			{
			bool ok = false;
			
			Dictionary<string, string> dct = await CercaPerCodiceSingolo();
			
			string cod, mod;
			cod = mod = string.Empty;

			if(dct.ContainsKey(imp.Config.CampoCodice))
				{
				cod = dct[imp.Config.CampoCodice];
				if(cod.Length > 0)	ok = true;
				}
			if(dct.ContainsKey(imp.Config.CampoModifica))
				{
				mod = dct[imp.Config.CampoModifica];
				}
			
			StringBuilder sb = new StringBuilder();
			foreach(var item in dct)
				{
				sb.AppendLine($"[{item.Key}]={item.Value}");
				}
			if (ok)
				{
				DialogData dd = new DialogData(imp);
				int nsetcampi;
				dd.Set(dct, out nsetcampi);
				dd.TipoDialog = DialogData.TipoDialogEnum.Modifica;
				dd.CanWrite = this.CanWrite;

				if(QueryDialog(ref dd))			// Modifica codice singolo...
					{
					ApplicaComandoDialogData(dd);
					}
				}
			}
#endif

		public async void ApplicaComandoDialogData(DialogData dd)
			{
			
			string[] p = null;										// Array dei parametri
			bool ok = false;
			Risposta res = null;

			switch(dd.DdResult)
				{
				case DialogData.DialogDataResult.Annulla:			// Non fa nulla
					break;
				case DialogData.DialogDataResult.Cerca:				// Cerca codice con parametri multipli
					{
					MessageBox.Show($"Ricerca con più parametri: al momento non disponibile");
					}
					break;
				case DialogData.DialogDataResult.Elimina:			// Elimina codice dal database
				case DialogData.DialogDataResult.Scrivi:			// Modifica o aggiunge codice nel database
					{
					if(dd.CanWrite)
						{
						p = dd.ToParamsArray();						// Ottiene l'array dei parametri
						if (p != null)
							{
							int n;
							res = await conn.Conta(p[1], p[2]);		// Richiede conteggio dei codici
							int.TryParse(res.datDB, out n);			// Estrae il numero
							switch(n)
								{
								case 0:								// Nessun codice trovato: ok se scrittura, no se eliminazione
									{
									if(dd.DdResult == DialogData.DialogDataResult.Scrivi)
										{
										ok = true;
										}
									else if(dd.DdResult == DialogData.DialogDataResult.Elimina)
										{
										log.ScriviLog($"Codice non trovato");
										}
									}
									break;
								case 1:								// Trovato un codice, ok se conferma dell'utente.
									{
									if(MessageBox.Show($"Codice {p[1]}{p[2]} esistente. Proseguire ?", dd.DdResult.ToString(),MessageBoxButtons.YesNo)==DialogResult.Yes)
										{
										ok = true;
										}
									}
									break;
								default:							// Trovati codici multipli. Errore.
									{
									log.ScriviLog($"Trovati {n} codici");
									}
									break;
								}
							}

						}
					else
						{
						log.ScriviLog("Scrittura non abilitata.");
						}
					}
					break;
				}	// Fine switch()

			res = null;
			if(ok && p!=null)
				{
				switch(dd.DdResult)
					{
					case DialogData.DialogDataResult.Scrivi:
						{
						res = await conn.InserisciNew(dd.Tipo, p);
						}
						break;
					case DialogData.DialogDataResult.Elimina:
						{
						res = await conn.Elimina(p[1], p[2]);				
						}
					break;
					}
				}

			if(res != null)
				{

				#warning Analizzare la risposta (se non ci sono errori)
	
				// MessageBox.Show(res.ToString());

				}
			}

		#endregion



		#region Query

		/// <summary>
		/// Apre la dialog di ricerca per codice, poi mostra quella dei risultati
		/// </summary>
		private async void Vedi(DialogData.TipoDialogEnum opSuDoppioClick)
			{
			int righe;

			string[] codmod = ChiediCodicePerRicerca();						// Richiede codice e modifica, per ricerca
			DialogData dd = new DialogData(imp, codmod[0], codmod[1]);		// Crea una DialogData con i campi (solo cod e mod) per la ricerca
			dd.TipoRicerca = DialogData.TipoRicercaEnum.PerCodice;			// Imposta il tipo di ricerca
			ResultForm qrf = new ResultForm("Risultati ricerca", imp, dd, opSuDoppioClick);		// Crea il form...
			righe = await qrf.Cerca();										// Esegue la ricerca (asincrona)

			UpdateLayout();
			}

		/// <summary>
		/// Richiede codice e modifica con un form.
		/// </summary>
		/// <returns>string[2] codice e modifica</returns>
		private string[] ChiediCodicePerRicerca()
			{
			string[] codmod = new string[2];
			if (conn != null)
				{
				InputForm lf = new InputForm("Codice", new string[] { "Codice", "Modifica" }, out codmod, "Cerca", "Annulla", "Usare * per tutti i caratteri o <spazio> per nessun carattere");

				if (lf.ShowDialog() == DialogResult.OK)
					{
					if (codmod[0] == "") codmod[0] = "*";
					if (codmod[1] == "") codmod[1] = "*";
					}
				}
			return codmod;
			}

		/// <summary>
		/// Esegue ricerca con codice e modifica
		/// </summary>
		/// <param name="cod">string[2]: codice, modifica</param>
		/// <returns>Task<Risposta></Risposta></returns>
		public async Task<Risposta> CercaPerCodice(string cod, string mod)
			{
			Risposta res = new Risposta();
			if (conn != null)
				{
				res = await conn.VediDescrizioni(cod, mod);
#if RESPONSE_DEBUG
				MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
				}
			return res;
			}

#if DEBUG
		/// <summary>
		/// Apre dialog di ricerca per codice (singolo)
		/// </summary>
		/// <returns>Dizionario con la risposta del database</returns>
		private async Task<Dictionary<string, string>> CercaPerCodiceSingolo()
			{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			if (conn != null)
				{
				string[] par;
				InputForm lf = new InputForm("Codice", new string[] { "Codice", "Modifica" }, out par, "Cerca", "Annulla");
				if (lf.ShowDialog() == DialogResult.OK)
					{
					dict = await EstraiDatiCodiceSingolo(par[0], par[1]);
					UpdateLayout();
					}
				}
			return dict;
			}

		/// <summary>
		/// Esegue la ricerca con codice e modifica (chiave primaria)
		/// </summary>
		/// <param name="cod">Codice</param>
		/// <param name="mod">Modifica</param>
		/// <returns>Dizionario con la risposta del database</returns>
		public async Task<Dictionary<string, string>> EstraiDatiCodiceSingolo(string cod, string mod)
			{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			if (conn != null)
				{
				Risposta res;
				res = await conn.EstraiDatiCodice(cod, mod);			// Ottiene tutti i dati del codice
				dict = res.GetValues(0);									// Li inserisce in un dizionario
#if RESPONSE_DEBUG
				MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
				log.ScriviLog(res.MessagesToString());
				}
			return dict;
			}
#endif 
		/// <summary>
		/// Estrae dal database tutti i dati del codice codmod,mod e li inserisce in una DialogData
		/// </summary>
		/// <param name="cod"></param>
		/// <param name="mod"></param>
		/// <returns></returns>
		public async Task<DialogData> DialogDataDaCodiceSingolo(string cod, string mod)
			{
			DialogData dd = new DialogData(imp);
			Dictionary<string, string> dict = new Dictionary<string, string>();
			if (conn != null)
				{
				Risposta res;
				res = await conn.EstraiDatiCodice(cod, mod);			// Ottiene tutti i dati del codice.
				dict = res.GetValues(0);								// Li estrae della prima riga e li inserisce in un dizionario 
#if RESPONSE_DEBUG
				MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
				int nsetcampi;
				dd.Set(dict, out nsetcampi);							// Imposta la DialogData con i dati del dizionario
				//log.ScriviLog(res.MessagesToString() +"\n" + dd.ToString());	// Log della risposta
				}
			return dd;
			}

		/// <summary>
		/// Aggiorna le liste della configurazione leggendole dal database
		/// </summary>
		private async void Liste()
			{
			if (conn != null)
				{
				foreach (DatiCampo dc in imp.CampiLista())
					{
					Risposta res = await conn.Liste(dc.tabella);
					imp.RiempiLista(dc.tabella, res);
#if RESPONSE_DEBUG
					MessageBox.Show($"NUOVO\n{res.ToString()}",$"{url}");
#endif
					}
				UpdateLayout();
				}
			}

		/// <summary>
		/// Imposta come 'dirty' (da aggiornare) la lista 'i' del database
		/// </summary>
		/// <param name="i"></param>
		private async void Dirty(DirtyEnum i)
			{
			if (conn != null)
				{
				Risposta res = await conn.Dirty(i);
#if RESPONSE_DEBUG
				MessageBox.Show($"{res.ToString()}",$"{url}");
#endif
				log.ScriviLog(res.MessagesToString());
				UpdateLayout();
				}
			}

		/// <summary>
		/// Apre una QueryDialog con i dati di DialogData
		/// </summary>
		/// <param name="dd"></param>
		/// <returns>true se i valori sono stati accettati</returns>
		public bool QueryDialog(ref DialogData dd)
			{
			bool ok = false;
			QueryFilterForm qf = new QueryFilterForm(ref dd, imp, CheckDialogData);
		
			if (qf.ShowDialog() == DialogResult.OK)
				{
				ok = true;
				}
			return ok;
			}

		#endregion



		#region Interfaccia

		/// <summary>
		/// Aggiorna il layout e
		/// avvia o arresta il timer in base allo stato della connessione
		/// </summary>
		void UpdateLayout()
			{
			if (conn != null)
				{
				switch (conn.connected)
					{
					case Risposta.STAT.lettura:			// Connesso
					case Risposta.STAT.scrittura:	
						mLogin.Enabled = false;
						mloginAutomatico.Enabled = false;
						mCheck.Enabled = true;
						mLogout.Enabled = true;
						mChangePassword.Enabled = true;
						mScrittura.Enabled = true;
						if (!timerRefresh.Enabled)      // Avvia il timer, equivale a Timer.Start()
							timerRefresh.Enabled = true;

						if (conn.connected == Risposta.STAT.scrittura)
							{
							mScrittura.Text = "Disabilita scrittura";
							tsStato.Text = "Utente " + conn.user + " CONNESSO, scrittura abilitata";
							tsStato.ForeColor = Color.Green;
							nuovoCodiceToolStripMenuItem.Enabled = true;
							eliminaCodiceToolStripMenuItem.Enabled = true;
							mnVedi.Enabled = true;
							mnModificaCodice.Enabled = true;
							mnListeTest.Enabled = true;
							mnQuery.Enabled = true;
							}
						else
							{
							mScrittura.Text = "Abilita scrittura";
							tsStato.Text = "Utente " + conn.user + " CONNESSO, scrittura non abilitata";
							tsStato.ForeColor = Color.Blue;
							nuovoCodiceToolStripMenuItem.Enabled = false;
							eliminaCodiceToolStripMenuItem.Enabled = false;
							mnVedi.Enabled = true;
							mnModificaCodice.Enabled = false;
							mnListeTest.Enabled = true;
							mnQuery.Enabled = true;
							}
						break;
					default:        // Disconnesso
						{
						mLogin.Enabled = true;
						mloginAutomatico.Enabled = true;
						//mCheck.Enabled = false;
						mLogout.Enabled = false;
						mChangePassword.Enabled = false;
						mnVedi.Enabled = true;   // false
						mScrittura.Enabled = false;
						tsStato.Text = "NON CONNESSO";
						tsStato.ForeColor = Color.Black;
						nuovoCodiceToolStripMenuItem.Enabled = false;
						eliminaCodiceToolStripMenuItem.Enabled = false;
						mnVedi.Enabled = false;
						mnModificaCodice.Enabled = false;
						mnListeTest.Enabled = false;
						mnQuery.Enabled = false;
						timerRefresh.Enabled = false;       // Arresta il timer, equivale a Timer.Stop()	
						}
					break;
					}
				}
			rtbMessages.ScrollToCaret();

			Invalidate();
			}
		/// <summary>
		/// Stringa con versione completa
		/// </summary>
		/// <returns></returns>
		public string Versione()
			{
			StringBuilder strb = new StringBuilder();
			//FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
			strb.Append("Informazioni sul programma" + System.Environment.NewLine + System.Environment.NewLine);
			strb.Append(Application.ProductName + System.Environment.NewLine);
			strb.Append("Copyright " + Application.CompanyName + System.Environment.NewLine);
			strb.Append("Versione: " + Application.ProductVersion + System.Environment.NewLine);
			strb.Append("Build: " + Build() + System.Environment.NewLine);
			strb.Append("Eseguibile: " + fileinfo.FullName + System.Environment.NewLine);
			strb.Append("Configurazione: " + configfile + System.Environment.NewLine);

			return strb.ToString();
			}
		/// <summary>
		/// String con numero della build
		/// </summary>
		/// <returns></returns>
		public string Build()
			{
			StringBuilder strb = new StringBuilder();
			//DateTime dt = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
			DateTime dt = fileinfo.LastWriteTime;
			strb.Append(dt.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
			strb.Append('.');
			strb.Append(dt.ToString("HHmm", System.Globalization.CultureInfo.InvariantCulture));
			return strb.ToString();
			}

		#endregion



		#region PER TEST

		private void LOGINPippoToolStripMenuItem_Click(object sender, EventArgs e)
			{
			Login("pippo", "123");
			}
		private void leggiValoreToolStripMenuItem_Click(object sender, EventArgs e)
			{
			int riga;
			string titolo = string.Empty;

			string[] par;
			InputForm lf = new InputForm("Leggi valore da titolo e riga", new string[] { "Titolo", "Riga" }, out par);
			if (lf.ShowDialog() == DialogResult.OK)
				{
				titolo = par[0];
				int.TryParse(par[1], out riga);

				MessageBox.Show("Titolo " + titolo + "\nRiga" + riga.ToString());

				}
			}
		private void costruttoriDirtyToolStripMenuItem_Click(object sender, EventArgs e)
			{
			Dirty(DirtyEnum.costruttori);
			}

		#endregion


		}
	}
