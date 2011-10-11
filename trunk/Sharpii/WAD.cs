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
    partial class WAD_Stuff
    {
        public static void WAD(string[] args)
        {
            if (args.Length < 3)
            {
                WAD_help();
                return;
            }
            
            //********************* PACK *********************
            if (args[1] == "-p")
            {
                if (args.Length < 4)
                {
                    WAD_help();
                    return;
                }

                Editor(args, false);
                return;
            }

            //********************* UNPACK *********************
            if (args[1] == "-u")
            {
                if (args.Length < 4)
                {
                    WAD_help();
                    return;
                }

                Unpack(args);
                return;
            }

            //********************* EDIT *********************
            if (args[1] == "-e")
            {
                if (args.Length < 4)
                {
                    WAD_help();
                    return;
                }

                Editor(args, true);           
                return;
            }

            //********************* INFO *********************
            if (args[1] == "-i")
            {
                Info(args);
                return;
            }

            //If tuser gets here, they entered something wrong
            System.Console.WriteLine("ERROR: The argument {0} is invalid", args[1]);

            return;
        }

        private static void Info(string[] args)
        {
            string input = args[2];
            string output = "";
            bool titles = false;

            //Check if file exists
            if (File.Exists(input) == false)
            {
                System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-O":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No output set");
                            return;
                        }
                        output = args[i + 1];
                        break;
                    case "-OUTPUT":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No output set");
                            return;
                        }
                        output = args[i + 1];
                        break;
                    case "-TITLES":
                        titles = true;
                        break;
                }
            }

            try
            {
                WAD wad = new WAD();

                if (Quiet.quiet > 2)
                    System.Console.Write("Loading file...");

                wad.LoadFile(input);

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                if (Quiet.quiet > 1 && output == "")
                {
                    System.Console.WriteLine("WAD Info:");
                    System.Console.WriteLine("");
                    if (titles == false)
                        System.Console.WriteLine("Title: {0}", wad.ChannelTitles[1]);
                    else
                    {
                        System.Console.WriteLine("Titles:\n");
                        System.Console.WriteLine("   Japanese: {0}", wad.ChannelTitles[0]);
                        System.Console.WriteLine("   English: {0}", wad.ChannelTitles[1]);
                        System.Console.WriteLine("   German: {0}", wad.ChannelTitles[2]);
                        System.Console.WriteLine("   French: {0}", wad.ChannelTitles[3]);
                        System.Console.WriteLine("   Spanish: {0}", wad.ChannelTitles[4]);
                        System.Console.WriteLine("   Italian: {0}", wad.ChannelTitles[5]);
                        System.Console.WriteLine("   Dutch: {0}", wad.ChannelTitles[6]);
                        System.Console.WriteLine("   Korean: {0}\n", wad.ChannelTitles[7]);
                    }
                    System.Console.WriteLine("Title ID: {0}", wad.UpperTitleID);
                    System.Console.WriteLine("IOS: {0}", ((int)wad.StartupIOS).ToString());
                    System.Console.WriteLine("Region: {0}", wad.Region);
                    System.Console.WriteLine("Version: {0}", wad.TitleVersion);
                    System.Console.WriteLine("Blocks: {0}", wad.NandBlocks);
                }
                else
                {
                    if (Quiet.quiet > 2)
                        System.Console.Write("Saving file...");

                    if (output.Substring(output.Length - 4, 4).ToUpper() != ".TXT")
                        output = output + ".txt";
                    
                    TextWriter txt = new StreamWriter(output);
                    txt.WriteLine("WAD Info:");
                    txt.WriteLine("");
                    if (titles == false)
                        txt.WriteLine("Title: {0}", wad.ChannelTitles[1]);
                    else
                    {
                        txt.WriteLine("Titles:");
                        txt.WriteLine("     Japanese: {0}", wad.ChannelTitles[0]);
                        txt.WriteLine("     English: {0}", wad.ChannelTitles[1]);
                        txt.WriteLine("     German: {0}", wad.ChannelTitles[2]);
                        txt.WriteLine("     French: {0}", wad.ChannelTitles[3]);
                        txt.WriteLine("     Spanish: {0}", wad.ChannelTitles[4]);
                        txt.WriteLine("     Italian: {0}", wad.ChannelTitles[5]);
                        txt.WriteLine("     Dutch: {0}", wad.ChannelTitles[6]);
                        txt.WriteLine("     Korean: {0}", wad.ChannelTitles[7]);
                    }
                    txt.WriteLine("Title ID: {0}", wad.UpperTitleID);
                    txt.WriteLine("IOS: {0}", ((int)wad.StartupIOS).ToString());
                    txt.WriteLine("Region: {0}", wad.Region);
                    txt.WriteLine("Version: {0}", wad.TitleVersion);
                    txt.WriteLine("Blocks: {0}", wad.NandBlocks);
                    txt.Close();
                    
                    if (Quiet.quiet > 2)
                        System.Console.Write("Done!\n");

                    if (Quiet.quiet > 1)
                        System.Console.WriteLine("Operation completed succesfully!");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An unknown error occured, please try again");
                System.Console.WriteLine("");
                System.Console.WriteLine("ERROR DETAILS: {0}", ex.Message);
                return;
            }
        }

        private static void Editor(string[] args, bool edit)
        {
            //Setting up variables
            string input = args[2];
            string output = args[3];
            string id = "";
            int ios = -1;
            string title = "";
            string lwrid = "";
            bool fake = false;
            string sound = "";
            string banner = "";
            string icon = "";
            string app = "";

            //Check if file/folder exists
            if (edit == true)
                if (File.Exists(input) == false)
                {
                    System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                    return;
                }
            if (edit == false)
                if (Directory.Exists(input) == false)
                {
                    System.Console.WriteLine("ERROR: Unable to open folder: {0}", input);
                    return;
                }

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-F":
                        fake = true;
                        break;
                    case "-ID":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No ID set");
                            return;
                        }
                        id = args[i + 1];
                        if (id.Length < 4)
                        {
                            System.Console.WriteLine("ERROR: ID too short");
                            return;
                        }
                        id = id.Substring(0, 4);
                        break;
                    case "-TYPE":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No type set");
                            return;
                        }
                        lwrid = args[i + 1];
                        if (args[i + 1].ToUpper() != "CHANNEL" && args[i + 1].ToUpper() != "DLC" && args[i + 1].ToUpper() != "GAMECHANNEL" && args[i + 1].ToUpper() != "HIDDENCHANNELS" && args[i + 1].ToUpper() != "SYSTEMCHANNELS" && args[i + 1].ToUpper() != "SYSTEMTITLES")
                        {
                            System.Console.WriteLine("ERROR: Unknown WAD type: {0}", args[i + 1]);
                            return;
                        }
                        break;
                    case "-IOS":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No type set");
                            return;
                        }
                        if (!int.TryParse(args[i + 1], out ios))
                        {
                            Console.WriteLine("Invalid slot {0}...", args[i + 1]);
                            return;
                        }
                        if (ios < 0 || ios > 255)
                        {
                            Console.WriteLine("Invalid slot {0}...", ios);
                            return;
                        }
                        break;
                    case "-TITLE":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No title set");
                            return;
                        }
                        title = args[i + 1];
                        break;
                    case "-SOUND":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No sound set");
                            return;
                        }
                        sound = args[i + 1];
                        if (File.Exists(sound) == false)
                        {
                            Console.WriteLine("ERROR: Unable to find sound wad");
                            return;
                        }
                        break;
                    case "-BANNER":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No banner set");
                            return;
                        }
                        banner = args[i + 1];
                        if (File.Exists(banner) == false)
                        {
                            Console.WriteLine("ERROR: Unable to find banner wad");
                            return;
                        }
                        break;
                    case "-ICON":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No sound set");
                            return;
                        }
                        icon = args[i + 1];
                        if (File.Exists(icon) == false)
                        {
                            Console.WriteLine("ERROR: Unable to find icon wad");
                            return;
                        }
                        break;
                    case "-DOL":
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("ERROR: No dol set");
                            return;
                        }
                        app = args[i + 1];
                        if (File.Exists(app) == false)
                        {
                            Console.WriteLine("ERROR: Unable to find dol wad");
                            return;
                        }
                        break;
                }
            }

            //Run main part, and check for exceptions
            try
            {
                WAD wad = new WAD();

                if (edit == true)
                {
                    if (Quiet.quiet > 2)
                        System.Console.Write("Loading file...");
                    wad.LoadFile(input);
                }
                else
                {
                    if (Quiet.quiet > 2)
                        System.Console.Write("Loading folder...");
                    wad.CreateNew(input);
                }

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                if (sound != "" || banner != "" || icon != "" || app != "")
                {
                    string temp = Path.GetTempPath() + "Sharpii.tmp";
                    if (Directory.Exists(temp) == true)
                        DeleteDir.DeleteDirectory(temp);

                    Directory.CreateDirectory(temp);

                    wad.Unpack(temp + "\\main");
                    U8 u = new U8();
                    u.LoadFile(temp + "\\main\\00000000.app");
                    u.Extract(temp + "\\main\\00000000");

                    WAD twad = new WAD();

                    if (sound != "")
                    {
                        if (Quiet.quiet > 2)
                            System.Console.Write("Grabbing sound...");

                        twad.LoadFile(sound);
                        twad.Unpack(temp + "\\sound");
                        U8 tu = new U8();
                        tu.LoadFile(temp + "\\sound\\00000000.app");
                        tu.Extract(temp + "\\sound\\00000000");

                        File.Copy(temp + "\\sound\\00000000\\meta\\sound.bin", temp + "\\main\\00000000\\meta\\sound.bin", true);

                        if (Quiet.quiet > 2)
                            System.Console.Write("Done!\n");
                    }
                    if (banner != "")
                    {
                        if (Quiet.quiet > 2)
                            System.Console.Write("Grabbing banner...");

                        twad.LoadFile(banner);
                        twad.Unpack(temp + "\\banner");
                        U8 tu = new U8();
                        tu.LoadFile(temp + "\\banner\\00000000.app");
                        tu.Extract(temp + "\\banner\\00000000");

                        File.Copy(temp + "\\banner\\00000000\\meta\\banner.bin", temp + "\\main\\00000000\\meta\\banner.bin", true);

                        if (Quiet.quiet > 2)
                            System.Console.Write("Done!\n");
                    }
                    if (icon != "")
                    {
                        if (Quiet.quiet > 2)
                            System.Console.Write("Grabbing icon...");

                        twad.LoadFile(icon);
                        twad.Unpack(temp + "\\icon");
                        U8 tu = new U8();
                        tu.LoadFile(temp + "\\icon\\00000000.app");
                        tu.Extract(temp + "\\icon\\00000000");

                        File.Copy(temp + "\\icon\\00000000\\meta\\icon.bin", temp + "\\main\\00000000\\meta\\icon.bin", true);

                        if (Quiet.quiet > 2)
                            System.Console.Write("Done!\n");
                    }
                    if (app != "")
                    {
                        if (Quiet.quiet > 2)
                            System.Console.Write("Grabbing dol...");

                        twad.LoadFile(app);
                        twad.Unpack(temp + "\\dol");

                        File.Copy(temp + "\\dol\\00000001.app", temp + "\\main\\00000001.app", true);

                        if (Quiet.quiet > 2)
                            System.Console.Write("Done!\n");
                    }
                    u.ReplaceFile(1, temp + "\\main\\00000000\\meta\\banner.bin");
                    u.ReplaceFile(2, temp + "\\main\\00000000\\meta\\icon.bin");
                    u.ReplaceFile(3, temp + "\\main\\00000000\\meta\\sound.bin");
                    u.Save(temp + "\\main\\00000000.app");
                    DeleteDir.DeleteDirectory(temp + "\\main\\00000000\\");
                    wad.CreateNew(temp + "\\main");
                    DeleteDir.DeleteDirectory(temp);
                }

                if (Quiet.quiet > 2 && fake == true)
                    System.Console.WriteLine("FakeSigning WAD");
                wad.FakeSign = fake;

                if (id != "")
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Changing channel ID to: {0}", id);

                    if (lwrid != "")
                    {
                        if (Quiet.quiet > 2)
                            System.Console.WriteLine("Changing channel type to: {0}", lwrid);
                    }
                    else
                    {
                        lwrid = "CHANNEL";
                    }

                    if (lwrid == "CHANNEL")
                        wad.ChangeTitleID(LowerTitleID.Channel, id);
                    if (lwrid == "DLC")
                        wad.ChangeTitleID(LowerTitleID.DLC, id);
                    if (lwrid == "GAMECHANNEL")
                        wad.ChangeTitleID(LowerTitleID.GameChannel, id);
                    if (lwrid == "HIDDENCHANNELS")
                        wad.ChangeTitleID(LowerTitleID.HiddenChannels, id);
                    if (lwrid == "SYSTEMCHANNELS")
                        wad.ChangeTitleID(LowerTitleID.SystemChannels, id);
                    if (lwrid == "SYSTEMTITLES")
                        wad.ChangeTitleID(LowerTitleID.SystemTitles, id);
                }
                if (ios > -1)
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Changing startup IOS to: {0}", ios);
                    wad.ChangeStartupIOS(ios);
                }
                if (title != "")
                {
                    if (Quiet.quiet > 2)
                        System.Console.WriteLine("Changing channel title to: {0}", title);
                    wad.ChangeChannelTitles(title);
                }

                if (Quiet.quiet > 2)
                    System.Console.Write("Saving file...");

                if (output.Substring(output.Length - 4, 4).ToUpper() != ".WAD")
                    output = output + ".wad";

                wad.Save(output);

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

        private static void Unpack(string[] args)
        {
            //setting up variables
            string input = args[2];
            string output = args[3];
            bool cid = false;

            //Check if file exists
            if (File.Exists(input) == false)
            {
                System.Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }

            //-cid argument
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-CID":
                        cid = true;
                        break;
                }
            }

            //Run main part, and check for exceptions
            try
            {
                WAD wad = new WAD();

                if (Quiet.quiet > 2)
                    System.Console.Write("Loading file...");

                wad.LoadFile(input);

                if (Quiet.quiet > 2)
                    System.Console.Write("Done!\n");

                if (Quiet.quiet > 2)
                    System.Console.Write("Unpacking WAD...");

                wad.Unpack(output, cid);

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

        public static void WAD_help()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Sharpii {0} - WAD - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Usage:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       Sharpii.exe WAD [-p | -u | -e | -i] input output [arguments]");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            System.Console.WriteLine("  Arguments:");
            System.Console.WriteLine("");
            System.Console.WriteLine("       input          The input file/folder");
            System.Console.WriteLine("       output         The output file/folder");
            System.Console.WriteLine("       -p             Pack WAD");
            System.Console.WriteLine("       -u             Unpack WAD");
            System.Console.WriteLine("       -e             Edit WAD");
            System.Console.WriteLine("       -i             Get WAD info");
            System.Console.WriteLine("");
            System.Console.WriteLine("    Arguments for unpacking:");
            System.Console.WriteLine("");
            System.Console.WriteLine("         -cid           Use Content ID as name");
            System.Console.WriteLine("");
            System.Console.WriteLine("    Arguments for info:");
            System.Console.WriteLine("");
            System.Console.WriteLine("         -o output      Output info to text file");
            System.Console.WriteLine("         -titles        Display titles in all languages");
            System.Console.WriteLine("");
            System.Console.WriteLine("    Arguments for packing/editing:");
            System.Console.WriteLine("");
            System.Console.WriteLine("         -id [TitleID]  Change the 4-character title id");
            System.Console.WriteLine("         -ios [IOS]     Change the Startup IOS");
            System.Console.WriteLine("         -title [title] Change the Channel name/title.");
            System.Console.WriteLine("                        If there are spaces, surround in quotes");
            System.Console.WriteLine("         -f             Fakesign the WAD");
            System.Console.WriteLine("         -type [type]   Change the Channel type. Possible values are:");
            System.Console.WriteLine("                        Channel, DLC, GameChannel, HiddenChannels,");
            System.Console.WriteLine("                        SystemChannels, or SystemTitles");
            System.Console.WriteLine("         -sound [wad]   Use the sound from 'wad'");
            System.Console.WriteLine("         -banner [wad]  Use the banner from 'wad'");
            System.Console.WriteLine("         -icon [wad]    Use the icon from 'wad'");
            System.Console.WriteLine("         -dol [wad]     Use the dol from 'wad'");
        }
    }
}