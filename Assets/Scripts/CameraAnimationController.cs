using UnityEngine;
using System.Collections;

public class CameraAnimationController : MonoBehaviour {

    public MainCamera mainCAmera;
    public Transform[] waveStartPoints;

	public void WaveStartAnim()
    {
        StartCoroutine(WaveStart());
	}

    IEnumerator WaveStart()
    {
        mainCAmera.isControllerOn = false;
        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;
        for (int i = 0; i <= waveStartPoints.Length; i++)
        {
            float elapsedTime = 0;
            float time = 3.0f;

            Vector3 startingPos = transform.position;
            Quaternion startingRot = transform.rotation;

            while (elapsedTime < time)
            {
                if (i < waveStartPoints.Length)
                {
                    transform.position = Vector3.Lerp(startingPos, waveStartPoints[i].position, (elapsedTime / time));
                    transform.rotation = Quaternion.Lerp(startingRot, waveStartPoints[i].rotation, (elapsedTime / time));
                }
                else if (i == waveStartPoints.Length)
                {
                    transform.position = Vector3.Lerp(startingPos, currentPos, (elapsedTime / time));
                    transform.rotation = Quaternion.Lerp(startingRot, currentRot, (elapsedTime / time));
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        mainCAmera.isControllerOn = true;
        yield return null;
    }
}
