/*
By InnieSharp(ix4/i#)
*/
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Globalization;
using System.Web;
using System.Net;
using System.Media;

namespace Zum
{
    class Program
    {
        /*
        ver Beta 0.5.4
        */
        public static string path;
        public static List<string> vars;
        public static bool debugmode;

        const int STD_OUTPUT_HANDLE = -11;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleDisplayMode(IntPtr ConsoleHandle, uint Flags, IntPtr NewScreenBufferDimensions);
        public static void Main(string[] args)
        {
            vars = new List<string>
            {
                "curdir=" + Environment.CurrentDirectory,
                "sysdir=" + Environment.SystemDirectory,
                "machinename=" + Environment.MachineName,
                "username=" + Environment.UserName,
                "osver=" + Environment.OSVersion,
                ""
            };

            bool have = false;
            foreach (string s in args)
            {
                if (s == "debug")
                {
                    debugmode = true;
                }
                else
                {
                    if(File.Exists(s))
                    {
                        path = s;
                        have = true;
                    }
                }
            }
            if (!have)
            {
                if (File.Exists("main.cds"))
                    path = "main.cds";
                else
                {
                    Console.WriteLine("Not found main.cds");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }

            oth();
        }

        public static void oth()
        {
            string[] l = File.ReadAllLines(path);
            for (int cam = 0; cam < l.Length; cam++)
            {
                if (l[cam].StartsWith("FUNC "))
                {
                    for (int i = cam; i < l.Length; i++)
                    {
                        try
                        {
                            if (SearchForEF(i, l) == true)
                            {
                                if (l[i] == "END_FUNC")
                                {
                                	cam = i;
                                	break;
                                }
                                else if (l[i].StartsWith("PRINT "))
                                {
                                    string var = l[i].Remove(0, 6);
                                    string text = "--nfv--";
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var + "="))
                                        {
                                            text = ln.Remove(0, var.Length + 1);
                                            break;
                                        }
                                    }
                                    Console.Write(text);
                                }
                                else if (l[i].StartsWith("CAPTION "))
                                {
                                    string var = l[i].Remove(0, 6);
                                    string text = "--nfv--";
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var + "="))
                                        {
                                            text = ln.Remove(0, var.Length + 1);
                                            break;
                                        }
                                    }
                                    Console.Title = text;
                                }
                                else if (l[i] == "PRINT;")
                                    Console.Write(Environment.NewLine);
                                else if (l[i] == "INPUT;")
                                    Console.ReadKey(true);
                                else if (l[i] == "EXIT")
                                    Environment.Exit(0);
                                else if (l[i] == "CLEAR")
                                    Console.Clear();
                                else if (l[i].StartsWith("INPUTSTR "))
                                {
                                    string var = l[i].Remove(0, 9);
                                    string val = Console.ReadLine();
                                    bool havevar = false;
                                    for (int cm = 0; cm < vars.Count; cm++)
                                    {
                                        if (vars[cm].StartsWith(var + "="))
                                        {
                                            vars[cm] = var + "=" + val;
                                            havevar = true;
                                            break;
                                        }
                                    }
                                    if (!havevar)
                                        vars.Add(var + "=" + val);
                                }
                                else if (l[i].StartsWith("INPUTINT "))
                                {
                                    string var = l[i].Remove(0, 9);
                                    string oldval = Console.ReadLine();
                                    int val = 13;
                                    if (oldval != null)
                                        val = int.Parse(oldval);
                                    bool havevar = false;
                                    for (int cm = 0; cm < vars.Count; cm++)
                                    {
                                        if (vars[cm].StartsWith(var + "="))
                                        {
                                            vars[cm] = var + "=" + val;
                                            havevar = true;
                                            break;
                                        }
                                    }
                                    if (!havevar)
                                        vars.Add(var + "=" + val);
                                }
                                else if (l[i].StartsWith("INPUTDBL "))
                                {
                                    string var = l[i].Remove(0, 9);
                                    string oldval = Console.ReadLine();
                                    double val = 13.2009;
                                    if (oldval != null)
                                        val = double.Parse(oldval);
                                    bool havevar = false;
                                    for (int cm = 0; cm < vars.Count; cm++)
                                    {
                                        if (vars[cm].StartsWith(var + "="))
                                        {
                                            vars[cm] = var + "=" + val;
                                            havevar = true;
                                            break;
                                        }
                                    }
                                    if (!havevar)
                                        vars.Add(var + "=" + val);
                                }
                                else if (l[i].StartsWith("INPUTKEY "))
                                {
                                    string var = l[i].Remove(0, 9);
                                    ConsoleKeyInfo val = Console.ReadKey(true);
                                    bool havevar = false;
                                    for (int cm = 0; cm < vars.Count; cm++)
                                    {
                                        if (vars[cm].StartsWith(var + "="))
                                        {
                                            vars[cm] = var + "=" + val.Key;
                                            havevar = true;
                                            break;
                                        }
                                    }
                                    if (!havevar)
                                        vars.Add(var + "=" + val.Key);
                                }
                                else if (l[i].StartsWith("VARIABLE "))
                                {
                                    string def = l[i].Remove(0, 9);
                                    string[] spltopt = { " " };
                                    string[] spltres = def.Split(spltopt, StringSplitOptions.None);
                                    string var = spltres[0];
                                    string val = def.Remove(0, var.Length + 1);
                                    if (val == "date")
                                    {
                                        val = DateTime.Now.ToString("dd.MM.yyyy");
                                    }
                                    else if (val == "time")
                                    {
                                        val = DateTime.Now.ToString("HH:mm:ss");
                                    }
                                    else if (val.StartsWith("/"))
                                    {
                                        val = val.Remove(0, 1);
                                    }
                                    else if (val.StartsWith("#"))
                                    {
                                        string var1 = val.Remove(0, 1);
                                        foreach (string ln in vars)
                                        {
                                            if (ln.StartsWith(var1 + "="))
                                            {
                                                val = ln.Remove(0, var1.Length + 1);
                                                break;
                                            }
                                        }
                                    }
                                    bool havevar = false;
                                    for (int cm = 0; cm < vars.Count; cm++)
                                    {
                                        if (vars[cm].StartsWith(var + "="))
                                        {
                                            vars[cm] = var + "=" + val;
                                            havevar = true;
                                            break;
                                        }
                                    }
                                    if (!havevar)
                                        vars.Add(var + "=" + val);
                                }
                                else if (l[i].StartsWith("IO "))
                                {
                                    string mdf = l[i].Remove(0, 3);
                                    string[] mmmmm = { " " };
                                    string[] resa = mdf.Split(mmmmm, StringSplitOptions.None);
                                    string type = resa[0];
                                    if(type == "FILE")
                                    {
                                        string action = resa[1];
                                        if(action == "EXISTS")
                                        {
                                            string varfile = resa[2];
                                            string var = resa[3];
                                            string file = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            string val = Exists("file", file);
                                            bool havevar = false;
                                            for (int cm = 0; cm < vars.Count; cm++)
                                            {
                                                if (vars[cm].StartsWith(var + "="))
                                                {
                                                    vars[cm] = var + "=" + val;
                                                    havevar = true;
                                                    break;
                                                }
                                            }
                                            if (!havevar)
                                                vars.Add(var + "=" + val);
                                        }
                                        else if (action == "CREATE")
                                        {
                                            string varfile = resa[2];
                                            string file = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            File.WriteAllText(file, "");
                                        }
                                        else if (action == "DELETE")
                                        {
                                            string varfile = resa[2];
                                            string file = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            File.Delete(file);
                                        }
                                        else if (action == "MOVE")
                                        {
                                            string varfile = resa[2];
                                            string varfile2 = resa[3];
                                            string file = "--nfv--";
                                            string file2 = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile2 + "="))
                                                {
                                                    file2 = ln.Remove(0, varfile2.Length + 1);
                                                    break;
                                                }
                                            }
                                            File.Move(file, file2);
                                        }
                                        else if (action == "COPY")
                                        {
                                            string varfile = resa[2];
                                            string varfile2 = resa[3];
                                            string file = "--nfv--";
                                            string file2 = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile2 + "="))
                                                {
                                                    file2 = ln.Remove(0, varfile2.Length + 1);
                                                    break;
                                                }
                                            }
                                            File.Copy(file, file2);
                                        }
                                        else if (action == "WRITE")
                                        {
                                            string varfile = resa[2];
                                            string vartext = resa[3];
                                            string file = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            string text = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vartext + "="))
                                                {
                                                    text = ln.Remove(0, vartext.Length + 1);
                                                    break;
                                                }
                                            }
                                            File.WriteAllText(file, text);
                                        }
                                        else if (action == "ADDLINE")
                                        {
                                            string varfile = resa[2];
                                            string vartext = resa[3];
                                            string file = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            string text = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vartext + "="))
                                                {
                                                    text = ln.Remove(0, vartext.Length + 1);
                                                    break;
                                                }
                                            }
                                            File.AppendAllText(file, Environment.NewLine + text);
                                        }
                                        else if (action == "READ")
                                        {
                                            string varfile = resa[2];
                                            string varline = resa[3];
                                            string var = resa[4];
                                            string file = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            int line = 0;
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varline + "="))
                                                {
                                                    line = int.Parse(ln.Remove(0, varline.Length + 1));
                                                    break;
                                                }
                                            }
                                            string[] lns = File.ReadAllLines(file);
                                            string val = lns[line];
                                            bool havevar = false;
                                            for (int cm = 0; cm < vars.Count; cm++)
                                            {
                                                if (vars[cm].StartsWith(var + "="))
                                                {
                                                    vars[cm] = var + "=" + val;
                                                    havevar = true;
                                                    break;
                                                }
                                            }
                                            if (!havevar)
                                                vars.Add(var + "=" + val);
                                        }
                                        else if (action == "SIZE")
                                        {
                                            string varfile = resa[2];
                                            string var = resa[3];
                                            string filea = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    filea = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            FileInfo file = new FileInfo(filea);
                                            double size = file.Length;
                                            string val = size.ToString();
                                            bool havevar = false;
                                            for (int cm = 0; cm < vars.Count; cm++)
                                            {
                                                if (vars[cm].StartsWith(var + "="))
                                                {
                                                    vars[cm] = var + "=" + val;
                                                    havevar = true;
                                                    break;
                                                }
                                            }
                                            if (!havevar)
                                                vars.Add(var + "=" + val);
                                        }
                                        else if (action == "LENGTH")
                                        {
                                            string varfile = resa[2];
                                            string var = resa[3];
                                            string filea = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    filea = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            string[] lgnth = File.ReadAllLines(filea);
                                            int val = lgnth.Length;
                                            bool havevar = false;
                                            for (int cm = 0; cm < vars.Count; cm++)
                                            {
                                                if (vars[cm].StartsWith(var + "="))
                                                {
                                                    vars[cm] = var + "=" + val;
                                                    havevar = true;
                                                    break;
                                                }
                                            }
                                            if (!havevar)
                                                vars.Add(var + "=" + val);
                                        }
                                    }
                                    else if (type == "DIRECTORY")
                                    {
                                        string action = resa[1];
                                        if (action == "EXISTS")
                                        {
                                            string vardir = resa[2];
                                            string var = resa[3];
                                            string dir = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vardir + "="))
                                                {
                                                    dir = ln.Remove(0, vardir.Length + 1);
                                                    break;
                                                }
                                            }
                                            string val = Exists("directory", dir);
                                            bool havevar = false;
                                            for (int cm = 0; cm < vars.Count; cm++)
                                            {
                                                if (vars[cm].StartsWith(var + "="))
                                                {
                                                    vars[cm] = var + "=" + val;
                                                    havevar = true;
                                                    break;
                                                }
                                            }
                                            if (!havevar)
                                                vars.Add(var + "=" + val);
                                        }
                                        else if (action == "CREATE")
                                        {
                                            string vardir = resa[2];
                                            string dir = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vardir + "="))
                                                {
                                                    dir = ln.Remove(0, vardir.Length + 1);
                                                    break;
                                                }
                                            }
                                            Directory.CreateDirectory(dir);
                                        }
                                        else if (action == "DELETE")
                                        {
                                            string vardir = resa[2];
                                            string dir = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vardir + "="))
                                                {
                                                    dir = ln.Remove(0, vardir.Length + 1);
                                                    break;
                                                }
                                            }
                                            Directory.Delete(dir);
                                        }
                                        else if (action == "GETFILES")
                                        {
                                            string vardir = resa[2];
                                            string varfile = resa[3];
                                            string dir = "--nfv--";
                                            string file = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vardir + "="))
                                                {
                                                    dir = ln.Remove(0, vardir.Length + 1);
                                                    break;
                                                }
                                            }
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    file = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            string[] fls = Directory.GetFiles(dir);
                                            File.WriteAllLines(file, fls);
                                        }
                                        else if (action == "MOVE")
                                        {
                                            string vardir = resa[2];
                                            string vardir2 = resa[3];
                                            string dir = "--nfv--";
                                            string dir2 = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vardir + "="))
                                                {
                                                    dir = ln.Remove(0, vardir.Length + 1);
                                                    break;
                                                }
                                            }
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(vardir2 + "="))
                                                {
                                                    dir2 = ln.Remove(0, vardir2.Length + 1);
                                                    break;
                                                }
                                            }
                                            Directory.Move(dir, dir2);
                                        }
                                        else if (action == "SIZE")
                                        {
                                            string varfile = resa[2];
                                            string var = resa[3];
                                            string filea = "--nfv--";
                                            foreach (string ln in vars)
                                            {
                                                if (ln.StartsWith(varfile + "="))
                                                {
                                                    filea = ln.Remove(0, varfile.Length + 1);
                                                    break;
                                                }
                                            }
                                            string[] files = Directory.GetFiles(filea);
                                            double size = 0;
                                            foreach (string fi in files)
                                            {
                                                System.IO.FileInfo file = new System.IO.FileInfo(fi);
                                                size += file.Length;
                                            }
                                            string val = size.ToString();
                                            bool havevar = false;
                                            for (int cm = 0; cm < vars.Count; cm++)
                                            {
                                                if (vars[cm].StartsWith(var + "="))
                                                {
                                                    vars[cm] = var + "=" + val;
                                                    havevar = true;
                                                    break;
                                                }
                                            }
                                            if (!havevar)
                                                vars.Add(var + "=" + val);
                                        }
                                    }
                                }
                                else if (l[i].StartsWith("IFSTR "))
                                {
                                    string mdf = l[i].Remove(0, 6);
                                    string[] mmmmm = { " " };
                                    string[] resa = mdf.Split(mmmmm, StringSplitOptions.None);
                                    string var1 = resa[0];
                                    string act = resa[1];
                                    string var2 = resa[2];
                                    string func = resa[3];
                                    string text1 = "--nfv--";
                                    string text2 = "--nfv--";
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var1 + "="))
                                        {
                                            text1 = ln.Remove(0, var1.Length + 1);
                                            break;
                                        }
                                    }
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var2 + "="))
                                        {
                                            text2 = ln.Remove(0, var2.Length + 1);
                                            break;
                                        }
                                    }
                                    if (act == "=")
                                    {
                                        if(text1 == text2)
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            i = ndx;
                                        }
                                    }
                                    if (act == "!")
                                    {
                                        if (text1 != text2)
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;
                                        }
                                    }
                                    if (act == "(")
                                    {
                                        if (text1.StartsWith(text2))
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;
                                        }
                                    }
                                    if (act == ")")
                                    {
                                        if (text1.EndsWith(text2))
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;
                                        }
                                    }
                                    if (act == "#")
                                    {
                                    	if (String.IsNullOrEmpty(text1) || String.IsNullOrWhiteSpace(text1))
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;;
                                        }
                                    }
                                }
                                else if (l[i].StartsWith("IFINT "))
                                {
                                    string mdf = l[i].Remove(0, 6);
                                    string[] mmmmm = { " " };
                                    string[] resa = mdf.Split(mmmmm, StringSplitOptions.None);
                                    string var1 = resa[0];
                                    string act = resa[1];
                                    string var2 = resa[2];
                                    string func = resa[3];
                                    string text1 = "--nfv--";
                                    string text2 = "--nfv--";
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var1 + "="))
                                        {
                                            text1 = ln.Remove(0, var1.Length + 1);
                                            break;
                                        }
                                    }
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var2 + "="))
                                        {
                                            text2 = ln.Remove(0, var2.Length + 1);
                                            break;
                                        }
                                    }
                                    double a = double.Parse(text1);
                                    double b = double.Parse(text2);
                                    if (act == "=")
                                    {
                                        if (a == b)
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;
                                        }
                                    }
                                    if (act == "!")
                                    {
                                        if (a != b)
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;
                                        }
                                    }
                                    if (act == "<")
                                    {
                                        if (a < b)
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;
                                        }
                                    }
                                    if (act == ">")
                                        if (a > b)
                                        {
                                            int ndx = SearchForSTR("FUNC " + func, l);
                                            cam = ndx;
                                            break;
                                        }
                                    }
                                else if (l[i].StartsWith("MATH "))
                                {
                                    string mdf = l[i].Remove(0, 5);
                                    string[] mmmmm = { " " };
                                    string[] resa = mdf.Split(mmmmm, StringSplitOptions.None);
                                    string var1 = resa[0];
                                    string act = resa[1];
                                    string var2 = resa[2];
                                    string var = resa[3];
                                    string text1 = "--nfv--";
                                    string text2 = "--nfv--";
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var1 + "="))
                                        {
                                            text1 = ln.Remove(0, var1.Length + 1);
                                            break;
                                        }
                                    }
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var2 + "="))
                                        {
                                            text2 = ln.Remove(0, var2.Length + 1);
                                            break;
                                        }
                                    }
                                    double a = double.Parse(text1);
                                    double b = double.Parse(text2);
                                    if (act == "+")
                                    {
                                        double val = a + b;
                                        bool havevar = false;
                                        for (int cm = 0; cm < vars.Count; cm++)
                                        {
                                            if (vars[cm].StartsWith(var + "="))
                                            {
                                                vars[cm] = var + "=" + val;
                                                havevar = true;
                                                break;
                                            }
                                        }
                                        if (!havevar)
                                            vars.Add(var + "=" + val);
                                    }
                                    if (act == "-")
                                    {
                                        double val = a - b;
                                        bool havevar = false;
                                        for (int cm = 0; cm < vars.Count; cm++)
                                        {
                                            if (vars[cm].StartsWith(var + "="))
                                            {
                                                vars[cm] = var + "=" + val;
                                                havevar = true;
                                                break;
                                            }
                                        }
                                        if (!havevar)
                                            vars.Add(var + "=" + val);
                                    }
                                    if (act == "*")
                                    {
                                        double val = a * b;
                                        bool havevar = false;
                                        for (int cm = 0; cm < vars.Count; cm++)
                                        {
                                            if (vars[cm].StartsWith(var + "="))
                                            {
                                                vars[cm] = var + "=" + val;
                                                havevar = true;
                                                break;
                                            }
                                        }
                                        if (!havevar)
                                            vars.Add(var + "=" + val);
                                    }
                                    if (act == "/")
                                    {
                                        double val = a / b;
                                        bool havevar = false;
                                        for (int cm = 0; cm < vars.Count; cm++)
                                        {
                                            if (vars[cm].StartsWith(var + "="))
                                            {
                                                vars[cm] = var + "=" + val;
                                                havevar = true;
                                                break;
                                            }
                                        }
                                        if (!havevar)
                                            vars.Add(var + "=" + val);
                                    }
                                    if (act == "%")
                                    {
                                        double val = a % b;
                                        bool havevar = false;
                                        for (int cm = 0; cm < vars.Count; cm++)
                                        {
                                            if (vars[cm].StartsWith(var + "="))
                                            {
                                                vars[cm] = var + "=" + val;
                                                havevar = true;
                                                break;
                                            }
                                        }
                                        if (!havevar)
                                            vars.Add(var + "=" + val);
                                    }
                                }
                                else if (l[i].StartsWith("BACKCOLOR "))
                                {
                                    string var = l[i].Remove(0, 10);
                                    string text = "--nfv--";
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var + "="))
                                        {
                                            text = ln.Remove(0, var.Length + 1);
                                            break;
                                        }
                                    }
                                    switch(text)
                                    {
                                        default:
                                            Console.BackgroundColor = ConsoleColor.Black;
                                            break;
                                        case "white":
                                            Console.BackgroundColor = ConsoleColor.White;
                                            break;
                                        case "black":
                                            Console.BackgroundColor = ConsoleColor.Black;
                                            break;
                                        case "green":
                                            Console.BackgroundColor = ConsoleColor.Green;
                                            break;
                                        case "red":
                                            Console.BackgroundColor = ConsoleColor.Red;
                                            break;
                                        case "blue":
                                            Console.BackgroundColor = ConsoleColor.Blue;
                                            break;
                                        case "yellow":
                                            Console.BackgroundColor = ConsoleColor.Yellow;
                                            break;
                                        case "gray":
                                            Console.BackgroundColor = ConsoleColor.Gray;
                                            break;
                                        case "cyan":
                                            Console.BackgroundColor = ConsoleColor.Cyan;
                                            break;
                                        case "magenta":
                                            Console.BackgroundColor = ConsoleColor.Magenta;
                                            break;
                                    }
                                }
                                else if (l[i].StartsWith("FORECOLOR "))
                                {
                                    string var = l[i].Remove(0, 10);
                                    string text = "--nfv--";
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var + "="))
                                        {
                                            text = ln.Remove(0, var.Length + 1);
                                            break;
                                        }
                                    }
                                    switch (text)
                                    {
                                        default:
                                            Console.ForegroundColor = ConsoleColor.Black;
                                            break;
                                        case "white":
                                            Console.ForegroundColor = ConsoleColor.White;
                                            break;
                                        case "black":
                                            Console.ForegroundColor = ConsoleColor.Black;
                                            break;
                                        case "green":
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            break;
                                        case "red":
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            break;
                                        case "blue":
                                            Console.ForegroundColor = ConsoleColor.Blue;
                                            break;
                                        case "yellow":
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            break;
                                        case "gray":
                                            Console.ForegroundColor = ConsoleColor.Gray;
                                            break;
                                        case "cyan":
                                            Console.ForegroundColor = ConsoleColor.Cyan;
                                            break;
                                        case "magenta":
                                            Console.ForegroundColor = ConsoleColor.Magenta;
                                            break;
                                    }
                                }
                                else if (l[i].StartsWith("PROCESS "))
                                {
                                    string mdf = l[i].Remove(0, 8);
                                    string[] mmmmm = { " " };
                                    string[] resa = mdf.Split(mmmmm, StringSplitOptions.None);
                                    string act = resa[0];
                                    if (act == "KILL")
                                    {
                                        string varproc = resa[1];
                                        string proc = "--nfv--";
                                        foreach (string ln in vars)
                                        {
                                            if (ln.StartsWith(varproc + "="))
                                            {
                                                proc = ln.Remove(0, varproc.Length + 1);
                                                break;
                                            }
                                        }
                                        foreach(Process p in Process.GetProcessesByName(proc))
                                             p.Kill();
                                    }
                                    if (act == "START")
                                    {
                                        string act1 = resa[1];
                                        string varproc = resa[2];
                                        string proc = "--nfv--";
                                        string varargs = resa[3];
                                        string args = "--nfv--";
                                        foreach (string ln in vars)
                                        {
                                            if (ln.StartsWith(varproc + "="))
                                            {
                                                proc = ln.Remove(0, varproc.Length + 1);
                                                break;
                                            }
                                        }
                                        foreach (string ln in vars)
                                        {
                                            if (ln.StartsWith(varargs + "="))
                                            {
                                                args = ln.Remove(0, varargs.Length + 1);
                                                break;
                                            }
                                        }
                                        if (act1 == "ARGS")
                                        {
                                            Process p = new Process();
                                            p.StartInfo.FileName = proc;
                                            p.StartInfo.Arguments = args;
                                            p.Start();
                                        }
                                        if (act1 == "DEFAULT")
                                        {
                                            Process p = new Process();
                                            p.StartInfo.FileName = proc;
                                            p.Start();
                                        }
                                    }
                                }
                                else if (l[i].StartsWith("MISC "))
                                {
                                    string mdf = l[i].Remove(0, 5);
                                    string[] mmmmm = { " " };
                                    string[] resa = mdf.Split(mmmmm, StringSplitOptions.None);
                                    string act = resa[0];
                                    if (act == "VARLENGTH")
                                    {
                                        string var1 = resa[1];
                                        string var = resa[2];
                                        string text1 = "--nfv--";
                                        foreach (string ln in vars)
                                        {
                                            if (ln.StartsWith(var1 + "="))
                                            {
                                                text1 = ln.Remove(0, var1.Length + 1);
                                                break;
                                            }
                                        }
                                        int val = text1.Length;
                                        bool havevar = false;
                                        for (int cm = 0; cm < vars.Count; cm++)
                                        {
                                            if (vars[cm].StartsWith(var + "="))
                                            {
                                                vars[cm] = var + "=" + val;
                                                havevar = true;
                                                break;
                                            }
                                        }
                                        if (!havevar)
                                            vars.Add(var + "=" + val);
                                    }
                                    else if (act == "REMOVE")
                                    {
                                        string var1 = resa[1];
	                                    string var2 = resa[2];
	                                    string var3 = resa[3];
	                                    string var = resa[4];
	                                    int a = 1;
	                                    int b = 3;
	                                    string text = "--nfv--";
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var1 + "="))
	                                        {
	                                        	a = int.Parse(ln.Remove(0, var1.Length + 1));
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var2 + "="))
	                                        {
	                                        	b = int.Parse(ln.Remove(0, var2.Length + 1));
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var3 + "="))
	                                        {
	                                        	text = ln.Remove(0, var3.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    string val = text.Remove(a, b);
	                                    bool havevar = false;
	                                    for (int cm = 0; cm < vars.Count; cm++)
	                                    {
	                                        if (vars[cm].StartsWith(var + "="))
	                                        {
	                                            vars[cm] = var + "=" + val;
	                                            havevar = true;
	                                            break;
	                                        }
	                                    }
	                                    if (!havevar)
	                                        vars.Add(var + "=" + val);
                                    }
                                    else if (act == "SUBSTRING")
                                    {
                                        string var1 = resa[1];
	                                    string var2 = resa[2];
	                                    string var3 = resa[3];
	                                    string var = resa[4];
	                                    int a = 1;
	                                    int b = 3;
	                                    string text = "--nfv--";
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var1 + "="))
	                                        {
	                                        	a = int.Parse(ln.Remove(0, var1.Length + 1));
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var2 + "="))
	                                        {
	                                        	b = int.Parse(ln.Remove(0, var2.Length + 1));
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var3 + "="))
	                                        {
	                                        	text = ln.Remove(0, var3.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    string val = text.Substring(a, b);
	                                    bool havevar = false;
	                                    for (int cm = 0; cm < vars.Count; cm++)
	                                    {
	                                        if (vars[cm].StartsWith(var + "="))
	                                        {
	                                            vars[cm] = var + "=" + val;
	                                            havevar = true;
	                                            break;
	                                        }
	                                    }
	                                    if (!havevar)
	                                        vars.Add(var + "=" + val);
                                    }
                                    else if (act == "INDEXOF")
                                    {
                                        string var1 = resa[1];
	                                    string var2 = resa[2];
	                                    string var = resa[3];
	                                    string symb = "--nfv--";
	                                    string text = "--nfv--";
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var2 + "="))
	                                        {
	                                        	text = ln.Remove(0, var2.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var1 + "="))
	                                        {
	                                        	symb = ln.Remove(0, var1.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    int val = text.IndexOf(symb);
	                                    bool havevar = false;
	                                    for (int cm = 0; cm < vars.Count; cm++)
	                                    {
	                                        if (vars[cm].StartsWith(var + "="))
	                                        {
	                                            vars[cm] = var + "=" + val;
	                                            havevar = true;
	                                            break;
	                                        }
	                                    }
	                                    if (!havevar)
	                                        vars.Add(var + "=" + val);
                                    }
                                    else if (act == "SPLIT")
                                    {
                                        string var1 = resa[1];
	                                    string var2 = resa[2];
	                                    string varf = resa[3];
	                                    string symb = "--nfv--";
	                                    string text = "--nfv--";
	                                    string file = "--nfv--";
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var2 + "="))
	                                        {
	                                        	text = ln.Remove(0, var2.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var1 + "="))
	                                        {
	                                        	symb = ln.Remove(0, var1.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(varf + "="))
	                                        {
	                                        	file = ln.Remove(0, varf.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    string[] spl = { symb };
	                                    string[] xxx = text.Split(spl, StringSplitOptions.None);
	                                    File.WriteAllLines(file, xxx);
                                    }
                                    else if (act == "DOWNLOAD")
                                    {
                                        string var1 = resa[1];
	                                    string var2 = resa[2];
	                                    string url = "--nfv--";
	                                    string file = "--nfv--";
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var1 + "="))
	                                        {
	                                        	url = ln.Remove(0, var1.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    foreach (string ln in vars)
	                                    {
	                                        if (ln.StartsWith(var2 + "="))
	                                        {
	                                        	file = ln.Remove(0, var2.Length + 1);
	                                            break;
	                                        }
	                                    }
	                                    WebClient webClient = new WebClient();
							            string patha = file;
							            webClient.DownloadFile(url, patha);
                                    }
                                    else if (act == "FULLSCREEN")
                                    	FullScreenOn();
                                }
                                else if (l[i].StartsWith("WAIT S "))
                                {
                                    string var = l[i].Remove(0, 7);
                                    int time = 13;
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var + "="))
                                        {
                                            time = int.Parse(ln.Remove(0, var.Length + 1));
                                            break;
                                        }
                                    }
                                    Thread.Sleep(time * 1000);
                                }
                                else if (l[i].StartsWith("WAIT MS "))
                                {
                                    string var = l[i].Remove(0, 8);
                                    int time = 13;
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var + "="))
                                        {
                                            time = int.Parse(ln.Remove(0, var.Length + 1));
                                            break;
                                        }
                                    }
                                    Thread.Sleep(time);
                                }
                                else if (l[i].StartsWith("RANDOM "))
                                {
                                    string mdf = l[i].Remove(0, 7);
                                    string[] mmmmm = { " " };
                                    string[] resa = mdf.Split(mmmmm, StringSplitOptions.None);
                                    string var1 = resa[0];
                                    string var2 = resa[1];
                                    string var = resa[2];
                                    int a = 1;
                                    int b = 3;
                                    Random rr = new Random();
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var1 + "="))
                                        {
                                        	a = int.Parse(ln.Remove(0, var1.Length + 1));
                                            break;
                                        }
                                    }
                                    foreach (string ln in vars)
                                    {
                                        if (ln.StartsWith(var2 + "="))
                                        {
                                        	b = int.Parse(ln.Remove(0, var2.Length + 1));
                                            break;
                                        }
                                    }
                                    int val = rr.Next(a, b);
                                    bool havevar = false;
                                    for (int cm = 0; cm < vars.Count; cm++)
                                    {
                                        if (vars[cm].StartsWith(var + "="))
                                        {
                                            vars[cm] = var + "=" + val;
                                            havevar = true;
                                            break;
                                        }
                                    }
                                    if (!havevar)
                                        vars.Add(var + "=" + val);
                                }
                                else if (l[i] == "_1")
                                	oth();
                                else if (l[i].StartsWith("_2"))
                                {
                                	path = l[i].Remove(0, 2);
                                	oth();
                                }
                                else if (l[i].StartsWith("REM ") || l[i].StartsWith(":: "))
                                {
                                	//cm
                                }
                                else
                                {
                                	cam = i;
                                    break;
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            if (debugmode != true)
                            {
                                if (Directory.Exists("LGSZMRRS"))
                                    File.WriteAllText(@"LGSZMRRS\log-" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + "_lgzmrrsnwchkt.txt", "LINE(" + i + "): " + error.Message);
                                else
                                {
                                    Directory.CreateDirectory("LGSZMRRS");
                                    File.WriteAllText(@"LGSZMRRS\log-" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + "_lgzmrrsnwchkt.txt", "LINE(" + i + "): " + error.Message);
                                }
                            }
                            else
                            {
                                Console.WriteLine("LINE(" + i + "): " + error.Message);
                            }
                        }
                    }
                }
            }
        }

        public static void FullScreenOn()
        {
            var hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
            SetConsoleDisplayMode(hConsole, 1, IntPtr.Zero);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        }

        public static bool SearchForEF(int startIndex, string[] from)
        {
            for(int i = startIndex; i < from.Length; i++)
                if (from[i] == "END_FUNC")
                    return true;
            return false;
        }
        public static int SearchForSTR(string str, string[] from)
        {
            for (int i = 0; i < from.Length; i++)
                if (from[i] == str)
                    return i;
            return 13;
        }

        public static string Exists(string type, string name)
        {
            if(type == "file")
            {
                if (File.Exists(name))
                    return "TRUE";
            }
            else if (type == "directory")
            {
                if (Directory.Exists(name))
                    return "TRUE";
            }
            return "FALSE";
        }
    }
}