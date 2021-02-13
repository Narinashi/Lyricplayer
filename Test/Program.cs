using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var sha = SHA384.Create();
            var result = sha.ComputeHash(File.ReadAllBytes("F:\\body"));
            var resultStr = Convert.ToBase64String(result);
            var hexResult = BitConverter.ToString(result).Replace("-", "");
            var hexResult2 = BitConverter.ToString(Encoding.ASCII.GetBytes(resultStr)).Replace("-", "");

            Console.WriteLine(resultStr);
            Console.WriteLine(hexResult);
            Console.WriteLine(hexResult2);


            var bodyData = File.ReadAllBytes("F:\\body");
            var IVstr = ("0x22, 0x14, 0x10, 0xA2, 0x68, 0xF8, 0xEA," +
                " 0xBC, 0xE1, 0x5B, 0x82, 0xDF, 0xD2, 0xFD, 0xE1," +
                " 0xFF, 0xF6, 0x06, 0x5E, 0x8D, 0x33, 0x66").Replace("0x", "").Replace(", ", "");
            var IV = StringToByteArray(IVstr);
            var keyStr = "Hpwyj80jANxYuas4iSTv7oVDpdyKTSDy/A9xiTt/TP8=";
            var key = Convert.FromBase64String(keyStr);
           // var decodedData = AES_Decrypt()
            Console.ReadLine();
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] key,byte[] iv)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    //AES.KeySize = 256;
                    //AES.BlockSize = 128;

                    AES.Key = key;
                    AES.IV = iv;

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

    }
}
