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
using libWiiSharp;

namespace Sharpii
{
    partial class U8_Stuff
    {
        public static void U8(string[] args)
        {
            if (args.Length < 4)
            {
                U8_help();
                return;
            }

            //********************* Pack *********************
            if (args[1] == "-p")
            {
                Pack(args);
                return;
            }

            //********************* Unpack *********************
            if (args[1] == "-u")
            {
                Unpack(args);
                return;
            }

            //If tuser gets here, they entered something wrong
            System.Console.WriteLine("ERROR: The argument {0} is invalid", args[1]);
            return;

        }

        private static void Unpack(string[] args)
        {
            string input = args[2];
            string output = args[3];

            //Check if file exists
            if (File.Exists(input) == false)
            {
                System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }
            //Check if file is U8
            if (libWiiSharp.U8.IsU8(input) != true)
            {
                System.Console.WriteLine("ERROR: File {0} is not a U8 archive", input);
                return;
            }
           
            //Run main part, and check for exceptions
            try
            {
                //Load U8
                U8 U8file = new U8();

                if (Quiet.quiet > 2)
                    System.Console.Write("Loading file...");

                U8file.LoadFile(input);

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                if (Quiet.quiet > 2)
                    System.Console.Write("Extracting file...");

                U8file.Extract(output);

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                if (Quiet.quiet > 1)
                    System.Console.WriteLine("Operation completed succesfully!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An unknown error occured, please try again");
                System.Console.WriteLine("");
                System.Console.WriteLine("ERROR DETAILS: {0}", ex.Message);
                return;
            }
        }

        private static void Pack(string[] args)
        {
            //Setting up variables
            string input = args[2];
            string output = args[3];
            bool lz77 = false;
            bool imd5 = false;
            string imet = "";

            //Check if folder exists
            if (Directory.Exists(input) == false)
            {
                System.Console.WriteLine("ERROR: Unable to open Folder: {0}", input);
                return;
            }

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-LZ77":
                        lz77 = true;
                        break;
                    case "-IMD5":
                        imd5 = true;
                        break;
                    case "-IMET":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No title set");
                            return;
                        }
                        imet = args[i + 1];
                        break;
                }
            }

            if (imd5 == true && imet != "")
            {
                System.Console.WriteLine("ERROR: Cannot use IMET and IMD5 at the same time.");
                return;
            }

            //Run main part, and check for exceptions
            try
            {
                U8 U8folder = new U8();
                
                if (Quiet.quiet > 2)
                    System.Console.Write("Loading folder...");

                U8folder.CreateFromDirectory(input);

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                if (imd5 == true)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Adding IMD5 Header");
                    U8folder.AddHeaderImd5();
                }

                if (imet != "")
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Adding IMET header with title: {0}", imet);
                    U8folder.AddHeaderImet(false, imet);
                }

                if (lz77 == true)
                {
                    //Yeah, I know this isnt where it actually compresses it
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Compressing U8 archive");
                    U8folder.Lz77Compress = true;
                }

                if (Quiet.quiet > 2)
                    System.Console.WriteLine("Saving file");

                U8folder.Save(output);
                
                if (Quiet.quiet > 1)
                    System.Console.WriteLine("Operation completed succesfully!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An unknown error occured, please try again");
                System.Console.WriteLine("");
                System.Console.WriteLine("ERROR DETAILS: {0}", ex.Message);
                return;
            }
        }

        public static void U8_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - U8 - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe U8 [-p | -u] input output [arguments]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       input          The input file/folder");
            System.Console.WriteLine("       output         The output file/folder");
            System.Console.WriteLine("       -p             Pack");
            System.Console.WriteLine("       -u             Unpack");
            System.Console.WriteLine("");
            System.Console.WriteLine("    Arguments for Packing:");
            System.Console.WriteLine("");
            System.Console.WriteLine("         -imet [title]  Pack with an IMET header (for 00000000.app)");
            System.Console.WriteLine("                        You MUST enter a channel title");
            System.Console.WriteLine("         -imd5          Pack with an IMD5 header (for Banner/Icon.bin)");
            System.Console.WriteLine("         -lz77          Compress with lz77");
        }
    }
}