using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Crosstales.FB;

public class gibgibEncryptSceneCanvasController : MonoBehaviour {
    [SerializeField]
    InputField inputFieldContent;
    [SerializeField]
    InputField inputFieldPassword;
    [SerializeField]
    InputField inputFieldHeader;
    [SerializeField]
    InputField inputFieldResult;

    [SerializeField]
    Text logText;

    [SerializeField]
    outputManager outputManager;
    public void OnButtonClick(bool isEncrypt) {
        if (isEncrypt) {
            inputFieldResult.text = gibgibEncryptSystem.Encrypt(inputFieldHeader.text, inputFieldContent.text, inputFieldPassword.text);
        } else {
            string header = "";
            string content = "";
            gibgibEncryptSystem.Decrypt(inputFieldContent.text, inputFieldPassword.text, out header, out content);
            inputFieldResult.text = content;
            inputFieldHeader.text = header;
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
                File.WriteAllText(path, inputFieldResult.text);
                StartCoroutine(timer(3.0f, "Successful output .GEC file"));
            }
        } else {
            StartCoroutine(timer(3.0f, "you not have any Encrypt Result"));
        }

    }

    public void openFileButton() {
        string path = FileBrowser.OpenSingleFile("open GEC file", "", "GEC");
        //string path = UnityEditor.EditorUtility.OpenFilePanel("open GEC file", "", "GEC");
        print("path:" + path);
        if (path.Length != 0) {
            StartCoroutine(timer(3.0f, "opened and try Decrypt .GEC"));
            string fileContent = File.ReadAllText(path);
            inputFieldContent.text = fileContent;
            OnButtonClick(false);
        }
    }

    public void cleanButton() {
        inputFieldContent.text = "";
        //inputFieldPassword.text = "";
        inputFieldHeader.text = "";
        inputFieldResult.text = "";
    }

    IEnumerator timer(float waitTime,string displayText) {
        Clipboard = inputFieldResult.text;
        logText.text = "Log:" + displayText;
        yield return new  WaitForSeconds(waitTime);
        logText.text = "Log:";
    }

    public static string Clipboard {
        get { return GUIUtility.systemCopyBuffer; }
        set { GUIUtility.systemCopyBuffer = value; }
    }

}
