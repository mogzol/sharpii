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
    partial class TPL_Stuff
    {
        public static void TPL(string[] args)
        {
            if (args.Length < 4)
            {
                TPL_help();
                return;
            }

            //********************* To TPL *********************
            if (args[1] == "-to")
            {
                To(args);
                return;
            }

            //********************* From TPL *********************
            if (args[1] == "-from")
            {
                From(args);            
                return;
            }

            //If tuser gets here, they entered something wrong
            System.Console.WriteLine("ERROR: The argument {0} is invalid", args[1]);
            return;

        }

        private static void From(string[] args)
        {
            string input = args[2];
            string output = args[3];

            //Check if file exists
            if (File.Exists(input) == false)
            {
                System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }

            //Run main part, and check for exceptions
            try
            {
                //Load tpl
                if (Quiet.quiet > 2)
                    System.Console.Write("Loading file...");

                TPL tplfile = libWiiSharp.TPL.Load(input);

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                //save image
                if (Quiet.quiet > 2)
                    System.Console.Write("Extracting texture...");

                tplfile.ExtractTexture(output);

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

        private static void To(string[] args)
        {
            //Setting up variables
            string input = args[2];
            string output = args[3];
            string tplFormat = "RGB565";

            //Check if file exists
            if (File.Exists(input) == false)
            {
                System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }

            //Check for arguments
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-FORMAT":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No format set");
                            return;
                        }
                        tplFormat = args[i + 1];
                        break;
                    case "-F":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No format set");
                            return;
                        }
                        tplFormat = args[i + 1];
                        break;
                }
            }

            //Check if valid format was entered
            if (tplFormat != "I4" & tplFormat != "I8" & tplFormat != "IA4" & tplFormat != "IA8" & tplFormat != "RGB565" & tplFormat != "RGB5A3" & tplFormat != "RGBA8")
            {
                System.Console.WriteLine("ERROR: Unknown format type: {0}", tplFormat);
                return;
            }


            //Run main part, and check for exceptions
            try
            {
                //Set format
                TPL_TextureFormat format = TPL_TextureFormat.RGB565;
                if (tplFormat == "I4")
                    format = TPL_TextureFormat.I4;
                if (tplFormat == "I8")
                    format = TPL_TextureFormat.I8;
                if (tplFormat == "IA4")
                    format = TPL_TextureFormat.IA4;
                if (tplFormat == "IA8")
                    format = TPL_TextureFormat.IA8;
                if (tplFormat == "RGB565")
                    format = TPL_TextureFormat.RGB565;
                if (tplFormat == "RGB5A3")
                    format = TPL_TextureFormat.RGB5A3;
                if (tplFormat == "RGBA8")
                    format = TPL_TextureFormat.RGBA8;

                if (Quiet.quiet > 2)
                    System.Console.WriteLine("Format set to: {0}", tplFormat);

                //Make tpl
                if (Quiet.quiet > 2)
                    System.Console.Write("Creating tpl file...");

                TPL tplfile = libWiiSharp.TPL.FromImage(input, format);

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                //save
                if (Quiet.quiet > 2)
                    System.Console.Write("Saving tpl file...");

                if (output.Substring(output.Length - 4, 4).ToUpper() != ".TPL")
                    output = output + ".tpl";

                tplfile.Save(output);

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

        public static void TPL_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - TPL - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe TPL [-to | -from] input output [arguments]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       -to            Convert image to tpl");
            System.Console.WriteLine("       -from          Create image from tpl");
            System.Console.WriteLine("       input          The input file/folder");
            System.Console.WriteLine("       output         The output file/folder");
            System.Console.WriteLine("");
            System.Console.WriteLine("    Arguments for Converting to TPL:");
            System.Console.WriteLine("");
            System.Console.WriteLine("         -format | -f   The format of the tpl. Possible values are:");
            System.Console.WriteLine("                          RGBA8     (High Quality with Alpha)");
            System.Console.WriteLine("                          RGB565    (Medium Quality without Alpha)");
            System.Console.WriteLine("                          RGB5A3    (Low Quality with Alpha)");
            System.Console.WriteLine("                          IA8       (High quality B/W with Alpha)");
            System.Console.WriteLine("                          IA4       (Low Quality B/W with Alpha)");
            System.Console.WriteLine("                          I8        (High Quality B/W without Alpha)");
            System.Console.WriteLine("                          I4        (Low Quality B/W without Alpha)");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Notes:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       If no format is specified when converting to TPL, RGB565 is used.");
            System.Console.WriteLine("       When converting to an image, the image format is chosen based on the extension");
        }
    }
}