# BC time zone regression tests

Run on Windows with the .NET 8 SDK, Node.js 20+, and the existing Legacy build prerequisites, from the tests folder:

```powershell
cd tests
.\verify-all-timezone.ps1
```

The script builds WebApp and Legacy, then runs all offline time-zone checks. It prints each test case with its inputs and result, and stops if a build fails, any test fails or is skipped, or no tests execute. Legacy checks also print the expected and actual values.

The final console summary lists each build/check phase, passed/failed/skipped test
counts and the overall result. If a phase fails, later phases show `NOT RUN`.
Build and setup errors are shown as phase failures; they are not counted as failed
test assertions. You do not need to open a report to see whether the run passed.

## Why the core tests run twice

| Run | Covers | Tests |
| --- | --- | --- |
| net8.0 | Modern .NET WebApp | 50 core tests, plus 14 WebApp business tests |
| net462 | .NET Framework Legacy | The same 50 core tests |

The runtimes implement `TimeZoneInfo` differently, so the shared code is verified on both. The WebApp business tests reference the .NET 8 application and run only on net8.0. The script also runs 4 browser-script tests and 19 checks against the freshly built Legacy assemblies.

## What is tested

| File | Covers |
| --- | --- |
| `TimeZone.Tests/BcTimeTests.cs` | `Shared/BcTime.cs`: conversion by date, historical and permanent offsets, fallback rules, missing or corrupt zones, BC midnight, schedule roundtrip and publication dates |
| `TimeZone.Tests/FreezeWindowTests.cs` | Server freeze rules: strictly after 16:05 and before 17:00 BC, headquarters exemption and historical winter time |
| `freeze-window.test.cjs` | Calendar browser pre-check for 16:00–17:00 BC at UTC-7 (23:00–24:00 UTC), initial page load, and handling of the server's freeze response |
| `BusinessTime.Tests/BusinessTimeTests.cs` | Actual Media Request/EOD email content, overdue and ASAP handling, Ministry EOD summary filtering around BC midnight, and SQL Server query translation |
| `verify-all-timezone.ps1` | Legacy assembly checks: News formatting, Carousel display/roundtrip, template dates and News model scheduling; also builds both applications and runs `git diff --check` |

Core tests compile the production sources, not a copy. Most use simulated time zones. `ProductionSystemResolverPreservesHistoricalAndFutureOffsets` uses this machine's real time zones and passes on either the normal or fallback path.

Business tests call the production code with in-memory data. EOD result tests use EF InMemory; the SQL translation check does not execute SQL. Browser checks run in Node with simulated controls and AJAX. Legacy checks use in-memory entities. No database connection is opened and no email is sent.

## Reports

TRX reports are saved in the repository root under `TestResults/` (ignored by Git):

- `timezone-net8.0.trx`
- `timezone-net462.trx`
- `timezone-business.trx`

Open them in Visual Studio to see each test, its inputs and, for failures, the expected and actual values. Each run overwrites the reports. Browser and Legacy results are printed to the console.

These local tests do not verify production OS patches, database records or Office/browser time-zone settings. Full page saves, real login/permissions, emergency pinning, publishing and EOD delivery still need a test environment. The script does not deploy applications.
