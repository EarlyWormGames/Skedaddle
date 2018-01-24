using System;
using System.Data;
using System.Security.Cryptography;
using System.IO;


public class Encryptonator
{
    // Change these keys
    private byte[] Key = { 178, 204, 36, 2, 134, 193, 115, 95, 189, 131, 56, 23, 158, 85, 167, 25, 98, 218, 231, 96, 155, 29, 164, 27, 116, 12, 209, 219, 181, 103, 73, 57 };
    private byte[] Vector = { 248, 130, 67, 195, 77, 14, 32, 200, 86, 76, 133, 255, 170, 34, 19, 150 };


    private ICryptoTransform EncryptorTransform, DecryptorTransform;
    private System.Text.UTF8Encoding UTFEncoder;

    public Encryptonator()
    {
        //This is our encryption method
        RijndaelManaged rm = new RijndaelManaged();

        //Create an encryptor and a decryptor using our encryption method, key, and vector.
        EncryptorTransform = rm.CreateEncryptor(Key, Vector);
        DecryptorTransform = rm.CreateDecryptor(Key, Vector);

        //Used to translate bytes to text and vice versa
        UTFEncoder = new System.Text.UTF8Encoding();
    }

    /// -------------- Two Utility Methods (not used but may be useful) -----------
    /// Generates an encryption key.
    static public byte[] GenerateEncryptionKey()
    {
        //Generate a Key.
        RijndaelManaged rm = new RijndaelManaged();
        rm.GenerateKey();
        return rm.Key;
    }

    /// Generates a unique encryption vector
    static public byte[] GenerateEncryptionVector()
    {
        //Generate a Vector
        RijndaelManaged rm = new RijndaelManaged();
        rm.GenerateIV();
        return rm.IV;
    }


    /// ----------- The commonly used methods ------------------------------    
    /// Encrypt some text and return a string suitable for passing in a URL.
    public string EncryptToString(string TextValue)
    {
        return Convert.ToBase64String(Encrypt(TextValue));
    }

    /// Encrypt some text and return an encrypted byte array.
    public byte[] Encrypt(string TextValue)
    {
        //Translates our text value into a byte array.
        byte[] bytes = UTFEncoder.GetBytes(TextValue);

        //Used to stream the data in and out of the CryptoStream.
        MemoryStream memoryStream = new MemoryStream();

        /*
         * We will have to write the unencrypted bytes to the stream,
         * then read the encrypted result back from the stream.
         */
        #region Write the decrypted value to the encryption stream
        CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
        cs.Write(bytes, 0, bytes.Length);
        cs.FlushFinalBlock();
        #endregion

        #region Read encrypted value back out of the stream
        memoryStream.Position = 0;
        byte[] encrypted = new byte[memoryStream.Length];
        memoryStream.Read(encrypted, 0, encrypted.Length);
        #endregion

        //Clean up.
        cs.Close();
        memoryStream.Close();

        return encrypted;
    }

    /// The other side: Decryption methods
    public string DecryptString(string EncryptedString)
    {
        return Decrypt(Convert.FromBase64String(EncryptedString));
    }

    /// Decryption when working with byte arrays.    
    public string Decrypt(byte[] EncryptedValue)
    {
        #region Write the encrypted value to the decryption stream
        MemoryStream encryptedStream = new MemoryStream();
        CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
        decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
        decryptStream.FlushFinalBlock();
        #endregion

        #region Read the decrypted value from the stream.
        encryptedStream.Position = 0;
        byte[] decryptedBytes = new byte[encryptedStream.Length];
        encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
        encryptedStream.Close();
        #endregion
        return UTFEncoder.GetString(decryptedBytes);
    }

    /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
    //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
    //      return encoding.GetBytes(str);
    // However, this results in character values that cannot be passed in a URL.  So, instead, I just
    // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
    public byte[] StrToByteArray(string str)
    {
        if (str.Length == 0)
            throw new Exception("Invalid string value in StrToByteArray");

        byte val;
        byte[] byteArr = new byte[str.Length / 3];
        int i = 0;
        int j = 0;
        do
        {
            val = byte.Parse(str.Substring(i, 3));
            byteArr[j++] = val;
            i += 3;
        }
        while (i < str.Length);
        return byteArr;
    }

    // Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
    //      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
    //      return enc.GetString(byteArr);    
    public string ByteArrToString(byte[] byteArr)
    {
        byte val;
        string tempStr = "";
        for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
        {
            val = byteArr[i];
            if (val < 10)
                tempStr += "00" + val.ToString();
            else if (val < 100)
                tempStr += "0" + val.ToString();
            else
                tempStr += val.ToString();
        }
        return tempStr;
    }
}