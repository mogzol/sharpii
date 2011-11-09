/* This file is part of Sharpii.
 * Copyright (C) 2011 Person66
 *
 * Sharpii is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Sharpii is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Sharpii. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Net;
using libWiiSharp;
using System.Windows.Forms;
using System.Diagnostics;

namespace Sharpii
{
    class MainApp
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                help();
                Environment.Exit(0);
            }

            if (!File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\libWiiSharp.dll"))
            {
                Console.WriteLine("ERROR: libWiiSharp.dll not found");
                Console.WriteLine("\n\nAttemp to download? [Y/N]");
                Console.Write("\n>>");
                string ans = Console.ReadLine();
                if (ans.ToUpper() == "Y")
                {
                    try
                    {
                        Console.Write("\nGrabbing libWiiSharp.dll...");
                        WebClient DLwadInstaller = new WebClient();
                        DLwadInstaller.DownloadFile("http://sharpii.googlecode.com/svn/trunk/Sharpii/libWiiSharp.dll", Path.GetDirectoryName(Application.ExecutablePath) + "\\libWiiSharp.dll");
                        Console.Write("Done!\n");
                    }
                    catch (Exception ex)
                    { Console.WriteLine("An error occured: {0}", ex.Message); Environment.Exit(0); }
                }
                else
                    Environment.Exit(0);
            }

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-QUIET":
                        Quiet.quiet = 1;
                        break;
                    case "-Q":
                        Quiet.quiet = 1;
                        break;
                    case "-LOTS":
                        Quiet.quiet = 3;
                        break;
                }
            }

            string Function = args[0].ToUpper();
            bool gotSomewhere = false;

            if (Function == "-H" || Function == "-HELP" || Function == "H" || Function == "HELP")
            {
                help();
                gotSomewhere = true;
            }

            if (Function == "BNS")
            {
                BNS_Stuff.BNS(args);
                gotSomewhere = true;
            }

            if (Function == "WAD")
            {
                WAD_Stuff.WAD(args);
                gotSomewhere = true;
            }

            if (Function == "TPL")
            {
                TPL_Stuff.TPL(args);
                gotSomewhere = true;
            }

            if (Function == "U8")
            {
                U8_Stuff.U8(args);
                gotSomewhere = true;
            }

            if (Function == "IOS")
            {
                IOS_Stuff.IOS(args);
                gotSomewhere = true;
            }

            if (Function == "NUS" || Function == "NUSD")
            {
                NUS_Stuff.NUS(args);
                gotSomewhere = true;
            }

            if (Function == "SENDDOL" || Function == "SENDOL")
            {
                HBC_Stuff.SendDol(args);
                gotSomewhere = true;
            }

            if (Function == "SENDWAD")
            {
                bool cont = HBC_Stuff.SendWad_Check(args);
                if (cont == true) HBC_Stuff.SendWad(args);
                gotSomewhere = true;
            }

            if (Function == "INSTALL")
            {
                Install();
                gotSomewhere = true;
            }

            if (Function == "UNINSTALL")
            {
                Uninstall();
                gotSomewhere = true;
            }

            if (Function == "WHICH CAME FIRST" || Function == "WHICH CAME FIRST?" || 
            (Function == "WHICH" && args[1].ToUpper() == "CAME" && args[2].Substring(0,5).ToUpper() == "FIRST"))
            {
                InconspicuousNotEasterEggThingamajig();
                gotSomewhere = true;
            }



            if (gotSomewhere == false)
            {
                //If tuser gets here, they entered something wrong
                Console.WriteLine("ERROR: The argument {0} is invalid", args[0]);
            }

            string temp = Path.GetTempPath() + "Sharpii.tmp";
            if (Directory.Exists(temp) == true)
                DeleteDir.DeleteDirectory(temp);

            Environment.Exit(0);
        }

        private static void Install()
        {
            try
            {
                if (Quiet.quiet > 1)
                    Console.WriteLine("Installing Sharpii...");
                if (Quiet.quiet > 1)
                    Console.WriteLine("Adding Variables");
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine) +
                ";" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\", EnvironmentVariableTarget.Machine);

                if (Quiet.quiet > 1)
                    Console.WriteLine("Creating Directory");
                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\"))
                    DeleteDir.DeleteDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\");

                if (Quiet.quiet > 1)
                    Console.WriteLine("Copying Files");
                File.Copy(Application.ExecutablePath, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\Sharpii.exe");
                if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\libWiiSharp.dll"))
                    File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\libWiiSharp.dll", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\libWiiSharp.dll");
                if (File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "\\WadInstaller.dll"))
                    File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\WadInstaller.dll", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\WadInstaller.dll");

                if (Quiet.quiet > 1)
                {
                    Console.WriteLine("Sharpii was successfully installed to: {0}", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\");
                    Console.WriteLine("You can now use Sharpii in any directory!");
                    Console.WriteLine("\nNOTE: You may need to restart your computer for this to take effect");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unknown error occured, please try again\n\nERROR DETAILS: {0}", ex.Message);
                return;
            }
        }

        private static void Uninstall()
        {
            try
            {
                if (Quiet.quiet > 1)
                    Console.WriteLine("Uninstalling Sharpii...");
                string path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("PATH", path.Replace(";" + Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\", ""), EnvironmentVariableTarget.Machine);
                Process.Start("cmd.exe", "/C mode con:cols=50 lines=4 & color 0B & echo Finishing Uninstallation... & sleep 2 & rmdir /s /q " + '"' + 
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Sharpii\\" + '"' + " & CLS & echo Sharpii has been successfully uninstalled! & echo. & pause");
                Application.Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unknown error occured, please try again\n\nERROR DETAILS: {0}", ex.Message);
                return;
            }
        }

        private static void InconspicuousNotEasterEggThingamajig()
        {
            WebClient egg = new WebClient(); string all = "";
            try { all = egg.DownloadString("http://sites.google.com/site/person66files/home/EASTEREGG.txt"); }
            catch (Exception) { Console.WriteLine("\n Easter eggs are more fun if you has internetz"); return; }
            int width = Console.WindowWidth; int height = Console.WindowHeight; int bwidth = Console.BufferWidth; int bheight = Console.BufferHeight;
            ConsoleKeyInfo key; Console.Clear(); Console.CursorVisible = false; Console.SetWindowSize(75, 5); Console.SetBufferSize(75, 5);
            Console.WriteLine("Complete the following: \n\n   UP, __ , __ , __ , __ , __ , __ , __ , __ , __ , START"); key = Console.ReadKey(true);
            if (key.Key.ToString() == "UpArrow") { Console.SetCursorPosition(7, 2); Console.Write("UP"); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "DownArrow") { Console.SetCursorPosition(12, 2); Console.Write("DOWN , __ , __ , __ , __ , __ , __ , __ , START"); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "DownArrow") { Console.SetCursorPosition(19, 2); Console.Write("DOWN , __ , __ , __ , __ , __ , __ , START"); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "LeftArrow") { Console.SetCursorPosition(26, 2); Console.Write("LEFT , __ , __ , __ , __ , __ , START"); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "RightArrow") { Console.SetCursorPosition(33, 2); Console.Write("RIGHT , __ , __ , __ , __ , START"); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "LeftArrow") { Console.SetCursorPosition(41, 2); Console.Write("LEFT , __ , __ , __ , START"); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "RightArrow") { Console.SetCursorPosition(48, 2); Console.Write("RIGHT , __ , __ , START"); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "B") { Console.SetCursorPosition(56, 2); Console.Write("B , __ , START   "); } else { goto ByeBye; }
            key = Console.ReadKey(true); if (key.Key.ToString() == "A") { Console.SetCursorPosition(60, 2); Console.Write("A , START   "); } else { goto ByeBye; }
            Console.SetBufferSize(95, 44); Console.SetWindowSize(95, 44); Console.SetCursorPosition(0, 0); Console.Clear(); Console.Write(all); Console.ReadKey(true);
            ByeBye: Console.Clear(); Console.CursorVisible = true; Console.SetWindowSize(width, height); Console.SetBufferSize(bwidth, bheight); Environment.Exit(0);                
        }

        private static void help()
        {
            Console.WriteLine("");
            Console.WriteLine("Sharpii {0} - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("  Usage:");
            Console.WriteLine("");
            Console.WriteLine("       Sharpii [function] [parameters] [-quiet | -q | -lots]");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("  Functions:");
            Console.WriteLine("");
            Console.WriteLine("       BNS            Convert a wav to bns, or vice versa");
            Console.WriteLine("       WAD            Pack/Unpack/Edit a wad file");
            Console.WriteLine("       TPL            Convert a image to a tpl, or vice versa");
            Console.WriteLine("       U8             Pack/Unpack a U8 archive");
            Console.WriteLine("       IOS            Apply various patches to an IOS");
            Console.WriteLine("       NUSD           Download files from NUS");
            Console.WriteLine("       SendDol        Send a dol to the HBC over wifi");
            Console.WriteLine("       SendWad        Send a wad to the HBC over wifi");
            Console.WriteLine("");
            Console.WriteLine("       NOTE: Too see more detailed descriptions of any of the above,");
            Console.WriteLine("             use 'Sharpii [function] -h'");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("  Other Functions:");
            Console.WriteLine("");
            Console.WriteLine("       Install        Install Sharpii to your computer so you can run it");
            Console.WriteLine("                      from anywhere without needing the exe");
            Console.WriteLine("       Uninstall      Uninstall Sharpii from your computer");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("  Global Arguments:");
            Console.WriteLine("");
            Console.WriteLine("       -quiet | -q    Try not to display any output");
            Console.WriteLine("       -lots          Display lots of output");
            Console.WriteLine("");


        }

        }
    }
    public class DeleteDir
    {
        public static bool DeleteDirectory(string target_dir)
        {
            bool result = false;

            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);

            return result;
        }
    }
    public class Quiet {
        //1 = little
        //2 = normal
        //3 = lots
        public static int quiet = 2;
    }
    public class Version
    {
        public static string version = "1.7";
    }