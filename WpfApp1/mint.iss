; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Minty"
#define MyAppVersion "1.0"
#define MyAppPublisher "KW Team"
#define MyAppExeName "Minty.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{A478A9A5-EF0C-4C65-8C41-5A1EA9D530DA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=C:\Users\Blair\Desktop\Mintlauncherinstaller
OutputBaseFilename=Minty
SetupIconFile=C:\Users\Blair\source\repos\Mintlauncher\WpfApp1\virus.ico
Compression=lzma
SolidCompression=yes
DisableWelcomePage=False

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "C:\Users\Blair\source\repos\Mintlauncher\WpfApp1\bin\Debug\net6.0-windows10.0.17763.0\Minty.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Blair\source\repos\Mintlauncher\WpfApp1\bin\Debug\net6.0-windows10.0.17763.0\Minty.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Blair\source\repos\Mintlauncher\WpfApp1\bin\Debug\net6.0-windows10.0.17763.0\Minty.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent