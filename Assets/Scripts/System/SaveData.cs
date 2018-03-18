using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private int _Peanuts = 0;
    private int _UnlockedArea = 1;
    private int _UnlockedLevel = 1;
    private List<int> _UnlockedChests = new List<int>();

    private static string key = "OHNCAG63UM3IZ5KRRR4D4SP97DNYZ7HY";
    private static int keySize = 256, ivSize = 16;

    private static SaveData save;
    private static SaveData instance
    {
        get
        {
            if (save == null)
            {
                string path = Application.persistentDataPath + "/save.data";

                if (!File.Exists(path))
                {
                    save = new SaveData();
                    return save;
                }

                byte[] bytes;
                bytes = Encoding.UTF8.GetBytes(key);

                //Open the file
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                //Decrypt it
                var crypto = CreateDecryptionStream(bytes, file);

                //Read the data
                var s = (SaveData)bf.Deserialize(crypto);

                //Close (to avoid leaks and such)
                crypto.Close();
                file.Close();

                //Set the local value
                save = s;
            }
            return save;
        }
    }

    void Save()
    {
        string path = Application.persistentDataPath + "/save.data";

        byte[] bytes;
        bytes = Encoding.UTF8.GetBytes(key);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        var crypto = CreateEncryptionStream(bytes, file);

        bf.Serialize(crypto, this);
        crypto.Close();
        file.Close();
    }

    //==============================================================
    //GET DATA
    public static int Area() { return instance._UnlockedArea; }
    public static int Level() { return instance._UnlockedLevel; }
    public static int Peanuts() { return instance._Peanuts; }
    public static bool IsChestUnlocked(int i) { return instance._UnlockedChests.Contains(i); }
    //==============================================================

    //==============================================================
    //SET DATA
    public static void UnlockLevel(int area, int level)
    {
        if (instance._UnlockedArea < area)
            instance._UnlockedArea = area;
        if (instance._UnlockedLevel < level)
            instance._UnlockedLevel = level;

        instance.Save();
    }

    public static void AddPeanut()
    {
        ++instance._Peanuts;
        instance.Save();
    }

    public static void UnlockChest(int i)
    {
        if (!instance._UnlockedChests.Contains(i))
        {
            instance._UnlockedChests.Add(i);
            instance.Save();
        }
    }
    //==============================================================

    public static CryptoStream CreateEncryptionStream(byte[] key, Stream outputStream)
    {
        byte[] iv = new byte[ivSize];

        var rng = new RNGCryptoServiceProvider();
        // Using a cryptographic random number generator
        rng.GetNonZeroBytes(iv);

        // Write IV to the start of the stream
        outputStream.Write(iv, 0, iv.Length);

        Rijndael rijndael = new RijndaelManaged();
        rijndael.KeySize = keySize;

        CryptoStream encryptor = new CryptoStream(
            outputStream,
            rijndael.CreateEncryptor(key, iv),
            CryptoStreamMode.Write);
        return encryptor;
    }

    public static CryptoStream CreateDecryptionStream(byte[] key, Stream inputStream)
    {
        byte[] iv = new byte[ivSize];

        if (inputStream.Read(iv, 0, iv.Length) != iv.Length)
        {
            Debug.LogError("Failed to read IV from stream.");
        }

        Rijndael rijndael = new RijndaelManaged();
        rijndael.KeySize = keySize;

        CryptoStream decryptor = new CryptoStream(
            inputStream,
            rijndael.CreateDecryptor(key, iv),
            CryptoStreamMode.Read);
        return decryptor;
    }
}