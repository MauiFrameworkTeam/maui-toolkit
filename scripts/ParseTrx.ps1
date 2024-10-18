# This script processes test result files (.xml and .trx) and generates HTML reports.
# It extracts test outcomes (passed, failed, skipped) and formats them in a human-readable HTML format.
# The reports are saved in the specified output directory with a summary and detailed breakdown of each test case.

# Define the input and output directories
param (
    [string]$inputDirectory = "..\maui\tests\Syncfusion.Maui.Toolkit.UnitTest\cireports\testresults",
    [string]$outputDirectory = ""
)

if (-not $outputDirectory) {
    $outputDirectory = "$inputDirectory/"
}

# Create the output directory if it does not exist
if (-not (Test-Path -Path $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory
}

# Get all .xml and .trx files in the input directory
$allFiles = Get-ChildItem -Path $inputDirectory | Where-Object { $_.Extension -eq ".xml" -or $_.Extension -eq ".trx" }

if ($allFiles.Count -eq 0) {
    Write-Host "No .trx or .xml files found in the directory."
} else {
    foreach ($file in $allFiles) {
        try {
            # Determine the file format
            $fileFormat = if ($file.Extension -eq ".trx") { "TRX" } elseif ($file.Extension -eq ".xml") { "XML" } else { "Unknown" }
            $filePath = $file.FullName
            $baseName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name) -replace "\.UnitTests-Debug", ""
            $baseName = $baseName.ToLower()
            $htmlFilePath = Join-Path -Path $outputDirectory -ChildPath "$baseName.html"

            Write-Host "Processing file: $filePath"


            if ($file.Extension -eq ".trx") {
                # Load the TRX file
                [xml]$trx = Get-Content $filePath
                # Extract information
                $testResults = $trx.TestRun.Results.UnitTestResult
                $testClassName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name) -replace "-DEBUG", ""

                # Initialize counts
                $totalTests = 0
                $passedTests = 0
                $failedTests = 0
                $skippedTests = 0
                $notExecutedTests = 0

                # Process test results
                foreach ($result in $testResults) {
                    $totalTests++
                    switch ($result.Outcome) {
                        "Passed" { $passedTests++ }
                        "Failed" { $failedTests++ }
                        "NotExecuted" { $notExecutedTests++ }
                    }
                }
            }

            elseif ($file.Extension -eq ".xml") {
                # Load the XML file
                [xml]$xml = Get-Content $filePath
                # Extract information
                $testResults = $xml.assemblies.assembly.collection.test
                $assembly = $xml.assemblies.assembly
                $testClassName = [string]$assembly.name -replace '\.dll$', ''

                $totalTests = [int]$assembly.total
                $passedTests = [int]$assembly.passed
                $failedTests = [int]$assembly.failed
                $skippedTests = [int]$assembly.skipped
                $notExecutedTests = 0
            }

            # Initialize HTML content
            $htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8"/>
    <title>Test Report</title>
<style type="text/css">
    html, body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size: 1rem; margin: 0; padding: 0; color: #333; }
    body { padding: 2rem 1rem; font-size: 0.9rem; }
    #html-content { margin: 0 auto; max-width: 70rem; }
    header { display: flex; align-items: center; }
    #title { margin: 0; flex-grow: 1; font-size: 1.6rem; font-weight: 600; color: #2a2a2a; }
    #timestamp, #file-format { display: inline-block; margin-right: 20px; }
    #filter-buttons { margin: 1.5rem 0; }
    .filter-button { 
        padding: 0.6rem 1.2rem; 
        margin-right: 0.6rem; 
        border: 1px solid #ccc; 
        border-radius: 0.3rem; 
        background-color: #f9f9f9; 
        cursor: pointer;
        font-size: 0.85rem;
    }
    .filter-button.active { 
        background-color: #e0e0e0; 
        border-color: #aaa; 
        font-weight: 700;
        border-width: 2px;
    }
    #summary { color: #444; margin: 0.5rem 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; font-size: 1.1rem; }
    #summary > div { margin-right: 1.5rem; background: #f7f7f7; padding: 1rem; min-width: 16rem; border: 1px solid #ddd; border-radius: 0.3rem; }
    #summary > div:last-child { margin-right: 0; }
    @media only screen and (max-width: 720px) {
        #summary { flex-direction: column; }
        #summary > div { margin-right: 0; margin-top: 2.5rem; }
        #summary > div:first-child { margin-top: 0; }
    }
    .summary-total { font-weight: 600; margin-bottom: 0.5rem; }
    .summary-passed { color: #3c763d; border-left: 0.5rem solid #3c763d; padding-left: 0.6rem; }
    .summary-failed { color: #a94442; border-left: 0.5rem solid #a94442; padding-left: 0.6rem; }
    .summary-skipped { color: #8a6d3b; border-left: 0.5rem solid #8a6d3b; padding-left: 0.6rem; }
    .summary-notexecuted { color: #666; border-left: 0.5rem solid #666; padding-left: 0.6rem; }
    .test-result { padding: 1.2rem; margin-bottom: 0.3rem; border: 1px solid #ddd; border-radius: 0.3rem; }
    .test-result:last-child { border: 0; }
    .test-result.passed, #passed-button { background-color: #d4edda; color: #3c763d; }
    .test-result.failed, #failed-button { background-color: #f2dede; color: #a94442; }
    .test-result.notexecuted, #notexecuted-button { background-color: #f9f9f9; color: #666; }
    .test-result.skipped, #skipped-button { background-color: #d9e1e8; color: #333; }
    .test-info { display: flex; justify-content: space-between; flex-wrap: wrap; }
    .test-info-header { display: flex; justify-content: space-between; padding: 1rem; background-color: #e0e0e0; border-bottom: 2px solid #ddd; font-weight: 600; }
    .test-method-name, .message, .stack-trace { width: 78%; text-align: left; font-weight: 600; word-break: break-word; }
    .test-class-name { width: 78%; font-size: 0.8rem;color: #777; }
    .test-status { width: 7%; text-align: left;}
    .test-duration { width: 10%; text-align: left; font-size: 0.8rem; }
    .test-class-container { margin-bottom: 2.5rem; }
    .test-class-info { padding: 1.2rem; background-color: #f7f7f7; color: #777; display: flex; align-items: center; margin-bottom: 0.3rem; border: 1px solid #ddd; border-radius: 0.3rem; }
    .test-class-info .class-name { word-break: break-word; flex-grow: 1; font-size: 1.1rem; }
    .no-cases { display: none; padding: 1rem; background-color: #f9d6d5; color: #a94442; border: 1px solid #ddd; border-radius: 0.3rem; }
</style>
    <script type="text/javascript">
        function filterResults(status) {
            var results = document.getElementsByClassName('test-result');
            var noCasesMessage = document.getElementById('no-cases-message');
            var hasResults = false;

            for (var i = 0; i < results.length; i++) {
                if (status === 'ALL' || results[i].classList.contains(status)) {
                    results[i].style.display = 'block';
                    hasResults = true;
                } else {
                    results[i].style.display = 'none';
                }
            }

            if (!hasResults) {
                noCasesMessage.style.display = 'block';
            } else {
                noCasesMessage.style.display = 'none';
            }

            var buttons = document.getElementsByClassName('filter-button');
            for (var j = 0; j < buttons.length; j++) {
                buttons[j].classList.remove('active');
            }
            document.getElementById(status + '-button').classList.add('active');
        }
    </script>
</head>
<body>
<div id="html-content">
    <header>
        <h1 id="title">Test Results for $testClassName</h1>
    </header>
    <div id="metadata-container">
        <div id="timestamp">Generated: $(Get-Date)</div>
        <div id="file-format">Format from: $fileFormat</div>
        <div id="summary">
            <div id="test-summary">
                <div class="summary-total">Total Test Cases: $totalTests</div>
                <div class="summary-passed">Passed: $passedTests</div>
                <div class="summary-failed">Failed: $failedTests</div>
                <div class="summary-skipped">Skipped: $skippedTests</div>
                <div class="summary-notexecuted">Not Executed: $notExecutedTests</div>
            </div>
        </div>
        <div id="filter-buttons">
            <span id="ALL-button" class="filter-button active" onclick="filterResults('ALL')">All</span>
            <span id="passed-button" class="filter-button" onclick="filterResults('passed')">Passed</span>
            <span id="failed-button" class="filter-button" onclick="filterResults('failed')">Failed</span>
            <span id="skipped-button" class="filter-button" onclick="filterResults('skipped')">Skipped</span>
            <span id="notexecuted-button" class="filter-button" onclick="filterResults('notexecuted')">Not Executed</span>
        </div>
    </div>
    <div class="test-info-header">
        <div class="test-method-name">Test Method Name</div>
        <div class="test-status">Status</div>
        <div class="test-duration">Duration</div>
    </div>
    <div id="no-cases-message" class="no-cases">No test cases found for this filter.</div>
"@

if ($file.Extension -eq ".trx") {

            # Append each test result to HTML content
            foreach ($result in $testResults) {
                $testName = $result.TestName
                $outcome = $result.Outcome
                $duration = $result.Duration
                $message = if ($result.Output -and $result.Output.ErrorInfo -and $result.Output.ErrorInfo.Message) { 
                    $result.Output.ErrorInfo.Message -replace "\r\n", "<br>" -replace "\n", "<br>"
                } else { 
                    "N/A" 
                }
                $stackTrace = if ($result.Output -and $result.Output.ErrorInfo -and $result.Output.ErrorInfo.StackTrace) { 
                    $result.Output.ErrorInfo.StackTrace -replace "\r\n", "<br>" -replace "\n", "<br>"
                } else { 
                    "N/A" 
                }

                $outcomeClass = switch ($outcome) {
                    "Passed" { "passed" }
                    "Failed" { "failed" }
                    "NotExecuted" { "notexecuted" }
                }

                $htmlContent += @"
    <div class="test-result $outcomeClass">
        <div class="test-info">
            <div class="test-method-name">$testName</div>
            <div class="test-status">$outcome</div>
            <div class="test-duration">$duration</div>
        </div>
        <div class="message">Message: $message</div>
        <div class="stack-trace">Stack Trace: $stackTrace</div>
    </div>
"@
            }
}
elseif ($file.Extension -eq ".xml") {

    # Append each test result to HTML content
            foreach ($result in $testResults) {
                $testName = $result.name
                $classname = $result.type
                $outcome = $result.result
                $duration = $result.time

                $outcomeClass = switch ($outcome) {
                    "Pass" { "passed" }
                    "Fail" { "failed" }
                    "Skip" { "skipped" }
                }

                $htmlContent += @"
    <div class="test-result $outcomeClass">
        <div class="test-info">
            <div class="test-class-name">$classname :</div>
            <div class="test-method-name">$testName</div>
            <div class="test-status">$outcome</div>
            <div class="test-duration">$duration</div>
        </div>
    </div>
"@
            }
}


            # Close HTML tags
            $htmlContent += @"
</div>
</body>
</html>
"@

            # Save the HTML content to a file
            $htmlContent | Out-File -FilePath $htmlFilePath -Encoding UTF8

            Write-Host "Test results have been saved to $htmlFilePath"
        } catch {
            Write-Host "Error processing file: $filePath"
            Write-Host $_.Exception.Message
        }
    }
    Write-Host "All .xml and .trx files have been processed."
}
