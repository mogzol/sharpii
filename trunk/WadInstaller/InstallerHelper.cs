/* This file is part of Sharpii.
 * Copyright (C) 2009 WiiCrazy / I.R.on
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
using System.IO.Compression;
using System.Security.Cryptography;

namespace WadInstaller
{
    public class WadInstaller_Plugin
    {
        const string version = "1.0";

        public static string GetVersion()
        {
            return version;
        }
    }

    public class InstallerHelper
    {
        public static MemoryStream CreateInstaller(string wadFile, byte iosToUse)
        {
            return CreateInstaller(File.ReadAllBytes(wadFile), iosToUse);
        }

        public static MemoryStream CreateInstaller(byte[] wadFileBytes, byte iosToUse)
        {
            const int injectionPosition = 0x665FC;
            const int maxAllowedSizeForWads = 8 * 1024 * 1024 - 32; //(Max 4MB-32bytes )

            //0. Read length of the wad to ensure it has an allowed size
            uint wadLength = (uint)wadFileBytes.Length;

            if (wadLength > maxAllowedSizeForWads)
            {
                throw new ArgumentException(String.Format("The file is sized above the max allowed limit of {1} for network installation.", maxAllowedSizeForWads));
            }

            //1. Open the stub installer from resources
            MemoryStream compressedStubInstallerStream = LoadCompressedStubInstaller("WadInstaller.dol.z");
            compressedStubInstallerStream.Seek(0, SeekOrigin.Begin);

            //2. Decompress compressed installer
            MemoryStream uncompressedStubInstallerStream = new MemoryStream();

            using (GZipStream gzipStream = new GZipStream(compressedStubInstallerStream, CompressionMode.Decompress)) 
            {
                byte[] decompressedBuff = new byte[1024];
                while (true) 
                {
                    int length = gzipStream.Read(decompressedBuff, 0, 1024);

                    if (length == 0) break;

                    uncompressedStubInstallerStream.Write(decompressedBuff, 0, length);
                }

            }            

            //3. Take SHA of the wad and store it in the stub installer along with the size of the wad

            byte[] shaHash;
            using (SHA1 shaGen = SHA1.Create())
            {
                shaHash = shaGen.ComputeHash(wadFileBytes);
            }

            //4. Inject the data into the installer

            //Write out the wad size
            uncompressedStubInstallerStream.Seek(injectionPosition, SeekOrigin.Begin);            
            uncompressedStubInstallerStream.WriteByte((byte)((wadLength >> 24) & 0xff));
            uncompressedStubInstallerStream.WriteByte((byte)((wadLength >> 16) & 0xff));
            uncompressedStubInstallerStream.WriteByte((byte)((wadLength >> 8) & 0xff));
            uncompressedStubInstallerStream.WriteByte((byte)(wadLength  & 0xff));

            //Write out the SHA1 value (Against corruption of the file on the network, this value will be checked by the installer)
            uncompressedStubInstallerStream.Write(shaHash, 0, 20);

            //Write out the ios to be used... 
            uncompressedStubInstallerStream.WriteByte(iosToUse);

            //pad it with three zeroes (to align it into 32-bit)
            uncompressedStubInstallerStream.WriteByte(0); uncompressedStubInstallerStream.WriteByte(0); uncompressedStubInstallerStream.WriteByte(0);


            //Write out to be installed wad file's contents...
            uncompressedStubInstallerStream.Write(wadFileBytes, 0, (int)wadLength);

            return uncompressedStubInstallerStream;
        }

        private static MemoryStream LoadCompressedStubInstaller(string installerResourceName)
        {
            using (BinaryReader resLoader = new BinaryReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("WadInstaller.Resources." + installerResourceName)))
            {
                MemoryStream ms = new MemoryStream();
                byte[] temp = resLoader.ReadBytes((int)resLoader.BaseStream.Length);
                ms.Write(temp, 0, temp.Length);
                return ms;
            }
        }
    }
}
