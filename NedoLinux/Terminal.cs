using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace NedoLinux
{
    class Terminal
    {
        public Dictionary<string, Delegate> commands = new Dictionary<string, Delegate>();

        public Terminal()
        {
            commands.Add("ls", new Action<string>(Ls));
            commands.Add("cd", new Action<string>(Directory.SetCurrentDirectory));
            commands.Add("ctrlc", new Action<string, string>(CopyToDirectory));
            commands.Add("del", new Action<string>(DeleteDirectory));
            commands.Add("ctrlx", new Action<string, string>(CtrX));
            commands.Add("clr", new Action(Console.Clear));
            commands.Add("crtfile", new Action<string, string>(CreateFile));
            commands.Add("openfile", new Action<string>(OpenFile));
            commands.Add("getat", new Action<string>(GetAttributes));
            commands.Add("renameall", new Action<string, string>(GroupRename));
            commands.Add("find", new Action<string, string>(FindFile));
            commands.Add("help", new Action(Help));
            commands.Add("crtfolder", new Action<string, string>(CreateFolder));
        }

        static void Ls(string path)
        {
            string[] strs = Directory.GetFileSystemEntries(path);
            IOManager.Ls(strs);
        }

        static void CopyTo(DirectoryInfo source, DirectoryInfo target)
        {
            if (!target.Exists)
                Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            foreach(DirectoryInfo subdir in source.GetDirectories())
            {
                DirectoryInfo nextSubdir = target.CreateSubdirectory(subdir.Name);
                CopyTo(subdir, nextSubdir);
            }

        }

        static void CopyToDirectory(string ssource, string sdirection)
        {
            DirectoryInfo source = new DirectoryInfo(ssource);
            Directory.CreateDirectory(sdirection + "\\" + source.Name);
            DirectoryInfo direction = new DirectoryInfo(sdirection + '\\' + source.Name);
            CopyTo(source, direction);
        }

        static void DeleteDirectory(string direction)
        {
            IOManager.TerminalMessage("Are you shure about deleting? yes/no");
            string ans = Console.ReadLine();
            if (ans == "yes")
            {
                Directory.Delete(direction, true);
            }
        }

        static void CtrX(string ssource, string sdirection)
        {
            CopyToDirectory(ssource, sdirection);
            DeleteDirectory(ssource);
        }

        static void CreateFile(string direction, string name)
        {
            File.Create(direction + "\\" + name);
        }

        static void OpenFile(string direction)
        {
            Console.Clear();
            IOManager.TerminalMessage(File.ReadAllText(direction));
            Console.ReadKey();
        }

        static void GetAttributes(string direction)
        {
            FileAttributes atr = File.GetAttributes(direction);
            IOManager.TerminalMessage(atr.ToString());
        }

        static void GroupRename(string direction, string name)
        {
            DirectoryInfo directory = new DirectoryInfo(direction);
            int i = 0;
            foreach (FileInfo fi in directory.GetFiles())
                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fi.Name, $"{name}({++i})");
            foreach (DirectoryInfo subdir in directory.GetDirectories())
            {
                Microsoft.VisualBasic.FileIO.FileSystem.RenameDirectory(subdir.Name, $"{name}({++i})");
                GroupRename(subdir.FullName, name);
            }
        }

        static void FindFile(string direction, string name)
        {
            DirectoryInfo dir = new DirectoryInfo(direction);
            foreach (FileInfo fi in dir.GetFiles())
                if (fi.Name == name)
                {
                    IOManager.TerminalMessage(fi.FullName);
                    return;
                }
            foreach (DirectoryInfo subdir in dir.GetDirectories())
                FindFile(subdir.FullName, name);

            IOManager.TerminalMessage("not found");

        }

        static void Help()
        {
            Console.Clear();
            IOManager.TerminalMessage("ls - shows everything in current directory \n" +
                "cd - moves to wanted directory (cd *directory name*) \n" +
                "ctrlc - copies current directory to wanted directory (ctrlc *directory name*) \n" +
                "del - deletes current directory and its files \n" +
                "ctrlx - moves current directory to wanted directory (ctrlx *directory name*) \n" +
                "clr - clears console \n" +
                "crtfile - creates file (crtfile *file name.extension*) \n" +
                "openfile - opens file ONLY to read it (openfile *file name.extension*) \n" +
                "getat - gets file attributes and writes them (getat *file name.extension*) \n" +
                "renameall - renames all files, subdirections and" +
                " files in subdirections on entered name (renameall *new name.extension*) \n" +
                "find - finds wanted file in current directory (find *file name.extension*)");
        }

        static void CreateFolder(string direction, string name)
        {
            Directory.CreateDirectory($"{direction} \\ {name}");
        }

    }
}
