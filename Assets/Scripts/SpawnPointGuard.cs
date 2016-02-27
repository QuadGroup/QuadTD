using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SpawnPointGuard : MonoBehaviour {

    public GuardsManager guardManager;
    public bool isEmpty = true;
    public GameObject emptyGO;
    public Vector3 guardStartPos;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnEmptyClick()
    {
        guardManager.ShowUnitSelection(this);
    }

    public void SetNotEmpty()
    {
        isEmpty = false;
        emptyGO.SetActive(false);
    }
    public void SetEmpty()
    {
        isEmpty = true;
        emptyGO.SetActive(true);
    }
}
