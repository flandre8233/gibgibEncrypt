using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Crosstales.FB;
using gibgibEncrypt.Encrypt;
using gibgibEncrypt.Cipher;
using gibgibEncrypt.Cipher.litJsonCipherClass;

public class gibgibEncryptSceneCanvasController : SingletonMonoBehavior<gibgibEncryptSceneCanvasController> {
    [SerializeField]
    InputField inputFieldContent;
    [SerializeField]
    InputField inputFieldPassword;
    [SerializeField]
    InputField inputFieldHeader;
    [SerializeField]
    InputField inputFieldResult;
    [SerializeField]
    Toggle cipherToggle;
    [SerializeField]
    Text loadedCipherText;

    [SerializeField]
    Text logText;

    [SerializeField]
    outputManager outputManager;

    private string loadedCipherName = "";
    private string lastKnowCipherName = "nothing";

    public void updateLoadedCipherText() {
        string orlText = "LoadedKey:";
        loadedCipherText.text = orlText + loadedCipherName;
    }

    public void OnCipherToggleValueChanged() {
        if (!cipherToggle.isOn) {
            lastKnowCipherName = loadedCipherName;
            loadedCipherText.enabled = false;
        } else {
            loadedCipherText.enabled = true;
            loadedCipherName = lastKnowCipherName;
            updateLoadedCipherText();
        }

    }

    public void OnButtonClick(bool isEncrypt) {
        if (isEncrypt) {
            string header = "";
            string content = "";
            if (cipherToggle.isOn) {
                if (gibgibCipherSystem.cipher == null) {
                    StartCoroutine(timer(3.0f, "No cipher detected cant encrypt"));
                } else {
                    inputFieldResult.text = gibgibEncryptSystem.Encrypt(inputFieldHeader.text, inputFieldContent.text, inputFieldPassword.text, gibgibCipherSystem.cipher);
                    gibgibEncryptSystem.Decrypt(inputFieldResult.text, inputFieldPassword.text, gibgibCipherSystem.cipher, out header, out content);
                }
            } else {
                inputFieldResult.text = gibgibEncryptSystem.Encrypt(inputFieldHeader.text, inputFieldContent.text, inputFieldPassword.text, null);
                gibgibEncryptSystem.Decrypt(inputFieldResult.text, inputFieldPassword.text, null, out header, out content);
            }



        } else {
            string header = "";
            string content = "";
            if (cipherToggle.isOn) {
                gibgibEncryptSystem.Decrypt(inputFieldContent.text, inputFieldPassword.text, gibgibCipherSystem.cipher, out header, out content);
            } else {
                gibgibEncryptSystem.Decrypt(inputFieldContent.text, inputFieldPassword.text, null, out header, out content);
            }
            inputFieldHeader.text = header;
            inputFieldResult.text = content;

        }
    }

    public void OnLoadCipherButtonClick() {
        string path = FileBrowser.OpenSingleFile("open .GKEY file", "", "GKEY");
        if (path.Length != 0) {
            string fileName = Path.GetFileName(path);
            string fileContent = File.ReadAllText(path);
            //must use list
            litJsonCipherArrayClass JSONClass = LitJson.JsonMapper.ToObject<litJsonCipherArrayClass>(fileContent.ToString());
            gibgibCipher cipher = litJsonCipherArrayClass.CipherListToCipherArray(JSONClass);
            gibgibCipherSystem.loadCipher(cipher);
            loadedCipherName = Path.GetFileName(path);
            lastKnowCipherName = loadedCipherName;
            updateLoadedCipherText();
            StartCoroutine(timer(3.0f, "Successful Loaded "+ fileName));
        }
    }

    public void OnGenCipherButtonClick() {
        string path = FileBrowser.SaveFile("Save Result as .GKEY", "", "gibgibCipher", "GKEY");
        if (path.Length != 0) {
            string fileName = Path.GetFileName(path);
            gibgibCipher cipher = new gibgibCipher();
            gibgibCipherSystem.loadCipher(cipher);
            loadedCipherName = fileName;
            lastKnowCipherName = loadedCipherName;
            updateLoadedCipherText();
            litJsonCipherArrayClass JSONClass = litJsonCipherArrayClass.CipherArrayToCipherList(cipher);
            string fileContent = LitJson.JsonMapper.ToJson(JSONClass);

            File.WriteAllText(path, fileContent);
            StartCoroutine(timer(3.0f, "Successful Generate "+ fileName + " file"));
        } else {
            StartCoroutine(timer(3.0f, "you dont have any key input"));
        }
    }

    public void showUpMSG() {
        StartCoroutine(timer(3.0f, "copied result text to Clipboard"));
    }

    public void saveFileButton() {
        OnButtonClick(true);
        //string path = EditorUtility.OpenFilePanel("save GEC file", "", "GEC");
        if (inputFieldResult.text != "") {
            //string path = UnityEditor.EditorUtility.SaveFilePanel("Save texture as PNG", "", "GEC" + ".GEC", "GEC");
            string path = FileBrowser.SaveFile("Save Result as .GEC", "", inputFieldHeader.text, "GEC");
            if (path.Length != 0) {
                string fileName = Path.GetFileName(path);

                File.WriteAllText(path, inputFieldResult.text);
                StartCoroutine(timer(3.0f, "Successful output "+fileName+" file"));
            }
        } else {
            StartCoroutine(timer(3.0f, "you dont have any Encrypt output"));
        }

    }

    public void openFileButton() {
        string path = FileBrowser.OpenSingleFile("open .GEC file", "", "GEC");
        if (path.Length != 0) {
            string fileName = Path.GetFileName(path);
            StartCoroutine(timer(3.0f, "opened and try Decrypt "+ fileName));
            string fileContent = File.ReadAllText(path);
            inputFieldContent.text = fileContent;
            OnButtonClick(false);
        }
    }

    /*
    public static void Save(saveGameData saveGame) {
        string jsonData = JsonMapper.ToJson(saveGame);
        File.WriteAllText(path, jsonData);
    }
    */

    public void cleanButton() {
        inputFieldContent.text = "";
        //inputFieldPassword.text = "";
        inputFieldHeader.text = "";
        inputFieldResult.text = "";
    }

    IEnumerator timer(float waitTime,string displayText) {
            Clipboard = inputFieldResult.text;
            Clipboard = "";
        logText.text = "Log:" + displayText;
        yield return new  WaitForSeconds(waitTime);
        logText.text = "Log:";
    }

    public static string Clipboard {
        get { return GUIUtility.systemCopyBuffer; }
        set { GUIUtility.systemCopyBuffer = value; }
    }

}



