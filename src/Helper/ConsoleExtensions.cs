using System;

namespace Zeus.Helper
{
    public static class ConsoleExtensions
    {
        public static void Escreve(this string texto)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(texto);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
