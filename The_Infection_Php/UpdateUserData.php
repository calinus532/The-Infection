<?php 
include('Connection.php'); 

$name = $_POST['editName']; 
$password = $_POST['editPassword']; 
$country = $_POST['editCountry']; 
$currentUser = $_POST['currentUser']; 

$sql = "UPDATE `UserData` SET `UserName` = '".$name."', `Password` = '".$password."' WHERE `UserData`.`UserName` = '".$currentUser."'"; 
mysqli_query($connect, $sql); 

$sql = "UPDATE `scor` SET `NickName` = '".$name."', `Country` = '".$country."' WHERE `NickName` = '".$currentUser."'"; 
mysqli_query($connect, $sql); 

$sql = "UPDATE `MazePositions` SET `UserName` = '".$name."' WHERE `MazePositions`.`UserName` = '".$currentUser."'"; 
mysqli_query($connect, $sql); 

?> 