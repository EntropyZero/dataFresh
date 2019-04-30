Import-Module $PSScriptRoot\CredentialReader.psm1

$creds = Get-Credentials
.\DataFreshUtil.exe -s $creds.Server -u $creds.UserId -p $creds.Password -d DataFreshSample -c REFRESH

pause