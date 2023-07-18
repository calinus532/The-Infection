<?php 
include('Connection.php'); 

$nickname = $_POST['addNickName']; 
$level = $_POST['addLevel']; 
$time = $_POST['addTime']; 
$country = $_POST['addCountry']; 
$password = $_POST['addPassword']; 

$sql = "INSERT INTO `scor` (`ID`, `NickName`, `Level`, `Time`) VALUES (NULL, '".$nickname."', '".$level."', '".$time."')"; 
mysqli_query($connect, $sql); 
$sql = "INSERT INTO `MazePositions` (`ID`, `UserName`) VALUES (NULL, '".$nickname."')"; 
mysqli_query($connect, $sql); 
$sql = "INSERT INTO `UserData` (`ID`, `UserName`, `Password`, `Country`) VALUES (NULL, '".$nickname."', '".$password."', '".$country."')"; 
mysqli_query($connect, $sql); 

?> 