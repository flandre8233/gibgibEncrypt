using System;

public static class gibgibEncryptSystem  {
    public static string Encrypt(string header,string data,string password) {
        string outPutData = "";

        int passwordInt = passwordStringToInt(password);

        int genPassWord = UnityEngine.Random.Range(1,128);
        outPutData += "#GIBGIB_ENC:";
        outPutData += Convert.ToString(passwordInt * genPassWord - genPassWord,16) + ":";
        if (header==null||header=="") {
            outPutData += "0:";
        } else {
            for (int i = 0; i < header.Length; i++) {
                int newVal = (header[ i ] * 13) - (changeNumber(5 * i, i));
                outPutData += Convert.ToString(newVal, 16) + " ";
            }
            outPutData += ":";
        }

        outPutData += Convert.ToString(genPassWord * 8233,16)  + " " ;
        for (int i = 0; i < data.Length; i++) {
            int newVal = ((data[ i ] + genPassWord - passwordInt) * 8233) - (changeNumber(1337*i,i)) ;
            outPutData += Convert.ToString(newVal, 16) + " ";
        }
        return outPutData;
    }

    public static void Decrypt(string data,string password, out string header,out string outPutData) {
        header = "";
        outPutData = "";

        int passwordInt = passwordStringToInt(password);
 
        string[] splitedArray = data.Split(':');

        if (splitedArray.Length != 4 || splitedArray[ 0 ] != "#GIBGIB_ENC") {
            outPutData = "Tried to decrypt an invalid valid GIBGIB_ENC file ";
        }

        string[] splitedPasswordArray = splitedArray[ 1 ].Split(' ');
        string[] splitedHeaderArray = splitedArray[ 2 ].Split(' ');
        string[] splitedMSGArray = splitedArray[3].Split(' ');

        int genPassWord = Int32.Parse(splitedMSGArray[ 0 ], System.Globalization.NumberStyles.HexNumber) / 8233 ;

        for (int i = 0; i < splitedHeaderArray.Length; i++) {
            int pareseResult;
            string HAXHeader = splitedHeaderArray[ i ];
            if (Int32.TryParse(HAXHeader, System.Globalization.NumberStyles.HexNumber, null, out pareseResult)) {
                header += (char)((((pareseResult) + (changeNumber(5 * i, i))) / 13));
            }
        }

        if (passwordInt != (Int32.Parse(splitedPasswordArray[0], System.Globalization.NumberStyles.HexNumber)   + genPassWord) / genPassWord) {
            outPutData = "wrong password";
            return;
        }

        for (int i = 1; i < splitedMSGArray.Length; i++) {
            int pareseResult;
            string HAXMsg = splitedMSGArray[ i ];
            if (Int32.TryParse(HAXMsg, System.Globalization.NumberStyles.HexNumber,null, out pareseResult)) {
                outPutData += (char)((((pareseResult) + (changeNumber(1337 * (i - 1), i - 1))) / 8233) - genPassWord + passwordInt);
            }
        }
        //return outPutData;
    }

    private static int passwordStringToInt(string password) {
        int passwordInt = 0;
        if (password != "") {
            for (int i = 0; i < password.Length; i++) {
                passwordInt += (password[ i ]*i+23);
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
