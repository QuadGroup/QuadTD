using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //StartCoroutine(Server.Listener());
        StartCoroutine(loading());
        
	}
	IEnumerator loading()
    {
        yield return new WaitForSeconds(2);
        Application.LoadLevel(3);
    }
}
