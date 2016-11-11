using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NguberAPI.Commons {
  public static class Utilities {
    #region Protected Properties
    #endregion


    #region Public Properties
    #endregion


    #region Constructors & Destructor
    #endregion


    #region Protected Methods
    #endregion


    #region Public Methods
    public static long GetEpoch (DateTime Date) {
      return (new DateTimeOffset(Date)).ToUnixTimeSeconds();
    }


    //public static bool Encrypt (out byte[] CipherBytes, byte[] PlainBytes, byte[] KeyBytes, CipherMode Mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7) {
    //  CipherBytes = null;

    //  if (null == PlainBytes)
    //    return false;

    //  if (null == KeyBytes)
    //    return false;

    //  using (var encrypt = Aes.Create()) {
    //    foreach (KeySizes keySize in encrypt.LegalKeySizes) {
    //      if (keySize.MinSize > (KeyBytes.Length * 8) || (keySize.MaxSize < KeyBytes.Length * 8))
    //        return false;
    //    }

    //    encrypt.Key = KeyBytes;
    //    encrypt.Mode = Mode;
    //    encrypt.Padding = padding;

    //    using (var encryptor = encrypt.CreateEncryptor()) {
    //      using (var msEncrypt = new MemoryStream()) {
    //        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
    //          csEncrypt.Write(PlainBytes, 0, PlainBytes.Length);
    //          csEncrypt.FlushFinalBlock();
    //        }
    //        CipherBytes = msEncrypt.ToArray();
    //      }
    //    }
    //  }
    //  return true;
    //}

    //public static bool Encrypt () {
    //  return false;
    //}
    #endregion
  }
}
