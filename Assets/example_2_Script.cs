using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Runtime.InteropServices;
using gibgibEncrypt.Encrypt;
using gibgibEncrypt.EncVar;
using gibgibEncrypt.Cipher;

public class example_2_Script : MonoBehaviour {
[SerializeField]
Text showtext;

int showMaxHP;
int showHP;

[SerializeField]
int MaxHP;
[SerializeField]
int HP;
	void Start () {
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


