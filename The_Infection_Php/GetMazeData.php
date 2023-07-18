<?php 
include('Connection.php'); 

$username = $_POST['currentUser'];
$sql = "SELECT `UserName`, `Walls`, `CorectPath`, `Corner`, `Finish`, `Player`, `Viruses`, `Masks`, `IsLight` from `MazePositions` 
	WHERE `UserName` LIKE '".$username."'"; 
$result = mysqli_query($connect, $sql); 

if(mysqli_num_rows($result) > 0) { 
    while($row = mysqli_fetch_assoc($result)) { 
        echo $row['Walls'] . "|" . $row['CorectPath'] . "|" . $row['Corner'] . "|" . $row['Finish'] . "|" 
			. $row['Player'] . "|" . $row['IsLight'] . "|" . $row['Viruses'] . "|" . $row['Masks']; 
    } 
} 

?> 