using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        StartCoroutine(timer(3.0f));
    }

    IEnumerator timer(float waitTime) {
        Clipboard = inputFieldResult.text;
        logText.text = "Log: copied result text to Clipboard";
        yield return new  WaitForSeconds(waitTime);
        logText.text = "Log:";
    }

    public static string Clipboard {
        get { return GUIUtility.systemCopyBuffer; }
        set { GUIUtility.systemCopyBuffer = value; }
    }

}
