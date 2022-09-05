param(
	[Parameter(Mandatory=$true)] $projectDir,
	[Parameter(Mandatory=$true)] $assemblyPath,
	[Parameter(Mandatory=$true)] $assemblyName
)

function GetModuleVersion($assemblyPath)
{
	$fileVersion = (Get-Command $assemblyPath).FileVersionInfo.FileVersion
	Write-Host ("    File version: {0}" -f $fileVersion)
	$versionObject = New-Object -TypeName System.Version -ArgumentList $fileVersion
	$moduleVersion = $versionObject.ToString(3)
	return $moduleVersion
}

function CreateCorrectlyNamedEmptyExport($projectDir, $moduleVersion, $assemblyName)
{
	Write-Host "    Creating empty module export file"
    $projectDirTrimmed = TrimTrailingSlash $projectDir
	$templateExportPath = $projectDirTrimmed + "\build\EmptyExport.zip"
	$targetFolderPath = ("{0}\content\App_Data\CMSModules\{1}\Install" -f $projectDirTrimmed, $assemblyName)
	$targetExportPath = ("{0}\KenticoCommunity.ContentReferenceUi_{1}.zip" -f $targetFolderPath, $moduleVersion)
	if (Test-Path -Path $targetFolderPath) {
		Write-Host ("      Deleting contents of: {0}" -f $targetFolderPath)
		Remove-Item ("{0}\*.*" -f $targetFolderPath)
	}
    else
    {
        New-Item $targetFolderPath -ItemType Directory
    }
	Write-Host ("      Export template: {0}" -f $templateExportPath)
	Write-Host ("      Target export path: {0}" -f $targetExportPath)

	Copy-Item $templateExportPath $targetExportPath
}

function TrimTrailingSlash($path)
{
    return $path.Trim(@("/","\"))
}

try {
	Write-Host "START: post-build.ps1"
	Write-Host ("    Project directory: {0}" -f $projectDir)
	Write-Host ("    Assembly name: {0}" -f $assemblyName)
	Write-Host ("    Assembly path: {0}" -f $assemblyPath)
	$moduleVersion = GetModuleVersion $assemblyPath
	Write-Host ("    Module version: {0}" -f $moduleVersion)
	CreateCorrectlyNamedEmptyExport $projectDir $moduleVersion $assemblyName

	exit 0
}
catch {
	exit 1
}