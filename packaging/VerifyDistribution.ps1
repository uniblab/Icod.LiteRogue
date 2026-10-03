param(
    [ValidateSet('Debug', 'Staging', 'Release')]
    [string]$Configuration = 'Release',

    [switch]$SkipPackageValidation,
    [string]$RuntimeIdentifier = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
Import-Module (Join-Path $PSScriptRoot 'RepositoryTools.psm1') -Force

$solutionPath = Get-RepositorySolution -RepositoryRoot $repositoryRoot
$validationRoot = Join-Path $repositoryRoot 'artifacts/distribution-validation'
$packageDirectory = Join-Path $validationRoot 'packages'

if (Test-Path -LiteralPath $validationRoot) {
    Remove-Item -LiteralPath $validationRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $packageDirectory -Force | Out-Null

Push-Location $repositoryRoot
try {
    Invoke-DotNet -Arguments @('restore', $solutionPath)
    Invoke-DotNet -Arguments @(
        'build', $solutionPath,
        '-c', $Configuration,
        '--no-restore',
        '-p:ContinuousIntegrationBuild=true'
    )
    Invoke-DotNet -Arguments @(
        'test', $solutionPath,
        '-c', $Configuration,
        '--no-build',
        '--no-restore',
        '--logger', 'trx'
    )

    if (-not $SkipPackageValidation) {
        Invoke-DotNet -Arguments @(
            'pack', $solutionPath,
            '-c', $Configuration,
            '--no-build',
            '--no-restore',
            '-o', $packageDirectory,
            '-p:ContinuousIntegrationBuild=true'
        )

        & (Join-Path $PSScriptRoot 'VerifyPackageArtifact.ps1') `
            -ArtifactDirectory $packageDirectory `
            -Configuration $Configuration
        $packages = @(Get-ChildItem -LiteralPath $packageDirectory -Filter '*.nupkg' -File)
        if ($packages.Count -ne 1) { throw 'Expected exactly one tool package.' }
        $version = (Get-PackageMetadata -PackagePath $packages[0].FullName).Version
        & (Join-Path $PSScriptRoot 'VerifyToolInstall.ps1') -PackageDirectory $packageDirectory -Version $version
        if (-not [string]::IsNullOrWhiteSpace($RuntimeIdentifier)) {
            & (Join-Path $PSScriptRoot 'BuildReleaseArchive.ps1') -RuntimeIdentifier $RuntimeIdentifier -Configuration $Configuration -Version $version -ArchiveBaseName 'Icod.LiteRogue'
            $extractedRoot = Join-Path $validationRoot "extracted/$RuntimeIdentifier"
            Expand-Archive -LiteralPath (Join-Path $repositoryRoot "artifacts/release/Icod.LiteRogue-$version-$RuntimeIdentifier.zip") -DestinationPath $extractedRoot
            $archiveExecutable = Join-Path $extractedRoot "Icod.LiteRogue-$version-$RuntimeIdentifier/Icod.LiteRogue"
            if ($RuntimeIdentifier.StartsWith('win-')) { $archiveExecutable += '.exe' }
            else { & chmod +x $archiveExecutable; if ($LASTEXITCODE -ne 0) { throw 'Could not set extracted executable permissions.' } }
            $reportedVersion = & $archiveExecutable --version
            if ($LASTEXITCODE -ne 0 -or ($reportedVersion -join '').Trim() -ne $version) { throw 'Archive executable version mismatch.' }
            & $archiveExecutable --help
            if ($LASTEXITCODE -ne 0) { throw 'Archive executable help failed.' }
        }
    }

    Write-Host ''
    Write-Host "Distribution verification completed successfully ($Configuration)."
    Write-Host "  Solution: $solutionPath"
} finally {
    Pop-Location
}
