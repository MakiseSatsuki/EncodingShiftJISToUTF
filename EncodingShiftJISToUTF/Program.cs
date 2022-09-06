using System;
using System.IO;
using System.Text;

namespace EncodingShiftJISToUTF
{
    class Program
    {
        
        static readonly Encoding ShiftJISEncoding = Encoding.GetEncoding("shift_jis");
        static readonly Encoding Gbk = Encoding.GetEncoding("GB2312");
        static readonly Encoding Utf8 = Encoding.UTF8;
        

        static void Main(string[] args)
        {

            if (args.Length != 3)
            {
                Console.WriteLine("Error Type");

                Console.WriteLine("Help:");
                Console.WriteLine("Shift-JIS To UTF-8:EncodingShiftJISToUTF.exe S-U [Shift-JIS] [UTF-8]            []---dirName");
                Console.WriteLine("UTF-8     To  GBK:EncodingShiftJISToUTF.exe U-G     [UTF-8]     [GBK]            []---dirName");

                Console.WriteLine("Example:");
                Console.WriteLine("Example:EncodingShiftJISToUTF.exe S-U  ShiftJ   UTF8");
                Console.WriteLine("Example:EncodingShiftJISToUTF.exe U-G   UTF8    GBK");

                Console.Write("Press any key to continue");
                Console.ReadKey();
                return;
            }


            if (args[0] == "S-U")
            {
                string ShiftJDir = args[1];
                string UTF8Dir = args[2];

                if (!Directory.Exists(ShiftJDir))
                {
                    Console.WriteLine("no {0} dir",ShiftJDir);
                    Environment.Exit(1);
                }

                if (!Directory.Exists(UTF8Dir))
                {
                    Directory.CreateDirectory(UTF8Dir);
                }

                string[] SourceListTxt = FindFile(ShiftJDir, "*.txt");


                foreach (var i in SourceListTxt)
                {
                    Console.WriteLine(i);
                    string[] FileNameArray = i.Split("\\");
                    if (FileNameArray.Length > 1)
                    {
                        string AimPath = UTF8Dir + "\\" + FileNameArray[1];
                        
                        ExportUTFStrings(i, AimPath);
                    }

                }
            }
            else if (args[0] == "U-G")
            {
                string UTF8Dir = args[1];
                string GbkDir = args[2];

                if (!Directory.Exists(UTF8Dir))
                {
                    Console.WriteLine("no {0} dir", UTF8Dir);
                    Environment.Exit(1);
                }

                if (!Directory.Exists(GbkDir))
                {
                    Directory.CreateDirectory(GbkDir);
                }

                string[] SourceListTxt = FindFile(UTF8Dir, "*.txt");

                foreach (var i in SourceListTxt)
                {
                    Console.WriteLine(i);
                    string[] FileNameArray = i.Split("\\");
                    if (FileNameArray.Length > 1)
                    {
                        string AimPath = GbkDir + "\\" + FileNameArray[1];
                        ExportGBStrings(i, AimPath);
                    }

                }
            }
            else
            {
                Console.WriteLine("Error Type");

                Console.WriteLine("Help:");
                Console.WriteLine("Shift-JIS To UTF-8:EncodingShiftJISToUTF.exe S-U [Shift-JIS] [UTF-8]            []---dirName");
                Console.WriteLine("UTF-8     To  GBK:EncodingShiftJISToUTF.exe U-G     [UTF-8]     [GBK]            []---dirName");

                Console.WriteLine("Example:");
                Console.WriteLine("Example:EncodingShiftJISToUTF.exe S-U  ShiftJ   UTF8");
                Console.WriteLine("Example:EncodingShiftJISToUTF.exe U-G   UTF8    GBK");

                Console.Write("Press any key to continue");
                Console.ReadKey();
                return;
            }

            
        }

        public static string[] FindFile(string sSourcePath, string type)
        {

            var files = Directory.GetFiles(sSourcePath, type, SearchOption.AllDirectories); // 遍历所有文件

            return files;
        }

        public static void ExportUTFStrings(string SourceFilePath, string AimFilePath)
        {
            using (var writer = File.CreateText(AimFilePath))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                string SourceTxt = File.ReadAllText(SourceFilePath, ShiftJISEncoding);
                string[] SourceTxtArray = SourceTxt.Split("\n");

                foreach (string line in SourceTxtArray)
                {
                    
                    
                    byte[] ShiftJISBytes = ShiftJISEncoding.GetBytes(line);

                    byte[] U8Bytes = Encoding.Convert(ShiftJISEncoding, Utf8, ShiftJISBytes);
                    string newline = Encoding.UTF8.GetString(U8Bytes);

                    writer.WriteLine($"◇{newline}");
                    writer.WriteLine($"◆{newline}");
                    writer.WriteLine();
                }
            }
        }

        public static void ExportGBStrings(string SourceFilePath, string AimFilePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (File.Exists(AimFilePath))
            {
                File.Delete(AimFilePath);
            }

            using (var writer = new StreamWriter(AimFilePath, true, Gbk))
            {
                
                string[] SourceTxtArray = File.ReadAllLines(SourceFilePath, Utf8);

                foreach (string line in SourceTxtArray)
                {
                    if (line.Length == 0 || line[0] != '◆')
                    {
                        continue;
                    }
                    string newline = line.Replace("◆", string.Empty);
                    byte[] U8Bytes = Utf8.GetBytes(newline);
                    byte[] GbkBytes = Encoding.Convert(Utf8, Gbk, U8Bytes);

                    writer.WriteLine(Gbk.GetString(GbkBytes));

                }

            }
            
        }
    }
}
