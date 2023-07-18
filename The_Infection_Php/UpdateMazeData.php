<?php 
include('Connection.php'); 

$walls = $_POST['editWalls']; 
$corectPath = $_POST['editCorectPath']; 
$corner = $_POST['editCorner']; 
$finish = $_POST['editFinish']; 
$player = $_POST['editPlayer']; 
$viruses = $_POST['editViruses']; 
$masks = $_POST['editMasks']; 
$isLight = $_POST['editLight']; 
$mazeType = $_POST['editType']; 
$currentUser = $_POST['currentUser']; 

$sql = "UPDATE `MazePositions` SET `Walls` = '".$walls."', `CorectPath` = '".$corectPath."', `Corner` = '".$corner."', 
	`Finish` = '".$finish."', `Player` = '".$player."', `Viruses` = '".$viruses."', `Masks` = '".$masks."', `IsLight` = '".$isLight."'
		WHERE `MazePositions`.`UserName` = '".$currentUser."'"; 
mysqli_query($connect, $sql); 

$sql = "UPDATE `scor` SET `MazeType` = '".$mazeType."' WHERE `NickName` = '".$currentUser."'"; 
mysqli_query($connect, $sql); 

?> 