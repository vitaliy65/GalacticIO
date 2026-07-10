using UnityEngine;

namespace builds
{
    [CreateAssetMenu(fileName = "ResourceData", menuName = "Game/Building/Resource Data")]
    public class ResourceData : ScriptableObject
    {
        [SerializeField] private string resourceName;
        [SerializeField] private Sprite icon;
        [SerializeField] private int income;

        public string ResourceName => resourceName;
        public Sprite Icon => icon;
        public int Income => income;
    }
}
