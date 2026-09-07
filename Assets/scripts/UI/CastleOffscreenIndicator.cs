using UnityEngine;

public class CastleOffscreenIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform arrow;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GenerateCastle castleGenerator;
    [SerializeField] private Camera targetCamera;
    [SerializeField, Range(0f, 0.45f)] private float screenEdgePadding = 0.08f;

    private void Awake()
    {
        targetCamera ??= Camera.main;
        castleGenerator ??= FindAnyObjectByType<GenerateCastle>();
        canvas ??= GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (!arrow || !canvas)
            return;

        targetCamera ??= Camera.main;
        castleGenerator ??= FindAnyObjectByType<GenerateCastle>();

        if (!targetCamera || !castleGenerator)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        Vector3 viewportPosition = targetCamera.WorldToViewportPoint(castleGenerator.castlePosition);
        bool isVisible = viewportPosition.z > 0f &&
                         viewportPosition.x > 0f && viewportPosition.x < 1f &&
                         viewportPosition.y > 0f && viewportPosition.y < 1f;

        if (isVisible)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);

        Vector2 direction = new Vector2(viewportPosition.x - 0.5f, viewportPosition.y - 0.5f);

        if (direction.sqrMagnitude < 0.0001f)
        {
            Vector3 toCastle = castleGenerator.castlePosition - targetCamera.transform.position;
            direction = new Vector2(
                Vector3.Dot(toCastle, targetCamera.transform.right),
                Vector3.Dot(toCastle, targetCamera.transform.up));
        }

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector2.up;

        direction.Normalize();
        float halfWidth = 0.5f - screenEdgePadding;
        float halfHeight = 0.5f - screenEdgePadding;
        float scale = Mathf.Min(halfWidth / Mathf.Abs(direction.x), halfHeight / Mathf.Abs(direction.y));
        Vector2 edgePosition = new Vector2(0.5f, 0.5f) + direction * scale;
        Vector2 screenPosition = new Vector2(edgePosition.x * Screen.width, edgePosition.y * Screen.height);

        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, canvasCamera, out Vector2 localPosition))
            arrow.anchoredPosition = localPosition;

        arrow.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f);
    }
}