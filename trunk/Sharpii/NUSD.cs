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
            string content = "";
            string version = "";
            int intver = -1;
            string output = "";
            bool local = false;
            List<StoreType> store = new List<StoreType>();
            bool entered = false;
            bool wad = false;
            bool NoOut = false;
            string ios = "";
            string temp = Path.GetTempPath() + "Sharpii.tmp";

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
                        version = args[i + 1];
                        if (version.ToUpper() == "LATEST")
                            break;
                        if (!int.TryParse(version, out intver))
                        {
                            Console.WriteLine("Invalid version {0}...", args[i + 1]);
                            return;
                        }
                        if (intver < 0 || intver > 65535)
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
                        version = args[i + 1];
                        if (version.ToUpper() == "LATEST")
                            break;
                        if (!int.TryParse(version, out intver))
                        {
                            Console.WriteLine("Invalid version {0}...", args[i + 1]);
                            return;
                        }
                        if (intver < 0 || intver > 65535)
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
                    case "-IOS":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No IOS specified");
                            return;
                        }
                        id = "00000001000000" + Convert.ToInt32(args[i + 1]).ToString("X2");
                        break;
                    case "-SINGLE":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No ID specified");
                            return;
                        }
                        content = args[i + 1];
                        break;
                    case "-S":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No ID specified");
                            return;
                        }
                        content = args[i + 1];
                        break;
                }
            }

            //Error checking & stuff
            if (id == "")
            {
                System.Console.WriteLine("ERROR: No ID specified");
                return;
            }

            if (version == "")
            {
                if (Quiet.quiet > 2)
                    System.Console.WriteLine("No version specified, using latest", version);
                version = "LATEST";
            }

            if (version.ToUpper() == "LATEST")
            {
                //Grab the TMD and get the latest version
                NusClient grabtmd = new NusClient();
                TMD tmd = grabtmd.DownloadTMD(id, "");
                version = tmd.TitleVersion.ToString();

                if (Quiet.quiet > 2)
                    System.Console.WriteLine("Found latest version: v{0}", version);
            }

            if (entered == false) //Will only be false if no store type argument was given
            {
                store.Add(StoreType.All);
                if (Quiet.quiet > 2)
                    System.Console.WriteLine("No store type specified, using all");
            }

            if (id.Length == 16 && Convert.ToInt32(id.Substring(14, 2), 16) >= 3 && Convert.ToInt32(id.Substring(14, 2), 16) <= 255 && id.Substring(0, 14) == "00000001000000")
                ios = "IOS" + Convert.ToInt32(id.Substring(14, 2), 16) + "-64-" + version + ".wad";

            if ((((output.Length >= 4 && output.Substring(output.Length - 4, 4).ToUpper() == ".WAD") || output == "") && Array.IndexOf(store.ToArray(), StoreType.WAD) != -1 && store.ToArray().Length == 1) || (output == "" && ios != "" && Array.IndexOf(store.ToArray(), StoreType.WAD) != -1 && store.ToArray().Length == 1))
            {
                wad = true;
                if (Directory.Exists(temp) == true)
                    DeleteDir.DeleteDirectory(temp);                

                Directory.CreateDirectory(temp);
            }

            if (output == "")
            {
                NoOut = true;
                output = ios == "" ? id + "v" + version : ios.Substring(0, ios.Length - 4);
                if (Quiet.quiet > 2)
                    System.Console.WriteLine("No output specified, using {0}", output);
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


                if (content != "")
                {
                    if (Quiet.quiet > 1)
                        System.Console.Write("Downloading content...");

                    nus.DownloadSingleContent(id, version, content, output);

                    if (Quiet.quiet > 1)
                        System.Console.Write("Done!\n");
                }
                else
                {
                    if (Quiet.quiet > 1)
                        System.Console.Write("Downloading title...");
                    
                    string realout = output;
                    if (wad == true)
                        output = temp;
                    
                    nus.DownloadTitle(id, version, output, store.ToArray());

                    WadIosNamingStuff(wad, temp, id, version, ios, NoOut, output, realout);

                    if (Quiet.quiet > 1)
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

        private static void WadIosNamingStuff(bool wad, string temp, string id, string version, string ios, bool NoOut, string output, string realout)
        {
            if (wad == true)
            {
                if (!File.Exists(temp + "\\" + id + "v" + version + ".wad"))
                {
                    System.Console.WriteLine("ERROR: Can't find WAD");
                    return;
                }
                if (ios != "" && NoOut == true)
                {
                    int index = realout.LastIndexOf("\\") > realout.LastIndexOf("/") ? realout.LastIndexOf("\\") : realout.LastIndexOf("/");
                    if (File.Exists(realout.Substring(0, index + 1) + ios))
                        File.Delete(realout.Substring(0, index + 1) + ios);
                    File.Move(temp + "\\" + id + "v" + version + ".wad", realout.Substring(0, index + 1) + ios);
                }
                else if (ios == "" && NoOut == true)
                {
                    if (File.Exists(realout + ".wad"))
                        File.Delete(realout + ".wad");
                    File.Move(temp + "\\" + id + "v" + version + ".wad", realout + ".wad");
                }
                else
                {
                    if (File.Exists(realout))
                        File.Delete(realout);
                    File.Move(temp + "\\" + id + "v" + version + ".wad", realout);
                }
                DeleteDir.DeleteDirectory(temp);
            }
            else if (ios != "")
            {
                if (output.Substring(output.Length - 1, 1) == "\\" || output.Substring(output.Length - 1, 1) == "/")
                    output = output.Substring(output.Length - 1, 1);
                if (File.Exists(output + "\\" + id + "v" + version + ".wad"))
                {
                    if (File.Exists(output + "\\" + ios))
                        File.Delete(output + "\\" + ios);
                    File.Move(output + "\\" + id + "v" + version + ".wad", output + "\\" + ios);
                }
            }
        }

        public static void NUS_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - NUSD - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe NUSD [-id titleID | -ios IOS] [-v version] [-o otput] [-all]");
            System.Console.WriteLine("                        [-wad] [-decrypt] [-encrypt] [-local] [-s content]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       -id titleID    [required] The Title ID of the file you wish to download");
            System.Console.WriteLine("       -v version     [required] The version of the file you wish to download");
            System.Console.WriteLine("                      NOTE: Use 'latest' to get the latest version");
            System.Console.WriteLine("       -ios IOS       The IOS you wish to download. This is an alternative to");
            System.Console.WriteLine("                      '-id', use one or the other, but not both.");
            System.Console.WriteLine("       -o output      Folder to output the files to");
            System.Console.WriteLine("       -local         Use local files if present");
            System.Console.WriteLine("       -s content     Download a single content from the file");
            System.Console.WriteLine("                      NOTE: When using this, output MUST have a path and a");
            System.Console.WriteLine("                      filename. For current directory use '.\\[filename]'");
            System.Console.WriteLine("       -all           Create and keep encrypted, decrypted, and WAD versions");
            System.Console.WriteLine("                      of the file you wish to download");
            System.Console.WriteLine("       -wad           Keep only the WAD version of the file you wish to");
            System.Console.WriteLine("                      download");
            System.Console.WriteLine("       -decrypt       Keep only the decrypted contents of the file you wish to");
            System.Console.WriteLine("                      download");
            System.Console.WriteLine("       -encrypt       Keep only the encrypted contents of the file you wish to");
            System.Console.WriteLine("                      download");
        }
    }
}