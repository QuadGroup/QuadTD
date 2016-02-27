using UnityEngine;
using System.Collections;

public class FXController : MonoBehaviour {

    public GameObject hitFX;

    public void Hit()
    {
        Debug.Log("Hit");
        Instantiate(hitFX, new Vector3(transform.position.x, 19, transform.position.z), Quaternion.identity);
    }

}
