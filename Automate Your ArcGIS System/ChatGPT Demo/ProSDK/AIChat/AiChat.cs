using AiCore;
using System;

namespace AiChat
{
    internal static class Program
    {        
        [STAThread]
        static void Main()
        {
            Console.WriteLine("Hallo Esri Connect Deelnemers.");
            Console.WriteLine("Dit is een demo met een AI Chatbot.");
            Console.WriteLine(" ");
            Console.WriteLine("-----------------------------");            
            Console.WriteLine("Wat is de oudste stad van Nederland?");
            Console.WriteLine("-----------------------------");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Core.GetAiResponse("Wat is de oudste stad van Nederland"));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-----------------------------");
        }
    }
}
