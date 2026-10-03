param(
    [Parameter(Mandatory = $true)][string]$PackageDirectory,
    [string]$Version = ''
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
Import-Module (Join-Path $PSScriptRoot 'RepositoryTools.psm1') -Force
$packageSource = if ([System.IO.Path]::IsPathRooted($PackageDirectory)) { [System.IO.Path]::GetFullPath($PackageDirectory) } else { [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $PackageDirectory)) }
if ([string]::IsNullOrWhiteSpace($Version)) {
    $packages = @(Get-ChildItem -LiteralPath $packageSource -Filter '*.nupkg' -File)
    if ($packages.Count -ne 1) { throw 'Expected exactly one package for the installation check.' }
    $Version = (Get-PackageMetadata -PackagePath $packages[0].FullName).Version
}
$toolPath = Join-Path $repositoryRoot 'artifacts/tool-smoke'
if (Test-Path -LiteralPath $toolPath) { Remove-Item -LiteralPath $toolPath -Recurse -Force }
Invoke-DotNet -Arguments @('tool', 'install', 'Icod.LiteRogue', '--version', $Version, '--add-source', $packageSource, '--tool-path', $toolPath, '--no-cache')
$commandName = if ([System.Environment]::OSVersion.Platform -eq [System.PlatformID]::Win32NT) { 'literogue.exe' } else { 'literogue' }
$command = Join-Path $toolPath $commandName
$actual = & $command --version
if ($LASTEXITCODE -ne 0 -or ($actual -join '').Trim() -ne $Version) { throw "Installed tool reports '$actual' instead of '$Version'." }
& $command --help
if ($LASTEXITCODE -ne 0) { throw 'Installed tool help failed.' }
Write-Host "Clean tool installation verified: $Version"
