using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MinecraftModManager.Classes
{
    class Logger : IDisposable
    {
        bool Disposed = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public void AddToLog(string log)
        {
            File.AppendAllText(Environment.CurrentDirectory + "log.txt", log);
            File.AppendAllText(Environment.CurrentDirectory + "log.txt", "------------------------" + DateTime.Now.ToString());
        }
        public void AddToLog(string[] log)
        {
            File.AppendAllLines(Environment.CurrentDirectory + "log.txt", log);
            File.AppendAllText(Environment.CurrentDirectory + "log.txt", "------------------------" + DateTime.Now.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposed)
                return;

            if (Disposing)
            {

                handle.Dispose();
            }
            Disposed = true;
        }

    }

    public static class StaticLogger
    {
        public static void AddToLog(string log)
        {
            File.AppendAllText(Environment.CurrentDirectory + "log.txt", log);
            File.AppendAllText(Environment.CurrentDirectory + "log.txt", "------------------------" + DateTime.Now.ToString());
        }
        public static void AddToLog(string[] log)
        {
            File.AppendAllLines(Environment.CurrentDirectory + "log.txt", log);
            File.AppendAllText(Environment.CurrentDirectory + "log.txt", "------------------------" + DateTime.Now.ToString());
        }
    }
}
