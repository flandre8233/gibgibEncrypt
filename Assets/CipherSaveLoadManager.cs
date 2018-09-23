using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using gibgibEncrypt.Cipher;
using LitJson;

public class CipherSaveLoadManager {
    static string notePath = Application.dataPath;

    public static string pathMixer(string filename) {
        return notePath + "/" + filename + ".txt";
    }

    public static void saveNote(gibgibCipher cipher, string filename) {
        string jsonData = JsonMapper.ToJson(cipher);
        File.WriteAllText(pathMixer(filename), jsonData);
        Debug.Log("kkk  " + pathMixer(filename));
    }
    public static void clearSaveNote(string filename) {
        string jsonData = "";
        File.WriteAllText(pathMixer(filename), jsonData);
    }

    public static gibgibCipher LoadNote(string filename) {
        if (!File.Exists(pathMixer(filename))) {
            return null;
        }
        string jsonData = File.ReadAllText(pathMixer(filename));
        gibgibCipher saveData = JsonMapper.ToObject<gibgibCipher>(jsonData);

        return saveData;
    }
}
