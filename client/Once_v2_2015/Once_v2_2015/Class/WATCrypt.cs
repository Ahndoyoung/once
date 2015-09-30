using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Once_v2_2015.Class
{
    public class WATCrypt
    {
        byte[] Skey = new byte[8];

        public WATCrypt(string strKey)
        {
            // 키를 byte로 변환함
            Skey = ASCIIEncoding.ASCII.GetBytes(strKey);
        }

        public string Encrypt(string p_data)
        {
            // 암호화
            if (Skey.Length != 8)
            {
                throw (new Exception("Invaild key. Key length must be 8 byte."));
            }

            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            rc2.Key = Skey; // 키를 지정함
            rc2.IV = Skey;

            MemoryStream ms = new MemoryStream();
            CryptoStream cryStream = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] data = Encoding.UTF8.GetBytes(p_data.ToCharArray()); // 입력한 값을 byte배열로 변환

            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray()); // 암호화된 값을 반환
        }

        public string Decrypt(string p_data)
        {
            // 복호화
            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            rc2.Key = Skey; // 키를 지정함
            rc2.IV = Skey;

            MemoryStream ms = new MemoryStream();
            CryptoStream cryStream = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] data = Convert.FromBase64String(p_data); // 암호화된 값을 byte배열로 변환

            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.GetBuffer()).Replace("\0", string.Empty); // 입력한 값으로 반환
        }
    }
}
