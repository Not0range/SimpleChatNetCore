using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static int port = 15465;
        static void Main(string[] args)
        {
            Server server = new Server(Console.WriteLine);
            server.Start();
            while(true)
            {
                string read = Console.ReadLine();
                switch(read)
                {
                    case "close":
                    case "stop":
                    case "exit":
                        server.Stop().ContinueWith((Task t) => Environment.Exit(0));
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                }
            }
        }
    }
}
