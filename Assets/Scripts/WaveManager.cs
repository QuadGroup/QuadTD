using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {

    public AstarPath astarpath;
    public BoxCollider SpawnZoneWave;
    public BoxCollider targetZoneWave;
    public GameObject[] enemyPrefabs;

    SpawnPointGuard selectedPoint;
    public List<Vector3> spawnPoints;
    public List<Vector3> targetPoints;
    public int NumberEnemiesinWave;

    public ParticleSystem enemiesMist;

    public bool isEnemiesSpawned = false;
    public int enemiesCounter = 0;

    public float spawnEnemiesDelay;

    // Use this for initialization
    void Start()
    {
        astarpath.graphs[0].GetNodes((node) =>
        {
            if (SpawnZoneWave.bounds.Contains((Vector3)node.position))
            {
                spawnPoints.Add((Vector3)node.position);
            }
            if (targetZoneWave.bounds.Contains((Vector3)node.position))
            {
                targetPoints.Add((Vector3)node.position);
            }
            return true;
        });
        DisableSpawnZoneEditorMode();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DisableSpawnZoneEditorMode()
    {
        //SpawnGuardsZone.enabled = false;
        SpawnZoneWave.GetComponent<MeshRenderer>().enabled = false;
    }

    public void SpawnEnemies(int stageId)
    {
        enemiesMist.Play();
        StartCoroutine(StartSpawnEnemies(stageId));
    }

    IEnumerator StartSpawnEnemies(int stageId)
    {
        yield return new WaitForSeconds(spawnEnemiesDelay);

        for (int i = 0; i < NumberEnemiesinWave; i++)
        {
            //GameObject enemy = Instantiate(enemyPrefabs[stageId], GetRandomSpawnZonePosition(), SpawnZoneWave.transform.rotation) as GameObject;
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0,enemyPrefabs.Length)], GetRandomSpawnZonePosition(), SpawnZoneWave.transform.rotation) as GameObject;
            enemy.GetComponent<UnitController>().endPoint = GetRandomTargetZonePosition();
            enemy.GetComponent<UnitController>().waveManager = this;
            //enemy.GetComponent<AstarAI>().targetPosition = GetRandomTargetZonePosition();
            //Debug.Log(enemy.name);
            enemiesCounter++;
            yield return new WaitForSeconds(Random.Range(0.1f,2f));
        }
        isEnemiesSpawned = true;
        yield return null;
    }

    Vector3 GetRandomSpawnZonePosition()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
    Vector3 GetRandomTargetZonePosition()
    {
        return targetPoints[Random.Range(0, spawnPoints.Count)];
    }
}
