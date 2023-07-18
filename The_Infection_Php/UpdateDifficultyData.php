<?php 
include('Connection.php'); 

$difficulty = $_POST['editDifficulty']; 
$currentNickName = $_POST['currentNickName']; 

$sql = "UPDATE `scor` SET `Difficulty` = '".$difficulty."' WHERE `scor`.`NickName` = '".$currentNickName."'"; 
mysqli_query($connect, $sql); 

?> 