using System;
using System.IO;
using gibgibEncrypt.Encrypt;
using gibgibEncrypt.Cipher;
using System.Collections.Generic;

namespace gibgibEncrypt {
    //encrypt algorithm Ver 1.05_EncVar
    namespace Encrypt {
        public static class gibgibEncryptSystem {
            private static string ver = "V1.04_EncVar";
            static Random rnd = new Random(Guid.NewGuid().GetHashCode());
            public static string Encrypt(string header, string data, string password, gibgibCipher cipher) {
                string outPutData = "";

                int passwordInt = passwordStringToInt(password);
                gibgibCipherSystem.changeKeyWord(password);

                int genPassWord = rnd.Next(1, 128);
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

            public static bool loadAndDecryptFile(string filePath, string fileName, string password, gibgibCipher cipher, out string header, out string outPutData) {
                header = "";
                outPutData = "";
                string path = filePath + fileName + ".GEC";
                if (!File.Exists(path)) {
                    return false;
                }
                Decrypt(File.ReadAllText(path), password, cipher, out header, out outPutData);
                return true;
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

            public static void loadCipherFromFilePath(string path) {
                string Data = File.ReadAllText(path);

                string[] arr = Data.Split('-');
                byte[] array = new byte[ arr.Length ];
                for (int i = 0; i < arr.Length; i++) array[ i ] = Convert.ToByte(arr[ i ], 16);

                string output = System.Text.Encoding.Default.GetString(array);

                gibgibCipher newCipher = gibgibCipherMapperToString(output);

                loadCipher(newCipher);
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

            public static void changeKeyWord(string newKeyWord) {
                keyWord = newKeyWord;
            }

            public static string Encrypt(string HEXNumber) {
                string resultString = "";

                char[] charArray = HEXNumber.ToCharArray();
                int o = charArray.Length - 1;

                int keyWordIndex = 0;

                string HEXPassword = Convert.ToString(passwordStringToInt(keyWord), 16);
                char[] HEXPasswordCharArray = HEXPassword.ToCharArray();


                for (int i = 0; i < charArray.Length; i++) {
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
                        return i;
                    }
                }
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
                    for (int o = 0; o < 16; o++) {
                        cipherArray[ o, i ] = o;
                    }

                    for (int j = 0; j < 32; j++) {
                        Random rand = new Random(Guid.NewGuid().GetHashCode());
                        Random rand2 = new Random(Guid.NewGuid().GetHashCode());
                        int randNum = rand.Next(0, 16);
                        int randNum2 = rand.Next(0, 16);
                        changeTwoIndex(i,randNum, randNum2);
                    }

                }

            }

            public gibgibCipher(int[,] intArray) {
                cipherArray = intArray;
            }

            public bool RListRepeatChecker(List<int> RList , int targetNum) {
                for (int i = 0; i < RList.Count; i++) {
                    if (RList[ i ] == targetNum) {
                        return true;
                    }
                }
                return false;
            }

            private void changeTwoIndex(int columnsIndex, int findIndex1, int findIndex2) {
                int saver = cipherArray[ findIndex1 , columnsIndex ];
                cipherArray[ findIndex1 , columnsIndex ] = cipherArray[ findIndex2 , columnsIndex ];
                cipherArray[ findIndex2 , columnsIndex ] = saver;
            }
            
            }
    }

    namespace EncVar {
        public struct EncInt {
            //加密後用string保存
            private string _ENC;
            //加密用密碼
            string pw;

            //隱轉換器 set 開新加密設定
            public static implicit operator EncInt(int value) {
                return new EncInt(value.ToString());
            }
            //隱轉換器 get 進行解密提取
            public static implicit operator int(EncInt Enc) {
                string head;
                string ret;
                gibgibEncryptSystem.Decrypt(Enc._ENC, Enc.pw, gibgibCipherSystem.cipher, out head, out ret);
                return int.Parse(ret);
            }
            //初始化加密設定
            public EncInt(string value) {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                pw = rnd.Next(1, 2048).ToString();
                _ENC = gibgibEncryptSystem.Encrypt("", value, pw, gibgibCipherSystem.cipher);
          
            }
        }
        public struct EncString {
            //加密後用string保存
            private string _ENC;
            //加密用密碼
            string pw;
            //隱轉換器 set 開新加密設定
            public static implicit operator EncString(string value) {
                return new EncString(value);
            }
            //隱轉換器 get 進行解密提取
            public static implicit operator string(EncString Enc) {
                string head;
                string ret;
                gibgibEncryptSystem.Decrypt(Enc._ENC, Enc.pw, gibgibCipherSystem.cipher, out head, out ret);
                return ret;
            }
            //初始化加密設定
            public EncString(string value) {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                pw = rnd.Next(1, 2048).ToString();
                _ENC = gibgibEncryptSystem.Encrypt("", value, pw, gibgibCipherSystem.cipher);
            }
        }
        public struct EncFloat {
            //加密後用string保存
            private string _ENC;
            //加密用密碼
            string pw;
            //隱轉換器 set 開新加密設定
            public static implicit operator EncFloat(float value) {
                return new EncFloat(value.ToString());
            }
            //隱轉換器 get 進行解密提取
            public static implicit operator float(EncFloat Enc) {
                string head;
                string ret;
                gibgibEncryptSystem.Decrypt(Enc._ENC, Enc.pw, gibgibCipherSystem.cipher, out head, out ret);
                return float.Parse(ret);
            }
            //初始化加密設定
            public EncFloat(string value) {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                pw = rnd.Next(1, 2048).ToString();
                _ENC = gibgibEncryptSystem.Encrypt("", value, pw, gibgibCipherSystem.cipher);
            }
        }
    }
}


