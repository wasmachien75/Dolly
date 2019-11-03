echo "Getting rcedit.exe..."
Invoke-WebRequest https://github.com/electron/rcedit/releases/download/v1.1.1/rcedit-x86.exe -OutFile rcedit.exe
.\rcedit.exe .\bin\Release\net451\Dolly.exe --set-icon .\dolly.ico
ECHO "OK...cleanup"
rm rcedit.exe