# C# Remove Process System Tray Icon
The code utilizes WinApi to remove a system tray icon of a process, using either the process name or pid of the process.

WinApi functions used (user32.dll): 
Shell_NotifyIcon - sends a message to the window handle of the process to remove the icon.
GetWindowThreadProcessId - gets process id using window handle.
EnumWindows - enumerates window handles.

