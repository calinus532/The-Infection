<?php 
include('Connection.php'); 

$username = $_POST['currentUser']; 

$sql = "SELECT `NickName`, `Country` from `scor` 
	WHERE `NickName` LIKE '".$username."'"; 
$result = mysqli_query($connect, $sql); 

if(mysqli_num_rows($result) > 0) { 
    while($row = mysqli_fetch_assoc($result)) { 
        $country = $row['Country']; 
    } 
} 

$sql = "SELECT * from `UserData` 
	WHERE `UserName` LIKE '".$username."'"; 
$result = mysqli_query($connect, $sql); 

if(mysqli_num_rows($result) > 0) { 
    while($row = mysqli_fetch_assoc($result)) { 
        $name = $row['UserName']; 
		$props = $row['LevelPropieties'] . "|" . $row['ShopPropieties'] . "|" . $row['PlayerPropieties']; 
    } 
} 

echo $name . "|" . $country . "|" . $props; 

?> 