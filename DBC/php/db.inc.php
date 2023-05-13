<?php
class Connessione
	{

	// Database
    //
	var $conn_file = 'conn.txt';
	var $servername = ""; // "127.0.0.1";
	var $username = ""; // "root";
	var $password = "";
	var $conn = null;
	var $DB = ""; // "dbc01";
	var $max_delay = 1; // in sec

	// Tabella utenti e nomi dei campi
    //
	var $utenti = "utenti";
	var $utente = "utente";
	var $id = "id";
	var $passwd = "password1";
	var $passwd2 = "password2";
	var $canwrite = "can_write";
	var $newpasswd = "newpassword";
	var $sigla = "sigla";

	// Array con i comandi SQL
    //
	protected $cmd = array();

	// Utente attualmente connesso
    //
	var $uid = -1;
	var $pwd;
	var $par;	// array parametri
	var $perm = 1;	// Permesso richiesto (0:disconnette, 1:lettura, 2:scrittura)

	// Valori restituiti con la risposta
    //
	var $err = "";		// Messaggi di errore
	var $msg = "";		// Messaggi informativi
	var $dat = "";		// Dati
	var $sts = "";		// Stati. 0=disconnesso, 1:in lettura, 2:in scrittura, -1:errore, -2:disconnesso per inattività
	var $usr = "";		// Nome utente
	var $sleepDelay = 200;	// ms Ritardo dopo il comando
    var $pwdDelay = 500; // ms Ritardo dopo inserimento password (contro attacchi ripetuti)

	// NOMI DEI PARAMETRI delle funzioni sql
    //
	var $p_limite = "limite";	// Limite righe
	var $p_uid = "_uid";		// UID
	var $p_st = "_st";			// stato
	var $p_cod = "_cod";
	var $p_mod = "_mod";
	var $p_desc = "_desc";
	var $p_mat = "_mat";
	var $p_cos = "_cos";
	var $p_pro = "_pro";
	var $p_model = "_model";
	var $p_dett = "_dett";
	var $p_dirty = "_dirty";

	// Per insert:
    //
	/*
	InsAssieme(_cod, _mod, _desc, _uid)
	InsSchema(_cod, _mod, _desc, _uid)
	InsCommerciale(_cod, _mod, _uid. _cos, _pro, _model, _dett). _desc costruita da altri dati
	InsParticolare(_cod, _mod, _desc, _uid, _mat)
	Quindi:
	1		2		3		4		4		6		7		8		9
	_cod	_mod	_desc	_uid	_mat	_cos	_pro	_model	_dett
	*/

    /**************************************/
	// Variabili di sessione (se connesso)
    /**************************************/
	// $_SESSION["USER"]		nome utente
	// $_SESSION["UID"]			id utente
	// $_SESSION["STS"]			stato utente: -1=errato, 0=disconnesso, 1=lettura, 2=scrittura
    /**************************************/

    // COSTRUTTORE.
    // Inizializza variabili, array/dizionario dei comandi
    public function __construct()
		{
		$this->par = array();
		$this->ClearMsg();
		$this->ConstructName($this->DB);
		if($f = fopen($this->conn_file,'r'))
			{
			$this->servername = trim(fgets($f));
			$this->username = trim(fgets($f));
			$this->password = trim(fgets($f));
			$this->DB = trim(fgets($f));
			$this->max_delay = trim(fgets($f));
			}
		}



    /**************************************/
    //		LETTURA del POST
    /**************************************/

    // Analizza il post e chiama le rispettive funzioni php
    //
	public function ProcessPost()
		{
		$sep = "<#>";
		switch($this->par[0])
			{
			case "login":
				{
				$this->Login($this->par[1],$this->par[2]);
				}
				break;
			case "stat":
				{
				$this->Stat();
				}
				break;
			case "logout":
				{
				$this->Logout("Utente <".$_SESSION["USER"]."> disconnesso");
				}
				break;
			case "setwrite":
				$this->SetWrite($this->par[1],$this->par[2]);
				break;
			case "changepwd1":
				{
				$this->ChangePwdIfLogged($this->par[1],$this->par[2],$this->par[3],"1");
				$this->Logout("Cambiata password. Riconnettersi.");
				}
				break;
			case "changepwd2":
				{
				$this->ChangePwdIfLogged($this->par[1],$this->par[2],$this->par[3],"2");
				$this->Logout("Cambiata password. Riconnettersi.");
				}
				break;
			case "vedi":
				{
				$this->Vedi($this->par[1],$this->par[2],$this->par[3]);
				}
				break;
            case "get_delay":
                {
                $this->GetDelay();
                }
			// Chiamata comune a Liste(par[0]).
			case "materiali":
			case "prodotti":
			case "costruttori":
				{
				$this->Liste($this->par[0]);
				}
				break;
			case "dirty":
				{
				$this->Dirty($this->par[1]);
				}
				break;
			//
			case "insert":
				{	// par[4] UID non viene usato. Usato direttamente il $_SESSION['UID']
				$this->Inserisci($this->par[1],$this->par[2],$this->par[3],$this->par[4],$this->par[5],$this->par[6],$this->par[7],$this->par[8],$this->par[9],$this->par[10]);
				}
				break;
			case "countCode":
				{
				$this->ContaCodici($this->par[1],$this->par[2]);
				}
				break;
			case "getCode":
				{
				$this->GetCode($this->par[1],$this->par[2]);
				}
				break;
            case "delete":
                {
                $this->Elimina($this->par[1], $this->par[2]);
                }
                break;
			}
		usleep($this->sleepDelay*1000);
		echo "\n".$sep."MSG".$sep.$this->msg.$sep."MSG".$sep."\n";
		echo "\n".$sep."ERR".$sep.$this->err.$sep."ERR".$sep."\n";
		echo "\n".$sep."STS".$sep.$this->sts.$sep."STS".$sep."\n";
		echo "\n".$sep."DAT".$sep.$this->dat.$sep."DAT".$sep."\n";
		echo "\n".$sep."USR".$sep.$this->usr.$sep."USR".$sep."\n";
		}

    // Ripulisce il testo da caratteri superflui (contro sql injection)
    //
	function TestInput($data)
		{
		$data = trim($data);
		$data = stripslashes($data);
		$data = htmlspecialchars($data);
		$data = preg_replace('/[^[:alnum:][:space:]°àòèéìù()!-.:=?*\/]/m','',$data);
		$data = preg_replace("/[*]/m","%",$data);
		$data = preg_replace("/[?]/m","_",$data);
		return $data;
		}

    // Legge un Post
    //
	public function ReadPost()
		{
		$max = 20;
		if ($_SERVER["REQUEST_METHOD"] == "POST")
			{
			for($i=0; $i<$max; $i++)
				{
				$this->par[$i] = "-";
				if(isset($_POST["P$i"]))
					$this->par[$i] = $this->TestInput($_POST["P$i"]);
				else
					$i = $max+1;
				}
			}
		}

    // Mostra il Post
    //
	public function ShowPost()
		{
		echo "Command: ".$this->par[0]."\n";
		echo "User: ".$this->par[1]."\n";
		echo "Hash: ".$this->par[2]."\n";
		}

    /**************************************/
    //		DATABASE e UTENTI
    /**************************************/

    // Connette al DB, restituisce true se connesso, false se errore
	// Aggiorna $this->conn e lo usa per evitare di riconnettersi, se lo è già.
	public function ConnectDB()
		{
		$ok = false;
		try {
			if($this->conn == null)
				{
				$this->conn = new PDO("mysql:host=$this->servername;dbname=$this->DB", $this->username, $this->password);
				$this->conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
				$ok = true;
				}
			else
				{
				$ok = true;
				}
			}
		catch(PDOException $e)
			{
			$this->Err("Fallita connessione. Errore: " . $e->getMessage());
			$ok = false;
			}
		return $ok;
		}

	// Disconnette dal DB
    //
	function DisconnectDB()
		{
		if($this->conn != null)
			{
			$this->conn = null;
			}
		}

	// Conta gli utenti con nome, password di lettura o scrittura
	// Restituisce 0 o 1
	// Deve essere già connesso al database
	public function CountUsers($uname, $upwd, $rw = 1)
		{
		$cnt = 0;
		try
			{
			switch($rw)
				{
				case 1:
					$stmt = $this->conn->prepare($this->cmd["count_users"]);
					$stmt->bindParam(":".$this->utente,$uname);
					$stmt->bindParam(":".$this->passwd,$upwd);
					$stmt->execute();
					$cnt = $stmt->fetchColumn();
					break;
				case 2:
					$stmt = $this->conn->prepare($this->cmd["count_wusers"]);
					$stmt->bindParam(":".$this->utente,$uname);
					$stmt->bindParam(":".$this->passwd2,$upwd);
					$stmt->execute();
					$cnt = $stmt->fetchColumn();
					break;
				default:
					break;
				}
			}
		catch(PDOException $e)
			{
			$this->Err("Fallito conteggio. Errore: " . $e->getMessage());
			}
		return $cnt;
		}

	// Restituisce l'ID dell' utente
	// Deve essere già connesso al database
	// Restituisce -1 oppure 0 se errore.
	public function GetUserID($uname)
		{
		$uid = -1;
		try
			{
			$stmt = $this->conn->prepare($this->cmd["get_uid"]);
			$stmt->bindParam(":".$this->utente, $uname);
			$stmt->execute();
			$uid = $stmt->fetchColumn();
			}
		catch(PDOException $e)
			{
			$this->Err("Fallita ricerca user id. Errore: " . $e->getMessage());
			}
		return $uid;
		}

	// Restituisce lo stato di connessione dell' utente: 0, 1 o 2
	// Aggiorna il timestamp sell'utente connesso
	// Deve essere già connesso al database
    public function GetUserStat($user_id)
		{
		$st = 0;
		try
			{
			$stmt = $this->conn->prepare($this->cmd["get_uid_stat"]);
			$stmt->bindParam(":".$this->p_uid, $user_id);
			$stmt->execute();
			$st = $stmt->fetchColumn();
			// $this->Msg("Stato = ".$st);
			}
		catch(PDOException $e)
			{
			$this->Err("Utente non connesso. Errore: " . $e->getMessage());
			}
		return $st;
		}

	// Restituisce i secondi trascorsi dall'ultima connessione dell'utente
	// Se non è connesso
	// Deve essere già connesso al database
	public function GetUserDelay($user_id)
		{
		$st = 0;
		try
			{
			$stmt = $this->conn->prepare($this->cmd["get_uid_delay"]);
			$stmt->bindParam(":".$this->p_uid, $user_id);
			$stmt->execute();
			$st = $stmt->fetchColumn();
			}
		catch(PDOException $e)
			{
			$this->Err("Utente non connesso. Errore: " . $e->getMessage());
			}
		return $st;
		}

	// Imposta lo stato di connessione dell'utente
	// Restituisce lo stato di connessione dell' utente: 0, 1 o 2
	public function SetUserStat($user_id, $stat)
		{
		$st = 0;
		if($this->ConnectDB())
			{
			try
				{
				$stmt = $this->conn->prepare($this->cmd["set_uid_stat"]);
				$stmt->bindParam(":".$this->p_uid, $user_id);
				$stmt->bindParam(":".$this->p_st, $stat);
				$stmt->execute();
				$st = $stmt->fetchColumn();
				//$this->Msg("Stato = ".$st);
				}
			catch(PDOException $e)
				{
				$this->Err("Utente non connesso. Errore: " . $e->getMessage());
				}
			}
		$this->DisconnectDB();
		return $st;
		}

    // Restituisce lo stato di connessione dell' utente: 0, 1 o 2 o disconnette se oltre il timeout.
    // Aggiorna il timestamp sell'utente connesso
    // Deve essere già connesso al database
	public function GetUserStatAndCheck($user_id)
		{
		$st = 0;
		$dl = $this->GetUserDelay($user_id);
		if($dl > $this->max_delay)
			{
			$_SESSION["UID"] = $user_id;
			$st = $this->Logout("Disconnesso per inattività prolungata: ".$dl." sec.",-2);
			}
		else
			{
			$st = $this->sts = $this->GetUserStat($user_id);
			}
		return $st;
		}

    // Restituisce il ritardo massimo (timout) di connessione
    // Valore letto dal file di configurazione
    public function GetDelay()
        {
        $dl = $this->max_delay;
        $this->Msg("Timeout= ".$dl." s.");
        $this->Dat($dl);
        }

	// Connette al DB e controlla che utente e password siano corretti
	// Restituisce -1=errato, 0=disconnesso, 1=lettura, 2=scrittura
	function CheckUser($uname, $upwd)
		{
		$ok = -1;
		if($this->ConnectDB())
			{
			$n = $this->CountUsers($uname,$upwd);
			if($n == 1)
				{
				$uid = $this->GetUserID($uname);
				if($uid > 0)
					{
					$this->uid = $uid;
					//$this->Msg("Utente riconosciuto: ".$uid);
					$ok = $this->GetUserStatAndCheck($uid);
					}
				}
			else
				{
				$this->uid = -1;
				$this->Msg("Utente o password errati");
				}
			}	// Esclusi utenti multipli: primary key
		$this->DisconnectDB();
		return $ok;
		}

	// Abilita la scrittura (stat=2 e password) o la disabilita (stat=1)
	// ma solo se è già connesso
	function SetWrite($upwd, $_stat)
		{
        usleep($this->pwdDelay * 1000);
		if(isset($_SESSION["USER"]) && isset($_SESSION["STS"]) && isset($_SESSION["UID"]))
			{
			//$this->Msg("Utente ".$_SESSION["USER"]." già connesso, permesso= ".$_SESSION["STS"]);
			$this->uid = $_SESSION["UID"];
			$this->sts = $_SESSION["STS"];
			$this->usr = $_SESSION["USER"];
			switch($_stat)
				{
				case 1:
                    if($_SESSION["STS"] == 2)
                        {
                        $this->Msg("Utente <".$_SESSION["USER"]."> disconnesso dalla scrittura");
                        }
					$this->sts = $this->SetUserStat($this->uid, $_stat);
					$_SESSION["STS"] = $this->sts;
					break;
				case 2:
					if($this->ConnectDB())
						{
						$n = $this->CountUsers($_SESSION["USER"], $upwd, 2);
						if($n == 1)
							{
							$this->Msg("Password corretta per scrittura.");
							$this->sts = $this->SetUserStat($this->uid, $_stat);
							if($this->sts != 2)
								$this->Msg("Utente non abilitato");
                            else
                                $this->Msg("Utente <" . $_SESSION["USER"] . "> connesso in scrittura");
							$_SESSION["STS"] = $this->sts;
							}
						else
							{
							$this->Msg("Password errata."); // ".$n." utenti trovati.");
							}
						}
					$this->DisconnectDB();
					break;
				}
			}
		else
			{
			$this->Msg("Utente non connesso");
			$this->sts = 0;
			$this->usr = "";
			}
		}

	// Esegue Login, o resta disconnesso se dati errati
	// Chiama CheckUser()
	function Login($uname, $upwd)
		{
        usleep($this->pwdDelay * 1000);
		if(isset($_SESSION["USER"]))
			{
			if($_SESSION["USER"] == $uname)
				{
				$this->Msg("Utente <".$_SESSION["USER"]."> già connesso");
				$this->sts = $_SESSION["STS"];
				$this->usr = $_SESSION["USER"];
				}
			else
				{
				$this->Msg("Connesso con un altro utente: ".$_SESSION["USER"]);
				$this->Logout("Disconnesso. Riconnettersi con il proprio utente.");
				}
			}
		else
			{
			$usname = "";
			if(isset($_SESSION["USER"]))
				$usname = $_SESSION["USER"];
			$this->perm = 1;
			$this->sts = $this->CheckUser($uname, $upwd);	// Qui si puo' disconnettere per inattività prolungata !!!
			switch($this->sts)
				{
				case -1:
					$this->Msg("Utente inesistente o password errata");
					break;
				case -2:
					$this->Msg("Utente disconnesso per inattività prolungata");
					break;
				case 0:
					$this->sts = $this->SetUserStat($this->uid, $this->perm);
					$_SESSION["USER"] = $this->usr = $uname;
					$_SESSION["UID"] = $this->uid;
					$_SESSION["STS"] = $this->sts;
					$this->Msg("Utente <".$_SESSION["USER"]."> connesso con successo");
                    // PROVA !!!
                    $this->GetDelay();
					break;
				case 1:
					$this->Msg("Utente ".$usname." già connesso in lettura");
					$this->Logout("Connessione annullata");
					break;
				case 2:
					$this->Msg("Utente ".$usname." già connesso in scrittura");
					$this->Logout("Connessione annullata");
					break;
				}
			}
		}

	// Cambia la password di lettura o di scrittura, poi disconnette comunque
    // $np=1 passwd di lettura, $np=2 passwd di scrittura
	function ChangePwd($uname, $upwd, $newpwd, $np)
		{
		$ok = false;
		if($this->ConnectDB())
			{
			$n = $this->CountUsers($uname,$upwd);
			if($n == 1)
				{
				try
					{
					$stmt = $this->conn->prepare($this->cmd["change_pwd".$np]);
					$stmt->bindParam(":".$this->utente,$uname);
					$stmt->bindParam(":".$this->passwd,$upwd);
					$stmt->bindParam(":".$this->newpasswd,$newpwd);
					$status = $stmt->execute();
					if($status)
						{
						$this->Msg("Password cambiata");
						$ok = true;
						}
					else
						{
						$this->Msg("La password NON è stata cambiata");
						}
					}
				catch(PDOException $e)
					{
					$this->Err("La password NON è stata cambiata. Errore:" . $e->getMessage());
					}
				}
			else
				{
				$this->Msg("Utente o password errati");
				}
			}
		$this->DisconnectDB();
		return $ok;
		}

    // Cambia la password di lettura o di scrittura,se connesso.
    //
	function ChangePwdIfLogged($uname, $upwd, $newpwd, $n)
		{
        usleep($this->pwdDelay * 1000);
		if(isset($_SESSION["USER"]))
			{
			if($_SESSION["USER"] == $uname)
				{
				$this->ChangePwd($uname, $upwd, $newpwd, $n);
				}
			else
				{
				$this->Msg("Connesso con un altro utente: ".$_SESSION["USER"]);
				$this->Logout("Disconnesso. Riconnettersi con il proprio utente.");
				}
			}
		}

	// Aggiunge messaggio con lo stato della connessione: 0: non connesso, 1: in lettura, 2: in scrittura.
	// Non interroga il database, usa le variabili di sessione.
	function Stat()
		{
		if(isset($_SESSION["USER"])&&isset($_SESSION["STS"])&&isset($_SESSION["UID"]))
			{
			$this->sts = $_SESSION["STS"];
			$this->usr = $_SESSION["USER"];
			if($this->sts > 0)
				{
				if($this->ConnectDB())
					{
					$this->GetUserStatAndCheck($_SESSION["UID"]);
					}
				$this->DisconnectDB();
				}

			if($this->sts == 1)
				$this->Msg("Utente <".$_SESSION["USER"]."> connesso in lettura.");
			else if($this->sts == 2)
				$this->Msg("Utente <".$_SESSION["USER"]."> connesso in scrittura.");
			else if(isset($_SESSION["USER"]))
				{
				$this->Msg("Utente <".$_SESSION["USER"]."> connesso con ERRORE.");
				$this->usr = "";
				}
			}
		else
			{
			$this->Msg("Nessun utente connesso");
			$this->sts = 0;
			}
		}

	// Disconnette l'utente in $_SESSION["UID"]
	// Messaggio e stato finale sono parametroi opzionali
	function Logout($_logoutMsg = "Disconnesso !", $_finalStat = 0)
		{
		if(isset($_SESSION["UID"]))
			{
			$this->sts = $this->SetUserStat($_SESSION["UID"], 0);
			}
		session_unset();	//unset($_SESSION["newsession"]);
		session_destroy();
		$this->Msg($_logoutMsg);
		$this->sts = $_finalStat;
		$this->usr = "";
		return $this->sts;
		}

	/**************************************/
	//		COMANDI MySql
    /**************************************/

    // Costruisce il dizionario dei comandi SQL, con relativi parametri.
	protected function ConstructName($dbname)
		{
		// Connessione
		$this->cmd["count_users"]  = "SELECT COUNT(*) FROM ".$this->utenti." WHERE ".$this->utente."= :".$this->utente." AND ".$this->passwd."= :" .$this->passwd.";";
		$this->cmd["count_wusers"] = "SELECT COUNT(*) FROM ".$this->utenti." WHERE ".$this->utente."= :".$this->utente." AND ".$this->passwd2."= :".$this->passwd2.";";
		$this->cmd["change_pwd1"] = "UPDATE ".$this->utenti." SET ".$this->passwd."= :".$this->newpasswd." WHERE ".$this->utente."= :".$this->utente." AND ".$this->passwd."= :".$this->passwd.";";
		$this->cmd["change_pwd2"] = "UPDATE ".$this->utenti." SET ".$this->passwd2."= :".$this->newpasswd." WHERE ".$this->utente."= :".$this->utente." AND ".$this->passwd."= :".$this->passwd.";";
		$this->cmd["get_uid"] = "SELECT ".$this->id." FROM ".$this->utenti." WHERE ".$this->utente."= :".$this->utente.";";
		$this->cmd["set_uid_stat"] = "SELECT Set_uid_stat( :".$this->p_uid.", "." :".$this->p_st.");";
		$this->cmd["get_uid_stat"] = "SELECT Get_uid_stat( :".$this->p_uid.");";
		$this->cmd["get_uid_delay"] = "SELECT Get_uid_delay( :".$this->p_uid.");";
        // Lettura
        $this->cmd["countCode"] = "CALL ContaCodici( :" . $this->p_cod . ", " . " :" . $this->p_mod . ");";
		$this->cmd["materiali"] = "CALL ListaMateriali( :".$this->p_uid.");";
		$this->cmd["prodotti"] = "CALL ListaProdotti( :".$this->p_uid.");";
		$this->cmd["costruttori"] = "CALL ListaCostruttori( :".$this->p_uid.");";
		$this->cmd["dirty"] = "CALL Dirty( :".$this->p_uid.", "." :".$this->p_dirty.");";
        $this->cmd["vedi"] = "CALL VediDescrizioni( :" . $this->p_limite . ", :" . $this->p_cod . ", :" . $this->p_mod . ");";
		$this->cmd["getCode"] = "CALL GetCode( :".$this->p_cod.", "." :".$this->p_mod.");";
		// Inserimento
		$this->cmd["insAsm"] = "CALL InsAssieme( :".$this->p_cod.", :".$this->p_mod.", :".$this->p_desc.", :".$this->p_uid.");";
		$this->cmd["insSch"] = "CALL InsSchema( :".$this->p_cod.", :".$this->p_mod.", :".$this->p_desc.", :".$this->p_uid.");";
		$this->cmd["insPar"] = "CALL InsParticolare( :".$this->p_cod.", :".$this->p_mod.", :".$this->p_desc.", :".$this->p_uid.", :".$this->p_mat.");";
		$this->cmd["insCom"] =  "CALL InsCommerciale( :".$this->p_cod.", :".$this->p_mod.", :".$this->p_uid.", :".$this->p_cos.", :".$this->p_pro.", :".$this->p_model.", :".$this->p_dett.");";
        // Eliminazione
        $this->cmd["delete"] = "CALL DelCodice( :" . $this->p_cod . ", " . " :" . $this->p_mod . ");";


        // SUPERFLUA $this->cmd["clear_conn"] = "CALL ClearConnUser( :".$this->p_uid.", "." :".$this->max_delay.");";
        // Liste (in futuro: bufferizzare, usando variabili di sessione; vedere se usare query multiple o query singola e buffer in PHP.
        //$this->cmd["codice"] = "";	// Numero e dati completi del codice
        }

    /**************************************/
	//		MESSAGGI di risposta
    /**************************************/

    // Cancella i messaggi di risposta
	function ClearMsg()
		{
		$this->err = "";
		$this->msg = "";
		$this->dat = "";
		}

    // Restituisce l'errore
    //
	public function GetError()
		{
		return $this->err;
		}

    // Aggiunge una linea con il messaggio di errore
    //
	public function Err($m)
		{
		$this->err .= $m."\n";
		}

    // Aggiunge una linea con il messaggio informativo
    //
	public function Msg($m)
		{
		$this->msg .= $m."\n";
		}

    // Aggiunge una linea con i dati
    //
	public function Dat($m)
		{
		$this->dat .= $m."\n";
		}

    /**************************************/
	//		ACCESSO e MODIFICA dei dati
    /**************************************/

	function Vedi($limit,$cod,$mod)
		{
		$txt = $txtm = "";

		if(isset($_SESSION["USER"])&&isset($_SESSION["STS"])&&isset($_SESSION["UID"]))
			{
            $this->usr = $_SESSION["USER"];
			if($this->ConnectDB())
				{
				try
					{
					$stmt = $this->conn->prepare($this->cmd["vedi"]);
					$stmt->bindParam(":".$this->p_limite,$limit);
					$stmt->bindParam(":".$this->p_cod,$cod);
					$stmt->bindParam(":".$this->p_mod,$mod);
					$stmt->execute();
					$result = $stmt->fetchAll(PDO::FETCH_ASSOC);
					$r = count($result);
					$txt .= "Righe=".$r."\n";		// Numero di elementi trovati
					$txtm .= "Trovati ".$r." elementi";
					if($r > 0)						// Titoli dei campi
						{
						$titoli = array_keys($result[0]);			// Titoli delle colonne
						$txt .= "Colonne=".count($titoli)."\n";		// Numero di colonne
						for($i=0, $nofirst=false; $i<count($titoli); $i++, $nofirst=true)
							{
							if($nofirst)	$txt .= ";";
							$txt .= $titoli[$i];
							}
						$txt .= "\n";
						//$txt .= json_encode(array_keys($result[0])); ...= json_encode(array('Elementi' => $r)); ...= json_encode($result);
						for($j=0; $j<$r; $j++)
							{
							for($i=0, $nofirst=false; $i<count($titoli); $i++, $nofirst=true)
								{
								if($nofirst)	$txt .= ";";
								$txt .= $result[$j][$titoli[$i]];
								}
							$txt .= "\n";
							}
						}
					$this->sts = $_SESSION["STS"];
					}
				catch(PDOException $e)
					{
					$this->Err("Errore: " . $e->getMessage());
					}
				}
			else
				{
				$this->sts = 0;
				}
			}
		else
			{
			$this->Msg("Nessun utente connesso");
			$this->sts = 0;
			}

		$this->Dat($txt);
		$this->Msg($txtm);
		$this->DisconnectDB();
		return $txt;
		}

	//-------------------------------------
	public function ContaCodici($_cod, $_mod)
		{
		$r = 0;
		if(isset($_SESSION["USER"])&&isset($_SESSION["STS"])&&isset($_SESSION["UID"]))
			{
			if($this->ConnectDB())
				{
				try
					{
					$stmt = $this->conn->prepare($this->cmd["countCode"]);
					$stmt->bindParam(":".$this->p_cod,$_cod);
					$stmt->bindParam(":".$this->p_mod,$_mod);
					$stmt->execute();
					$r = $stmt->fetchColumn();
					}
				catch(PDOException $e)
					{
					$this->Err("Fallito conteggio. Errore: " . $e->getMessage());
					}
				$this->Dat($r);
				$this->Msg("Trovati ".$r." articoli con codice ".$_cod.$_mod);
				$this->sts = $_SESSION["STS"];
				}
			else
				{
				$this->sts = 0;
				$this->Msg("Utente non connesso");
				}
			}
		else
			{
			$this->sts = 0;
			$this->Msg("Utente non connesso");
			}
		$this->DisconnectDB();
		return $r;
		}

	public function GetCode($_cod, $_mod)
		{
		$txt = $txtm = "";
		if(isset($_SESSION["USER"])&&isset($_SESSION["STS"])&&isset($_SESSION["UID"]))
			{
            $this->usr = $_SESSION["USER"];
			if($this->ConnectDB())
				{
				try
					{
					$stmt = $this->conn->prepare($this->cmd["getCode"]);
					$stmt->bindParam(":".$this->p_cod,$_cod);
					$stmt->bindParam(":".$this->p_mod,$_mod);
					$stmt->execute();
					$result = $stmt->fetchAll(PDO::FETCH_ASSOC);
					$r = count($result);
					$txt .= "Righe=".$r."\n";		// Numero di elementi trovati
					$txtm .= "Trovati ".$r." elementi";
					if($r > 0)						// Titoli dei campi
						{
						$titoli = array_keys($result[0]);			// Titoli delle colonne
						$txt .= "Colonne=".count($titoli)."\n";		// Numero di colonne
						for($i=0, $nofirst=false; $i<count($titoli); $i++, $nofirst=true)
							{
							if($nofirst)	$txt .= ";";
							$txt .= $titoli[$i];
							}
						$txt .= "\n";
						//$txt .= json_encode(array_keys($result[0])); ...= json_encode(array('Elementi' => $r)); ...= json_encode($result);
						for($j=0; $j<$r; $j++)
							{
							for($i=0, $nofirst=false; $i<count($titoli); $i++, $nofirst=true)
								{
								if($nofirst)	$txt .= ";";
								$txt .= $result[$j][$titoli[$i]];
								}
							$txt .= "\n";
							}
						}
					$this->sts = $_SESSION["STS"];
					}
				catch(PDOException $e)
					{
					$this->Err("Fallita ricerca. Errore: " . $e->getMessage());
					}
				//$this->Msg("Trovati ".$r." articoli con codice ".$_cod.$_mod);
				}
			else
				{
				$this->sts = 0;
				$this->Msg("Utente non connesso");
				}
			}
		else
			{
			$this->sts = 0;
			$this->Msg("Utente non connesso");
			}
		$this->Dat($txt);
		$this->Msg($txtm);
		$this->DisconnectDB();
		return $txt;
		}

	// Inserisce nuovo codice
    // Parametro $_uid non usato
    //
	function Inserisci($_cod, $_mod, $_desc, $_uid, $_mat, $_cos, $_pro, $_model, $_dett, $_typ)
		{
		$txt = "";
		if(isset($_SESSION["USER"])&&isset($_SESSION["STS"])&&isset($_SESSION["UID"]))
			{
			if($this->ConnectDB())
				{
				$this->sts = $this->GetUserStat($_SESSION["UID"]);
				if($this->sts == 2)
					{
					$txt = "";
					$_uid = $_SESSION["UID"];
					try
						{
						// MySql procedures:
						// InsAssieme(_cod, _mod, _desc, _uid)
						// InsSchema(_cod, _mod, _desc, _uid)
						// InsParticolare(_cod, _mod, _desc, _uid, _mat)
						// InsCommerciale(_cod, _mod, _uid. _cos. _pro, _model, _dett)
						switch($_typ)
							{
							case 'assieme':
								{
								$txt = "\nTIPO: ".$_typ."\n";
								$stmt = $this->conn->prepare($this->cmd["insAsm"]);
								$stmt->bindParam(":".$this->p_cod,$_cod);
								$stmt->bindParam(":".$this->p_mod,$_mod);
								$stmt->bindParam(":".$this->p_desc,$_desc);
								$stmt->bindParam(":".$this->p_uid,$_uid);
								$stmt->execute();
								}
								break;
							case 'schema':
								{
								$txt = "\nTIPO: ".$_typ."\n";
								$stmt = $this->conn->prepare($this->cmd["insSch"]);
								$stmt->bindParam(":".$this->p_cod,$_cod);
								$stmt->bindParam(":".$this->p_mod,$_mod);
								$stmt->bindParam(":".$this->p_desc,$_desc);
								$stmt->bindParam(":".$this->p_uid,$_uid);
								$stmt->execute();
								}
								break;
							case 'particolare':
								{
								$txt = "\nTIPO: ".$_typ."\n";
								$stmt = $this->conn->prepare($this->cmd["insPar"]);
								$stmt->bindParam(":".$this->p_cod,$_cod);
								$stmt->bindParam(":".$this->p_mod,$_mod);
								$stmt->bindParam(":".$this->p_desc,$_desc);
								$stmt->bindParam(":".$this->p_uid,$_uid);
								$stmt->bindParam(":".$this->p_mat,$_mat);
								$stmt->execute();
								}
								break;
							case 'commerciale':
								{
								$txt = "\nTIPO: ".$_typ."\n";
								$stmt = $this->conn->prepare($this->cmd["insCom"]);
								$stmt->bindParam(":".$this->p_cod,$_cod);
								$stmt->bindParam(":".$this->p_mod,$_mod);
								$stmt->bindParam(":".$this->p_uid,$_uid);
								$stmt->bindParam(":".$this->p_cos,$_cos);
								$stmt->bindParam(":".$this->p_pro,$_pro);
								$stmt->bindParam(":".$this->p_model,$_model);
								$stmt->bindParam(":".$this->p_dett,$_dett);
								$stmt->execute();
								}
								break;

							default:
								{
								$txt = "\nTIPO: "."non gestito"."\n";
								}
								break;
							}
						}
					catch(PDOException $e)
						{
						$this->Err("Errore inserimento " . $_typ . $e->getMessage());
						}

					$txt .= "codice: ".$_cod."\n";
					$txt .= "modifica: ".$_mod."\n";
					$txt .= "descrizione: ".$_desc."\n";
					$txt .= "User ID: ".$_uid."\n";
					$txt .= "materiale: ".$_mat."\n";
					$txt .= "costruttore: ".$_cos."\n";
					$txt .= "prodotto: ".$_pro."\n";
					$txt .= "modello: ".$_model."\n";
					$txt .= "dettagli: ".$_dett."\n";

					// --------------------------------------------------
					// Richiamare la lettura del codice appena inserito
					// --------------------------------------------------

					$this->Dat($txt);
					$this->Msg($txt);

					}
				else
					{
					$this->Msg("Utente non abilitato alla scrittura");
					}
				}
			else
				{
				$this->sts = 0;
				$this->Msg("Utente non connesso");
				}
			}
		else
			{
			$this->sts = 0;
			$this->Msg("Utente non connesso");
			}

		$this->DisconnectDB();
		return $txt;
		}

    // Elimina codice
    //
    function Elimina($_cod, $_mod)
        {
        $txt = "";
        if (isset($_SESSION["USER"]) && isset($_SESSION["STS"]) && isset($_SESSION["UID"]))
            {
            $this->usr = $_SESSION["USER"];
            if ($this->ConnectDB())
                {
                $this->sts = $this->GetUserStat($_SESSION["UID"]);
                if ($this->sts == 2)
                    {
                    $txt = "";
                    try
                        {
                        $stmt = $this->conn->prepare($this->cmd["delete"]);
                        $stmt->bindParam(":" . $this->p_cod, $_cod);
                        $stmt->bindParam(":" . $this->p_mod, $_mod);
                        $stmt->execute();
                        $result = $stmt->fetchColumn();
                        }
                    catch (PDOException $e)
                        {
                        $this->Err("Errore eliminazione " . $_cod . $_mod. ": " . $e->getMessage());
                        }
                    $this->Dat($result);
                    $this->Msg($result);
                    $this->sts = $_SESSION["STS"];
                    }
                else
                    {
                    $this->Msg("Utente non abilitato alla scrittura");
                    }
                }
            else
                {
                $this->sts = 0;
                $this->Msg("Utente non connesso");
                }
            }
        else
            {
            $this->sts = 0;
            $this->Msg("Utente non connesso");
            }
        $this->DisconnectDB();
        return $result;
        }

    function Liste($_tabella)
		{
		$txt = $txtm = "";
		if(isset($_SESSION["USER"])&&isset($_SESSION["STS"])&&isset($_SESSION["UID"]))
			{
            $this->usr = $_SESSION["USER"];
			if($this->ConnectDB())
				{
				try
					{
					$stmt = $this->conn->prepare($this->cmd[$_tabella]);
					$stmt->bindParam(":".$this->p_uid, $_SESSION["UID"]);
					$stmt->execute();
					$result = $stmt->fetchAll(PDO::FETCH_ASSOC);

					$r = count($result);
					$txt .= "Righe=".$r."\n";		// Numero di elementi trovati
					$txtm .= "Trovati ".$r." elementi";
					if($r > 0)						// Titoli dei campi
						{
						$titoli = array_keys($result[0]);			// Titoli delle colonne
						$txt .= "Colonne=".count($titoli)."\n";		// Numero di colonne
						for($i=0, $nofirst=false; $i<count($titoli); $i++, $nofirst=true)
							{
							if($nofirst)	$txt .= ";";
							$txt .= $titoli[$i];
							}
						$txt .= "\n";
						for($j=0; $j<$r; $j++)
							{
							for($i=0, $nofirst=false; $i<count($titoli); $i++, $nofirst=true)
								{
								if($nofirst)	$txt .= ";";
								$txt .= $result[$j][$titoli[$i]];
								}
							$txt .= "\n";
							}
						}
					$this->sts = $_SESSION["STS"];
					}
				catch(PDOException $e)
					{
					$this->Err("Errore: " . $e->getMessage());
					}
				}
			else
				{
				$this->sts = 0;
				}
			}
		else
			{
			$this->sts = 0;
			$this->Msg("Utente non connesso");
			}
		$this->Dat($txt);
		$this->Msg($txtm);
		$this->DisconnectDB();

		return $txt;
		}

	function Dirty($_dirtytab)
		{
		if(isset($_SESSION["USER"])&&isset($_SESSION["STS"])&&isset($_SESSION["UID"]))
			{
			$this->sts = $_SESSION["STS"];
			$this->usr = $_SESSION["USER"];
			if($this->sts > 0)
				{
				if($this->ConnectDB())
					{
					try
						{
						$stmt = $this->conn->prepare($this->cmd["dirty"]);
						$stmt->bindParam(":".$this->p_uid, $_SESSION["UID"]);
						$stmt->bindParam(":".$this->p_dirty, $_dirtytab);
						$stmt->execute();
						$txt = $stmt->fetchColumn();
						}
					catch(PDOException $e)
						{
						$this->Err("Fallito dirty. Errore: " . $e->getMessage());
						}
					}
				$this->DisconnectDB();
				}

			$this->Msg($txt);
			}
		else
			{
			$this->Msg("Nessun utente connesso");
			$this->sts = 0;
			}
		}

    }
?>