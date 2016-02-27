using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StagesController : MonoBehaviour {

    public float playerStageTime=0;
    public Text timerUIText;
    public RectTransform StageInfoPanel;
    public GuardsManager guardsManager;
    public WaveManager waveManager;

    public CameraAnimationController cameraAnim;
    public AudioManager audioManager;

    public bool activewaveAnim = false;

	// Use this for initialization
	void Start () {
        PrepearingStage();
	}
	
	// Update is called once per frame
	void Update () {
        if (waveManager.isEnemiesSpawned)
        {
            if (waveManager.enemiesCounter <= 0)
            {
                EndBattleStage();
            }
        }
	}

    public void PrepearingStage()
    {
        audioManager.PrepearingStage();

        foreach (CanvasGroup stageInfo in StageInfoPanel.GetComponentsInChildren<CanvasGroup>())
        {
            stageInfo.alpha = 1;
        }
        guardsManager.showSpawnZone();

        StartCoroutine(PrepearingStageTimer());
    }

    public void EndPrepearingStage()
    {
        Debug.Log("End Prepearing");
        foreach (CanvasGroup stageInfo in StageInfoPanel.GetComponentsInChildren<CanvasGroup>())
        {
            stageInfo.alpha = 0;
        }
        guardsManager.hideSpawnZone();

        BattleStage();
    }

    public void BattleStage()
    {
        audioManager.BattleStage();

        if (activewaveAnim)
        cameraAnim.WaveStartAnim();
        waveManager.SpawnEnemies(0);
    }

    public void EndBattleStage()
    {
        waveManager.isEnemiesSpawned = false;
        waveManager.enemiesCounter = 0;
        PrepearingStage();
        waveManager.enemiesMist.Stop();
    }

    public IEnumerator PrepearingStageTimer()
    {
        float time = playerStageTime;
        while (time >= 0)
        {
            timerUIText.text = "Бой начнется через " + time + " секунд";
            yield return new WaitForSeconds(1);
            time -= 1;
        }
        EndPrepearingStage();
        yield return null;
    }
}
