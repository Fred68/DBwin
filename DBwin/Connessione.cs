using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;

#if DEBUG
using System.Windows.Forms;
#endif

namespace DBwin
	{
	public class Connessione
		{
		static HttpClient _httpClient;
		Uri _uri;
		Risposta.STAT _connected;
		string _user;
		int _maxRowResponse;


		/// <summary>
		/// Corrispondenze argomenti e parametri inviati
		/// </summary>
		public static Dictionary<string, string> Comandi = new Dictionary<string, string>()
			{
				{"COMMAND","P0"},
				{"USER","P1"},
				{"PASSWORD","P2"},
				{"NEWPASSWORD","P3"},
				{"STAT","P3"},
				{"P1","P1"},
				{"P2","P2"},
				{"P3","P3"},
				{"P4","P4"},
				{"P5","P5"},
				{"P6","P6"},
				{"P7","P7"},
				{"P8","P8"},
				{"P9","P9"},
				{"P10","P10"}
			};

		public string uri => _uri.ToString();
		public Risposta.STAT connected => _connected;
		public string user => _user;
		/// <summary>
		/// COSTRUTTORE
		/// </summary>
		/// <param name="url">Connection Url</param>
		public Connessione(string url, int maxRowResp)
			{
			_uri = new Uri(url);
			_connected = Risposta.STAT.disconnesso;
			_httpClient = new HttpClient();
			_maxRowResponse = maxRowResp;
			}

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="user"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<Risposta> Login(string user, string password)
			{
			Risposta r = new Risposta();
			if ((int)_connected < 1)
				{
				SHA256 hash = SHA256.Create();
				byte[] bt = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
				StringBuilder strb = new StringBuilder();
				for (int i = 0; i < bt.Length; i++)
					{
					strb.Append(bt[i].ToString("x2"));
					}
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "login"),
					new KeyValuePair<string, string>(Comandi["USER"], user.ToLower()),
					new KeyValuePair<string, string>(Comandi["PASSWORD"], strb.ToString())
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			return r;
			}

		/// <summary>
		/// Check stato connessione
		/// </summary>
		/// <returns></returns>
		public async Task<Risposta> Check()
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "stat")
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts; //UpdateConnected(res);
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		/// <summary>
		/// Stored routine: Vedi descrizioni
		/// </summary>
		/// <returns></returns>
		public async Task<Risposta> VediDescrizioni(string _cod, string _mod)
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "vedi"),
					new KeyValuePair<string, string>(Comandi["P1"], _maxRowResponse.ToString()),
					new KeyValuePair<string, string>(Comandi["P2"], _cod),
					new KeyValuePair<string, string>(Comandi["P3"], _mod)
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		public async Task<Risposta> EstraiDatiCodice(string _cod, string _mod)
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "getCode"),
					new KeyValuePair<string, string>(Comandi["P1"], _cod),
					new KeyValuePair<string, string>(Comandi["P2"], _mod)
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		/// <summary>
		/// Stored routine: Conta codici
		/// </summary>
		/// <param name="_cod"></param>
		/// <param name="_mod"></param>
		/// <returns></returns>
		public async Task<Risposta> Conta(string _cod, string _mod)
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "countCode"),
					new KeyValuePair<string, string>(Comandi["P1"], _cod),
					new KeyValuePair<string, string>(Comandi["P2"], _mod)
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		/// <summary>
		/// Inserisce o aggiorna un codice del database
		/// </summary>
		/// <param name="t"></param>
		/// <param name="par"></param>
		/// <returns></returns>
#warning Da completare !
		public async Task<Risposta> Inserisci(TabelleDati.Tipi t, string[] par)
			{
			Risposta r = new Risposta();
			if (((int)_connected > 0) && (par.Length == 10))
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "insert"),
					new KeyValuePair<string, string>(Comandi["P1"], par[1]),
					new KeyValuePair<string, string>(Comandi["P2"], par[2]),
					new KeyValuePair<string, string>(Comandi["P3"], par[3]),
					new KeyValuePair<string, string>(Comandi["P4"], par[4]),
					new KeyValuePair<string, string>(Comandi["P5"], par[5]),
					new KeyValuePair<string, string>(Comandi["P6"], par[6]),
					new KeyValuePair<string, string>(Comandi["P7"], par[7]),
					new KeyValuePair<string, string>(Comandi["P8"], par[8]),
					new KeyValuePair<string, string>(Comandi["P9"], par[9]),
					new KeyValuePair<string, string>(Comandi["P10"], t.ToString())
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		public async Task<Risposta> InserisciNew(Impostazioni.TipoCodice t, string[] par)
			{
			Risposta r = new Risposta();
			if (((int)_connected > 0) && (par.Length == 10))
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "insert"),
					new KeyValuePair<string, string>(Comandi["P1"], par[1]),
					new KeyValuePair<string, string>(Comandi["P2"], par[2]),
					new KeyValuePair<string, string>(Comandi["P3"], par[3]),
					new KeyValuePair<string, string>(Comandi["P4"], par[4]),
					new KeyValuePair<string, string>(Comandi["P5"], par[5]),
					new KeyValuePair<string, string>(Comandi["P6"], par[6]),
					new KeyValuePair<string, string>(Comandi["P7"], par[7]),
					new KeyValuePair<string, string>(Comandi["P8"], par[8]),
					new KeyValuePair<string, string>(Comandi["P9"], par[9]),
					new KeyValuePair<string, string>(Comandi["P10"], t.ToString())
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		public async Task<Risposta> Elimina(string _cod, string _mod)
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "delete"),
					new KeyValuePair<string, string>(Comandi["P1"], _cod),
					new KeyValuePair<string, string>(Comandi["P2"], _mod)
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		public async Task<Risposta> Liste(string tabella)
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				//string res = string.Empty;
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], tabella),
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts;
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		/// <summary>
		/// Imposta la lista 'i' come 'dirty'
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public async Task<Risposta> Dirty(DirtyEnum i)
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "dirty"),
					new KeyValuePair<string, string>(Comandi["P1"], ((int)i).ToString())
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts; //UpdateConnected(res);
				_user = r.usrDB;
				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		/// <summary>
		/// Esegue logout
		/// </summary>
		/// <returns></returns>
		public async Task<Risposta> Logout()
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "logout")
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts; //UpdateConnected(res);
				_user = r.usrDB;

				}
			else
				{
				r.msg = "Utente non connesso";
				}
			return r;
			}

		/// <summary>
		/// Cambia password
		/// </summary>
		/// <param name="user">Utente</param>
		/// <param name="password">Vecchia password</param>
		/// <param name="newpassword">Nuova password</param>
		/// <param name="np">Numero password: 1 di accesso, 2 di scrittura</param>
		/// <returns></returns>
		public async Task<Risposta> ChangePwd(string user, string password, string newpassword, int np)
			{
			string comm = "changepwd" + np.ToString();
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				SHA256 hash = SHA256.Create();
				byte[] bt = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
				StringBuilder strb = new StringBuilder();
				for (int i = 0; i < bt.Length; i++)
					{
					strb.Append(bt[i].ToString("x2"));
					}
				bt = hash.ComputeHash(Encoding.UTF8.GetBytes(newpassword));
				StringBuilder strb2 = new StringBuilder();
				for (int i = 0; i < bt.Length; i++)
					{
					strb2.Append(bt[i].ToString("x2"));
					}
				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], comm),
					new KeyValuePair<string, string>(Comandi["USER"], user.ToLower()),
					new KeyValuePair<string, string>(Comandi["PASSWORD"], strb.ToString()),
					new KeyValuePair<string, string>(Comandi["NEWPASSWORD"], strb2.ToString())
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts; //UpdateConnected(res);
				_user = r.usrDB;
				}
			return r;
			}

		public async Task<Risposta> WriteEnable(bool enable, string password2)
			{
			Risposta r = new Risposta();
			if ((int)_connected > 0)
				{
				SHA256 hash = SHA256.Create();
				byte[] bt = hash.ComputeHash(Encoding.UTF8.GetBytes(password2));
				StringBuilder strb = new StringBuilder();
				for (int i = 0; i < bt.Length; i++)
					{
					strb.Append(bt[i].ToString("x2"));
					}

				FormUrlEncodedContent stringContent = new FormUrlEncodedContent(new[]{
					new KeyValuePair<string, string>(Comandi["COMMAND"], "setwrite"),
					new KeyValuePair<string, string>(Comandi["P1"], strb.ToString()),
					new KeyValuePair<string, string>(Comandi["P2"], enable ? "2" : "1")
					});
				r = await SendHttpAndRead(stringContent);
				_connected = r.sts; //UpdateConnected(res);
				_user = r.usrDB;
				}

			return r;
			}

		/// <summary>
		/// Esegue chiamata httpRequest e legge la risposta
		/// </summary>
		/// <param name="post"></param>
		/// <returns></returns>
		protected async Task<Risposta> SendHttpAndRead(FormUrlEncodedContent post)
			{
			Risposta r = new Risposta();
			string res = string.Empty;
			try
				{
				HttpResponseMessage msgResp = await _httpClient.PostAsync(_uri, post);
				res = await msgResp.Content.ReadAsStringAsync();
				}
			catch (Exception e)
				{
				res = Risposta.SEP_Err + e.ToString() + Risposta.SEP_Err + Risposta.SEP_righe;
				}
			r.SetRisposta(res);

#if DEBUG
			MessageBox.Show($"{res.ToString()}", $":::");
#endif
			return r;
			}

		}
	}
