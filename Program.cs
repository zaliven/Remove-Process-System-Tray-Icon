using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MainProgram
{
    class Program
    {
        [DllImport("shell32.dll")]
        static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA pnid);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowThreadProcessId(IntPtr hwnd, out int processId);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        struct NOTIFYICONDATA
        {
            public int cbSize;
            public IntPtr hwnd;
            public int uID;
            public int uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            public int dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
            public string szInfo;
            public int dwInfoFlags;
        }
        
        // The function collects the process's window handles, either by processName or pid.
        // If function doesn't receive pid, it searches the handles by processName.
        public static List<IntPtr> GetWindowsHandles(string processName, int pid = 0)
        {
            List<IntPtr> handleList = new List<IntPtr>();
            List<int> procIds = new List<int>();

            if(pid == 0)
            {
                Process[] processes = Process.GetProcessByName(processName);
                foreach(Process p in processes)
                {
                    procIds.Add(p.Id);
                }
            }
            else
            {
                procIds.Add(pid);
            }

            EnumWindows(delegate (IntPtr hwnd, IntPtr lParam)
            {
                GetWindowThreadProcessId(hwnd, out int processId);
                if(procIds.Contains(processId))
                {
                    handleList.Add(hwnd);
                }
            }, IntPtr.Zero);

            return handleList;
        }
    }

    public static void RemoveIcon(int pid = 0)
    {
        List<IntPtr> handles = GetWindowsHandles("PROCESS NAME", pid);
        foreach(var handle in handles)
        {
            NOTIFYICONDATA pnid = new NOTIFYICONDATA();
            pnid.cbSize = Marshal.SizeOf(typeof(NOTIFYICONDATA));
            pnid.uCallbackMessage = 0x800;
            pnid.uFlags = 0;
            pnid.hwnd = handle;
            pnid.szInfo = "";

            // The following is the ID of the app.
            // This may vary from app to app. To get the ID, you can use 
            // Sysinternals Process Explorer or PSTray.
            // ID usually stays constant.
            pnid.uID = 1; 

            Shell_NotifyIcon(0x00000002, ref pnid); // removes icon
            // int error = Marshal.GetLastWin32Error();
            // Console.WriteLine(error);
        }
    }

    static void Main(string[] args)
    {
        RemoveIcon();
    }
}
