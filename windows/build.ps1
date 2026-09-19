param([string]$Configuration = "Release")
$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet publish "$Root\src\Compositor.Windows\Compositor.Windows.csproj" -c $Configuration -r win-x64 --self-contained true -o "$Root\publish"
$Iscc = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
if (-not (Test-Path $Iscc)) { throw "Inno Setup 6 is required." }
& $Iscc "$Root\installer\Compositor.iss"
