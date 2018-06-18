using System;

public static class gibgibEncryptSystem  {
    public static string Encrypt(string data,string password) {
        string outPutData = "";

        int passwordInt = passwordStringToInt(password);

        int genPassWord = UnityEngine.Random.Range(1,128);
        outPutData += "#GIBGIB_ENC:";
        outPutData += Convert.ToString(passwordInt * genPassWord - genPassWord,16) + ":";

        outPutData += Convert.ToString(genPassWord * 8233,16)  + " " ;
        for (int i = 0; i < data.Length; i++) {
            int newVal = ((data[ i ] + genPassWord - passwordInt) * 8233) - (changeNumber(1337*i,i)) ;
            outPutData += Convert.ToString(newVal, 16) + " ";
        }
        return outPutData;
    }

    public static string Decrypt(string data,string password) {
        string outPutData = "";

        int passwordInt = passwordStringToInt(password);
 
        string[] splitedArray = data.Split(':');

        if (splitedArray.Length != 3 || splitedArray[ 0 ] != "#GIBGIB_ENC") {
            return "Tried to decrypt an invalid valid GIBGIB_ENC file ";
        }

        string[] splitedPasswordArray = splitedArray[ 1 ].Split(' ');
        string[] splitedMSGArray = splitedArray[2].Split(' ');

        int genPassWord = Int32.Parse(splitedMSGArray[ 0 ], System.Globalization.NumberStyles.HexNumber) / 8233 ;

        if (passwordInt != (Int32.Parse(splitedPasswordArray[0], System.Globalization.NumberStyles.HexNumber)   + genPassWord) / genPassWord) {
            return "wrong password";
        }

        for (int i = 1; i < splitedMSGArray.Length; i++) {
            int pareseResult;
            string HAXMsg = splitedMSGArray[ i ];
            if (Int32.TryParse(HAXMsg, System.Globalization.NumberStyles.HexNumber,null, out pareseResult)) {
                outPutData += (char)((((pareseResult) + (changeNumber(1337 * (i - 1), i - 1))) / 8233) - genPassWord + passwordInt);
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
