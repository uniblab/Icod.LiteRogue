param(
    [Parameter(Mandatory = $true)]
    [string]$ArtifactDirectory,

    [ValidateSet('Debug', 'Staging', 'Release')]
    [string]$Configuration = 'Release',

    [string]$ExpectedVersion = '',

    [switch]$AllowNoPackages,

    [string]$GitHubOutputPath = ''
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
Import-Module (Join-Path $PSScriptRoot 'RepositoryTools.psm1') -Force

if (-not [System.IO.Path]::IsPathRooted($ArtifactDirectory)) {
    $ArtifactDirectory = Join-Path $repositoryRoot $ArtifactDirectory
}
$ArtifactDirectory = [System.IO.Path]::GetFullPath($ArtifactDirectory)
if (-not (Test-Path -LiteralPath $ArtifactDirectory -PathType Container)) {
    throw "Artifact directory '$ArtifactDirectory' does not exist."
}

$packages = @(
    Get-ChildItem -LiteralPath $ArtifactDirectory -Filter '*.nupkg' -File |
        Where-Object { -not $_.Name.EndsWith('.symbols.nupkg', [System.StringComparison]::OrdinalIgnoreCase) } |
        Sort-Object Name
)

if (-not [string]::IsNullOrWhiteSpace($ExpectedVersion)) {
    $packages = @(
        $packages |
            Where-Object {
                (Get-PackageMetadata -PackagePath $_.FullName).Version -eq $ExpectedVersion
            }
    )
}

if (0 -eq $packages.Count -and -not $AllowNoPackages) {
    $suffix = if ([string]::IsNullOrWhiteSpace($ExpectedVersion)) { '' } else { " with version '$ExpectedVersion'" }
    throw "No NuGet packages$suffix were found in '$ArtifactDirectory'."
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
if ($packages.Count -gt 1) { throw 'Icod.LiteRogue must produce exactly one .NET tool package.' }
foreach ($package in $packages) {
    $metadata = Get-PackageMetadata -PackagePath $package.FullName
    Write-Host "Verifying $($metadata.Id) $($metadata.Version): $($package.FullName)"

    $archive = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)
    try {
        if ($metadata.Id -ne 'Icod.LiteRogue') { throw "Unexpected package '$($metadata.Id)'." }
        $required = @('LICENSE', 'README.md', 'docs/Playing.md', 'THIRD-PARTY-NOTICES.md',
            'LICENSES/Icod.DCurses.LICENSE', 'LICENSES/Icod.Terminal.LICENSE', 'LICENSES/Icod.TermInfo.LICENSE', 'LICENSES/Icod.Timing.LICENSE',
            'tools/net10.0/any/DotnetToolSettings.xml', 'tools/net10.0/any/Icod.LiteRogue.dll', 'tools/net10.0/any/Icod.LiteRogue.Model.dll',
            'tools/net10.0/any/Icod.DCurses.dll', 'tools/net10.0/any/Icod.Terminal.dll', 'tools/net10.0/any/Icod.TermInfo.dll', 'tools/net10.0/any/Icod.Timing.dll')
        foreach ($name in $required) {
            if ($null -eq $archive.GetEntry($name)) { throw "Package is missing '$name'." }
        }
        foreach ($name in @('LICENSE', 'LICENSES/Icod.DCurses.LICENSE', 'LICENSES/Icod.Terminal.LICENSE', 'LICENSES/Icod.TermInfo.LICENSE', 'LICENSES/Icod.Timing.LICENSE')) {
            $stream = $archive.GetEntry($name).Open()
            $sha = [System.Security.Cryptography.SHA256]::Create()
            try { $actual = [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-', '') }
            finally { $sha.Dispose(); $stream.Dispose() }
            if ($actual -ne (Get-FileHash -LiteralPath (Join-Path $repositoryRoot $name) -Algorithm SHA256).Hash) { throw "License mismatch: $name" }
        }
        $reader = [System.IO.StreamReader]::new($archive.GetEntry('tools/net10.0/any/DotnetToolSettings.xml').Open())
        try { [xml]$settings = $reader.ReadToEnd() } finally { $reader.Dispose() }
        $commands = @($settings.DotNetCliTool.Commands.Command)
        if ($commands.Count -ne 1 -or $commands[0].Name -ne 'literogue' -or $commands[0].EntryPoint -ne 'Icod.LiteRogue.dll') { throw 'Invalid tool command metadata.' }
        $reader = [System.IO.StreamReader]::new($archive.GetEntry('Icod.LiteRogue.nuspec').Open())
        try { [xml]$nuspec = $reader.ReadToEnd() } finally { $reader.Dispose() }
        if ($nuspec.package.metadata.license.type -ne 'file' -or $nuspec.package.metadata.license.InnerText -ne 'LICENSE' -or $nuspec.package.metadata.packageTypes.packageType.name -ne 'DotnetTool') { throw 'Package must declare the LICENSE file and DotnetTool type.' }
        if (0 -eq $archive.Entries.Count) {
            throw "Package '$($package.FullName)' is empty."
        }

        if (-not [string]::IsNullOrWhiteSpace($metadata.Readme)) {
            $readmeEntry = $archive.Entries |
                Where-Object { $_.FullName -eq $metadata.Readme } |
                Select-Object -First 1
            if ($null -eq $readmeEntry) {
                throw "Package '$($package.FullName)' declares missing readme '$($metadata.Readme)'."
            }
        }

        $toolSettings = @(
            $archive.Entries |
                Where-Object { $_.FullName.EndsWith('/DotnetToolSettings.xml', [System.StringComparison]::OrdinalIgnoreCase) }
        )
        if (1 -lt $toolSettings.Count) {
            throw "Package '$($package.FullName)' contains multiple DotnetToolSettings.xml files."
        }
    } finally {
        $archive.Dispose()
    }
}

if (-not [string]::IsNullOrWhiteSpace($GitHubOutputPath)) {
    "package_count=$($packages.Count)" >> $GitHubOutputPath
    "has_packages=$((0 -lt $packages.Count).ToString().ToLowerInvariant())" >> $GitHubOutputPath
}

Write-Host "Exact package verification completed successfully for $($packages.Count) package(s) ($Configuration)."
