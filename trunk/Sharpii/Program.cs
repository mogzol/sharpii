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
    class MainApp
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                help();
                return;
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

            string Function = args[0];

            if (Function.ToUpper() == "-H" || Function.ToUpper() == "-HELP" || Function.ToUpper() == "H" || Function.ToUpper() == "HELP")
            {
                help();
                Environment.Exit(0);
            }

            if (Function.ToUpper() == "BNS")
            {
                BNS_Stuff.BNS(args);
                Environment.Exit(0);
            }

            if (Function.ToUpper() == "WAD")
            {
                WAD_Stuff.WAD(args);
                Environment.Exit(0);
            }

            if (Function.ToUpper() == "TPL")
            {
                TPL_Stuff.TPL(args);
                Environment.Exit(0);
            }

            if (Function.ToUpper() == "U8")
            {
                U8_Stuff.U8(args);
                Environment.Exit(0);
            }

            if (Function.ToUpper() == "IOS")
            {
                IOS_Stuff.IOS(args);
                Environment.Exit(0);
            }

            if (Function.ToUpper() == "SENDDOL" || Function.ToUpper() == "SENDOL")
            {
                HBC_Stuff.SendDol(args);
                Environment.Exit(0);
            }



            //If tuser gets here, they entered something wrong
            System.Console.WriteLine("ERROR: The argument {0} is invalid", args[0]);

            Environment.Exit(0);
        }

        private static void help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii [function] [parameters] [-quiet | -q | -lots]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Functions:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       BNS            Convert a wav to bns, or vice versa");
            System.Console.WriteLine("       WAD            Pack/Unpack/Edit a wad file");
            System.Console.WriteLine("       TPL            Convert a image to a tpl, or vice versa");
            System.Console.WriteLine("       U8             Pack/Unpack a U8 archive");
            System.Console.WriteLine("       IOS            Apply various patches to an IOS");
            System.Console.WriteLine("       SendDol        Send a dol to the HBC over wifi");
            System.Console.WriteLine("");
            System.Console.WriteLine("       NOTE: Too see more detailed descriptions of any of the above,");
            System.Console.WriteLine("             use 'Sharpii [function] -h'");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Global Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       -quiet | -q    Try not to display any output");
            System.Console.WriteLine("       -lots          Display lots of output");
            System.Console.WriteLine("");


        }

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
        public static string version = "1.0";
    }