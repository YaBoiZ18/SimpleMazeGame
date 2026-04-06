using UnityEngine;
using System.Collections;

public class BeaconPulse : MonoBehaviour
{
    [Header("Settings")]
    public GameObject beamPrefab;
    public float interval = 20f; // time between each pulse
    public float beamDuration = 3f; // total time the beam is active (including fade in/out)
    public float fadeDuration = 1f; // time for fade in/out

    private void Start()
    {
        StartCoroutine(PulseRoutine());
    }

    IEnumerator PulseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            StartCoroutine(SpawnBeam());
        }
    }

    IEnumerator SpawnBeam()
    {
        if (beamPrefab == null) yield break;

        GameObject beam = Instantiate(beamPrefab, transform.position, Quaternion.identity);

        // Ensure the beam has a material that supports transparency (e.g., Standard shader, Transparent mode)
        Renderer renderer = beam.GetComponent<Renderer>();
        if (renderer == null)
        {
            Destroy(beam, beamDuration);
            yield break;
        }

        Material mat = renderer.material;
        Color color = mat.color;

        // Fade in
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            mat.color = color;
            yield return null;
        }

        // Fully visible for the remaining time
        yield return new WaitForSeconds(beamDuration - 2 * fadeDuration);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            mat.color = color;
            yield return null;
        }

        Destroy(beam);
    }
}