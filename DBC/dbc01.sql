-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versione server:              10.4.27-MariaDB - mariadb.org binary distribution
-- S.O. server:                  Win64
-- HeidiSQL Versione:            12.4.0.6659
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dump della struttura del database dbc01
CREATE DATABASE IF NOT EXISTS `dbc01` /*!40100 DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci */;
USE `dbc01`;

-- Dump della struttura di tabella dbc01.assiemi
CREATE TABLE IF NOT EXISTS `assiemi` (
  `cod` char(20) NOT NULL,
  `mod` char(1) NOT NULL DEFAULT '',
  PRIMARY KEY (`cod`,`mod`) USING BTREE,
  CONSTRAINT `FK_assiemi_codici` FOREIGN KEY (`cod`, `mod`) REFERENCES `codici` (`cod`, `mod`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='ASM';

-- Dump dei dati della tabella dbc01.assiemi: ~14 rows (circa)
REPLACE INTO `assiemi` (`cod`, `mod`) VALUES
	('101.10.100', ''),
	('201.10.001', ''),
	('201.11.001', ''),
	('301.10.001', ''),
	('555.55.555', 'a'),
	('789.10.001', ''),
	('789.10.001', 'a'),
	('789.10.001', 'b'),
	('987.22.123', ''),
	('987.65.432', ''),
	('998.01.001', ''),
	('998.01.001', 'a'),
	('999.80.678', 'a'),
	('999.88.776', 'a'),
	('999.99.002', '');

-- Dump della struttura di procedura dbc01.ClearConnUser
DELIMITER //
CREATE PROCEDURE `ClearConnUser`(
	IN `_uid` INT,
	IN `secTimeout` INT
)
    COMMENT 'Rimuove gli utenti connessi ma non aggiornati oltre il timeout [NON USATA]'
BEGIN
DELETE FROM u_connessi WHERE u_connessi.idc = _uid AND (SECOND, u_connessi.lasttime, current_timestamp()) > secTimeout;
END//
DELIMITER ;

-- Dump della struttura di tabella dbc01.codici
CREATE TABLE IF NOT EXISTS `codici` (
  `cod` char(20) NOT NULL,
  `mod` char(1) NOT NULL DEFAULT '',
  `descrizione` varchar(255) NOT NULL DEFAULT '-',
  `id_utente` int(10) unsigned NOT NULL,
  `creazione` datetime NOT NULL DEFAULT current_timestamp(),
  `aggiornamento` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `id_ultimo` int(10) unsigned NOT NULL,
  PRIMARY KEY (`cod`,`mod`) USING BTREE,
  KEY `FK_utente` (`id_utente`),
  KEY `FK_ultimo` (`id_ultimo`),
  CONSTRAINT `FK_ultimo` FOREIGN KEY (`id_ultimo`) REFERENCES `utenti` (`id`),
  CONSTRAINT `FK_utente` FOREIGN KEY (`id_utente`) REFERENCES `utenti` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista di tutti i codici.\r\nCon informazioni generiche.';

-- Dump dei dati della tabella dbc01.codici: ~49 rows (circa)
REPLACE INTO `codici` (`cod`, `mod`, `descrizione`, `id_utente`, `creazione`, `aggiornamento`, `id_ultimo`) VALUES
	('100.11.123', '', 'Staffa', 5, '2019-06-22 08:33:51', '2019-06-22 08:33:51', 5),
	('100.11.123', 'a', 'STAFFA ALLUNGATA', 1, '2019-06-22 08:18:19', '2019-09-14 14:22:29', 2),
	('100.11.123', 'b', 'Staffa', 1, '2019-06-24 08:20:25', '2019-06-24 08:20:25', 1),
	('101.10.100', '', 'AAA', 2, '2019-11-01 10:34:21', '2019-11-01 10:34:21', 2),
	('102.11.100', '', 'STAFFA', 2, '2023-04-23 17:43:31', '2023-04-23 17:43:31', 2),
	('102.11.100', 'a', 'STAFFA', 2, '2023-04-23 17:44:58', '2023-04-23 17:45:27', 2),
	('1055.45567.122', '', 'Vite', 2, '2019-06-24 08:31:22', '2019-08-17 17:03:04', 2),
	('1055.54789.123', '', 'Guida lineare Bosch 1605.302.10.20', 2, '2019-11-02 14:09:12', '2019-11-02 14:09:12', 2),
	('1058.32765.001', '', 'Guida lineare Bosch 1605.20.345', 2, '2019-11-02 14:27:02', '2019-11-02 14:27:02', 2),
	('1058.54567.123', '', 'Guida lineare Bosch 1605.302.20.20', 2, '2023-04-23 17:47:00', '2023-04-23 17:47:00', 2),
	('1059.11111.234', '', 'Valvola Camozzi EV', 2, '2023-04-23 23:58:46', '2023-04-23 23:58:46', 2),
	('113.10.100', '', 'Supporto', 1, '2019-06-22 11:40:51', '2019-06-22 11:40:51', 1),
	('201.10.001', '', 'ASSIEME', 2, '2019-11-02 11:45:33', '2019-11-02 11:45:33', 2),
	('201.10.100', '', 'PARTICOLARE', 2, '2019-11-02 12:32:53', '2019-11-02 12:32:53', 2),
	('201.10.900', '', 'SCHEMA', 2, '2019-11-02 12:22:14', '2019-11-02 12:22:14', 2),
	('201.11.001', '', 'ASSIEME STAFFA', 2, '2019-11-09 18:16:35', '2019-11-09 18:16:35', 2),
	('201.11.100', '', 'STAFFA', 2, '2019-11-09 18:17:22', '2019-11-09 18:17:22', 2),
	('201.11.100', 'a', 'STAFFA', 2, '2023-04-26 19:15:21', '2023-04-26 19:15:21', 2),
	('20111.98765.001', '', 'Vite Umbrako M12x30', 2, '2019-11-09 18:18:07', '2019-11-09 18:18:07', 2),
	('300.33.330', 'a', 'STAFFA', 2, '2019-11-25 00:51:07', '2019-11-25 00:52:35', 2),
	('301.10.001', '', 'ASSIEME', 2, '2019-11-02 14:25:32', '2019-11-02 14:25:32', 2),
	('301.10.100', '', 'STAFFA', 2, '2019-11-02 14:26:18', '2019-11-02 14:26:18', 2),
	('301.10.900', '', 'SCHEMA', 2, '2019-11-02 14:25:57', '2019-11-02 14:25:57', 2),
	('333.44.555', '', 'Valvola Festo ASD', 2, '2019-11-25 00:55:48', '2019-11-25 00:55:48', 2),
	('554.55.554', '', 'AAA', 2, '2023-04-24 07:13:26', '2023-04-24 07:13:26', 2),
	('555.55.555', 'a', 'ASSIEME PROVA', 2, '2019-09-11 21:23:16', '2019-09-11 21:23:16', 2),
	('567.89.111', '', 'LAST', 2, '2023-04-24 08:10:05', '2023-04-24 08:10:05', 2),
	('789.10.001', '', 'Assieme testa', 1, '2019-05-21 21:08:18', '2019-06-07 14:53:10', 5),
	('789.10.001', 'a', 'Assieme testa', 1, '2019-05-21 21:08:18', '2019-06-07 14:53:12', 5),
	('789.10.001', 'b', 'Assieme testa', 2, '2019-05-21 21:08:48', '2019-06-07 14:53:12', 5),
	('789.10.100', '', 'Corpo', 1, '2019-05-21 21:09:33', '2019-06-07 12:15:58', 1),
	('789.10.100', 'a', 'Corpo nuovo', 1, '2019-05-21 21:28:05', '2019-06-07 14:53:15', 5),
	('789.10.101', '', 'Supporto', 2, '2019-05-21 21:10:01', '2019-06-07 12:16:08', 1),
	('789.10.101', 'a', 'Supporto', 1, '2019-05-21 21:10:28', '2019-06-07 12:16:09', 1),
	('789.10.101', 'b', 'Supporto rettificato', 1, '2019-05-21 21:10:57', '2019-06-07 12:16:11', 1),
	('789.10.200', '', 'FUSIONE TESTA', 2, '2019-11-10 23:08:01', '2019-11-10 23:08:01', 2),
	('820.10.900', '', 'Schema lavorazione', 1, '2019-06-03 00:01:36', '2019-06-07 14:53:17', 5),
	('820.10.900', 'a', 'Schema lavorazione', 1, '2019-06-03 00:01:22', '2019-06-07 14:53:16', 5),
	('820.10.900', 'b', 'Schema lavorazione', 1, '2019-06-02 23:58:51', '2019-06-07 14:53:16', 5),
	('987.22.123', '', 'Studio montaggio', 5, '2019-06-24 07:35:59', '2019-06-24 07:35:59', 5),
	('987.43.444', 'a', 'SCHEMA AGGIORNATO', 2, '2019-11-10 19:24:27', '2019-11-10 19:24:27', 2),
	('987.65.432', '', 'ASSIEME DISCENDENTE', 5, '2023-04-25 09:42:13', '2023-04-25 09:42:13', 5),
	('998.01.001', '', 'new', 1, '2019-06-19 22:18:43', '2019-06-19 22:18:43', 1),
	('998.01.001', 'a', 'new modificati', 5, '2019-06-19 22:19:29', '2019-06-19 22:19:29', 5),
	('999.80.678', 'a', 'TEST', 2, '2019-09-11 20:55:22', '2019-09-11 20:55:22', 2),
	('999.88.776', '', 'SCHEMA', 2, '2019-11-26 21:24:38', '2019-11-26 21:24:38', 2),
	('999.88.776', 'a', 'ANTANI', 2, '2023-04-23 15:58:16', '2023-04-23 15:58:16', 2),
	('999.88.777', '', ' Festo ', 2, '2019-11-26 21:19:55', '2019-11-26 21:19:55', 2),
	('999.99.002', '', 'AAA2', 1, '2019-06-19 22:11:20', '2019-06-19 22:11:20', 1);

-- Dump della struttura di tabella dbc01.commerciali
CREATE TABLE IF NOT EXISTS `commerciali` (
  `cod` char(20) NOT NULL,
  `mod` char(1) NOT NULL,
  `modello` varchar(255) NOT NULL DEFAULT '',
  `dettagli` varchar(255) NOT NULL DEFAULT '',
  `costruttore` int(10) unsigned NOT NULL,
  `prodotto` int(10) unsigned NOT NULL,
  PRIMARY KEY (`cod`,`mod`),
  KEY `FK_costruttori` (`costruttore`),
  KEY `FK_commerciali_prodotti` (`prodotto`),
  CONSTRAINT `FK_commerciali_codici` FOREIGN KEY (`cod`, `mod`) REFERENCES `codici` (`cod`, `mod`),
  CONSTRAINT `FK_commerciali_prodotti` FOREIGN KEY (`prodotto`) REFERENCES `prodotti` (`id`),
  CONSTRAINT `FK_costruttori` FOREIGN KEY (`costruttore`) REFERENCES `costruttori` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista dei codici commerciali.\r\nCon dati specifici.\r\n';

-- Dump dei dati della tabella dbc01.commerciali: ~7 rows (circa)
REPLACE INTO `commerciali` (`cod`, `mod`, `modello`, `dettagli`, `costruttore`, `prodotto`) VALUES
	('1055.45567.122', '', 'M12x50', 'interamente filettata', 1, 1),
	('1055.54789.123', '', '1605.302.10.20', 'TAGLI 20, CLASSE P', 2, 4),
	('1058.32765.001', '', '1605.20.345', 'taglia 20, classe P', 2, 4),
	('1058.54567.123', '', '1605.302.20.20', 'Classe SP', 2, 4),
	('1059.11111.234', '', 'EV', '53 c.a.', 4, 2),
	('20111.98765.001', '', 'M12x30', '12.9', 1, 1),
	('333.44.555', '', 'ASD', 'ASDFRE', 3, 2),
	('999.88.777', '', '', '', 3, 11);

-- Dump della struttura di procedura dbc01.ContaCodici
DELIMITER //
CREATE PROCEDURE `ContaCodici`(
	IN `_cod` VARCHAR(255),
	IN `_mod` CHAR(1)


)
BEGIN
SELECT COUNT(*) FROM codici WHERE (codici.cod LIKE _cod) AND (codici.mod LIKE _mod);
END//
DELIMITER ;

-- Dump della struttura di tabella dbc01.costruttori
CREATE TABLE IF NOT EXISTS `costruttori` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `costruttore` varchar(255) NOT NULL DEFAULT '-',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Nome_costruttore` (`costruttore`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista dei nomi di costruttori o fornitori.\r\nID_dirty = 3.';

-- Dump dei dati della tabella dbc01.costruttori: ~6 rows (circa)
REPLACE INTO `costruttori` (`id`, `costruttore`) VALUES
	(2, 'Bosch'),
	(4, 'Camozzi'),
	(3, 'Festo'),
	(7, 'Fluido sistem'),
	(5, 'Siemens'),
	(1, 'Umbrako');

-- Dump della struttura di procedura dbc01.DelCodice
DELIMITER //
CREATE PROCEDURE `DelCodice`(
	IN `_cod` CHAR(20),
	IN `_mod` CHAR(1)
)
    COMMENT 'Cancella un codice'
BEGIN
DECLARE n INT DEFAULT 0;		# numero codici trovati: deve essere 1, solo codice univoco
DECLARE np, ns, na, nc INT DEFAULT 0;
DECLARE ok BOOL DEFAULT FALSE;
DECLARE done BOOL DEFAULT FALSE;
SELECT COUNT(*) FROM codici c WHERE c.cod = _cod AND c.mod = _mod INTO n;
IF n = 1 THEN
	SELECT COUNT(*) FROM particolari c WHERE c.cod = _cod AND c.mod = _mod INTO np;
	IF np = 0 THEN
		SELECT COUNT(*) FROM schemi c WHERE c.cod = _cod AND c.mod = _mod INTO ns;
	END IF;
	IF ns = 0 THEN
		SELECT COUNT(*) FROM assiemi c WHERE c.cod = _cod AND c.mod = _mod INTO na;
	END IF;
	IF na = 0 THEN
		SELECT COUNT(*) FROM commerciali c WHERE c.cod = _cod AND c.mod = _mod INTO nc;
	END IF;
	IF np + ns + na + nc = 1 then
		SET ok = TRUE;
	END IF;
END IF;
IF ok THEN
	START TRANSACTION;
	IF np = 1 THEN
		DELETE FROM particolari WHERE particolari.cod = _cod AND particolari.mod = _mod;
	ELSEIF ns = 1 THEN
		DELETE FROM schemi WHERE schemi.cod = _cod AND schemi.mod = _mod;
	ELSEIF na = 1 THEN
		DELETE FROM assiemi WHERE assiemi.cod = _cod AND assiemi.mod = _mod;
	ELSEIF nc = 1 THEN
		DELETE FROM commerciali WHERE commerciali.cod = _cod AND commerciali.mod = _mod;
	END IF;
	DELETE FROM codici WHERE codici.cod = _cod AND codici.mod = _mod;
	SET done = TRUE;
	COMMIT;
END IF;
IF done THEN
	SELECT CONCAT(_cod,_mod," cancellato") AS RISPOSTA;
ELSE
	SELECT CONCAT(_cod,_mod," non cancellato") AS RISPOSTA;
END IF;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.Dirty
DELIMITER //
CREATE PROCEDURE `Dirty`(
	IN `_uid` INT,
	IN `_dirty` INT
)
    COMMENT 'Imposta una lista come dirty per tutti gli utenti (da ricaricare in memoria).'
BEGIN
DECLARE st TINYINT;
DECLARE n INT;
SET st = 0;
SELECT COUNT(*) FROM u_connessi WHERE u_connessi.idc = _uid INTO n;
IF n = 1 THEN
	IF _dirty = 1 THEN UPDATE u_connessi SET materiali_dirty = 1;
	ELSEIF _dirty = 2 THEN UPDATE u_connessi SET prodotti_dirty = 1;
	ELSEIF _dirty = 3 THEN UPDATE u_connessi SET costruttori_dirty = 1;
	END IF;
END IF;
SELECT "Dirty";
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.GetCode
DELIMITER //
CREATE PROCEDURE `GetCode`(
	IN `_cod` VARCHAR(255),
	IN `_mod` VARCHAR(1)
)
BEGIN
DECLARE n_cod INT;
SELECT COUNT(*) FROM codici WHERE (codici.cod LIKE _cod) AND (codici.mod LIKE _mod) INTO n_cod;
IF n_cod = 1 THEN
	SELECT c.cod AS CODICE,
	c.mod AS MODIFICA,
	c.descrizione AS DESCRIZIONE,
	CONCAT (IF(ISNULL(cm.cod),"","COM"),IF(ISNULL(cs.cod),"","PAR"),IF(ISNULL(ca.cod),"","ASM"),IF(ISNULL(ck.cod),"","SCH")) AS TIPO,
	mat.materiale AS MATERIALE,
	cm.modello AS MODELLO,
	cm.dettagli AS DETTAGLI,
	cstr.costruttore AS COSTRUTTORE,
	prd.prodotto AS PRODOTTO,
	u.sigla AS OPERATORE,
	c.creazione AS CREAZIONE,
	ulast.sigla AS ULTIMO,
	c.aggiornamento AS AGGIORNAMENTO
	
	FROM codici c
	
	LEFT JOIN particolari cs
	ON c.cod = cs.cod AND c.mod = cs.mod
	LEFT JOIN commerciali cm
	ON c.cod = cm.cod AND c.mod = cm.mod
	LEFT JOIN assiemi ca
	ON c.cod = ca.cod AND c.mod = ca.mod
	LEFT JOIN schemi ck
	ON c.cod = ck.cod AND c.mod = ck.mod
	LEFT JOIN utenti u
	ON c.id_utente = u.id
	LEFT JOIN utenti ulast
	ON c.id_ultimo = ulast.id
	LEFT JOIN costruttori cstr
	ON cm.costruttore = cstr.id
	LEFT JOIN materiali mat
	ON cs.materiale = mat.id
	LEFT JOIN prodotti prd
	ON cm.prodotto = prd.id
	WHERE (c.cod LIKE _cod) AND (c.mod LIKE _mod);
ELSEIF(n_cod = 0) THEN
	SELECT "-" AS ERRORE;
ELSE
	SELECT CONCAT("+",n_cod) AS MULTIPLI;
END IF;
END//
DELIMITER ;

-- Dump della struttura di funzione dbc01.Get_uid_delay
DELIMITER //
CREATE FUNCTION `Get_uid_delay`(`_uid` INT




) RETURNS int(11)
    COMMENT 'Get_uid_delay(_uid) ->  secondi (indice idc è UNIQUE)'
BEGIN
DECLARE st TINYINT;
DECLARE x INT;
DECLARE tstmp DATETIME;
DECLARE dl INT;
SET st = 0;
SET dl = 0;
SELECT count(*) FROM u_connessi uc WHERE uc.idc =_uid INTO x;
IF	x=1 THEN
	SELECT u.write_en FROM u_connessi u WHERE u.idc = _uid INTO st;
	IF st>0 THEN
	SELECT u.lasttime FROM u_connessi u WHERE u.idc = _uid INTO tstmp;
	SET dl = TIMESTAMPDIFF(SECOND, tstmp, NOW());
	END IF;
END IF;
RETURN dl;
END//
DELIMITER ;

-- Dump della struttura di funzione dbc01.Get_uid_stat
DELIMITER //
CREATE FUNCTION `Get_uid_stat`(`_uid` INT




















) RETURNS tinyint(1)
    COMMENT 'Get_uid_stat(_uid) ->  0=non connesso, 1=connesso, 2=scrittura (indice idc è UNIQUE)'
BEGIN
DECLARE st TINYINT;
DECLARE x INT;
DECLARE tstmp DATETIME;
SET st = 0;
SELECT count(*) FROM u_connessi uc WHERE uc.idc =_uid INTO x;
IF	x=1 THEN	
	SELECT u.write_en FROM u_connessi u WHERE u.idc = _uid INTO st;
	UPDATE u_connessi SET lasttime = NOW() WHERE idc = _uid;
END IF;
RETURN st;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.InsAssieme
DELIMITER //
CREATE PROCEDURE `InsAssieme`(
	IN `_cod` CHAR(20),
	IN `_mod` CHAR(1),
	IN `_desc` VARCHAR(255),
	IN `_uid` INT







)
    COMMENT 'InsAssieme(_cod, _mod, _desc, _uid)'
BEGIN
DECLARE n INT;
START TRANSACTION;
SET n = _InsCodice(_cod, _mod, _desc, _uid);
IF n = 1 THEN
	INSERT INTO assiemi(cod, assiemi.mod) VALUES(_cod, _mod);
ELSEIF n=-1 THEN
	UPDATE assiemi SET cod = _cod WHERE assiemi.cod = _cod and assiemi.mod = _mod;
ELSE
	ROLLBACK;
END IF;
COMMIT;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.InsCommerciale
DELIMITER //
CREATE PROCEDURE `InsCommerciale`(
	IN `_cod` CHAR(20),
	IN `_mod` CHAR(1),
	IN `_uid` INT,
	IN `_cos` VARCHAR(255),
	IN `_pro` VARCHAR(255),
	IN `_model` VARCHAR(255),
	IN `_dett` VARCHAR(255)








)
    COMMENT 'InsCommerciale(_cod, _mod, _uid. _cos. _pro, _model, _dett). _desc costruita da altri dati'
BEGIN
DECLARE n INT;
DECLARE id_pro INT;
DECLARE id_cos INT;
DECLARE _desc VARCHAR(100);
START TRANSACTION;
SET id_pro = _InsProdotto(_pro, TRUE);	# Inserisce _pro in tab. prodotti
SET id_cos = _InsCostruttore(_cos, TRUE);	# Inserisce _cos in tab. costruttori
SET _desc = CONCAT(_pro, " ", _cos, " ", _model); # Prepara _desc con descrizione oppure vuota
SET n = _InsCodice(_cod, _mod, _desc, _uid); # Inserire _InsCodice
IF n = 1 THEN
	INSERT INTO commerciali(cod, commerciali.mod, modello, dettagli, costruttore, prodotto)
	VALUES(_cod, _mod, _model, _dett, id_cos, id_pro);
ELSEIF n=-1 THEN
	UPDATE commerciali SET modello = _model, dettagli = _dett, costruttore = id_cos, prodotto = id_pro
	WHERE commerciali.cod = _cod and commerciali.mod = _mod;
ELSE
	ROLLBACK;
END IF;
COMMIT;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.InsParticolare
DELIMITER //
CREATE PROCEDURE `InsParticolare`(
	IN `_cod` CHAR(20),
	IN `_mod` CHAR(1),
	IN `_desc` VARCHAR(255),
	IN `_uid` INT,
	IN `_mat` VARCHAR(255)









)
    COMMENT 'InsParticolare(_cod, _mod, _desc, _uid, _mat)'
BEGIN
DECLARE n INT;
DECLARE id_m INT;
START TRANSACTION;
SET id_m = _InsMateriale(_mat, TRUE);
SET n = _InsCodice(_cod, _mod, _desc, _uid);
IF n = 1 THEN	
	INSERT INTO particolari(cod, particolari.mod, particolari.materiale) VALUES(_cod, _mod, id_m);
ELSEIF n=-1 THEN
	UPDATE particolari SET materiale = id_m WHERE particolari.cod = _cod and particolari.mod = _mod;
ELSE
	ROLLBACK;
END IF;
COMMIT;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.InsSchema
DELIMITER //
CREATE PROCEDURE `InsSchema`(
	IN `_cod` CHAR(20),
	IN `_mod` CHAR(1),
	IN `_desc` VARCHAR(255),
	IN `_uid` INT







)
    COMMENT 'InsSchema(_cod, _mod, _desc, _uid)'
BEGIN
DECLARE n INT;
START TRANSACTION;
SET n = _InsCodice(_cod, _mod, _desc, _uid);
IF n = 1 THEN
	INSERT INTO schemi(cod, schemi.mod) VALUES(_cod, _mod);
ELSEIF n=-1 THEN
	UPDATE schemi SET cod = _cod WHERE schemi.cod = _cod and schemi.mod = _mod;
ELSE
	ROLLBACK;
END IF;
COMMIT;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.ListaCostruttori
DELIMITER //
CREATE PROCEDURE `ListaCostruttori`(
	IN `_uid` INT
)
BEGIN
DECLARE st TINYINT;
DECLARE n INT;
SET st = 0;
SELECT COUNT(*) FROM u_connessi WHERE u_connessi.idc = _uid INTO n;
IF n = 1 THEN
	SELECT u.costruttori_dirty FROM u_connessi u WHERE u.idc = _uid INTO st;
END IF;
IF st = 1 THEN
	SELECT c.id, c.costruttore FROM costruttori c ORDER BY c.costruttore;
	UPDATE u_connessi SET costruttori_dirty = 0 WHERE u_connessi.idc = _uid;
ELSE
	SELECT "NO CHANGE";
END IF;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.ListaMateriali
DELIMITER //
CREATE PROCEDURE `ListaMateriali`(
	IN `_uid` INT
)
BEGIN
DECLARE st TINYINT;
DECLARE n INT;
SET st = 0;
SELECT COUNT(*) FROM u_connessi WHERE u_connessi.idc = _uid INTO n;
IF n = 1 THEN
	SELECT u.materiali_dirty FROM u_connessi u WHERE u.idc = _uid INTO st;
END IF;
IF st = 1 THEN
	SELECT m.id, m.materiale FROM materiali m ORDER BY m.materiale;
	UPDATE u_connessi SET materiali_dirty = 0 WHERE u_connessi.idc = _uid;
ELSE
	SELECT "NO CHANGE";
END IF;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.ListaProdotti
DELIMITER //
CREATE PROCEDURE `ListaProdotti`(
	IN `_uid` INT
)
BEGIN
DECLARE st TINYINT;
DECLARE n INT;
SET st = 0;
SELECT COUNT(*) FROM u_connessi WHERE u_connessi.idc = _uid INTO n;
IF n = 1 THEN
	SELECT u.prodotti_dirty FROM u_connessi u WHERE u.idc = _uid INTO st;
END IF;
IF st = 1 THEN
	SELECT p.id, p.prodotto FROM prodotti p ORDER BY p.prodotto;
	UPDATE u_connessi SET prodotti_dirty = 0 WHERE u_connessi.idc = _uid;
ELSE
	SELECT "NO CHANGE";
END IF;
END//
DELIMITER ;

-- Dump della struttura di tabella dbc01.materiali
CREATE TABLE IF NOT EXISTS `materiali` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `materiale` varchar(255) NOT NULL DEFAULT '-',
  PRIMARY KEY (`id`),
  UNIQUE KEY `Nome_materiale` (`materiale`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista dei nomi dei materiali.\r\nID_dirty = 1.';

-- Dump dei dati della tabella dbc01.materiali: ~8 rows (circa)
REPLACE INTO `materiali` (`id`, `materiale`) VALUES
	(6, '39NiCrMo3'),
	(5, 'AlSi1MgMn'),
	(8, 'Cu Te'),
	(1, 'Fe360'),
	(10, 'Fe360B'),
	(2, 'Fe430'),
	(3, 'Fe510'),
	(4, 'GAlSi7');

-- Dump della struttura di tabella dbc01.particolari
CREATE TABLE IF NOT EXISTS `particolari` (
  `cod` char(20) NOT NULL,
  `mod` char(1) NOT NULL DEFAULT '',
  `materiale` int(10) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`cod`,`mod`),
  KEY `FK_materiali` (`materiale`),
  CONSTRAINT `FK_costruttivi_codici` FOREIGN KEY (`cod`, `mod`) REFERENCES `codici` (`cod`, `mod`),
  CONSTRAINT `FK_materiali` FOREIGN KEY (`materiale`) REFERENCES `materiali` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista dei particolari.\r\nCon dati specifici.';

-- Dump dei dati della tabella dbc01.particolari: ~15 rows (circa)
REPLACE INTO `particolari` (`cod`, `mod`, `materiale`) VALUES
	('100.11.123', 'a', 1),
	('102.11.100', 'a', 1),
	('301.10.100', '', 1),
	('789.10.100', '', 1),
	('789.10.100', 'a', 1),
	('201.10.100', '', 2),
	('789.10.101', '', 2),
	('789.10.101', 'a', 2),
	('100.11.123', 'b', 3),
	('789.10.101', 'b', 3),
	('789.10.200', '', 4),
	('100.11.123', '', 6),
	('201.11.100', '', 6),
	('113.10.100', '', 8),
	('102.11.100', '', 10),
	('201.11.100', 'a', 10);

-- Dump della struttura di tabella dbc01.prodotti
CREATE TABLE IF NOT EXISTS `prodotti` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `prodotto` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `Indice 2` (`prodotto`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista dei nomi dei tipi di prodotti commerciali.\r\nID_dirty = 2.';

-- Dump dei dati della tabella dbc01.prodotti: ~9 rows (circa)
REPLACE INTO `prodotti` (`id`, `prodotto`) VALUES
	(11, ''),
	(12, 'Giunto'),
	(4, 'Guida lineare'),
	(5, 'Pattino a sfere'),
	(6, 'Raccordo'),
	(3, 'Regolatore di pressione'),
	(2, 'Valvola'),
	(1, 'Vite'),
	(9, 'Vite a ricircolo di sfere');

-- Dump della struttura di funzione dbc01.Revmax
DELIMITER //
CREATE FUNCTION `Revmax`(`codice` CHAR(50)






) RETURNS int(11)
BEGIN
DECLARE x INT;
SELECT count(*) FROM codici c WHERE c.cod=codice INTO x;
RETURN x;
END//
DELIMITER ;

-- Dump della struttura di tabella dbc01.schemi
CREATE TABLE IF NOT EXISTS `schemi` (
  `cod` char(20) NOT NULL,
  `mod` char(1) NOT NULL DEFAULT '',
  PRIMARY KEY (`cod`,`mod`),
  CONSTRAINT `FK_schemi_codici` FOREIGN KEY (`cod`, `mod`) REFERENCES `codici` (`cod`, `mod`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista degli schemi.';

-- Dump dei dati della tabella dbc01.schemi: ~8 rows (circa)
REPLACE INTO `schemi` (`cod`, `mod`) VALUES
	('201.10.900', ''),
	('300.33.330', 'a'),
	('301.10.900', ''),
	('554.55.554', ''),
	('567.89.111', ''),
	('820.10.900', ''),
	('820.10.900', 'a'),
	('820.10.900', 'b'),
	('987.43.444', 'a'),
	('999.88.776', '');

-- Dump della struttura di funzione dbc01.Set_uid_stat
DELIMITER //
CREATE FUNCTION `Set_uid_stat`(`_uid` INT,
	`_st` TINYINT










) RETURNS tinyint(1)
    COMMENT 'Set_uid_stat(_uid, _st) -> st finale. Aggiorna o inserisce, controllando se _uid non ha permesso di scrittura. Se _st=0: rimuove. '
BEGIN
DECLARE x INT;
DECLARE stn TINYINT;
DECLARE canw TINYINT;
DECLARE ret TINYINT;
SET stn = _st;
SET canw = 0;
-- Verifica che l'_uid esista
SELECT count(*) FROM utenti ut WHERE ut.id = _uid INTO x;
IF x = 1 THEN
	-- Se richiesta scrittura ma l'utente non ne ha il permesso, lo imposta in sola lettura
	SELECT ut.can_write FROM utenti ut WHERE ut.id = _uid INTO canw;
	IF canw = 0 AND stn = 2 THEN
		SET stn = 1;
	END IF;
	-- Se stn = 0: rimuove l'utente _uid dalla tabella u_connessi
	IF stn = 0 THEN
		DELETE FROM u_connessi WHERE idc = _uid;
	ELSE
	 	-- Aggiorna oppure aggiunge
		SELECT count(*) FROM u_connessi uc WHERE uc.idc = _uid INTO x;
		IF	x=1 THEN	
			UPDATE u_connessi u SET u.write_en = stn WHERE u.idc = _uid;
		ELSE
			INSERT INTO u_connessi(u_connessi.idc, u_connessi.write_en) VALUES(_uid, stn); 
		END IF;
	END IF;
END IF;
SET ret = Get_uid_stat(_uid);
RETURN ret;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.TEST_SCHEDULE
DELIMITER //
CREATE PROCEDURE `TEST_SCHEDULE`()
BEGIN
SET GLOBAL event_scheduler = ON;
SHOW PROCESSLIST;
SET GLOBAL event_scheduler = OFF;
END//
DELIMITER ;

-- Dump della struttura di tabella dbc01.utenti
CREATE TABLE IF NOT EXISTS `utenti` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `utente` varchar(20) NOT NULL DEFAULT '',
  `password1` char(64) DEFAULT NULL COMMENT 'sha256',
  `password2` char(64) DEFAULT NULL COMMENT 'sha256',
  `sigla` char(5) NOT NULL DEFAULT '',
  `can_write` tinyint(1) DEFAULT 0 COMMENT 'Modiificabile solo da ADMIN',
  PRIMARY KEY (`id`),
  UNIQUE KEY `utente` (`utente`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista degli utenti.\r\nContiene hash (sha256) delle password di accesso e scrittura e se l''utente ha i diritti di scrittura.';

-- Dump dei dati della tabella dbc01.utenti: ~3 rows (circa)
REPLACE INTO `utenti` (`id`, `utente`, `password1`, `password2`, `sigla`, `can_write`) VALUES
	(1, 'pluto', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'PLU', 0),
	(2, 'pippo', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'PIP', 1),
	(5, 'minnie', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'MIN', 1);

-- Dump della struttura di tabella dbc01.u_connessi
CREATE TABLE IF NOT EXISTS `u_connessi` (
  `idc` int(10) unsigned NOT NULL,
  `write_en` tinyint(1) unsigned NOT NULL DEFAULT 1 COMMENT '1=lettura, 2=scrittura',
  `lasttime` datetime DEFAULT current_timestamp() COMMENT 'Tolto ON UPDATE CURRENT TIMESTAMP',
  `materiali_dirty` tinyint(1) DEFAULT 1,
  `prodotti_dirty` tinyint(1) DEFAULT 1,
  `costruttori_dirty` tinyint(1) DEFAULT 1,
  UNIQUE KEY `idc` (`idc`),
  CONSTRAINT `FK_utenti` FOREIGN KEY (`idc`) REFERENCES `utenti` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci COMMENT='Lista degli utenti connessi.\r\nContiene flag delle liste da caricare in memoria, per sapere se sono aggiornate oppure no (dirty).';

-- Dump dei dati della tabella dbc01.u_connessi: ~0 rows (circa)

-- Dump della struttura di vista dbc01.vedicodici
-- Creazione di una tabella temporanea per risolvere gli errori di dipendenza della vista
CREATE TABLE `vedicodici` (
	`CODICE` VARCHAR(21) NOT NULL COLLATE 'latin1_swedish_ci'
) ENGINE=MyISAM;

-- Dump della struttura di procedura dbc01.VediCodici
DELIMITER //
CREATE PROCEDURE `VediCodici`(
	IN `limite` INT








)
BEGIN
SELECT c.cod, c.mod, c.descrizione, u.utente, c.creazione, uu.utente, c.aggiornamento
FROM codici c
LEFT JOIN utenti u
ON c.id_utente = u.id
LEFT JOIN utenti uu
ON c.id_ultimo = uu.id
ORDER BY c.cod, c.mod
LIMIT limite;
END//
DELIMITER ;

-- Dump della struttura di procedura dbc01.VediDescrizioni
DELIMITER //
CREATE PROCEDURE `VediDescrizioni`(
	IN `_limite` INT,
	IN `_codice` VARCHAR(255),
	IN `_modifica` CHAR(1)
)
BEGIN
-- SELECT CONCAT(c.cod, IFNULL(c.mod,"")) AS CODICE,
SELECT c.cod AS CODICE,
c.mod AS MODIFICA,
CONCAT(c.descrizione, IFNULL(UPPER(CONCAT(" ",cstr.costruttore)),""), IFNULL(CONCAT(" ",cm.modello),"")) AS DESCRIZIONE,
CONCAT (IF(ISNULL(cm.cod),"","COM"),IF(ISNULL(cs.cod),"","PAR"),IF(ISNULL(ca.cod),"","ASM"),IF(ISNULL(ck.cod),"","SCH")) AS TIPO,
u.sigla AS OPERATORE,
c.creazione AS CREAZIONE
FROM codici c
LEFT JOIN particolari cs
ON c.cod = cs.cod AND c.mod = cs.mod
LEFT JOIN commerciali cm
ON c.cod = cm.cod AND c.mod = cm.mod
LEFT JOIN assiemi ca
ON c.cod = ca.cod AND c.mod = ca.mod
LEFT JOIN schemi ck
ON c.cod = ck.cod AND c.mod = ck.mod
LEFT JOIN utenti u
ON c.id_utente = u.id
LEFT JOIN costruttori cstr
ON cm.costruttore = cstr.id
WHERE (c.cod LIKE _codice) AND (c.`mod` LIKE _modifica)
ORDER BY CODICE
LIMIT _limite;
END//
DELIMITER ;

-- Dump della struttura di funzione dbc01._InsCodice
DELIMITER //
CREATE FUNCTION `_InsCodice`(`_cod` CHAR(20),
	`_mod` CHAR(1),
	`_desc` VARCHAR(255),
	`_uid` INT
) RETURNS tinyint(4)
    COMMENT '_InsCodice(_cod, _mod, _desc, _uid) -> +1=aggiunto, -1=aggiornato, 0=errore'
BEGIN
DECLARE n INT;	# return value
DECLARE x INT;	# numero di codici trovati / stato _uid
SET n = 0;
IF Get_uid_stat(_uid) = 2 THEN
	SELECT COUNT(*) FROM codici c WHERE c.cod = _cod AND c.mod = _mod INTO x;
	IF x = 0 THEN
		INSERT INTO codici(cod, codici.mod, descrizione, id_utente, id_ultimo) VALUES(_cod, _mod, _desc, _uid, _uid);
		SET n = 1;
	ELSEIF(x = 1) THEN
		UPDATE codici SET descrizione = _desc, id_ultimo = _uid WHERE codici.cod = _cod AND codici.mod = _mod;
		SET n = -1;
	ELSE
		SET n = 0;
	END IF;
END IF;
RETURN n;
END//
DELIMITER ;

-- Dump della struttura di funzione dbc01._InsCostruttore
DELIMITER //
CREATE FUNCTION `_InsCostruttore`(`_cos` VARCHAR(255),
	`_add` TINYINT
) RETURNS int(11)
    COMMENT 'Restituisce l''id di un costruttore e, se richiesto, lo inserisce'
BEGIN
DECLARE n INT;
DECLARE c INT;
SET c = 0;
SELECT COUNT(*) FROM costruttori m WHERE m.costruttore = _cos INTO n;
IF(n = 1) THEN
	SELECT m.id FROM costruttori m WHERE m.costruttore = _cos INTO c;
ELSEIF(n = 0 AND _add = TRUE) THEN
	INSERT INTO costruttori(costruttori.costruttore) VALUES(_cos);
	UPDATE u_connessi SET u_connessi.costruttori_dirty = 1;
	SELECT m.id FROM costruttori m WHERE m.costruttore = _cos INTO c;
END IF;
RETURN c;
END//
DELIMITER ;

-- Dump della struttura di funzione dbc01._InsMateriale
DELIMITER //
CREATE FUNCTION `_InsMateriale`(`_mat` VARCHAR(255),
	`_add` TINYINT
) RETURNS int(11)
    COMMENT 'Restituisce l''id di un materiale o, se richiesto, lo inserisce'
BEGIN
DECLARE n INT;
DECLARE c INT;
SET c = 0;
SELECT COUNT(*) FROM materiali m WHERE m.materiale = _mat INTO n;
IF(n = 1) THEN
	SELECT m.id FROM materiali m WHERE m.materiale = _mat INTO c;
ELSEIF(n = 0 AND _add = TRUE) THEN
	INSERT INTO materiali(materiale) VALUES(_mat);
	UPDATE u_connessi SET u_connessi.materiali_dirty = 1;
	SELECT m.id FROM materiali m WHERE m.materiale = _mat INTO c;
END IF;
RETURN c;
END//
DELIMITER ;

-- Dump della struttura di funzione dbc01._InsProdotto
DELIMITER //
CREATE FUNCTION `_InsProdotto`(`_pro` VARCHAR(255),
	`_add` TINYINT
) RETURNS int(11)
BEGIN
DECLARE n INT;
DECLARE c INT;
SET c = 0;
SELECT COUNT(*) FROM prodotti m WHERE m.prodotto = _pro INTO n;
IF(n = 1) THEN
	SELECT m.id FROM prodotti m WHERE m.prodotto = _pro INTO c;
ELSEIF(n = 0 AND _add = TRUE) THEN
	INSERT INTO prodotti(prodotto) VALUES(_pro);
	UPDATE u_connessi SET u_connessi.prodotti_dirty = 1;
	SELECT m.id FROM prodotti m WHERE m.prodotto = _pro INTO c;
END IF;
RETURN c;
END//
DELIMITER ;

-- Dump della struttura di vista dbc01.vedicodici
-- Rimozione temporanea di tabella e creazione della struttura finale della vista
DROP TABLE IF EXISTS `vedicodici`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vedicodici` AS SELECT 
CONCAT(c.cod, IFNULL(c.mod,"")) AS CODICE
FROM
codici c ;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
