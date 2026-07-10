using System.Collections.Generic;
using UnityEngine;

namespace builds
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Game/Building/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [SerializeField] private string buildingName;
        [SerializeField] private string title;
        [SerializeField] private int baseCost;
        [SerializeField] private float incomeModifier;
        [SerializeField] private Sprite sprite;
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private List<ResourceData> CanBePlacedOnResources = new List<ResourceData>();

        public string BuildingName => buildingName;
        public string Title => title;
        public int BaseCost => baseCost;
        public float IncomeModifier => incomeModifier;
        public Sprite Sprite => sprite;
        public GameObject BuildingPrefab => buildingPrefab;
        public IReadOnlyList<ResourceData> Resources => CanBePlacedOnResources;
    }
}
