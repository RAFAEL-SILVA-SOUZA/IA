using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Zeus.Helper
{
    public static class ConsoleExtensions
    {
        public static ProcessStartInfo start;
        public static Process process;

        public static string Escreve(this string texto)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(texto);
            Console.ForegroundColor = ConsoleColor.White;

            return texto;
        }

        public static string Falar(this string texto)
        {
            if (process != null)
                PararDeFalar();

            ExecuteCommand(
                $@"Add-Type -AssemblyName System.speech; 
                $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; 
                $speak.Speak(""{texto}"");");

            void ExecuteCommand(string command)
            {
                string path = Path.GetTempPath() + Guid.NewGuid() + ".ps1";

                // make sure to be using System.Text
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.Write(command);

                    start = new ProcessStartInfo()
                    {
                        FileName = @"C:\Windows\System32\windowspowershell\v1.0\powershell.exe",
                        LoadUserProfile = false,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = $"-executionpolicy bypass -File {path}",
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    process = Process.Start(start);
                }
            }

            return texto;
        }

        public static void PararDeFalar()
        {
            process.WaitForExit();
        }
    }
}
