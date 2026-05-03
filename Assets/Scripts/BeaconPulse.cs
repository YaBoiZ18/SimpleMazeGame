using UnityEngine;
using System.Collections;

public class BeaconPulse : MonoBehaviour
{
    [Header("Settings")]
    public GameObject beamPrefab;
    public float pulseEverySeconds = 20f;
    public float beamDuration = 3f;
    public float fadeDuration = 1f;

    private MazeTimer mazeTimer;
    private GameObject currentBeam;

    private int nextPulseTime = 20;
    private bool disabledForever = false;
    private bool spawningBeam = false;

    void Start()
    {
        mazeTimer = FindObjectOfType<MazeTimer>();
    }

    void Update()
    {
        if (disabledForever || mazeTimer == null)
            return;

        float time = mazeTimer.GetElapsedTime();

        if (time >= nextPulseTime && !spawningBeam)
        {
            StartCoroutine(SpawnBeam());

            nextPulseTime += (int)pulseEverySeconds;
        }
    }

    IEnumerator SpawnBeam()
    {
        spawningBeam = true;

        if (beamPrefab == null)
        {
            spawningBeam = false;
            yield break;
        }

        currentBeam = Instantiate(
            beamPrefab,
            transform.position,
            Quaternion.identity
        );

        Renderer renderer = currentBeam.GetComponent<Renderer>();

        if (renderer == null)
        {
            Destroy(currentBeam);
            spawningBeam = false;
            yield break;
        }

        Material mat = renderer.material;
        Color color = mat.color;

        // Fade In
        float t = 0f;

        while (t < fadeDuration && currentBeam != null)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            mat.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(beamDuration - fadeDuration * 2f);

        // Fade Out
        t = 0f;

        while (t < fadeDuration && currentBeam != null)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            mat.color = color;
            yield return null;
        }

        if (currentBeam != null)
            Destroy(currentBeam);

        spawningBeam = false;
    }

    public void DisableBeacon()
    {
        disabledForever = true;

        StopAllCoroutines();

        if (currentBeam != null)
            Destroy(currentBeam);

        gameObject.SetActive(false);
    }
}