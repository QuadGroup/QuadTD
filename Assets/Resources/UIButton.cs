using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIButton : MonoBehaviour {

    public Text ButtonText;
    public int UpTextSize;
    public int DownTextSize;

    public void OnButtonUp()
    {
        ButtonText.fontSize = UpTextSize;
    }

    public void OnButtonDown()
    {
        ButtonText.fontSize = DownTextSize;
    }
}
