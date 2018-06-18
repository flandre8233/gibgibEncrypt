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
    InputField inputFieldResult;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnButtonClick() {
        inputFieldResult.text = gibgibEncryptSystem.Encrypt(inputFieldContent.text,inputFieldPassword.text);
    }

}
