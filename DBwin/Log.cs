using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBwin
	{
	class Log
		{
		bool _writeEnabled;					// Scrittura abilitata (solo se utente ha fatto il login)
		bool _active;						// Scrittura su file attiva o disattiva
		
		#warning	Vedere bene su quali file scrivere: se con il nome dell'utente attivo, un nome casuale o un nome fisso o impostabile.

		StreamWriter sw = null;
		DelegateMsg _funzioneAggiuntiva;

		public Log(DelegateMsg _func_scrivi)
			{
			_funzioneAggiuntiva = _func_scrivi;
			_active = _writeEnabled = false;
			sw = new StreamWriter("Log.txt",true);
			sw.AutoFlush = true;
			}

		public void ScriviLog(string msg)
			{
			_funzioneAggiuntiva(msg);
			}


		}
	}
