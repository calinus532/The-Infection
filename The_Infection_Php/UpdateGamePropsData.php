<?php 
include('Connection.php'); 

$currentUser = $_POST['currentUser']; 
$country = $_POST['editCountry']; 
$levelProps = $_POST['editLevelProps']; 
$shopProps = $_POST['editShopProps']; 
$playerProps = $_POST['editPlayerProps']; 

$sql = "UPDATE `UserData` SET `LevelPropieties` = '".$levelProps."', `ShopPropieties` = '".$shopProps."', `PlayerPropieties` = '".$playerProps."'
	WHERE `UserData`.`UserName` = '".$currentUser."'"; 
mysqli_query($connect, $sql); 

$sql = "UPDATE `scor` SET `Country` = '".$country."' WHERE `scor`.`NickName` = '".$currentUser."'"; 
mysqli_query($connect, $sql); 

?> 