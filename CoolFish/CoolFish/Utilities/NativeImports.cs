using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace CoolFishNS.Utilities
{
    public class NativeImports
    {

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        /// A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.
        /// <para>
        /// Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        const UInt32 SW_HIDE = 0;
        const UInt32 SW_SHOWNORMAL = 1;
        const UInt32 SW_NORMAL = 1;
        const UInt32 SW_SHOWMINIMIZED = 2;
        const UInt32 SW_SHOWMAXIMIZED = 3;
        const UInt32 SW_MAXIMIZE = 3;
        const UInt32 SW_SHOWNOACTIVATE = 4;
        const UInt32 SW_SHOW = 5;
        const UInt32 SW_MINIMIZE = 6;
        const UInt32 SW_SHOWMINNOACTIVE = 7;
        const UInt32 SW_SHOWNA = 8;
        const UInt32 SW_RESTORE = 9;

        private struct WINDOWPLACEMENT
        {
            public int length;
            public uint flags;
            public uint showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);


        public static bool isWindowMinimized(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            var result = GetWindowPlacement(hWnd, ref placement);
            if (result)
            {
                return placement.showCmd == SW_SHOWMINIMIZED;
            }
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not get window placement");

        }

        public static void HideWindow(IntPtr hWnd)
        {
            if (!ShowWindowAsync(hWnd, (int)SW_HIDE))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to hide window");
            }
        }

        public static void ShowWindow(IntPtr hWnd)
        {
            if (!ShowWindowAsync(hWnd, (int) SW_SHOWNORMAL))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to show window");
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, string windowClass, string windowTitle);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr window, out int process);

        public static IntPtr[] GetProcessWindows(int processId)
        {
            IntPtr[] apRet = (new IntPtr[256]);
            int iCount = 0;
            IntPtr pLast = IntPtr.Zero;
            do
            {
                pLast = FindWindowEx(IntPtr.Zero, pLast, null, null);
                int iProcess;
                GetWindowThreadProcessId(pLast, out iProcess);
                if (iProcess == processId)
                {
                    apRet[iCount++] = pLast;
                }
            } while (pLast != IntPtr.Zero);
            Array.Resize(ref apRet, iCount);
            return apRet;
        }
    }
}
