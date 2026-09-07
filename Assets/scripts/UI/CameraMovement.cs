using tiles;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    [SerializeField] private GenerateCastle castleGenerator;
    [SerializeField] private BoxCollider cameraBounds;
    [SerializeField] private float boundsPadding = 0.5f;

    private void OnEnable()
    {
        WorldGenerator.OnWorldGenerationEnd += SetStartLocation;
    }

    private void OnDisable()
    {
        WorldGenerator.OnWorldGenerationEnd -= SetStartLocation;
    }

    private void Start()
    {
        SetStartLocation();
    }

    private void SetStartLocation()
    {
        castleGenerator ??= FindFirstObjectByType<GenerateCastle>();
        if (castleGenerator == null)
            return;

        Vector3 castlePosition = castleGenerator.castlePosition;
        transform.position = new Vector3(castlePosition.x - 5f, transform.position.y, castlePosition.z - 8f);
        ClampToBounds();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        transform.position += DirectionFromInput(input) * moveSpeed * Time.deltaTime;
        ClampToBounds();
    }

    private void ClampToBounds()
    {
        cameraBounds ??= FindFirstObjectByType<BoxCollider>();
        if (cameraBounds == null)
            return;

        Bounds bounds = cameraBounds.bounds;
        Vector3 position = transform.position;
        float padding = Mathf.Max(0f, boundsPadding);

        position.x = Mathf.Clamp(position.x, bounds.min.x + padding, bounds.max.x - padding);
        position.y = Mathf.Clamp(position.y, bounds.min.y + padding, bounds.max.y - padding);
        position.z = Mathf.Clamp(position.z, bounds.min.z + padding, bounds.max.z - padding);

        transform.position = position;
    }

    private Vector3 DirectionFromInput(Vector2 input)
    {
        // Направление строится только из поворота камеры по Y (yaw), pitch не участвует:
        // "вперёд" (W) — это всегда +Z, повёрнутый на yaw, "вправо" (D) — всегда +X,
        // повёрнутый на yaw. В отличие от сплющивания transform.forward/right, это не
        // ломается при крутом наклоне камеры и продолжает работать корректно, даже если
        // потом добавишь вращение камеры в рантайме.
        Quaternion yaw = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        Vector3 localDirection = new Vector3(input.x, 0f, input.y);
        return yaw * localDirection;
    }
}
