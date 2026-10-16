# Build first, then verify current production assemblies. No app startup or publish targets.
param([string]$Dotnet = '', [string]$Msbuild = '', [switch]$LegacyChecksOnly, [string]$WebsiteBin = '')

$script:taskSummary = [ordered]@{}
foreach ($taskPhase in @('WebApp build', 'Legacy build', 'Core net8.0', 'Core net462', 'Browser JavaScript', 'WebApp business', 'Legacy assemblies', 'Git diff check')) {
    $script:taskSummary[$taskPhase] = [pscustomobject]@{
        Phase = $taskPhase; Result = 'NOT RUN'; Passed = 0; Failed = 0; Skipped = 0
    }
}
$script:taskCurrentPhase = ''

function Start-TimePhase([string]$Name) {
    $script:taskCurrentPhase = $Name
    $script:taskSummary[$Name].Result = 'RUNNING'
}

function Read-TimeTestReport([string]$Name, [string]$Path) {
    [xml]$taskReport = Get-Content -LiteralPath $Path
    $taskCounters = $taskReport.TestRun.ResultSummary.Counters
    $taskRow = $script:taskSummary[$Name]
    $taskRow.Passed = [int]$taskCounters.passed
    $taskRow.Failed = [int]$taskCounters.failed
    $taskRow.Skipped = [int]$taskCounters.total - [int]$taskCounters.executed
    $taskRow.Result = if ([int]$taskCounters.total -gt 0 -and $taskRow.Passed -eq [int]$taskCounters.total) { 'PASS' } else { 'FAIL' }
    if ($taskRow.Result -ne 'PASS') { throw "Incomplete or failed test execution: $Name" }
}

function Show-TimeSummary {
    Write-Host "`n=== Time-zone verification summary ==="
    $script:taskSummary.Values | ForEach-Object {
        $taskIsBuildOrDiff = $_.Phase -in @('WebApp build', 'Legacy build', 'Git diff check')
        [pscustomobject]@{
            Phase = $_.Phase
            Result = $_.Result
            Passed = if ($taskIsBuildOrDiff) { '-' } else { $_.Passed }
            Failed = if ($taskIsBuildOrDiff) { '-' } else { $_.Failed }
            Skipped = if ($taskIsBuildOrDiff) { '-' } else { $_.Skipped }
        }
    } | Format-Table Phase, Result, Passed, Failed, Skipped -AutoSize | Out-String | Write-Host
    $taskRows = @($script:taskSummary.Values)
    $taskPassed = ($taskRows | Measure-Object Passed -Sum).Sum
    $taskFailed = ($taskRows | Measure-Object Failed -Sum).Sum
    $taskSkipped = ($taskRows | Measure-Object Skipped -Sum).Sum
    $taskOverall = if (@($taskRows | Where-Object Result -ne 'PASS').Count -eq 0) { 'PASS' } else { 'FAIL / INCOMPLETE' }
    Write-Host "Test/check executions: $taskPassed passed, $taskFailed failed, $taskSkipped skipped."
    Write-Host 'Builds and the Git diff check are listed separately and do not add to test counts.'
    Write-Host "Overall: $taskOverall"
}

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
$script:taskFailedChecks = 0
function Assert-TimeResult([string]$Name, $Expected, $Actual) {
    if ($Expected -ne $Actual) {
        $script:taskFailedChecks++
        throw "$Name expected '$Expected', got '$Actual'."
    }
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
    # A small machine-readable result lets the parent include child-process checks.
    Write-Output ('TIMEZONE_LEGACY_RESULT:' + (@{ Passed = $script:taskChecks; Failed = $script:taskFailedChecks } | ConvertTo-Json -Compress))
}


}

function Invoke-CoreTimeChecks {
    param([string]$Dotnet)
    $Frameworks = @('net8.0', 'net462')
$taskResults = Join-Path (Split-Path $PSScriptRoot -Parent) 'TestResults'
Push-Location $PSScriptRoot
try {
    foreach ($framework in $Frameworks) {
        Start-TimePhase "Core $framework"
        Write-Host "=== Core conversion and server freeze tests ($framework) ==="
        $taskResultName = "timezone-$framework.trx"
        $taskReportPath = Join-Path $taskResults $taskResultName
        if (Test-Path -LiteralPath $taskReportPath) { Remove-Item -LiteralPath $taskReportPath }
        & $Dotnet test TimeZone.Tests/TimeZone.Tests.csproj -f $framework --settings timezone.runsettings --logger 'console;verbosity=normal' --logger "trx;LogFileName=$taskResultName" --results-directory $taskResults
        $taskTestExit = $LASTEXITCODE
        if (Test-Path -LiteralPath $taskReportPath) { Read-TimeTestReport "Core $framework" $taskReportPath }
        if ($taskTestExit -ne 0 -or $script:taskSummary["Core $framework"].Result -ne 'PASS') { throw "Time zone tests failed for $framework." }
    }
    Start-TimePhase 'Browser JavaScript'
    Write-Host '=== Browser UTC pre-check, initial load and freeze response tests ==='
    $taskBrowserTotal = 0
    & node --test --test-reporter=tap (Join-Path $PSScriptRoot 'freeze-window.test.cjs') | ForEach-Object {
        Write-Host $_
        if ($_ -match '^# tests (\d+)$') { $taskBrowserTotal = [int]$Matches[1] }
        if ($_ -match '^# pass (\d+)$') { $script:taskSummary['Browser JavaScript'].Passed = [int]$Matches[1] }
        if ($_ -match '^# fail (\d+)$') { $script:taskSummary['Browser JavaScript'].Failed = [int]$Matches[1] }
        if ($_ -match '^# skipped (\d+)$') { $script:taskSummary['Browser JavaScript'].Skipped = [int]$Matches[1] }
    }
    if ($LASTEXITCODE -ne 0) { throw 'Corp Cal browser checks failed.' }
    if ($taskBrowserTotal -eq 0 -or $script:taskSummary['Browser JavaScript'].Passed -ne $taskBrowserTotal) { throw 'Incomplete Corp Cal browser checks.' }
    $script:taskSummary['Browser JavaScript'].Result = 'PASS'
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
    Start-TimePhase 'WebApp build'
    Write-Host '=== Build WebApp ==='
    & $Dotnet build Gcpe.Hub.WebApp/Gcpe.Hub.WebApp.csproj --configuration Debug --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw 'WebApp build failed.' }
    $script:taskSummary['WebApp build'].Result = 'PASS'
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
    Start-TimePhase 'Legacy build'
    Write-Host '=== Build Legacy website and dependencies ==='
    & $Msbuild @taskBuildArgs
    if ($LASTEXITCODE -ne 0) { throw 'Legacy build failed; assembly checks were not run.' }
    $script:taskSummary['Legacy build'].Result = 'PASS'
    Invoke-CoreTimeChecks -Dotnet $Dotnet
    Start-TimePhase 'WebApp business'
    Write-Host '=== Business tests: actual email rendering, EOD summary and SQL translation ==='
    $taskBusinessReport = Join-Path $taskRoot 'TestResults/timezone-business.trx'
    if (Test-Path -LiteralPath $taskBusinessReport) { Remove-Item -LiteralPath $taskBusinessReport }
    & $Dotnet test tests/BusinessTime.Tests/BusinessTime.Tests.csproj --configuration Debug --verbosity quiet `
        --logger 'console;verbosity=normal' --logger 'trx;LogFileName=timezone-business.trx' --results-directory (Join-Path $taskRoot 'TestResults')
    $taskTestExit = $LASTEXITCODE
    if (Test-Path -LiteralPath $taskBusinessReport) { Read-TimeTestReport 'WebApp business' $taskBusinessReport }
    if ($taskTestExit -ne 0 -or $script:taskSummary['WebApp business'].Result -ne 'PASS') { throw 'Offline business tests failed.' }
    Start-TimePhase 'Legacy assemblies'
    Write-Host '=== Legacy assembly checks: news, Carousel, templates and scheduling model ==='
    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $PSCommandPath -LegacyChecksOnly | ForEach-Object {
        if ($_ -like 'TIMEZONE_LEGACY_RESULT:*') {
            $taskChildResult = $_.Substring('TIMEZONE_LEGACY_RESULT:'.Length) | ConvertFrom-Json
            $script:taskSummary['Legacy assemblies'].Passed = [int]$taskChildResult.Passed
            $script:taskSummary['Legacy assemblies'].Failed = [int]$taskChildResult.Failed
        } else { Write-Host $_ }
    }
    if ($LASTEXITCODE -ne 0) { throw 'Legacy production-assembly checks failed.' }
    if ($script:taskSummary['Legacy assemblies'].Passed -eq 0) { throw 'No Legacy checks executed.' }
    $script:taskSummary['Legacy assemblies'].Result = 'PASS'
    Start-TimePhase 'Git diff check'
    Write-Host '=== Check diff whitespace ==='
    & git diff --check
    if ($LASTEXITCODE -ne 0) { throw 'git diff --check failed.' }
    $script:taskSummary['Git diff check'].Result = 'PASS'
    Write-Host 'All offline time-zone verification passed. Real HTTP, database and publishing workflows still require test-environment acceptance.'
} catch {
    if ($script:taskCurrentPhase) { $script:taskSummary[$script:taskCurrentPhase].Result = 'FAIL' }
    throw
} finally {
    Pop-Location
    Show-TimeSummary
}
