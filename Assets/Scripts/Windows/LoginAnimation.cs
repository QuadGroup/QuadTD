using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginAnimation : MonoBehaviour {

    private Sprite[] sprites;
	// Use this for initialization
	void Start () {
        sprites = Resources.LoadAll<Sprite>("UI/Login/Animation");
        Debug.Log("Sprites:" + sprites.Length);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
