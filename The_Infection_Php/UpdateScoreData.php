<?php 
include('Connection.php'); 

$level = $_POST['editLevel']; 
$time = $_POST['editTime']; 
$score = $_POST['editScore']; 
$currentNickName = $_POST['currentNickName']; 

$sql = "UPDATE `scor` SET `Level` = '".$level."', `Time` = '".$time."', `Score` = '".$score."' WHERE `scor`.`NickName` = '".$currentNickName."'"; 
mysqli_query($connect, $sql); 

?> 