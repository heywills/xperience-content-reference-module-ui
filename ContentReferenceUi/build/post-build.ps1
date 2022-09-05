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

function CreateCorrectlyNamedEmptyExport($projectDir, $moduleVersion)
{
	Write-Host "    Creating empty module export file"
	$templateExportPath = $projectDir + "build\EmptyExport.zip"
	$targetFolder = ("{0}content\App_Data\CMSModules\KenticoCommunity.ContentReferenceUi\Install\" -f $projectDir)
	$targetExportPath = ("{0}KenticoCommunity.ContentReferenceUi_{1}.zip" -f $targetFolder, $moduleVersion)
	if (Test-Path -Path $targetFolder) {
		Write-Host ("      Deleting contents of: {0}" -f $targetFolder)
		Remove-Item ("{0}*.*" -f $targetFolder)
	}
    else
    {
        New-Item $targetFolder -ItemType Directory
    }
	Write-Host ("      Export template: {0}" -f $templateExportPath)
	Write-Host ("      Target export path: {0}" -f $targetExportPath)

	Copy-Item $templateExportPath $targetExportPath
}

try {
	Write-Host "START: post-build.ps1"
	Write-Host ("    Project directory: {0}" -f $projectDir)
	Write-Host ("    Assembly name: {0}" -f $assemblyName)
	Write-Host ("    Assembly path: {0}" -f $assemblyPath)
	$moduleVersion = GetModuleVersion $assemblyPath
	Write-Host ("    Module version: {0}" -f $moduleVersion)
	CreateCorrectlyNamedEmptyExport $projectDir $moduleVersion

	exit 0
}
catch {
	exit 1
}