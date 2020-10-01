using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Client
{
    class Program
    {
        public static int port = 15465;

        public static string ipAdress = "127.0.0.1";

        private static Client client;

        private static bool typingMessage = false;

        private static int indexMessage = 0;

        private static StringBuilder currentMessage;

        private static string[] users;

        private static int selectedUser = 0;

        private static bool busy = false;

        Dictionary<string, List<string>> chats = new Dictionary<string, List<string>>();

        static void Main(string[] args)
        {
            Console.ResetColor();
            Console.WindowWidth = 85;
            Console.WindowHeight = 30;
            Console.BufferWidth = 85;
            Console.BufferHeight = 30;

            Console.Write("Введите имя пользователя: ");
            string username;
            while (true)
            {
                username = Console.ReadLine();
                if (username.Length < 10)
                {
                    int i = 0;
                    for (; i < username.Length; i++)
                        if (!char.IsLetterOrDigit(username[i]))
                            break;

                    if (i == username.Length)
                        break;
                }

                Console.Write("Введите допустимое имя пользователя");
                Console.CursorTop--;
                Console.CursorLeft = 26;
                Console.Write(new string(' ', username.Length));
                Console.CursorLeft = 26;
            }

            Console.Title = "В системе как " + username;
            Console.Clear();
            Console.CursorVisible = false;

            client = new Client(username);
            client.Connect();

            Console.CancelKeyPress += (s, e) => client.SendDisconnect();
            BuildFrame();
            Console.CursorTop = 1;
            Console.CursorLeft = 1;
            Client_UsersRefresh(client.SendRequest());
            client.UsersRefresh += Client_UsersRefresh;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                while (busy) ;
                busy = true;
                switch(key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (!typingMessage)
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(users[selectedUser].PadRight(15));

                            if (--selectedUser < 0)
                                selectedUser = users.Length - 1;

                            Console.CursorTop = 1 + selectedUser;
                            Console.CursorLeft = 1;
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(users[selectedUser].PadRight(15));
                            Console.CursorLeft = 1;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (!typingMessage)
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(users[selectedUser].PadRight(15));

                            if (++selectedUser >= users.Length)
                                selectedUser = 0;

                            Console.CursorTop = 1 + selectedUser;
                            Console.CursorLeft = 1;
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(users[selectedUser].PadRight(15));
                            Console.CursorLeft = 1;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (--indexMessage < 0)
                        {
                            indexMessage = 0;
                            Console.CursorLeft = 17;
                        }
                        else
                            Console.CursorLeft--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (++indexMessage > currentMessage.Length)
                        {
                            indexMessage = currentMessage.Length;
                            Console.CursorLeft = 17 + currentMessage.Length;
                        }
                        else
                            Console.CursorLeft++;
                        break;
                    case ConsoleKey.Enter:
                        if(!typingMessage)
                        {
                            Console.ResetColor();
                            typingMessage = true;
                            Console.CursorVisible = true;
                            Console.CursorLeft = 17;
                            Console.CursorTop = 24;
                            indexMessage = 0;
                            currentMessage = new StringBuilder();
                        }
                        break;
                    case ConsoleKey.Escape:
                        if (typingMessage)
                        {
                            ClearMessageBox();
                            typingMessage = false;
                            Console.CursorVisible = false;
                            Console.CursorLeft = 1;
                            Console.CursorTop = 1 + selectedUser;
                        }
                        break;
                    default:
                        if (typingMessage)
                        {
                            if (key.Key == ConsoleKey.Backspace)
                            {
                                if (indexMessage > 0)
                                    currentMessage.Remove(--indexMessage, 1);
                            }
                            else if (key.Key == ConsoleKey.Delete)
                            {
                                if (indexMessage < currentMessage.Length)
                                    currentMessage.Remove(indexMessage, 1);
                            }
                            else
                                currentMessage.Insert(indexMessage++, key.KeyChar);

                            ClearMessageBox();
                            Console.CursorLeft = 17;
                            Console.CursorTop = 24;
                            Console.Write(currentMessage.ToString());
                            Console.CursorLeft = 17 + indexMessage;
                        }
                        break;
                }
                busy = false;
            }
        }

        private static void Client_UsersRefresh(string[] users)
        {
            while (busy) ;
            busy = true;

            string select = "";
            if (Program.users != null && selectedUser < Program.users.Length)
                select = Program.users[selectedUser];
            Program.users = users;

            int top = Console.CursorTop;
            int left = Console.CursorLeft;
            Console.ResetColor();

            Console.CursorVisible = false;
            Console.CursorTop = users.Length + 1;
            for (int i = 0; i < 28 - users.Length; i++)
            {
                Console.CursorLeft = 1;
                Console.Write(new string(' ', 15));
                Console.CursorTop++;
            }
            Console.CursorTop = 1;
            int index = Array.IndexOf(users, select);
            for (int i = 0; i < users.Length && i < 28; i++)
            {
                Console.CursorLeft = 1;
                if (i != index)
                    Console.Write(users[i].PadRight(15));
                else
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(users[i].PadRight(15));
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.CursorTop++;
            }

            if (index != -1)
            {
                Console.CursorTop = top;
                Console.CursorLeft = left;
                Console.CursorVisible = typingMessage;
            }
            else
            {
                Console.CursorTop = 1;
                Console.CursorLeft = 1;
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(users[0].PadRight(15));
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.CursorLeft = 1;
            }

            busy = false;
        }

        private static void BuildFrame()
        {
            Console.Write("┌" + new string('─', 15) + "┬" + new string('─', 67) + "┐");
            for(int i = 0; i < 22; i++)
                Console.Write("│" + new string(' ', 15) + "│" + new string(' ', 67) + "│");
            Console.Write("│" + new string(' ', 15) + "├" + new string('─', 67) + "┤");
            for (int i = 0; i < 5; i++)
                Console.Write("│" + new string(' ', 15) + "│" + new string(' ', 67) + "│");
            Console.Write("└" + new string('─', 15) + "┴" + new string('─', 67) + "┘");
        }

        private static void ClearMessageBox()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.CursorLeft = 17;
                Console.CursorTop = 24 + i;
                Console.Write(new string(' ', 67));
            }
        }
    }
}
