using System;
using System.IO;
using gibgibEncrypt.Encrypt;
using gibgibEncrypt.Cipher;
using System.Collections.Generic;

namespace gibgibEncrypt {
    //encrypt algorithm Ver 1.04
    namespace Encrypt {
        public static class gibgibEncryptSystem {
            private static string ver = "V1.04";
            public static string Encrypt(string header, string data, string password, gibgibCipher cipher) {
                string outPutData = "";

                int passwordInt = passwordStringToInt(password);
                gibgibCipherSystem.changeKeyWord(password);

                int genPassWord = UnityEngine.Random.Range(1, 128);
                outPutData += "#GIBGIB_ENC:";
                outPutData += ver + ":";

                outPutData += Convert.ToString(passwordInt * genPassWord - genPassWord, 16) + ":";
                if (header == null || header == "") {
                    outPutData += "0:";
                } else {
                    for (int i = 0; i < header.Length; i++) {
                        long newVal = (header[ i ] * 13) - (changeNumber(5 * i, i));
                        outPutData += Convert.ToString(newVal, 16) + " ";
                    }
                    outPutData += ":";
                }

                outPutData += Convert.ToString(genPassWord * 8233, 16) + " ";
                for (int i = 0; i < data.Length; i++) {
                    int firstStep = (data[ i ] + passwordInt - genPassWord) * 8233;
                    long newVal = (firstStep) - (changeNumber(17 * i, i));
                    string HAXMsg = Convert.ToString(newVal, 16);
                    string cipherEncryptResult = "";
                    if (cipher != null) {
                        cipherEncryptResult = gibgibCipherSystem.Encrypt(HAXMsg);
                    } else {
                        cipherEncryptResult = HAXMsg;
                    }

                    outPutData += cipherEncryptResult + " ";
                    //gibgibEncryptTester.instance.printer("Encrypt" + " text : " + data[ i ] + " textNum : " + (int)data[ i ] + " pw : " + passwordInt + " genPw : " + genPassWord + " chN : " + (changeNumber(17 * i, i)) + " ANS : " + newVal + " HAXMsg : " + HAXMsg + " cipherEncryptRe : " + cipherEncryptResult);
                }
                gibgibCipherSystem.changeKeyWord("");
                return outPutData;
            }

            public static void Decrypt(string data, string password, gibgibCipher cipher, out string header, out string outPutData) {
                header = "";
                outPutData = "";

                int passwordInt = passwordStringToInt(password);
                gibgibCipherSystem.changeKeyWord(password);

                string[] splitedArray = data.Split(':');

                if (splitedArray.Length != 5 || splitedArray[ 0 ] != "#GIBGIB_ENC") {
                    outPutData = "Tried to decrypt an invalid valid GIBGIB_ENC file ";
                    return;
                }

                string[] splitedPasswordArray = splitedArray[ 2 ].Split(' ');
                string[] splitedHeaderArray = splitedArray[ 3 ].Split(' ');
                string[] splitedMSGArray = splitedArray[ 4 ].Split(' ');

                int genPassWord = Int32.Parse(splitedMSGArray[ 0 ], System.Globalization.NumberStyles.HexNumber);
                genPassWord /= 8233;

                for (int i = 0; i < splitedHeaderArray.Length; i++) {
                    long pareseResult;
                    string HAXHeader = splitedHeaderArray[ i ];
                    if (long.TryParse(HAXHeader, System.Globalization.NumberStyles.HexNumber, null, out pareseResult)) {

                        header += (char)((((pareseResult) + (changeNumber(5 * i, i))) / 13));
                    }
                }

                /*
                if (passwordInt != (Int32.Parse(splitedPasswordArray[ 0 ], System.Globalization.NumberStyles.HexNumber) + genPassWord) / genPassWord) {
                    outPutData = "wrong password";
                    return;
                }
                */

                for (int i = 1; i < splitedMSGArray.Length; i++) {
                    string cipherDecryptResult = "";
                    string HAXMsg = splitedMSGArray[ i ];

                    if (cipher != null) {
                        cipherDecryptResult = gibgibCipherSystem.Decrypt(HAXMsg);
                    } else {
                        cipherDecryptResult = HAXMsg;
                    }
                    long pareseResult;
                    if (long.TryParse(cipherDecryptResult, System.Globalization.NumberStyles.HexNumber, null, out pareseResult)) {

                        
                        long step = (((pareseResult) + (changeNumber(17 * (i - 1), i - 1))) / 8233);

                        long finAns = (step - passwordInt + genPassWord);
                        if (finAns >= char.MaxValue) {
                            finAns = 0;
                        }
                        outPutData += (char)finAns;
                        //gibgibEncryptTester.instance.printer("finAns : " + finAns);
                        //gibgibEncryptTester.instance.printer("Decrypt" + " HAXMsg : " + HAXMsg + " parseVal : " + pareseResult + " cipherDecryptRe : " + cipherDecryptResult + " data : " + (char)finAns + " pw : " + passwordInt + " genPw : " + genPassWord + " chN : " + (changeNumber(17 * (i - 1), i - 1)) );
                    }
                }
            }

            private static int passwordStringToInt(string password) {
                int passwordInt = 0;
                if (password != "") {
                    for (int i = 0; i < password.Length; i++) {
                        passwordInt += (password[ i ] * i + 23);
                    }
                    passwordInt /= password.Length;
                }
                return passwordInt;
            }

            private static int changeNumber(int number, int index) {
                if (index % 2 == 0) {
                    return -number;
                } else {
                    return number;
                }
            }

            public static void EncryptAndSaveAsGEC(string header, string data, string password, string filePath, string fileName, gibgibCipher cipher) {
                string encryptedData = Encrypt(header, data, password, cipher);
                saveAsGEC(encryptedData, filePath, fileName);
            }

            public static void saveAsGEC(string encryptedData, string filePath, string fileName) {
                File.WriteAllText(filePath + fileName + ".GEC", encryptedData);
            }

            public static void loadAndDecryptFile(string encryptedData, string filePath, string fileName, string password, gibgibCipher cipher, out string header, out string outPutData) {
                header = "";
                outPutData = "";
                string path = filePath + fileName + ".GEC";
                if (!File.Exists(path)) {
                    return;
                }
                Decrypt(File.ReadAllText(path), password, cipher, out header, out outPutData);
            }
        }
    }

    namespace Cipher {
        public static class gibgibCipherSystem {
            public static gibgibCipher cipher = null;
            static string keyWord;

            public static void loadCipher(gibgibCipher newCipher) {
                cipher = newCipher;
            }

            public static void changeKeyWord(string newKeyWord) {
                keyWord = newKeyWord;
            }

            public static string Encrypt(string HEXNumber) {
                string resultString = "";

                char[] charArray = HEXNumber.ToCharArray();
                int o = charArray.Length - 1;

                int keyWordIndex = 0;

                //short[] numberToSingleIntArray = longToSingleIntArray(HEXNumber);
                string HEXPassword = Convert.ToString(passwordStringToInt(keyWord), 16);
                char[] HEXPasswordCharArray = HEXPassword.ToCharArray();


                for (int i = 0; i < charArray.Length; i++) {
                    //gibgibEncryptTester.instance.printer("passwordStringToInt(keyWord) : " + passwordStringToInt(keyWord));
                    //gibgibEncryptTester.instance.printer("keyWordIndex : " + keyWordIndex);
                    char singleHEXString = charArray[ i ];
                    int singleEncryptedHEXNum = (EncryptHEXChar(singleHEXString, HEXPasswordCharArray[ keyWordIndex ]));

                    resultString += cipherConverter(singleEncryptedHEXNum);

                    o--;
                    keyWordIndex++;
                    if (keyWordIndex >= HEXPasswordCharArray.Length) {
                        keyWordIndex = 0;
                    }

                }

                return resultString;
            }

            private static string cipherConverter(int HEX) {
                string ret = HEX.ToString();
                switch (HEX) {
                    case 10:
                        ret = "a";
                        break;
                    case 11:
                        ret = "b";
                        break;
                    case 12:
                        ret = "c";
                        break;
                    case 13:
                        ret = "d";
                        break;
                    case 14:
                        ret = "e";
                        break;
                    case 15:
                        ret = "f";
                        break;
                }
                return ret;
            }

            private static int cipherConverter(char HEX) {
                int ret = HEX;
                switch (HEX) {
                    case '0':
                        ret = 0;
                        break;
                    case '1':
                        ret = 1;
                        break;
                    case '2':
                        ret = 2;
                        break;
                    case '3':
                        ret = 3;
                        break;
                    case '4':
                        ret = 4;
                        break;
                    case '5':
                        ret = 5;
                        break;
                    case '6':
                        ret = 6;
                        break;
                    case '7':
                        ret = 7;
                        break;
                    case '8':
                        ret = 8;
                        break;
                    case '9':
                        ret = 9;
                        break;
                    case 'a':
                        ret = 10;
                        break;
                    case 'b':
                        ret = 11;
                        break;
                    case 'c':
                        ret = 12;
                        break;
                    case 'd':
                        ret = 13;
                        break;
                    case 'e':
                        ret = 14;
                        break;
                    case 'f':
                        ret = 15;
                        break;
                }
                return ret;
            }

            private static int EncryptHEXChar(char SinglePlainHEXChar, char HEXKeyWordIndex) {
                //gibgibEncryptTester.instance.printer("EncryptSingleInt  " + "PlainNumber : " + SinglePlainHEXChar + " EncryptedNumber : " + cipher.cipherArray[ cipherConverter(SinglePlainHEXChar), cipherConverter(HEXKeyWordIndex) ] + " KeyWordIndex : " + HEXKeyWordIndex);
                return cipher.cipherArray[ cipherConverter(SinglePlainHEXChar), cipherConverter(HEXKeyWordIndex) ];
            }

            public static string Decrypt(string HEXNumber) {
                string resultString = "";

                char[] charArray = HEXNumber.ToCharArray();
                int o = charArray.Length - 1;

                int keyWordIndex = 0;

                string HEXPassword = Convert.ToString(passwordStringToInt(keyWord), 16);
                char[] HEXPasswordCharArray = HEXPassword.ToCharArray();

                for (int i = 0; i < charArray.Length; i++) {
                    char singleHEXString = charArray[ i ];
                    int singleDecryptedHEXNum = (DecryptHEXChar(singleHEXString, HEXPasswordCharArray[ keyWordIndex ]));

                    resultString += cipherConverter(singleDecryptedHEXNum);

                    o--;
                    keyWordIndex++;
                    if (keyWordIndex >= HEXPasswordCharArray.Length) {
                        keyWordIndex = 0;
                    }

                }

                return resultString;
            }

            private static int DecryptHEXChar(char SingleCipherNumber, char KeyWordIndex) {
                int SingleCipherNumberInt = cipherConverter(SingleCipherNumber);
                int KeyWordIndexInt = cipherConverter(KeyWordIndex);
                for (int i = 0; i < 16; i++) {
                    int item = cipher.cipherArray[ i, KeyWordIndexInt ];
                    if (item == SingleCipherNumberInt) {
                        //gibgibEncryptTester.instance.printer("DecryptSingleInt  " + "EncryptedNumber : " + item + " DecryptedNumber : " + i + " KeyWordIndex : " + KeyWordIndexInt);
                        return i;
                    }
                }
                //gibgibEncryptTester.instance.printer("DecryptFailed need find " + SingleCipherNumberInt + "[" + "?" + "," + KeyWordIndexInt + "]");
                return -1;
            }

            private static short[] longToSingleIntArray(long Number) {
                int length = Number.ToString().Length;
                short[] ret = new short[ length ];
                int var = length;
                long processedNumber = 0;
                for (int i = 0; i < ret.Length; i++) {
                    processedNumber *= 10;
                    short nowNumber = (short)Math.Floor(((Number / (Math.Pow(10, (var - 1))))) - processedNumber);
                    ret[ i ] = nowNumber;
                    processedNumber += nowNumber;
                    var--;
                }
                return ret;
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
        public class gibgibCipher {
            public int[,] cipherArray;
            public gibgibCipher() {
                cipherArray = new int[ 16, 16 ];
                for (int i = 0; i < 16; i++) {
                    for (int j = 0; j < 16; j++) {
                        cipherArray[ i, j ] = ((j + i) % 16 + 1) - 1;
                        //cipherArray[ i,j ] = ((i * 1 + i / 1 + j) % 10 + 1)-1 ;
                    }
                }

                for (int i = 0; i < 256; i++) {
                    Random rand = new Random(Guid.NewGuid().GetHashCode());
                    Random rand2 = new Random(Guid.NewGuid().GetHashCode());
                    changeTwoCell(rand.Next(0, 15), rand2.Next(0, 15));
                }


            }

            private void changeTwoCell(int findValue1, int findValue2) {
                int xParam1, yParam1, xParam2, yParam2;
                xParam1 = yParam1 = xParam2 = yParam2 = 0;
                bool find1 = false, find2 = false;
                for (int i = 0; i < 16; i += 1) {
                    for (int k = 0; k < 16; k += 1) {
                        if (cipherArray[ i, k ] == findValue1) {
                            xParam1 = i;
                            yParam1 = k;
                            find1 = true;
                        }
                        if (cipherArray[ i, k ] == findValue2) {
                            xParam2 = i;
                            yParam2 = k;
                            find2 = true;
                        }
                        if (find1 && find2) {
                            cipherArray[ xParam1, yParam1 ] = findValue2;
                            cipherArray[ xParam2, yParam2 ] = findValue1;
                        }
                    }
                }

            }
        }

        namespace litJsonCipherClass {
            public class litJsonCipherArrayClass {
                public List<oneLineClass> cipherList = new List<oneLineClass>(new oneLineClass[ 16 ]);

                public static litJsonCipherArrayClass init() {
                    litJsonCipherArrayClass List = new litJsonCipherArrayClass();
                    for (int i = 0; i < List.cipherList.Count; i++) {
                        List.cipherList[ i ] = new oneLineClass();
                    }
                    return List;
                }

                public static litJsonCipherArrayClass CipherArrayToCipherList(gibgibCipher cipher) {
                    litJsonCipherArrayClass List = init();
                    for (int i = 0; i < List.cipherList.Count; i++) {
                        List.cipherList[ i ] = new oneLineClass();
                    }

                    for (int x = 0; x < 16; x++) {
                        for (int y = 0; y < 16; y++) {
                            List.cipherList[ y ].oneLine[ x ] = cipher.cipherArray[ x, y ];
                        }
                    }
                    return List;
                }
                public static gibgibCipher CipherListToCipherArray(litJsonCipherArrayClass cipher) {
                    gibgibCipher cipherArray = new gibgibCipher();
                    for (int x = 0; x < 16; x++) {
                        for (int y = 0; y < 16; y++) {
                            cipherArray.cipherArray[ x, y ] = cipher.cipherList[ y ].oneLine[ x ];
                        }
                    }
                    return cipherArray;
                }
            }
            public class oneLineClass {
                public List<int> oneLine = new List<int>(new int[ 16 ]);

            }
        }
    }
}


