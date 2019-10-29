function Get-Credentials() {
    
    $rootXml = (Select-Xml -Path "$PSScriptRoot\..\..\TestConnectionStrings.xml" -XPath "/connectionStrings").Node
    
    $credentialObject = New-Object -TypeName PSObject
    $credentialObject | Add-Member -MemberType NoteProperty -Name UserId -TypeName String -Value (Select-Xml -Node $rootXml -XPath "properties/userId").Node.InnerText
    $credentialObject | Add-Member -MemberType NoteProperty -Name Password -TypeName String -Value (Select-Xml -Node $rootXml -XPath "properties/password").Node.InnerText
    $credentialObject | Add-Member -MemberType NoteProperty -Name Server -TypeName String -Value (Select-Xml -Node $rootXml -XPath "properties/server").Node.InnerText

    $credentialObject
}