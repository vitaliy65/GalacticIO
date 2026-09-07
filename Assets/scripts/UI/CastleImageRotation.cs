using UnityEngine;

public class CastleImageRotation : MonoBehaviour
{
    [SerializeField]
    private RectTransform castleArrowTransform;

    // Update this image rotation to oposite of the castleArrowTransform if castleArrowTransform is changed
    void Update()
    {
        if (castleArrowTransform != null)
        {
            Quaternion currentRotation = castleArrowTransform.transform.rotation;
            transform.eulerAngles = new Vector3(0, 0, -currentRotation.z);
        }
    }
}
