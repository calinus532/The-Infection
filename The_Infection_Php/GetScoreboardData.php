<?php 
include('Connection.php'); 

$difficulty = $_POST['getDifficulty']; 
$type = $_POST['getMazeType']; 
$sql = "SELECT NickName, Level, Time, Country, Difficulty, MazeType from scor 
	WHERE Difficulty LIKE '".$difficulty."' AND MazeType LIKE '".$type."' 
	ORDER BY `Level` DESC, `Time` ASC, `Country` LIMIT 10"; 
$result = mysqli_query($connect, $sql); 

if(mysqli_num_rows($result) > 0) { 
    while($row = mysqli_fetch_assoc($result)) { 
        echo $row['NickName'] . "," . $row['Level'] . "," . $row['Time'] . "," . $row['Country'] . "," . $row['Difficulty'] . "/"; 
    } 
} 

?> 