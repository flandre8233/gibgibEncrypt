using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Runtime.InteropServices;
using gibgibEncrypt.Encrypt;
using gibgibEncrypt.EncVar;
using gibgibEncrypt.Cipher;

using System.IO;

public class example_2_Script : MonoBehaviour {
[SerializeField]
Text showtext;

int showMaxHP;
int showHP;

[SerializeField]
EncInt MaxHP;
[SerializeField]
EncInt HP;
	void Start () {
        gibgibCipherSystem.loadCipherFromFilePath("Assets/Resources/gibgibCipher_1-25.GKEY");
		HP = 75;
		MaxHP = 100;
	}
	void Update () {
		SyncHPData();
		string result = showHP + " / " + showMaxHP;
		showtext.text = result;
		if(Input.GetKeyDown(KeyCode.N) ){
			HP++;
		}
		if(Input.GetKeyDown(KeyCode.M) ){
			HP--;
		}
	}
	void SyncHPData(){
		showHP = HP;
		showMaxHP = MaxHP;
	}
}


