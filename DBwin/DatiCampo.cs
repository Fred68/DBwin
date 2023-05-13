using System;

using System.Reflection;

namespace DBwin
	{
	public class DatiCampo : IComparable<DatiCampo>
		{
		/// <summary>
		/// Nome del campo della risposta alla query
		/// </summary>
		public string query;
		/// <summary>
		/// Numero d'ordine
		/// </summary>
		public int ordine;
		/// <summary>
		/// Lista L o testo T
		/// </summary>
		public Impostazioni.TipoInput tipo;
		/// <summary>
		/// Nome della tabella
		/// </summary>
		public string tabella;
		/// <summary>
		/// Visibilità con Assieme, Schema, Particolare, Commerciale (v true . false)
		/// </summary>
		public bool[] flag;
		/// <summary>
		/// Testo della label a fianco del campo
		/// </summary>
		public string label;
		/// <summary>
		/// Se sola lettura (v true)
		/// </summary>
		public bool readOnly;
		/// <summary>
		/// Numero del parametro del POST al programma PHP del database
		/// </summary>
		public int par;
		/// <summary>
		/// Numero massimo di caratteri del campo (o del testo della tabella)
		/// </summary>
		public int lmax;

		bool bOk;

		static int nParm;       // Numero di parametri della funzione (costruttore o Set())
		static int nFlg;        // Numero di flag, lunghezza array

		/// <summary>
		/// COSTRUTTORE statico
		/// Usato solo per impostare il numero di campi (con Reflection)
		/// </summary>
		static DatiCampo()
			{
			nFlg = Enum.GetValues(typeof(Impostazioni.TipoCodice)).Length - 1;
			new DatiCampo();		// Chiama il costruttore per un oggetto vuoto (poi eliminato dal GC)
			}

		public static int nParam => nParm;
		public static int nFlag => nFlg;
		public bool isOk => bOk;

		/// <summary>
		/// COSTRUTTORE vuoto (chiamato solo dal costruttore statico, per inizalizzare i parametri
		/// </summary>
		public DatiCampo()
			{
			flag = new bool[nFlg];
			this.Set("", "", "", "", "", "", "", "");
			lmax = 0;
			bOk = false;
			}

		/// <summary>
		/// COSTRUTTORE con parametri
		/// Il 1° campo (query) è l'indice per i Dictionary<string,...>
		/// </summary>
		/// <param name="_query">Nome del campo della risposta alla query</param>
		/// <param name="_ordine">Numero d'ordine</param>
		/// <param name="_tipo">Lista L o testo T</param>
		/// <param name="_tabella">Nome della tabella</param>
		/// <param name="_flag"> Visibilità con Assieme, Schema, Particolare, Commerciale (v true . false)</param>
		/// <param name="_label">Testo della label a fianco del campo</param>
		/// <param name="_readOnly">Se sola lettura (v true)</param>
		/// <param name="_par">Numero del parametro per il POST[] al database</param>
		public DatiCampo(string _query, string _ordine, string _tipo, string _tabella, string _flag, string _label, string _readOnly, string _par)
			{
			this.Set(_query, _ordine, _tipo, _tabella, _flag, _label, _readOnly, _par);
			}

		/// <summary>
		/// Imposta i campi (vd. ctor per i dettagli)
		/// </summary>
		/// <param name="_query"></param>
		/// <param name="_ordine"></param>
		/// <param name="_tipo"></param>
		/// <param name="_tabella"></param>
		/// <param name="_flag"></param>
		/// <param name="_label"></param>
		/// <param name="_readOnly"></param>
		/// <param name="_par"></param>
		void Set(string _query, string _ordine, string _tipo, string _tabella, string _flag, string _label, string _readOnly, string _par)
			{
			bOk = true;
			nParm = MethodBase.GetCurrentMethod().GetParameters().Length;
			query = _query;
			int lc = 0;								// Lunghezza campo
			if (!int.TryParse(_ordine, out ordine))
				{
				ordine = -1;
				bOk = false;
				}
			if(_tipo.Length == 0)	_tipo = "E";	// Se string vuota, la imposta a lunghezza non nulla, ma con valore non riconosciuto
			switch (_tipo[0])						// Usa solo il primo carattere della stringa.
				{
				case 'L':
					{
					tipo = Impostazioni.TipoInput.lista;
					if(int.TryParse(_tipo.Substring(1), out lc))
						{
						lmax = lc;
						}
					}
					break;
				case 'T':
					{
					tipo = Impostazioni.TipoInput.testo;
					if(int.TryParse(_tipo.Substring(1), out lc))
						{
						lmax = lc;
						}
					}
					break;
				default:
					tipo = Impostazioni.TipoInput.nessuno;
					bOk = false;
					break;
				}
			tabella = _tabella;
			string flagP = _flag.PadRight(Impostazioni.nTipi, '.');
			flag = new bool[Impostazioni.nTipi];
			for (int i = 0; i < Impostazioni.nTipi; i++)
				flag[i] = (flagP[i] == 'v') || (flagP[i] == 'V');
			label = _label;
			switch (_readOnly)
				{
				case "v":
				case "V":
					readOnly = true;
					break;
				default:
					readOnly = false;
					break;
				}
			if (!int.TryParse(_par, out par))
				{
				par = -1;
				bOk = false;
				}
			}
		
		/// <summary>
		/// Per ordinamento (interfaccia IComparable<>)
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(DatiCampo other)
			{
			if (other == null)
				return 1;
			else
				return this.ordine.CompareTo(other.ordine);
			}
		}

	}
