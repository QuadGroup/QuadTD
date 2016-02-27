using UnityEngine;
using System.Collections;

public class AS_GUITextToggle : MonoBehaviour {
	// Update is called once per frame
	private GUIText theText;
	
	void Start()
	{
		theText = GetComponent(typeof(GUIText)) as GUIText;
	}
	
	void Update () {
		if (Input.GetKeyUp(KeyCode.H))
		{
			theText.enabled = !theText.enabled;
		}
	}
}
