using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections;

namespace Wren.Core.Services
{
    public class EncryptionService : IEncryptionService
    {
        private const Int32 KeySize = 128;

        public String EncryptString(String inputString)
        {           
            RSACryptoServiceProvider rsaCryptoServiceProvider =
                                          new RSACryptoServiceProvider();

            var p = rsaCryptoServiceProvider.ExportParameters(true);
            // rsaCryptoServiceProvider.FromXmlString(xmlString);

            byte[] bytes = Encoding.UTF32.GetBytes(inputString);

            byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(bytes, false);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
