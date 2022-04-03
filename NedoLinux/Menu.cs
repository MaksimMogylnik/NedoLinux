using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace NedoLinux
{
    class Menu
    {

        static Terminal terminal = new Terminal();

        public static void Switcher()
        {
            while(true)
            {
                Console.Clear();

                IOManager.TerminalMessage("F1 - start command line\n" +
                    "F2 - start graphic app\n" +
                    "Esc - exit");

                ConsoleKeyInfo consoleKey = Console.ReadKey();
                if (consoleKey.Key == ConsoleKey.F1)
                    CommandLine();
                else if (consoleKey.Key == ConsoleKey.F2)
                    GraphicLine();
                else if (consoleKey.Key == ConsoleKey.Escape)
                    break;
            }

        }
        public static void CommandLine()
        {
            while (true)
            {
                try
                {

                    Console.Write($"{Environment.CurrentDirectory} >  ");
                    string[] act = Console.ReadLine().Split(' ');

                    if (act[0] == "quit")
                        break;

                    if (act.Length == 1 && terminal.commands[act[0]].Method.GetParameters().Length == 1)
                    {
                        terminal.commands[act[0]]?.DynamicInvoke(Environment.CurrentDirectory);
                        continue;
                    }
                    else if (act.Length == 1 && terminal.commands[act[0]].Method.GetParameters().Length == 0)
                        terminal.commands[act[0]]?.DynamicInvoke();
                    else if (act.Length == 2 && terminal.commands[act[0]].Method.GetParameters().Length == 1)
                    {
                        if (act[1] == "..")
                        {
                            terminal.commands[act[0]]?.DynamicInvoke(Directory.GetParent(Environment.CurrentDirectory).FullName);
                            continue;
                        }
                        terminal.commands[act[0]]?.DynamicInvoke(act[1]);
                        continue;
                    }
                    else
                        terminal.commands[act[0]]?.DynamicInvoke(Environment.CurrentDirectory, act[1]);

                }
                catch
                {
                    IOManager.ConsoleTextColor = ConsoleColor.Red;
                    IOManager.TerminalMessage("Wrong arguments!");
                    IOManager.ConsoleTextColor = ConsoleColor.White;
                }
 
            }
        }
        public static void GraphicLine()
        {
            int highlighted = 0;
            bool window = false;
            DirectoryInfo dir1 = new DirectoryInfo(Environment.CurrentDirectory);
            DirectoryInfo dir2 = new DirectoryInfo(Environment.CurrentDirectory);

            IOManager.Controls();
            IOManager.Draw(dir1.FullName, dir2.FullName, window, highlighted);
            FileSystemInfo[] highl;

            while (true)
            {
                try
                {
                    Console.Clear();
                    IOManager.Draw(dir1.FullName, dir2.FullName, window, highlighted);
                    IOManager.Controls();

                    if (window == false)
                        highl = dir1.GetFileSystemInfos();
                    else
                        highl = dir2.GetFileSystemInfos();

                    int max1 = Directory.GetFiles(dir1.FullName).Length + Directory.GetDirectories(dir1.FullName).Length;
                    int max2 = Directory.GetFiles(dir2.FullName).Length + Directory.GetDirectories(dir2.FullName).Length;
                    ConsoleKeyInfo consoleKey = Console.ReadKey();

                    if (consoleKey.Key == ConsoleKey.UpArrow && highlighted > 0)
                        highlighted--;
                    else if (consoleKey.Key == ConsoleKey.DownArrow && highlighted < highl.Length)
                        highlighted++;
                    else if (consoleKey.Key == ConsoleKey.Tab)
                        window = ChangeWindow(window);
                    else if (consoleKey.Key == ConsoleKey.F5)
                    {
                        if (window == false)
                            dir2 = dir1;
                        else
                            dir1 = dir2;
                    }
                    else if (consoleKey.Modifiers == ConsoleModifiers.Alt && consoleKey.Key == ConsoleKey.H)
                        terminal.commands["help"]?.DynamicInvoke();
                    else if (consoleKey.Modifiers == ConsoleModifiers.Alt && consoleKey.Key == ConsoleKey.C)
                    {
                        if (window == false)
                            terminal.commands["ctrlc"]?.DynamicInvoke(highl[highlighted].FullName, dir2.FullName);
                        else
                            terminal.commands["ctrlc"]?.DynamicInvoke(highl[highlighted].FullName, dir1.FullName);
                    }
                    else if (consoleKey.Key == ConsoleKey.Delete)
                        terminal.commands["del"]?.DynamicInvoke(highl[highlighted].FullName);
                    else if (consoleKey.Key == ConsoleKey.Enter)
                    {
                        if (highl[highlighted].Attributes == FileAttributes.Archive)
                            terminal.commands["openfile"]?.DynamicInvoke(highl[highlighted].FullName);
                        else
                        {
                            if (window == false)
                            {
                                dir1 = highl[highlighted] as DirectoryInfo;
                                highlighted = 0;
                            }
                            else
                            {
                                dir2 = highl[highlighted] as DirectoryInfo;
                                highlighted = 0;
                            }
                        }
                    }
                    else if (consoleKey.Modifiers == ConsoleModifiers.Alt && consoleKey.Key == ConsoleKey.X)
                    {
                        if (window == false)
                            terminal.commands["ctrlx"]?.DynamicInvoke(highl[highlighted].FullName, dir2.FullName);
                        else
                            terminal.commands["ctrlx"]?.DynamicInvoke(highl[highlighted].FullName, dir1.FullName);
                    }
                    else if (consoleKey.Key == ConsoleKey.F3)
                    {
                        Switcher();
                        break;
                    }
                    else if (consoleKey.Key == ConsoleKey.Escape)
                    {
                        if (window == false)
                        {
                            dir1 = Directory.GetParent(dir1.FullName);
                            highlighted = 0;
                        }
                        else
                        {
                            dir2 = Directory.GetParent(dir1.FullName);
                            highlighted = 0;
                        }
                    }
                    else
                        continue;
                }
                catch
                {
                    IOManager.ConsoleTextColor = ConsoleColor.Red;
                    IOManager.TerminalMessage("Oops! Wrong key");
                    IOManager.ConsoleTextColor = ConsoleColor.White;
                }

            }
        }

        static bool ChangeWindow(bool window) => window == false ? true : false;

    }
}
