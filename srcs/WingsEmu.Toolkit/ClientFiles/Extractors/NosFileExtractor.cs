// WingsEmu
// 
// Developed by NosWings Team

namespace WingsEmu.Toolkit.ClientFiles.Extractors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class NFile
    {
        public string Name { get; }
        public int FileNumber { get; set; }
        public int IsDat { get; set; }
        public byte[] Data { get; set; }

        public NFile(string name, int fileNumber, int isDat, byte[] data)
        {
            Name = name;
            FileNumber = fileNumber;
            IsDat = isDat;
            Data = data;
        }
    }

    public static class NosTxtDecryptor
    {
        private static readonly byte[] CryptoArray = { 0x00, 0x20, 0x2D, 0x2E, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x0A, 0x00 };

        private static byte[] Decrypt(IReadOnlyList<byte> array)
        {
            List<byte> decryptedFile = new List<byte>();
            int currindex = 0;
            while (currindex < array.Count)
            {
                char currentByte = Convert.ToChar(array[currindex++]);
                if (currentByte == 0xFF)
                {
                    decryptedFile.Add(0xD);
                    continue;
                }

                int validate = currentByte & 0x7F;

                if ((currentByte & 0x80) != 0)
                {
                    for (; validate > 0; validate -= 2)
                    {
                        if (currindex >= array.Count)
                        {
                            break;
                        }

                        currentByte = Convert.ToChar(array[currindex++]);

                        decryptedFile.Add(CryptoArray[(currentByte & 0xF0) >> 4]);
                        if (validate <= 1)
                        {
                            break;
                        }

                        if (CryptoArray[currentByte & 0xF] == 0)
                        {
                            break;
                        }

                        decryptedFile.Add(CryptoArray[currentByte & 0xF]);
                    }
                }
                else
                {
                    for (; validate > 0; --validate)
                    {
                        if (currindex >= array.Count)
                        {
                            break;
                        }

                        currentByte = Convert.ToChar(array[currindex++]);
                        decryptedFile.Add(Convert.ToByte(currentByte ^ 0x33));
                    }
                }
            }

            return decryptedFile.ToArray();
        }

        public static List<NFile> GetNostaleTxtFiles(FileStream f)
        {
            List<NFile> retList = new List<NFile>();

            f.Seek(0, SeekOrigin.Begin);
            int fileAmount = f.ReadNextInt();
            for (int i = 0; i < fileAmount; i++)
            {
                int fileNumber = f.ReadNextInt();
                int stringNameSize = f.ReadNextInt();
                string stringName = f.ReadString(stringNameSize);
                int isDat = f.ReadNextInt();
                int fileSize = f.ReadNextInt();
                byte[] fileContent = f.ReadLength(fileSize);

                retList.Add(new NFile(stringName, fileNumber, isDat, Decrypt(fileContent)));
            }
            return retList;
        }

        private static int ReadNextInt(this Stream fileSteam)
        {
            byte[] buffer = new byte[4];
            fileSteam.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private static string ReadString(this Stream fileStream, int count)
        {
            byte[] buffer = new byte[count];
            fileStream.Read(buffer, 0, count);
            return Encoding.Default.GetString(buffer);
        }

        private static byte[] ReadLength(this Stream fileStream, int count)
        {
            byte[] buffer = new byte[count];
            fileStream.Read(buffer, 0, count);
            return buffer;
        }
    }
}