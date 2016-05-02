﻿/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Globalization;

namespace Commons
{
    public class Utility
    {
        public static string ToHexString(MemoryStream stream, bool withSpaces)
        {
            string result = "";
            long position = stream.Position;
            while (stream.Position < stream.Length)
            {
                int b = stream.ReadByte();
                result += string.Format("{0:X2}", b);
                if (withSpaces) result += " ";
            }
            stream.Position = position;
            return result;
        }

        public static string ToHexString(byte[] data, bool withSpaces)
        {
            string result = "";
            foreach (byte b in data)
            {
                result += string.Format("{0:X2}", b);
                if (withSpaces) result += " ";
            }
            return result;
        }

        public static byte[] Reverse(byte[] data)
        {
            Array.Reverse(data);
            return data;
        }

        public static string HexDump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                  8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }

        public static void WriteLegal()
        {
            string text = "This software is a property of The Emulator Nexus Engineering Force.\n";
            text += "Copyright Emulator Nexus (c) 2012\n";
            text += "Any use, exploitation, decompression, decompilation, analysis,\n";
            text += "or generally any attempt to interact with the software in any way\n";
            text += "without prior written agreement from the copyright holders will be\n";
            text += "prosecuted as stated by international copyright protection laws.\n";
            Console.WriteLine(text);
        }

        static public byte[] RSADecrypt(byte[] DataToDecrypt, bool DoOAEPPadding)
        {
            byte[] decryptedData;

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAKey = new RSAParameters();

            byte[] key = new byte[]
                {
                    0x30, 0x82, 0x04, 0xBC, 0x02, 0x01, 0x00, 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 
                    0x0D, 0x01, 0x01, 0x01, 0x05, 0x00, 0x04, 0x82, 0x04, 0xA6, 0x30, 0x82, 0x04, 0xA2, 0x02, 0x01, 
                    0x00, 0x02, 0x82, 0x01, 0x01, 0x00, 0xB2, 0x3B, 0x14, 0xD0, 0x60, 0xC3, 0x0D, 0xDB, 0x90, 0x53, 
                    0x29, 0x94, 0xFD, 0x63, 0xF3, 0x57, 0x0D, 0x02, 0x55, 0x41, 0xCD, 0x08, 0x6A, 0x6F, 0xFF, 0x0D, 
                    0x44, 0xE5, 0x19, 0xA8, 0x04, 0xE6, 0x3C, 0x31, 0x28, 0x1C, 0x71, 0x74, 0x40, 0xAD, 0x7B, 0xAB, 
                    0x8F, 0xE3, 0x3E, 0x06, 0xF7, 0xBD, 0x10, 0xF5, 0x3D, 0x8E, 0x0F, 0xA9, 0x00, 0xB8, 0xB6, 0xA0, 
                    0x8F, 0xE4, 0xCB, 0xE4, 0x13, 0x3D, 0x84, 0xBC, 0xE9, 0x19, 0x91, 0x6E, 0xCE, 0x58, 0x84, 0x50, 
                    0xDC, 0x79, 0x15, 0xD3, 0x16, 0xEE, 0x6B, 0x36, 0xEC, 0xDF, 0x81, 0x1E, 0x8F, 0x03, 0x9B, 0x20, 
                    0xB1, 0x8E, 0x56, 0x4E, 0x51, 0x66, 0xED, 0xC7, 0xFC, 0x7E, 0x03, 0xC4, 0xCC, 0xD2, 0xCD, 0x31, 
                    0x1C, 0xAC, 0x1C, 0x17, 0x3E, 0xB3, 0xF6, 0x5F, 0xB8, 0xAA, 0x05, 0x5A, 0xAE, 0xB5, 0xB1, 0x50, 
                    0x3E, 0xE8, 0x90, 0x69, 0x1F, 0xBA, 0x84, 0x0E, 0xDB, 0x62, 0x58, 0x64, 0x4A, 0x4B, 0x64, 0xE7, 
                    0xB6, 0x5A, 0x2D, 0xA3, 0x6C, 0x8E, 0x6C, 0x26, 0x02, 0xF6, 0x08, 0xF6, 0x7A, 0x03, 0x20, 0xC0, 
                    0x68, 0x63, 0xB1, 0x19, 0xEF, 0x18, 0x9A, 0x60, 0xB3, 0xDD, 0x89, 0x21, 0xF6, 0x9A, 0x01, 0x1E, 
                    0x3D, 0x51, 0x8F, 0x03, 0x0E, 0x5D, 0xD8, 0x96, 0x22, 0x06, 0x7C, 0x47, 0x21, 0x66, 0xF1, 0x29, 
                    0xBC, 0x28, 0x3E, 0x8D, 0xBE, 0xEE, 0x4B, 0x6B, 0x7D, 0x57, 0xE1, 0x35, 0x18, 0x6A, 0x87, 0xB5, 
                    0x1F, 0xCC, 0x17, 0xAE, 0xC7, 0x46, 0x73, 0x79, 0x6E, 0xF8, 0xA6, 0xD9, 0xE5, 0x98, 0x52, 0xE9, 
                    0xE6, 0x1D, 0x8A, 0x6D, 0x0E, 0xEE, 0xBC, 0x6B, 0x93, 0xF5, 0xF8, 0x7F, 0x7D, 0x30, 0x69, 0xB6, 
                    0x21, 0x50, 0x3D, 0xA1, 0x27, 0x72, 0x99, 0xC8, 0x22, 0x00, 0x51, 0xB5, 0x95, 0xB9, 0x41, 0x20, 
                    0x7E, 0xFA, 0x93, 0x55, 0x3A, 0x31, 0x02, 0x01, 0x11, 0x02, 0x82, 0x01, 0x00, 0x09, 0x2C, 0x73, 
                    0xE5, 0x14, 0x0A, 0x0A, 0x20, 0x01, 0xC8, 0x0B, 0x8D, 0x50, 0xCE, 0x8E, 0x68, 0x3E, 0xC9, 0x88, 
                    0x27, 0x26, 0xC9, 0xD8, 0x4D, 0x4B, 0x3E, 0xCC, 0xF5, 0x34, 0x24, 0xE2, 0x22, 0x70, 0x46, 0x4B, 
                    0x79, 0xEF, 0x40, 0x56, 0x27, 0x0C, 0x03, 0x2E, 0xEF, 0x76, 0x04, 0x1F, 0x93, 0x24, 0xA3, 0x35, 
                    0xFD, 0xE6, 0x73, 0xFE, 0x27, 0x9F, 0xFC, 0xF8, 0x58, 0xF3, 0xE7, 0xF9, 0x75, 0xFD, 0x6B, 0x9B, 
                    0x0E, 0x7E, 0x12, 0xE1, 0x35, 0x7F, 0x47, 0xED, 0x3A, 0xF0, 0x2E, 0xA1, 0x2E, 0x27, 0x66, 0x97, 
                    0x7C, 0x71, 0xF0, 0xA1, 0xB6, 0xC6, 0x1A, 0x27, 0x41, 0xAD, 0xDA, 0x9E, 0x8A, 0xF1, 0xE2, 0xC3, 
                    0x95, 0x8B, 0x7C, 0xF3, 0xF4, 0x43, 0x08, 0x2C, 0xC5, 0x18, 0x08, 0xB9, 0xD0, 0xCA, 0xCC, 0x74, 
                    0xCC, 0x84, 0x0A, 0x50, 0x85, 0x96, 0x4D, 0x8A, 0xC4, 0x70, 0xD8, 0x3B, 0xFC, 0x56, 0x4C, 0xE6, 
                    0x52, 0xFF, 0x7C, 0x44, 0x2C, 0xAF, 0xCD, 0x01, 0xA2, 0xC1, 0xFB, 0x9B, 0x3A, 0xAF, 0x03, 0xE0, 
                    0x24, 0xB7, 0xEB, 0xE2, 0x55, 0xC0, 0xA7, 0xD6, 0x9A, 0xA4, 0xB3, 0x2C, 0x56, 0x8E, 0x5B, 0x19, 
                    0x7F, 0x4F, 0x2E, 0x77, 0xD1, 0x26, 0x2F, 0x02, 0x87, 0x56, 0xDC, 0xF3, 0x29, 0x13, 0x8D, 0x8C, 
                    0xFA, 0xF0, 0x54, 0x26, 0x7C, 0x89, 0x0B, 0xB3, 0xBC, 0x4A, 0xCE, 0xF7, 0x33, 0x02, 0x56, 0xEB, 
                    0x88, 0x90, 0x94, 0x44, 0x61, 0x53, 0xE0, 0x75, 0x07, 0x6A, 0x70, 0xF5, 0x1A, 0x69, 0x69, 0x71, 
                    0x9A, 0xAA, 0x15, 0xBD, 0x49, 0x38, 0xA1, 0xE5, 0x0A, 0x70, 0x6A, 0x1B, 0x9C, 0xCE, 0x04, 0x39, 
                    0x7C, 0x68, 0xCF, 0xF8, 0x17, 0xEE, 0xA3, 0x9C, 0x40, 0x4A, 0xE1, 0x5A, 0xEE, 0x71, 0x64, 0x5E, 
                    0x64, 0x23, 0xCE, 0xAC, 0x70, 0xFC, 0x39, 0x19, 0x9C, 0x28, 0xBC, 0x3F, 0xA9, 0x02, 0x81, 0x81, 
                    0x00, 0xF0, 0x85, 0x37, 0x1B, 0xC0, 0x64, 0xA9, 0x02, 0xDC, 0x48, 0x21, 0x6E, 0xFC, 0x04, 0x0F, 
                    0x80, 0xC4, 0x3C, 0x7D, 0x5D, 0x58, 0x7B, 0x98, 0x35, 0xBF, 0x3C, 0x3F, 0xC5, 0xA9, 0x11, 0x38, 
                    0xC5, 0x90, 0x75, 0xB7, 0x7F, 0x2A, 0x30, 0x53, 0x9D, 0xAB, 0x65, 0x3E, 0x21, 0x45, 0x04, 0x06, 
                    0xAF, 0xCB, 0x57, 0x2E, 0x34, 0xB6, 0x5B, 0xE3, 0xCB, 0xAF, 0x58, 0x65, 0x9B, 0x7C, 0x86, 0xFC, 
                    0x17, 0x87, 0x50, 0x82, 0x5D, 0x51, 0x44, 0xA1, 0x04, 0x03, 0x93, 0x61, 0x81, 0xFF, 0x23, 0x91, 
                    0xA1, 0x64, 0x06, 0x63, 0xD0, 0x5D, 0x45, 0x1E, 0x9D, 0x0E, 0x4C, 0x2B, 0xA1, 0x14, 0xE1, 0xE2, 
                    0x09, 0x62, 0xD7, 0x2D, 0x27, 0x73, 0x2E, 0xC3, 0xF0, 0x86, 0x9E, 0xE6, 0xC7, 0x63, 0x70, 0x70, 
                    0xA9, 0x41, 0x60, 0xCD, 0x03, 0xF5, 0xA1, 0x95, 0xF4, 0xB6, 0x46, 0x10, 0xAA, 0x48, 0x07, 0x6B, 
                    0xF9, 0x02, 0x81, 0x81, 0x00, 0xBD, 0xB3, 0x97, 0x1E, 0xA1, 0xFF, 0x07, 0x08, 0xE7, 0xEC, 0x07, 
                    0x6C, 0x78, 0xD0, 0x27, 0x74, 0xF1, 0xE7, 0x5E, 0x3D, 0x04, 0x4A, 0xD9, 0x02, 0x30, 0x35, 0x03, 
                    0xBC, 0xF5, 0xDD, 0x58, 0xE5, 0xF3, 0x01, 0x0C, 0xFE, 0x13, 0x6A, 0xD4, 0x41, 0x6F, 0xDE, 0x84, 
                    0xEA, 0xD6, 0xF1, 0x25, 0xB4, 0x91, 0x5C, 0x40, 0xC6, 0x64, 0x1A, 0x9B, 0x03, 0x24, 0x5B, 0xB2, 
                    0x8B, 0xA6, 0xE4, 0x00, 0x1C, 0x4D, 0x82, 0x3F, 0x78, 0xBB, 0x8B, 0x46, 0xDF, 0x01, 0xF4, 0x4A, 
                    0x4C, 0x4D, 0xAF, 0x77, 0x11, 0xA1, 0xBB, 0x90, 0x8F, 0xF3, 0x5C, 0x20, 0x48, 0x1C, 0xE6, 0xA1, 
                    0xB1, 0xA4, 0x59, 0x14, 0x6C, 0x92, 0x66, 0xE5, 0x5D, 0x6C, 0x63, 0x9C, 0x01, 0x34, 0x63, 0x5E, 
                    0xCC, 0x4F, 0xF1, 0xD0, 0x1E, 0x74, 0x37, 0x06, 0x87, 0x5B, 0x40, 0x0E, 0x0C, 0xD4, 0x3B, 0xEA, 
                    0x76, 0x58, 0x72, 0x1D, 0xF9, 0x02, 0x81, 0x81, 0x00, 0xA9, 0xC7, 0x72, 0x31, 0xB4, 0xFB, 0xC2, 
                    0x98, 0x9B, 0x7E, 0x35, 0xB7, 0xC0, 0xF3, 0xCE, 0xB5, 0x3F, 0x39, 0xC1, 0xE7, 0x89, 0xC0, 0xA7, 
                    0xAD, 0x77, 0xEE, 0x4B, 0x22, 0x1C, 0xFD, 0x19, 0x03, 0xED, 0x80, 0x45, 0x4A, 0xB4, 0x5E, 0x59, 
                    0x24, 0x00, 0x83, 0xB3, 0x62, 0xC7, 0x4E, 0x22, 0xD6, 0x71, 0x6A, 0xB7, 0x34, 0x44, 0x7D, 0x19, 
                    0x44, 0x7B, 0xC5, 0xED, 0x5E, 0xB2, 0x41, 0x2A, 0x6A, 0xF6, 0x1A, 0xB6, 0x5F, 0xFD, 0x21, 0x62, 
                    0x99, 0x6B, 0xEF, 0x90, 0x1F, 0x86, 0xEB, 0xEE, 0x53, 0xCE, 0x22, 0xA0, 0xCF, 0x50, 0xE5, 0x7F, 
                    0x05, 0x73, 0x81, 0x0F, 0xBC, 0xFF, 0xAE, 0x81, 0x70, 0x09, 0x88, 0xD4, 0x94, 0x51, 0x4E, 0x2F, 
                    0xF5, 0x13, 0xBB, 0x75, 0xB9, 0xEB, 0xD6, 0xE6, 0x1D, 0x1F, 0x17, 0x27, 0x4E, 0x16, 0xCC, 0x69, 
                    0xD9, 0xEA, 0x13, 0x57, 0x0E, 0xC9, 0x6E, 0xA6, 0x91, 0x02, 0x81, 0x80, 0x2C, 0xA2, 0xBA, 0x25, 
                    0x53, 0x4B, 0x10, 0xB6, 0xCD, 0x28, 0x7A, 0x37, 0xA3, 0xF4, 0xBD, 0xFD, 0x66, 0x18, 0x52, 0x68, 
                    0xB5, 0xB7, 0x42, 0x1E, 0xA1, 0xEE, 0x5B, 0x3B, 0x85, 0x25, 0x05, 0xDB, 0xC0, 0xB4, 0xF3, 0xFF, 
                    0x8C, 0x19, 0x22, 0xE2, 0x38, 0x70, 0x97, 0xBE, 0xC9, 0x29, 0xAE, 0x84, 0xD6, 0xE8, 0x87, 0xB6, 
                    0x35, 0xAB, 0xE8, 0x3C, 0xF9, 0x7E, 0xFC, 0xD5, 0x90, 0xAE, 0x1E, 0x24, 0xC6, 0xF1, 0x78, 0x58, 
                    0xA4, 0x99, 0x3D, 0xDA, 0x1E, 0x93, 0xD5, 0x3F, 0x21, 0x56, 0x76, 0x5E, 0x80, 0x68, 0x5E, 0x3F, 
                    0xFD, 0x06, 0x9E, 0x2F, 0x15, 0xDB, 0xE9, 0xCF, 0x71, 0xF6, 0xD7, 0xA1, 0x13, 0x63, 0x81, 0x43, 
                    0x28, 0x8F, 0xE8, 0x78, 0xC1, 0x08, 0x52, 0x8A, 0x6D, 0x29, 0xD6, 0x9D, 0xC0, 0xFD, 0xE3, 0x6B, 
                    0x24, 0x87, 0x8A, 0xD5, 0xD7, 0x95, 0xA0, 0x94, 0x51, 0x0B, 0xCA, 0xD1, 0x02, 0x81, 0x81, 0x00, 
                    0xD5, 0x44, 0xDE, 0xAB, 0x3A, 0x9F, 0x9D, 0x5C, 0x7E, 0xB4, 0x6F, 0x7C, 0x62, 0xA1, 0x5A, 0x87, 
                    0xB6, 0x30, 0x27, 0xE7, 0xEE, 0x58, 0x86, 0x1D, 0x46, 0x1C, 0x8C, 0x0D, 0x02, 0x8F, 0x1C, 0x33, 
                    0x4C, 0x18, 0xB7, 0xC1, 0xE0, 0x0A, 0xD4, 0x61, 0x8A, 0x0C, 0x00, 0xCE, 0xCC, 0x75, 0x01, 0x91, 
                    0xCE, 0x56, 0xB3, 0xD4, 0xDA, 0x33, 0x50, 0xB1, 0x7D, 0x1B, 0x35, 0x3E, 0xC2, 0x9E, 0x63, 0x80, 
                    0xFD, 0xE8, 0x49, 0x65, 0xEF, 0xD4, 0x01, 0xDA, 0xD0, 0x8F, 0x02, 0x27, 0xC5, 0x24, 0xA2, 0xCD, 
                    0x0D, 0xD5, 0xD8, 0xC5, 0xE0, 0xD0, 0xFD, 0x3F, 0x6A, 0xCB, 0x86, 0x35, 0x5D, 0x56, 0xC5, 0x90, 
                    0xA9, 0xF6, 0x4D, 0xD5, 0xED, 0x93, 0x86, 0x85, 0xD9, 0x29, 0x2F, 0xC8, 0x3C, 0x99, 0xD7, 0xD8, 
                    0xE0, 0xEF, 0x58, 0x89, 0x98, 0x42, 0x65, 0xA6, 0x34, 0x9D, 0x9E, 0x0C, 0xA5, 0x78, 0xE7, 0x80, 
                };

            RSAKey.Modulus = key;

            RSA.ImportParameters(RSAKey);
            decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);

            return decryptedData;
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < hexString.Length; i+= 2)
            {
                try
                {
                    string byteshere = hexString.Substring(i, 2);
                    result.Add(byte.Parse(byteshere, NumberStyles.HexNumber));
                }
                catch (ArgumentOutOfRangeException)
                {
                    result.Add(0x07);
                }
            }
            return result.ToArray<byte>();
        }

    }

    public class FileInteropFunctions
    {
        public const uint GENERIC_READ = (0x80000000);
        public const uint GENERIC_WRITE = (0x40000000);
        public const uint GENERIC_EXECUTE = (0x20000000);
        public const uint GENERIC_ALL = (0x10000000);

        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const uint TRUNCATE_EXISTING = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            String lpFileName,              // Filename
            uint dwDesiredAccess,              // Access mode
            uint dwShareMode,              // Share mode
            IntPtr attr,                   // Security Descriptor
            uint dwCreationDisposition,           // How to create
            uint dwFlagsAndAttributes,           // File attributes
            uint hTemplateFile);               // Handle to template file

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FlushFileBuffers(IntPtr hFile);
    }

}
