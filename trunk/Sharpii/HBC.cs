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

namespace Sharpii
{
    partial class HBC_Stuff
    {
        public static void SendDol(string[] args)
        {
            if (args.Length < 2)
            {
                SendDol_help();
                return;
            }
            if (args[1].ToUpper() == "-H" || args[1].ToUpper() == "-HELP")
            {
                SendDol_help();
                return;
            }
            string input = "";
            string ip = "";
            string protocol = "JODI";
            string arguments = "";
            bool compress = true;

            //Get parameters
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-IP":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No ip set");
                            return;
                        }
                        ip = args[i + 1];
                        break;
                    case "-DOL":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No dol set");
                            return;
                        }
                        input = args[i + 1];
                        //Check if file exists
                        if (File.Exists(input) == false)
                        {
                            System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                            return;
                        }

                        if (i + 1 < args.Length)
                        {
                            for (int n = i + 2; n < args.Length; n++)
                            {
                                arguments = arguments + "\x0000";
                                arguments = arguments + args[n];
                            }
                        }
                        break;
                    case "-NOCOMP":
                        compress = false;
                        break;
                    case "-OLD":
                        protocol = "HAXX";
                        break;
                }
            }

            //Run main part, and check for exceptions
            try
            {
                libWiiSharp.Protocol proto = Protocol.JODI;

                if (Quiet.quiet > 2 && protocol == "HAXX")
                    System.Console.WriteLine("Using old protocol");

                if (protocol == "HAXX")
                    proto = Protocol.HAXX;

                if (Quiet.quiet > 2)
                    System.Console.Write("Loading File...");

                HbcTransmitter file = new HbcTransmitter(proto, ip);
                
                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");


                if (Quiet.quiet > 2 && compress == true)
                    System.Console.Write("Compressing File...");
                
                file.Compress = compress;
                
                if (Quiet.quiet > 2 && compress == true)
                    System.Console.Write("Done!\n");

                if (Quiet.quiet > 1)
                    System.Console.Write("Sending file...");

                file.TransmitFile(Path.GetFileName(input) + arguments, File.ReadAllBytes(input));

                if (Quiet.quiet > 1)
                    System.Console.Write("Done!\n");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An unknown error occured, please try again");
                System.Console.WriteLine("");
                System.Console.WriteLine("ERROR DETAILS: {0}", ex.Message);
                return;
            }

            return;

        }

        public static bool SendWad_Check(string[] args)
        {
            if (args.Length < 2)
            {
                SendWad_help();
                return false;
            }
            if (args[1].ToUpper() == "-H" || args[1].ToUpper() == "-HELP")
            {
                SendWad_help();
                return false;
            }

            if (!File.Exists("WadInstaller.dll"))
            {
                System.Console.WriteLine("ERROR: WadInstaller.dll not found");
                System.Console.WriteLine("\n\nAttemp to download? [Y/N]");
                System.Console.Write("\n>>");
                string ans = Console.ReadLine();
                if (ans.ToUpper() == "Y")
                {
                    try
                    {
                        Console.Write("\nGrabbing WadInstaller.dll...");
                        WebClient DLwadInstaller = new WebClient();
                        DLwadInstaller.DownloadFile("http://sharpii.googlecode.com/files/WadInstaller.dll", "WadInstaller.dll");
                        Console.Write("Done!\n");
                    }
                    catch (Exception ex)
                    { System.Console.WriteLine("An error occured: {0}", ex.Message); return false; }
                }
                else
                    return false;
            }

            return true;
        }

        public static void SendWad(string[] args)
        {
            string input = "";
            string ip = "";
            string ios = "";
            string protocol = "JODI";

            //Get parameters
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-IOS":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No ip set");
                            return;
                        }
                        ios = args[i + 1];
                        if (!(Convert.ToInt32(ios) >= 3 && Convert.ToInt32(ios) <= 255))
                        {
                            Console.WriteLine("ERROR: Invalid IOS number");
                            return;
                        }
                        break;
                    case "-IP":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No ip set");
                            return;
                        }
                        ip = args[i + 1];
                        break;
                    case "-WAD":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No WAD set");
                            return;
                        }
                        input = args[i + 1];
                        //Check if file exists
                        if (File.Exists(input) == false)
                        {
                            System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                            return;
                        }
                        break;
                    case "-OLD":
                        protocol = "HAXX";
                        break;
                }
            }

            //Run main part, and check for exceptions
            try
            {
                libWiiSharp.Protocol proto = Protocol.JODI;

                if (Quiet.quiet > 2 && protocol == "HAXX")
                    System.Console.WriteLine("Using old protocol");

                if (protocol == "HAXX")
                    proto = Protocol.HAXX;

                if (Quiet.quiet > 2)
                    System.Console.Write("Loading File...");

                HbcTransmitter file = new HbcTransmitter(proto, ip);
                byte[] Installer = WadInstaller.InstallerHelper.CreateInstaller(input, (byte)Convert.ToInt32(ios)).ToArray();

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                if (Quiet.quiet > 1)
                    System.Console.Write("Sending file...");

                file.TransmitFile("WadInstaller.dol", Installer);

                if (Quiet.quiet > 1)
                    System.Console.Write("Done!\n");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An unknown error occured, please try again");
                System.Console.WriteLine("");
                System.Console.WriteLine("ERROR DETAILS: {0}", ex.Message);
                return;
            }

            return;

        }

        private static void SendDol_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - SendDol - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe SendDol -ip ip_adress [-old] [-nocomp] -dol file [args]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       -dol file      The dol file to send");
            System.Console.WriteLine("       -ip ip_adress  The IP address of your wii");
            System.Console.WriteLine("       -old           Use for the old (1.0.4 and below) HBC");
            System.Console.WriteLine("       -nocomp        Disable compression");
            System.Console.WriteLine("       args           Dol arguments");
            System.Console.WriteLine("");
            System.Console.WriteLine("       NOTE: Any arguments after '-dol file' will be sent as dol");
            System.Console.WriteLine("             arguments");
        }

        public static void SendWad_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - SendWad - A tool by person66, using libWiiSharp.dll by leathl,", Version.version);
            System.Console.WriteLine("                        and CRAP's installer by WiiCrazy/I.R.on");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe SendWad -ip ip_adress -wad file -ios ios [-old] [-nocomp]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       -dol file      The dol file to send");
            System.Console.WriteLine("       -ip ip_adress  The IP address of your wii");
            System.Console.WriteLine("       -ios ios       The ios to use to install the wad");
            System.Console.WriteLine("       -old           Use for the old (1.0.4 and below) HBC");
            System.Console.WriteLine("       -nocomp        Disable compression");
            System.Console.WriteLine("");
            System.Console.WriteLine("       NOTE: WAD files must be less than 8MB large");
        }
    }
}