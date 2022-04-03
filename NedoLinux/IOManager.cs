using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace NedoLinux
{
    class IOManager
    {

        public static ConsoleColor ConsoleBackgroundColor { set => Console.BackgroundColor = value; }

        public static ConsoleColor ConsoleTextColor { set => Console.ForegroundColor = value; }

        public static void Ls(string[] strs)
        {
            foreach(string str in strs)
                Console.WriteLine(str);
        }

        public static void Draw(string location1, string location2, bool window, int highlighted)
        {
            const char border = '║';

            DirectoryInfo loc1 = new DirectoryInfo(location1);
            DirectoryInfo loc2 = new DirectoryInfo(location2);
            FileSystemInfo[] loc1Info = loc1.GetFileSystemInfos();
            FileSystemInfo[] loc2Info = loc2.GetFileSystemInfos();

            //левая часть консоли

            ConsoleBackgroundColor = ConsoleColor.Cyan;
            ConsoleTextColor = ConsoleColor.Magenta;
            Console.WriteLine($"{location1,-50}");
            ConsoleBackgroundColor = ConsoleColor.Black;
            ConsoleTextColor = ConsoleColor.White;

            for (int i = 0; i < loc1Info.Length + loc2Info.Length; i++)
            {

                if (i >= loc1Info.Length)
                {
                    Console.WriteLine(border);
                }
                else
                {
                    Console.Write(border + " ");

                    if (window == false)
                        if (i == highlighted)
                        {
                            ConsoleBackgroundColor = ConsoleColor.Blue;
                            TerminalMessage(loc1Info[i].Name);
                            ConsoleBackgroundColor = ConsoleColor.Black;
                            continue;
                        }

                    Console.WriteLine(loc1Info[i].Name);
                }
            }

            //центр + правая часть консоли

            Console.SetCursorPosition(Console.WindowWidth / 2 + 2, 0);
            ConsoleBackgroundColor = ConsoleColor.Cyan;
            ConsoleTextColor = ConsoleColor.Magenta;
            Console.WriteLine($"{location2,-50}");
            ConsoleBackgroundColor = ConsoleColor.Black;
            ConsoleTextColor = ConsoleColor.White;

            for (int y = 0; y < loc2Info.Length + loc1Info.Length; y++)
            {
                if (y >= loc2Info.Length)
                {
                    Console.SetCursorPosition(Console.WindowWidth - 1, y);
                    Console.Write(border);
                    Console.SetCursorPosition((Console.WindowWidth / 2), y);
                    Console.Write(border);
                }
                else
                {
                    Console.SetCursorPosition((Console.WindowWidth / 2), y + 1);
                    Console.Write(border + " ");
                    if (window == true)
                        if (y == highlighted)
                        {
                            ConsoleBackgroundColor = ConsoleColor.Blue;
                            TerminalMessage(loc2Info[y].Name);
                            ConsoleBackgroundColor = ConsoleColor.Black;
                            Console.SetCursorPosition(Console.WindowWidth - 1, y);
                            Console.Write(border);
                            continue;
                        }
                    Console.Write(loc2Info[y].Name);
                    Console.SetCursorPosition(Console.WindowWidth - 1, y + 1);
                    Console.Write(border);
                }
            }

            Console.SetCursorPosition(0, loc1Info.Length + loc2Info.Length);
            Console.WriteLine("\n");
        }

        public static void Controls()
        {
            TerminalMessage("|Ctrl+H - Help|" +
                " |Arrows - navigation|" +
                " |Enter - open|" +
                " |Tab - change window|" +
                " |F5 - mirror windows|" +
                " |Alt+C - cpy to another window|" +
                " |Alt+X - move to another window|" +
                " |F3 - choose mode|" +
                " |Esc - previous folder|");
        }

        public static void TerminalMessage(string message)
        {
            Console.WriteLine(message);
        }

    }
}
