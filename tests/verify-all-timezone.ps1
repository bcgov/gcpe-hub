# Build first, then verify current production assemblies. No app startup or publish targets.
param([string]$Dotnet = '', [string]$Msbuild = '', [switch]$LegacyChecksOnly, [string]$WebsiteBin = '')

function Invoke-LegacyTimeChecks {
    param([string]$WebsiteBin = '')
$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSEdition -ne 'Desktop') {
    throw 'Legacy assembly checks require Windows PowerShell (.NET Framework).'
}
if (-not $WebsiteBin) {
    $WebsiteBin = Join-Path (Split-Path $PSScriptRoot -Parent) 'Hub.Legacy/Gcpe.Hub.Legacy.Website/bin'
}
foreach ($taskAssembly in @('Gcpe.Hub.Data_Legacy', 'Gcpe.News.ReleaseManagement.Templates',
    'Gcpe.News.ReleaseManagement.Legacy', 'Gcpe.Hub.Legacy.Website')) {
    $taskAssemblyPath = Join-Path $WebsiteBin "$taskAssembly.dll"
    if (-not (Test-Path -LiteralPath $taskAssemblyPath)) { throw "Build the Legacy website first: $taskAssemblyPath" }
    [void][Reflection.Assembly]::LoadFrom($taskAssemblyPath)
}

$script:taskChecks = 0
function Assert-TimeResult([string]$Name, $Expected, $Actual) {
    if ($Expected -ne $Actual) { throw "$Name expected '$Expected', got '$Actual'." }
    $script:taskChecks++
    Write-Host "PASS: $Name | expected: '$Expected' | actual: '$Actual'"
}

$taskOriginalCulture = [Threading.Thread]::CurrentThread.CurrentCulture
try {
    [Threading.Thread]::CurrentThread.CurrentCulture = [Globalization.CultureInfo]::InvariantCulture
    $taskOldSchedule = [DateTimeOffset]'2026-11-02T09:00:00-08:00'
    Assert-TimeResult 'News page displays BC hour for a stored old offset' $true (
        [Gcpe.Hub.News.Page]::FormatDateTime($taskOldSchedule).EndsWith(' at 10:00 am'))
    Assert-TimeResult 'Release list displays BC hour for a stored old offset' $true (
        [Gcpe.Hub.News.ReleaseManagement.ReleaseModel]::FormatDateTime($taskOldSchedule).EndsWith(' at 10:00 am'))
    Assert-TimeResult 'Historical winter display stays at UTC-8' $true (
        [Gcpe.Hub.News.Page]::FormatDateTime([DateTimeOffset]'2025-12-01T17:00:00Z').EndsWith(' at 9:00 am'))
    $taskBcNoon = [Gcpe.Time.BcTime]::FromLocal([Gcpe.Time.BcTime]::Today.AddHours(12))
    Assert-TimeResult 'Relative date uses BC today for a UTC input' 'Today at 12:00 pm' (
        [Gcpe.Hub.News.ReleaseManagement.ReleaseModel]::FormatDateTime($taskBcNoon.ToUniversalTime()))
    Assert-TimeResult 'Release date keeps late BC wall time after the switch' $true (
        [Gcpe.Hub.News.Page]::FormatBcWallTime([DateTime]'2026-11-12T23:30:00').EndsWith(' at 11:30 pm'))
    $taskGapTime = [DateTime]'2026-03-08T02:30:00'
    Assert-TimeResult 'Nonexistent BC release date falls back instead of throwing' (
        [Gcpe.Hub.News.Page]::FormatDateTime([DateTimeOffset]$taskGapTime)) (
        [Gcpe.Hub.News.Page]::FormatBcWallTime($taskGapTime))

    # Plain entities and an uninitialised page getter; no request, context or database.
    $taskCarousel = New-Object Gcpe.Hub.Data.Entity.Carousel
    $taskCarousel.PublishDateTime = $taskOldSchedule
    $taskPage = New-Object Gcpe.Hub.News.Carousel
    $taskFlags = [Reflection.BindingFlags]'NonPublic,Instance'
    $taskPage.GetType().GetField('carousel', $taskFlags).SetValue($taskPage, $taskCarousel)
    $taskDisplayed = $taskPage.GetType().GetProperty('CarouselPublishDateTime', $taskFlags).GetValue($taskPage, $null)
    Assert-TimeResult 'Carousel input displays the converted BC hour' 10 $taskDisplayed.Hour
    $taskInputText = $taskDisplayed.ToString('yyyy-MM-dd hh:mm tt', [Globalization.CultureInfo]::InvariantCulture)
    $taskParsedInput = [DateTime]::ParseExact($taskInputText, 'yyyy-MM-dd hh:mm tt', [Globalization.CultureInfo]::InvariantCulture)
    Assert-TimeResult 'Carousel unchanged input preserves stored instant' $taskOldSchedule.UtcDateTime (
        [Gcpe.Time.BcTime]::FromLocal($taskParsedInput).UtcDateTime)

    $taskRelease = New-Object Gcpe.Hub.Data.Entity.NewsRelease
    $taskLanguage = New-Object Gcpe.Hub.Data.Entity.Language
    $taskLanguage.Id = 4105
    $taskLanguage.Name = 'English'
    $taskReleaseLanguage = New-Object Gcpe.Hub.Data.Entity.NewsReleaseLanguage
    $taskReleaseLanguage.Language = $taskLanguage
    $taskReleaseLanguage.Location = 'Victoria'
    [void]$taskRelease.Languages.Add($taskReleaseLanguage)
    $taskRelease.PublishDateTime = $taskOldSchedule
    Assert-TimeResult 'Draft template converts stored old-offset schedule' ([DateTime]'2026-11-02T10:00:00') (
        [Gcpe.News.ReleaseManagement.Templates.Release]::FromEntity($taskRelease).ReleaseDateTime)
    $taskRelease.PublishDateTime = [DateTimeOffset]'2026-11-02T00:30:00Z'
    Assert-TimeResult 'Draft template uses BC calendar date at UTC midnight' ([DateTime]'2026-11-01T17:30:00') (
        [Gcpe.News.ReleaseManagement.Templates.Release]::FromEntity($taskRelease).ReleaseDateTime)
    $taskRelease.ReleaseDateTime = [DateTime]'2025-12-01T09:00:00'
    Assert-TimeResult 'Published template keeps stored BC release wall time' ([DateTime]'2025-12-01T09:00:00') (
        [Gcpe.News.ReleaseManagement.Templates.Release]::FromEntity($taskRelease).ReleaseDateTime)
    $taskRelease.ReleaseDateTime = $null
    $taskRelease.PublishDateTime = $null
    Assert-TimeResult 'Unscheduled draft template uses BC today' ([Gcpe.Time.BcTime]::Today) (
        [Gcpe.News.ReleaseManagement.Templates.Release]::FromEntity($taskRelease).ReleaseDateTime)

    # Exercise the real News model boundary without its DB-loading constructor.
    # This checks input conversion/model/formatter, NOT the Web Forms save handler or DB write.
    $taskModel = [Runtime.Serialization.FormatterServices]::GetUninitializedObject(
        [Gcpe.Hub.News.ReleaseManagement.ReleaseModel])
    [Gcpe.Hub.News.ReleaseManagement.ReleaseManagementModel].GetField('Release', $taskFlags).SetValue($taskModel, $taskRelease)
    $taskRelease.ReleaseDateTime = [DateTime]'2025-12-01T09:00:00'
    foreach ($taskCase in @(
        @('2026-11-02 09:00', '2026-11-02T16:00:00Z', '9:00 am'),
        @('2026-11-02 10:00', '2026-11-02T17:00:00Z', '10:00 am'),
        @('2025-12-01 09:00', '2025-12-01T17:00:00Z', '9:00 am')
    )) {
        $taskModel.PublishDateTime = [Gcpe.Time.BcTime]::FromLocal([DateTime]::ParseExact(
            $taskCase[0], 'yyyy-MM-dd HH:mm', [Globalization.CultureInfo]::InvariantCulture))
        Assert-TimeResult "News model stores the expected instant for $($taskCase[0])" (
            ([DateTimeOffset]$taskCase[1]).UtcDateTime) $taskRelease.PublishDateTime.UtcDateTime
        Assert-TimeResult "News model formats the entered BC hour for $($taskCase[0])" $true (
            [Gcpe.Hub.News.ReleaseManagement.ReleaseModel]::FormatDateTime($taskModel.PublishDateTime).EndsWith($taskCase[2]))
    }
    Assert-TimeResult 'Editing a schedule preserves historical release wall time' ([DateTime]'2025-12-01T09:00:00') $taskRelease.ReleaseDateTime

    Write-Host "Legacy time-zone checks passed: $taskChecks"
} finally {
    [Threading.Thread]::CurrentThread.CurrentCulture = $taskOriginalCulture
}


}

function Invoke-CoreTimeChecks {
    param([string]$Dotnet)
    $Frameworks = @('net8.0', 'net462')
$taskResults = Join-Path (Split-Path $PSScriptRoot -Parent) 'TestResults'
Push-Location $PSScriptRoot
try {
    foreach ($framework in $Frameworks) {
        Write-Host "=== Core conversion and server freeze tests ($framework) ==="
        $taskResultName = "timezone-$framework.trx"
        & $Dotnet test TimeZone.Tests/TimeZone.Tests.csproj -f $framework --settings timezone.runsettings --logger 'console;verbosity=normal' --logger "trx;LogFileName=$taskResultName" --results-directory $taskResults
        if ($LASTEXITCODE -ne 0) { throw "Time zone tests failed for $framework." }
        [xml]$taskReport = Get-Content -LiteralPath (Join-Path $taskResults $taskResultName)
        $taskCounters = $taskReport.TestRun.ResultSummary.Counters
        if ([int]$taskCounters.total -eq 0 -or [int]$taskCounters.failed -gt 0 -or [int]$taskCounters.executed -ne [int]$taskCounters.total) {
            throw "Incomplete or failed test execution for $framework."
        }
    }
    Write-Host '=== Browser UTC pre-check, initial load and freeze response tests ==='
    & node --test (Join-Path $PSScriptRoot 'freeze-window.test.cjs')
    if ($LASTEXITCODE -ne 0) { throw 'Corp Cal browser checks failed.' }
} finally { Pop-Location }


}

# Internal child-process mode: load Framework assemblies in Windows PowerShell.
# Users run this file without this switch to execute the complete suite.
if ($LegacyChecksOnly) {
    Invoke-LegacyTimeChecks -WebsiteBin $WebsiteBin
    return
}
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$taskTools = Join-Path ([Environment]::GetFolderPath('UserProfile')) '.codex/toolchains'
if (-not $Dotnet) {
    $taskCandidate = Join-Path $taskTools 'dotnet5/dotnet.exe'
    $Dotnet = if (Test-Path -LiteralPath $taskCandidate) { $taskCandidate } else { 'dotnet' }
}
$taskBundledBuild = $false
if (-not $Msbuild) {
    $taskCandidate = Join-Path $taskTools 'msbuild/Microsoft.Build.Runtime.17.11.4/contentFiles/any/net472/MSBuild.exe'
    if (Test-Path -LiteralPath $taskCandidate) {
        $Msbuild = $taskCandidate
        $taskBundledBuild = $true
    } else {
        $Msbuild = (Get-Command MSBuild.exe -ErrorAction Stop).Source
    }
}
Push-Location $taskRoot
try {
    Write-Host '=== Build WebApp ==='
    & $Dotnet build Gcpe.Hub.WebApp/Gcpe.Hub.WebApp.csproj --configuration Debug --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw 'WebApp build failed.' }
    $taskBuildArgs = @('Hub.Legacy/Gcpe.Hub.Legacy.Website/Gcpe.Hub.Legacy.Website.csproj',
        '/t:Build', '/p:Configuration=Debug', '/v:quiet', '/nologo')
    if ($taskBundledBuild) {
        $taskOpenXml = Get-ChildItem (Join-Path $taskTools 'openxml') -Recurse -Filter DocumentFormat.OpenXml.dll |
            Select-Object -First 1 -ExpandProperty DirectoryName
        if (-not $taskOpenXml) { throw 'OpenXML build reference missing.' }
        $taskBuildArgs += @(
            "/p:RoslynTargetsPath=$taskTools/compilers/Microsoft.Net.Compilers.Toolset.4.11.0/tasks/net472",
            "/p:TargetFrameworkRootPath=$taskTools/frameworkrefs/", '/p:VisualStudioVersion=14.0',
            "/p:VSToolsPath=$taskTools/webtargets/MSBuild.Microsoft.VisualStudio.Web.targets.14.0.0.3/tools/VSToolsPath",
            "/p:ReferencePath=$taskOpenXml")
    }
    Write-Host '=== Build Legacy website and dependencies ==='
    & $Msbuild @taskBuildArgs
    if ($LASTEXITCODE -ne 0) { throw 'Legacy build failed; assembly checks were not run.' }
    Invoke-CoreTimeChecks -Dotnet $Dotnet
    Write-Host '=== Business tests: actual email rendering, EOD summary and SQL translation ==='
    & $Dotnet test tests/BusinessTime.Tests/BusinessTime.Tests.csproj --configuration Debug --verbosity quiet `
        --logger 'console;verbosity=normal' --logger 'trx;LogFileName=timezone-business.trx' --results-directory (Join-Path $taskRoot 'TestResults')
    if ($LASTEXITCODE -ne 0) { throw 'Offline business tests failed.' }
    [xml]$taskReport = Get-Content (Join-Path $taskRoot 'TestResults/timezone-business.trx')
    $taskCounters = $taskReport.TestRun.ResultSummary.Counters
    if ([int]$taskCounters.total -eq 0 -or [int]$taskCounters.failed -gt 0 -or
        [int]$taskCounters.executed -ne [int]$taskCounters.total) { throw 'Incomplete business tests.' }
    Write-Host '=== Legacy assembly checks: news, Carousel, templates and scheduling model ==='
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $PSCommandPath -LegacyChecksOnly
    if ($LASTEXITCODE -ne 0) { throw 'Legacy production-assembly checks failed.' }
    Write-Host '=== Check diff whitespace ==='
    & git diff --check
    if ($LASTEXITCODE -ne 0) { throw 'git diff --check failed.' }
    Write-Host 'All offline time-zone verification passed. Real HTTP, database and publishing workflows still require test-environment acceptance.'
} finally { Pop-Location }
