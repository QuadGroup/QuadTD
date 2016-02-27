using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine.EventSystems;

public class GuardsManager : MonoBehaviour {

    public AstarPath astarpath;
    public BoxCollider SpawnZoneGuards;
    public GameObject spawnPointGuardPrefab;

    public Camera mainCamera;
    public RectTransform UnitSelectionPanel;

    SpawnPointGuard selectedPoint;
    public List<SpawnPointGuard> points;

	// Use this for initialization
	void Start () {
        astarpath.graphs[0].GetNodes ((node) => {
            if (SpawnZoneGuards.bounds.Contains((Vector3)node.position))
            {
                GameObject spawnPoint = Instantiate(spawnPointGuardPrefab, (Vector3)node.position, Quaternion.identity) as GameObject;
                SpawnPointGuard point = spawnPoint.GetComponent<SpawnPointGuard>();
                point.guardManager = this;
                points.Add(point);
            }
            return true;
        });
        DisableSpawnZoneEditorMode();
	}

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag != "SpawnPointGuard" && UnitSelectionPanel.gameObject.activeInHierarchy)
                {
                    HideUnitSelection();
                }
            }
            else if (UnitSelectionPanel.gameObject.activeInHierarchy)
            {
                HideUnitSelection();
            }
        }
    }
    public void DisableSpawnZoneEditorMode()
    {
        //SpawnGuardsZone.enabled = false;
        SpawnZoneGuards.GetComponent<MeshRenderer>().enabled = false;
    }

    public void ShowUnitSelection(SpawnPointGuard point)
    {
        selectedPoint = point;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(point.transform.position);
        UnitSelectionPanel.transform.position = screenPos;
        UnitSelectionPanel.gameObject.SetActive(true);
    }

    public void HideUnitSelection()
    {
        UnitSelectionPanel.gameObject.SetActive(false);
    }

    public void SpawnGuard(GameObject guardPrefab)
    {
        selectedPoint.SetNotEmpty();
        GameObject guardGo = Instantiate(guardPrefab, selectedPoint.transform.position, SpawnZoneGuards.transform.rotation) as GameObject;
        guardGo.transform.parent = selectedPoint.transform;
        guardGo.GetComponent<AstarAI>().targetPosition = guardGo.transform.position;
        HideUnitSelection();
    }

    public void hideSpawnZone()
    {
        foreach (SpawnPointGuard point in points)
        {
            if (point.isEmpty)
            {
                point.gameObject.SetActive(false);
            }
        }
    }

    public void showSpawnZone()
    {
        foreach (SpawnPointGuard point in points)
        {
            if (point.isEmpty)
            {
                point.gameObject.SetActive(true);
                point.emptyGO.SetActive(true);
            }
            else
            {
                point.GetComponentInChildren<UnitController>().transform.position = point.guardStartPos;
            }
        }
    }
}
