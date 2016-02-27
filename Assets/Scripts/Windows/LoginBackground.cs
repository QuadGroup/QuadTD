using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginBackground : MonoBehaviour {

    MovieTexture movie;

	// Use this for initialization
	void Start () {
        movie = GetComponent<RawImage>().texture as MovieTexture;
        movie.loop = true;
        movie.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
