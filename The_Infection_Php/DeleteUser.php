<?php
include('Connection.php'); 

$username = $_POST['currentNickName']; 

$sql = "DELETE FROM `scor` WHERE `NickName` = '".$username."'"; 
mysqli_query($connect, $sql); 

$sql = "DELETE FROM `MazePositions` WHERE `UserName` = '".$username."'"; 
mysqli_query($connect, $sql); 

$sql = "DELETE FROM `UserData` WHERE `UserName` = '".$username."'"; 
mysqli_query($connect, $sql); 
?>