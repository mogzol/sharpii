/* This file is part of Sharpii.
 * Copyright (C) 2013 Person66
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
    partial class BNS_Stuff
    {
        public static void BNS(string[] args)
        {
            if (args.Length < 4)
            {
                Wav2BNS_help();
                return;
            }

            //********************* To BNS *********************
            if (args[1] == "-to")
            {
                To(args);
                return;
            }

            if (args[1] == "-from")
            {
                From(args);
                return;
            }

            return;
        }

        private static void From(string[] args)
        {
            string input = args[2];
            string output = args[3];

            //Check if file exists
            if (File.Exists(input) == false)
            {
                Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }

            //Run main part, and check for exceptions
            try
            {
                //Now convert it
                if (Quiet.quiet > 2)
                    Console.Write("Loading file...");

                Wave WavFile = libWiiSharp.BNS.BnsToWave(input);

                if (Quiet.quiet > 2)
                    Console.Write("Done!\n");

                if (Quiet.quiet > 2)
                    Console.Write("Saving wav...");

                if (output.Substring(output.Length - 4, 4).ToUpper() != ".WAV")
                    output = output + ".wav";

                WavFile.Save(output);

                if (Quiet.quiet > 2)
                    Console.Write("Done!\n");

                if (Quiet.quiet > 1)
                    Console.WriteLine("Operation completed succesfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unknown error occured, please try again");
                Console.WriteLine("");
                Console.WriteLine("ERROR DETAILS: {0}", ex.Message);
                return;
            }
        }

        private static void To(string[] args)
        {
            string input = args[2];
            string output = args[3];
            bool loop = false;
            bool mono = false;

            //Check if file exists
            if (File.Exists(input) == false)
            {
                Console.WriteLine("ERROR: Unable to open file: {0}", input);
                return;
            }

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i].ToUpper())
                {
                    case "-L":
                        loop = true;
                        break;
                    case "-LOOP":
                        loop = true;
                        break;
                    case "-M":
                        mono = true;
                        break;
                    case "-MONO":
                        mono = true;
                        break;
                }
            }

            //Run main part, and check for exceptions
            try
            {
                if (Quiet.quiet > 2)
                    Console.Write("Loading file...");

                BNS WavFile = new BNS(input);

                if (Quiet.quiet > 2)
                    Console.Write("Done!\n");

                if (loop == true)
                {
                    if (Quiet.quiet > 2)
                        Console.WriteLine("Applying loop");
                    WavFile.SetLoop(1);
                }
                if (mono == true)
                {
                    if (Quiet.quiet > 2)
                        Console.WriteLine("Converting to mono");
                    WavFile.StereoToMono = true;
                }

                if (Quiet.quiet > 2)
                    Console.Write("Saving BNS...");

                WavFile.Convert();

                if (output.Substring(output.Length - 4, 4).ToUpper() != ".BNS")
                    output = output + ".bns";

                WavFile.Save(output);

                if (Quiet.quiet > 2)
                    Console.Write("Done!\n");

                if (Quiet.quiet > 1)
                    Console.WriteLine("Operation completed succesfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unknown error occured, please try again");
                Console.WriteLine("");
                Console.WriteLine("ERROR DETAILS: {0}", ex.Message);
                return;
            }
        }

        public static void Wav2BNS_help()
        {
            Console.WriteLine("");
            Console.WriteLine("Sharpii {0} - BNS - A tool by person66, using libWiiSharp.dll by leathl", Version.version);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("  Usage:");
            Console.WriteLine("");
            Console.WriteLine("       Sharpii BNS [-to | -from] input.wav output.bns [-l/-loop] [-m/-mono]");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("  Arguments:");
            Console.WriteLine("");
            Console.WriteLine("       -to            Convert wav to bns");
            Console.WriteLine("       -from          Create wav from bns");
            Console.WriteLine("       input.wav      The input wave sound file");
            Console.WriteLine("       output.bns     The output BNS sound file");
            Console.WriteLine("");
            Console.WriteLine("    Arguments for converting to BNS:");
            Console.WriteLine("");
            Console.WriteLine("         -l | -loop     Creates a looping BNS");
            Console.WriteLine("         -m | -mono     Convert stereo sound to mono BNS");
        }
    }
}
