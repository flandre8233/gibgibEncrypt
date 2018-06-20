using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class outputManager : MonoBehaviour {
    [SerializeField]
     Text[] text;
    [SerializeField]
     Text verText;

    public bool isEncryptOutPut;

    private void Start() {
       // replaceText();
        verText.text = "Ver : "+Application.version;
    }

    /*
     void replaceText() {
        if (isEncryptOutPut) {
            return;
        }

        for (int i = 0; i < text.Length; i++) {
            Text item = text[ i ];
            item.text = item.text.Replace("En","De");
        }
    }
    */
}
