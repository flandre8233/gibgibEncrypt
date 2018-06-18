using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gibgibEncryptSystem  {
    public static string Encrypt(string data,string password) {
        string outPutData = "";

        int passwordInt = passwordStringToInt(password);

        int genPassWord = Random.Range(1,128);
        outPutData += "#GIBGIB_ENC:";
        outPutData += passwordInt * genPassWord - genPassWord + ":";

        outPutData += genPassWord * 8233  + " " ;
        for (int i = 0; i < data.Length; i++) {
            int newVal = ((data[ i ] + genPassWord - passwordInt) * 8233) - (changeNumber(1337*i,i)) ;
            outPutData += newVal + " ";
        }
        return outPutData;
    }

    public static string Decrypt(string data,string password) {
        string outPutData = "";

        int passwordInt = passwordStringToInt(password);
 
        string[] splitedArray = data.Split(':');

        string[] splitedPasswordArray = splitedArray[ 1 ].Split(' ');
        string[] splitedMSGArray = splitedArray[2].Split(' ');

        int genPassWord = int.Parse(splitedMSGArray[ 0 ] ) / 8233 ;

        if (passwordInt != (int.Parse(splitedPasswordArray[0])   + genPassWord) / genPassWord) {
            return "wrong password";
        }

        for (int i = 1; i < splitedMSGArray.Length; i++) {
            int pareseResult;
            if (int.TryParse(splitedMSGArray[ i ], out pareseResult)) {
                outPutData += (char)( ( ((pareseResult) + (changeNumber(1337 *(i - 1), i - 1))) / 8233) - genPassWord + passwordInt) ;
            }
        }
        return outPutData;
    }

    private static int passwordStringToInt(string password) {
        int passwordInt = 0;
        if (password != "") {
            for (int i = 0; i < password.Length; i++) {
                passwordInt += (password[ i ]);
            }
            passwordInt /= password.Length;
        }
        return passwordInt;
    }

    private static int changeNumber(int number,int index) {
        if (index % 2 == 0) {
            return -number;
        } else {
            return number;
        }
    }

}
