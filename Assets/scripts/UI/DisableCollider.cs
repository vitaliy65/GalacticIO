using UnityEngine;

public class DisableCollider : MonoBehaviour
{
    void Update()
    {
        var colliders = GetComponentsInChildren<BoxCollider>(true);
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
