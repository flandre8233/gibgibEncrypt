
using UnityEngine;
using UnityEngine.UI;
public class gibgibEncryptSceneCanvasController : MonoBehaviour {
    [SerializeField]
    InputField inputFieldContent;
    [SerializeField]
    InputField inputFieldPassword;
    [SerializeField]
    InputField inputFieldResult;

    [SerializeField]
    outputManager outputManager;
    public void OnButtonClick() {
        if (outputManager.isEncryptOutPut) {
            inputFieldResult.text = gibgibEncryptSystem.Encrypt(inputFieldContent.text, inputFieldPassword.text);
        } else {
            inputFieldResult.text = gibgibEncryptSystem.Decrypt(inputFieldContent.text, inputFieldPassword.text);

        }
    }

}
