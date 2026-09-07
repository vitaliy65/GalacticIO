using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
[RequireComponent(typeof(Light))]
public class CloudShadowScroller : MonoBehaviour
{
    public Vector2 windDirection = new Vector2(1f, 0.4f);
    public float speed = 0.5f;

    UniversalAdditionalLightData lightData;

    void OnEnable()
    {
        lightData = GetComponent<UniversalAdditionalLightData>();
    }

    void Update()
    {
        if (lightData == null) return;
        lightData.lightCookieOffset = windDirection.normalized * speed * Time.time;
    }
}