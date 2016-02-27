using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FriendsListItem : MonoBehaviour {

    public Text nameText;
    public Text statusText;

    public void SetData(string name,string status)
    {
        nameText.text = name;
        statusText.text = status;
    }

    public string GetName()
    {
        return nameText.text;
    }
}
