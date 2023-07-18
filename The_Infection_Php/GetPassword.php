<?php 
include('Connection.php'); 

$username = $_POST['currentUser']; 
$sql = "SELECT `UserName`, `Password` from `UserData` WHERE `UserName` LIKE '".$username."'"; 
$result = mysqli_query($connect, $sql); 

if(mysqli_num_rows($result) > 0) { 
    while($row = mysqli_fetch_assoc($result)) { 
        echo $row['Password']; 
    } 
} 

?> 