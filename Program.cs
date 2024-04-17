using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SignatureAnalysis
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string directoryPath;
            string outputPath;
            bool includeSubdirectories;

            // Read directory path from command line
            Console.Write("Enter the directory path: ");
            directoryPath = Console.ReadLine();

            // Read output file path from command line
            Console.Write("Enter the output file path: ");
            outputPath = Console.ReadLine();

            // Read the flag from command line
            Console.Write("Include subdirectories? (true/false): ");
            includeSubdirectories = bool.Parse(Console.ReadLine());

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Invalid directory path.");
                return;
            }

            var files = GetFiles(directoryPath, includeSubdirectories);

            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("File Path,File Type,MD5 Hash");

                foreach (var file in files)
                {
                    string filePath = file.FullName;
                    string fileType = GetFileType(file);
                    string md5Hash = CalculateMD5Hash(filePath);

                    writer.WriteLine($"{filePath},{fileType},{md5Hash}");
                }
            }

            Console.WriteLine("File analysis completed.");
        }

        static FileInfo[] GetFiles(string directoryPath, bool includeSubdirectories)
        {
            var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return new DirectoryInfo(directoryPath).GetFiles("*.*", searchOption);
        }

        static string GetFileType(FileInfo file)
        {
            byte[] signatureBytes = new byte[4];
            using (var fileStream = file.OpenRead())
            {
                fileStream.Read(signatureBytes, 0, 4);
            }

            if (signatureBytes[0] == 0xFF && signatureBytes[1] == 0xD8)
            {
                return "JPG";
            }
            else if (signatureBytes[0] == 0x25 && signatureBytes[1] == 0x50 && signatureBytes[2] == 0x44 && signatureBytes[3] == 0x46)
            {
                return "PDF";
            }
            else
            {
                return "Unknown";
            }
        }

        static string CalculateMD5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }

                    return sb.ToString();
                }
            }
        }
    }
}

