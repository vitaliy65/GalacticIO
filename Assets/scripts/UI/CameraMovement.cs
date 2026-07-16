using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        transform.position += DirectionFromInput(input) * moveSpeed * Time.deltaTime;
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
