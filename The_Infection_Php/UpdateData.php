<?php 
include('Connection.php'); 

$level = $_POST['editLevel']; 
$time = $_POST['editTime']; 
$currentNickName = $_POST['currentNickName']; 
$country = $_POST['editCountry']; 

$sql = "UPDATE `scor` SET  `Level` = '".$level."', `Time` = '".$time."', `Country` = '".$country."' WHERE `scor`.`NickName` = '".$currentNickName."'"; 
mysqli_query($connect, $sql); 

?> 