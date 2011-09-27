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
 * along with Sharpii.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using libWiiSharp;

namespace Sharpii
{
    partial class HBC_Stuff
    {
        public static void SendDol(string[] args)
        {
            if (args.Length < 3)
            {
                SendDol_help();
                return;
            }
            string input = args[1];
            string ip = "";
            string protocol = "JODI";
            bool compress = true;

            //Check if file exists
            if (File.Exists(input) == false)
            {
                System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }

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

                file.TransmitFile(input);

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

        public static void SendDol_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - SendDol - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe SendDol file -ip ip_adress [-old] [-nocomp]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       file           The dol file to send");
            System.Console.WriteLine("       -ip ip_adress  The IP address of your wii");
            System.Console.WriteLine("       -old           Use for the old (1.0.4 and below) HBC");
            System.Console.WriteLine("       -nocomp        Disable compression");
        }
    }
}