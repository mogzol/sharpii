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
using System.Collections.Generic;
using libWiiSharp;

namespace Sharpii
{
    partial class NUS_Stuff
    {
        public static void NUS(string[] args)
        {
            if (args.Length < 2)
            {
                NUS_help();
                return;
            }
            if (args[1].ToUpper() == "-H" || args[1].ToUpper() == "-HELP")
            {
                NUS_help();
                return;
            }

            //Set up variables
            string id = "";
            int version = -1;
            string output = "";
            bool local = false;
            List<StoreType> store = new List<StoreType>();
            bool entered = false;

            //Get arguments
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-ALL":
                        store.Add(StoreType.All);
                        entered = true;
                        break;
                    case "-WAD":
                        store.Add(StoreType.WAD);
                        entered = true;
                        break;
                    case "-ENCRYPT":
                        store.Add(StoreType.EncryptedContent);
                        entered = true;
                        break;
                    case "-ENCRYPTED":
                        store.Add(StoreType.EncryptedContent);
                        entered = true;
                        break;
                    case "-DECRYPT":
                        store.Add(StoreType.DecryptedContent);
                        entered = true;
                        break;
                    case "-DECRYPTED":
                        store.Add(StoreType.DecryptedContent);
                        entered = true;
                        break;
                    case "-LOCAL":
                        local = true;
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
                    case "-VERSION":
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
                    case "-ID":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No ID specified");
                            return;
                        }
                        id = args[i + 1];
                        break;
                }
            }

            //Error checking
            if (id == "")
            {
                System.Console.WriteLine("ERROR: No ID specified");
                return;
            }

            if (version == -1)
            {
                System.Console.WriteLine("ERROR: No version specified");
                return;
            }

            if (output == "")
            {
                output = id + "v" + version;
                if (Quiet.quiet > 2)
                    System.Console.WriteLine("No output specified, using {0}", output);
            }

            if (entered == false) //Will only be false if no store type argument was given
            {
                store.Add(StoreType.All);
                if (Quiet.quiet > 2)
                    System.Console.WriteLine("No store type specified, using all");
            }

            //Main part, catches random/unexpected exceptions
            try
            {
                NusClient nus = new NusClient();

                if (local == true)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Using local files if present...");
                    nus.UseLocalFiles = true;
                }
                
                if (Quiet.quiet > 1)
                    System.Console.Write("Downloading title...");

                nus.DownloadTitle(id, version.ToString(), output, store.ToArray());
                
                if (Quiet.quiet > 1)
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

            return;

        }

        public static void NUS_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - NUSD - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe NUSD [-id titleID] [-v version] [-o otput] [-all] [-wad]");
            System.Console.WriteLine("                        [-decrypt] [-encrypt] [-local]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       -id titleID    [required] The Title ID of the file you wish to download");
            System.Console.WriteLine("       -v version     [required] The version of the file you wish to download");
            System.Console.WriteLine("       -o output      Folder to output the files to");
            System.Console.WriteLine("       -local         Use local files if present");
            System.Console.WriteLine("       -all           Create and keep encrypted, decrypted, and WAD versions");
            System.Console.WriteLine("                      of the file you wish to download");
            System.Console.WriteLine("       -wad           Keep only the WAD version of the file you wish to");
            System.Console.WriteLine("                      download");
            System.Console.WriteLine("       -decrypt       Keep only the Decrypted contents of the file you wish to");
            System.Console.WriteLine("                      download");
            System.Console.WriteLine("       -encrypt       Keep only the Encrypted contents of the file you wish to");
            System.Console.WriteLine("                      download");
        }
    }
}