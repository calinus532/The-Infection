<?php 
include('Connection.php'); 

$nickname = $_POST['currentNickName'];
$sql = "SELECT NickName, Level, Time, Score, MazeType from scor WHERE NickName LIKE '".$nickname."'"; 
$result = mysqli_query($connect, $sql); 

if(mysqli_num_rows($result) > 0) { 
    while($row = mysqli_fetch_assoc($result)) { 
        echo $row['Level'] . "/" . $row['Time'] . "/" . $row['Score'] . "/" . $row['MazeType']; 
    } 
} 

?> 