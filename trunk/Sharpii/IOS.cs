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
    partial class IOS_Stuff
    {
        public static void IOS(string[] args)
        {
            if (args.Length < 3)
            {
                IOS_help();
                return;
            }
            string input = args[1];
            string output = "";
            bool fs = false;
            bool es = false;
            bool np = false;
            bool vp = false;
            int slot = -1;
            int version = -1;

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
                    case "-FS":
                        fs = true;
                        break;
                    case "-ES":
                        es = true;
                        break;
                    case "-NP":
                        np = true;
                        break;
                    case "-VP":
                        vp = true;
                        break;
                    case "-SLOT":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No slot set");
                            return;
                        }
                        if (!int.TryParse(args[i + 1], out slot))
                        { 
                            Console.WriteLine("Invalid slot {0}...", args[i + 1]); 
                            return; 
                        }
                        if (slot < 3 || slot > 255)
                        { 
                            Console.WriteLine("Invalid slot {0}...", slot); 
                            return; 
                        }
                        break;
                    case "-S":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No slot set");
                            return;
                        }
                        if (!int.TryParse(args[i + 1], out slot))
                        {
                            Console.WriteLine("Invalid slot {0}...", args[i + 1]);
                            return;
                        }
                        if (slot < 3 || slot > 255)
                        {
                            Console.WriteLine("Invalid slot {0}...", slot);
                            return;
                        }
                        break;
                    case "-V":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No version set");
                            return;
                        }
                        if (!int.TryParse(args[i + 1], out version))
                        { 
                            Console.WriteLine("Invalid version {0}...", args[i + 1]); 
                            return; 
                        }
                        if (version < 0 || version > 65535)
                        { 
                            Console.WriteLine("Invalid version {0}...", version); 
                            return; 
                        }
                        break;
                    case "-O":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No output set");
                            return;
                        }
                        output = args[i + 1];
                        break;
                }
            }

            //Main part (most of it was borrowed from PatchIOS)
            try
            {
                WAD ios = new WAD();
                ios.KeepOriginalFooter = true;

                if (Quiet.quiet > 2)
                    System.Console.Write("Loading File...");
                
                ios.LoadFile(input);
                
                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                //Check if WAD is an IOS
                if ((ios.TitleID >> 32) != 1 || (ios.TitleID & 0xffffffff) > 255 || (ios.TitleID & 0xffffffff) < 3)
                {
                    Console.WriteLine("Only IOS WADs can be patched...");
                    return;
                }

                IosPatcher patcher = new IosPatcher();

                patcher.LoadIOS(ref ios);

                //apply patches
                if (fs == true)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Applying Fakesigning patch");
                    patcher.PatchFakeSigning();
                }

                if (es == true)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Applying ES_Identify patch");
                    patcher.PatchEsIdentify();
                }

                if (np == true)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Applying NAND permissions patch");
                    patcher.PatchNandPermissions();
                }

                if (vp == true)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Applying Version patch");
                    patcher.PatchVP();
                }

                if (slot > -1 || version > -1)
                    ios.FakeSign = true;

                if (slot > -1)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Changing IOS slot to: {0}", slot);
                    ios.TitleID = (ulong)((1UL << 32) | (uint)slot);
                }

                if (version > -1)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Changing title version to: {0}", version);
                    ios.TitleVersion = (ushort)version;
                }

                //check if output was set
                if (output != "")
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Saving to file: {0}", output);
                    ios.Save(output);
                }
                else
                {
                    if (Quiet.quiet > 2)
                        System.Console.Write("Saving file...");

                    if (output != "")
                    {
                        if (output.Substring(output.Length - 4, 4).ToUpper() != ".WAD")
                            output = output + ".wad";
                    }

                    ios.Save(input);

                    if (Quiet.quiet > 2)
                        System.Console.Write("Done!\n");
                }
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

            return;

        }

        public static void IOS_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - IOS - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("                    Code based off PatchIOS by leathl");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe IOS input [-o output] [-fs] [-es] [-np] [-vp] [-s slot]");
            System.Console.WriteLine("                             [-v version]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       input          The input file");
            System.Console.WriteLine("       -o output      The output file");
            System.Console.WriteLine("       -fs            Patch Fakesigning");
            System.Console.WriteLine("       -es            Patch ES_Identify");
            System.Console.WriteLine("       -np            Patch NAND Permissions");
            System.Console.WriteLine("       -vp            Add version patch");
            System.Console.WriteLine("       -s #           Change IOS slot to #");
            System.Console.WriteLine("       -v #           Change IOS version to #");
        }
    }
}