$packageName = 'NegativeScreen'
$url = 'http://arcanesanctum.net/wp-content/uploads/negativescreen/Binary.zip'

Install-ChocolateyZipPackage "$packageName" "$url" "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$processor = Get-WmiObject Win32_Processor
$is64bit = $processor.AddressWidth -eq 64
$target = ''

if ($is64bit)
{
	$target = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "$($packageName).exe"
}
else
{
	$target = Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) "$($packageName)x86.exe"
}

#Install-ChocolateyDesktopLink $target
Install-ChocolateyPinnedTaskBarItem $target