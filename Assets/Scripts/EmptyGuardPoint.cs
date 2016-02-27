using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EmptyGuardPoint : MonoBehaviour {

    public SpawnPointGuard parent;

    void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            parent.OnEmptyClick();
        }
    }
}
