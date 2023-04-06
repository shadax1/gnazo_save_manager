using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gnazo_app
{
    class ProcessMemory
    {
        #region process & address
        //function imports
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        //access values
        const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        //process handle
        static Process p = Process.GetProcessesByName("gnazo")[0];
        IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, p.Id);

        //amount of bytes written/read
        private int bytesWritten = 0;
        private int bytesRead = 0;
        #endregion

        public ProcessMemory()
        {
            new Thread(ProcessRun) { IsBackground = true }.Start(); //starts thread with method ProcessRun
        }

        void ProcessRun() //checks if gnazo is running
        {
            while (true)
            {
                if (p.HasExited)
                    Environment.Exit(1);
                Thread.Sleep(50);
            }
        }

        public void WriteGnazo(int address, byte[] buffer)
        {
            WriteProcessMemory((int)processHandle, (int)p.Modules[0].BaseAddress + address, buffer, buffer.Length, ref bytesWritten);
        }

        public byte[] ReadGnazo(int address, byte[] buffer)
        {
            ReadProcessMemory((int)processHandle, (int)p.Modules[0].BaseAddress + address, buffer, buffer.Length, ref bytesRead);
            return buffer;
        }
    }
}
