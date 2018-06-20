using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gibgibEncryptTester : MonoBehaviour {
    public string EncryptPassword;
    public string InputSting ;
    public string DecryptPassword;
    public string OutputSting;

    //static string path = Application.persistentDataPath + "/playerData.GEC";


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /*
        if (Input.GetKeyDown(KeyCode.E) ) {
            print( InputSting.Length );
            OutputSting = gibgibEncryptSystem.Encrypt(InputSting, EncryptPassword);
            print(OutputSting);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            print(InputSting.Length);
            OutputSting = gibgibEncryptSystem.Decrypt(InputSting, DecryptPassword);
            print(OutputSting);
        }
        */

        if (Input.GetKeyDown(KeyCode.N)) {
        }
    }
 
}
