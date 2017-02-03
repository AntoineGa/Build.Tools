If (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Error "Need to run as an administrator"
    return -1
}

$source = "https://letsencrypt.org/certs/lets-encrypt-x3-cross-signed.pem"
$file = $source.Split("/")[-1]
$out = Join-Path "$($env:USERPROFILE)\Downloads" -ChildPath $file

Invoke-WebRequest -Uri $source -OutFile $out

$args = @(" -trustcacerts -keystore "+'"' + $($env:JAVA_HOME) + '\lib\security\cacerts"'+ " -storepass changeit -noprompt -importcert -file " + $out )

echo $args

$process = Start-Process -FilePath $env:JAVA_HOME\bin\keytool.exe -ArgumentList $args -Wait -PassThru -NoNewWindow
