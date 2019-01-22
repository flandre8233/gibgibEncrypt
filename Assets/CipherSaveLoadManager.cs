using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using gibgibEncrypt.Cipher;
using System.Text;

public class CipherSaveLoadManager {
    static string notePath = Application.dataPath;

    public static string pathMixer(string filename) {
        return notePath + "/" + filename + ".txt";
    }

    public static void saveNote(gibgibCipher cipher, string filename) {
        string Data = gibgibCipherMapperToGKEY(cipher);

        byte[] ba = Encoding.Default.GetBytes(Data);

        File.WriteAllText(filename, BitConverter.ToString(ba));
        Debug.Log("kkk  " + pathMixer(filename));
    }

    static string gibgibCipherMapperToGKEY(gibgibCipher cipher) {
        string outData = "";
        int[,] CArray = cipher.cipherArray;
        for (int y = 0; y < 16; y++) {
            for (int x = 0; x < 16; x++) {
                outData += CArray[ x, y ] + ";";
            }
        }
        return outData;
    }

    public static void clearSaveNote(string filename) {
        string jsonData = "";
        File.WriteAllText(pathMixer(filename), jsonData);
    }

    public static gibgibCipher LoadNote(string path) {
        string Data = File.ReadAllText(path);

        string[] arr = Data.Split('-');
        byte[] array = new byte[ arr.Length ];
        for (int i = 0; i < arr.Length; i++) array[ i ] = Convert.ToByte(arr[ i ], 16);

        string output = Encoding.Default.GetString(array);

        gibgibCipher saveData = gibgibCipherMapperToString(output);

        return saveData;
    }

    static gibgibCipher gibgibCipherMapperToString(string Data) {
        int[,] CArray = new int[ 16, 16 ];
        string[] ary = Data.Split(';');

        int count = 0;
        for (int y = 0; y < 16; y++) {
            for (int x = 0; x < 16; x++) {
                CArray[ x, y ] = int.Parse(ary[ count ]);
                count++;
            }
        }

        return new gibgibCipher(CArray);
    }



}
