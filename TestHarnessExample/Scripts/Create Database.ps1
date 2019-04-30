Import-Module $PSScriptRoot\CredentialReader.psm1

$creds = Get-Credentials

osql.exe -S $creds.Server -U $creds.UserId -P $creds.Password -d DataFreshSample -i $PSScriptRoot\database.sql

.\DataFreshUtil.exe -s $creds.Server -u $creds.UserId -p $creds.Password -d DataFreshSample -c PREPARE

pause