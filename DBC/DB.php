<?php
session_start();
include 'php/db.inc.php';
$x = new Connessione();
$x->ReadPost();
//$x->ShowPost();
$x->ProcessPost();
?>