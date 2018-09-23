using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gibgibEncrypt.Cipher;

public class gibgibEncryptTester : SingletonMonoBehavior<gibgibEncryptTester> {
    public string EncryptPassword;
    public string InputSting ;
    public string DecryptPassword;
    public string OutputSting;

    public string passWordInt;

    //static string path = Application.persistentDataPath + "/playerData.GEC";

    public void printer(string s) {
        print(s);
    }
    public void printer(int s) {
        print(s);
    }
    public void printer(double s) {
        print(s);
    }
    public void printer(long s) {
        print(s);
    }

    // Use this for initialization
    void Start() {
        /*
        gibgibEncrypt.gibgibCipher cipher = gibgibEncrypt.gibgibCipherSystem.cipher;
        foreach (var item in cipher.cipherArray) {
            if (item >= 10) {
                print(item);
            }
            if (item == 9) {
                print(item);
            }
            if (item == 8) {
                print(item);
            }
            if (item == 7) {
                print(item);
            }
            if (item == 6) {
                print(item);
            }
            if (item == 5) {
                print(item);
            }
            if (item == 4) {
                print(item);
            }
            if (item == 3) {
                print(item);
            }
            if (item == 2) {
                print(item);
            }
            if (item == 1) {
                print(item);
            }
            if (item == 0) {
                print(item);
            }
        }
        */

    
    }

    // Update is called once per frame
    void Update () {
        /*
        if (Input.GetKeyDown(KeyCode.E) ) {
            OutputSting = gibgibCipherSystem.Encrypt(InputSting).ToString();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            OutputSting = gibgibCipherSystem.Decrypt(OutputSting).ToString();
        }


        if (Input.GetKeyDown(KeyCode.N)) {
            CipherSaveLoadManager.saveNote(gibgibCipherSystem.cipher,"cipher");
        }
        */
    }
    private static int passwordStringToInt(string password) {
        int passwordInt = 0;
        if (password != "") {
            for (int i = 0; i < password.Length; i++) {
                passwordInt += (password[ i ] * i + 37);
            }
        }
        return passwordInt;
    }
}
