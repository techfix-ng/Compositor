#define MyAppName "Compositor"
#define MyAppVersion "0.1.0"
[Setup]
AppId={{3C4D599C-ABBA-47C4-B76E-64C42B956557}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\Compositor
DefaultGroupName=Compositor
OutputDir=..\artifacts
OutputBaseFilename=Compositor-Windows-v{#MyAppVersion}-Setup-x64
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\Compositor.exe
[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
[Icons]
Name: "{autoprograms}\Compositor"; Filename: "{app}\Compositor.exe"
Name: "{autodesktop}\Compositor"; Filename: "{app}\Compositor.exe"; Tasks: desktopicon
[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"
[Run]
Filename: "{app}\Compositor.exe"; Description: "Launch Compositor"; Flags: nowait postinstall skipifsilent
