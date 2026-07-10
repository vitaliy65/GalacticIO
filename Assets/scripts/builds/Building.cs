using currency;
using UnityEngine;

namespace builds
{
    public class Building : MonoBehaviour, IIncomeProvider
    {
        [SerializeField] private BuildingData buildingData;
        [SerializeField] private int level = 1;
        [SerializeField] private ResourceData isPlacedOnResource;

        private bool hasRegistered;

        public BuildingData BuildingData => buildingData;
        public int Level => level;

        public ResourceData IsPlacedOnResource
        {
            get => isPlacedOnResource;
            set => isPlacedOnResource = value;
        }

        private void Start()
        {
            RegisterWithManager();
        }

        private void RegisterWithManager()
        {
            if (hasRegistered)
            {
                return;
            }

            if (CurrencyManager.Instance.TrySpendCoins(buildingData.BaseCost))
            {
                BuildingManager.Instance.AddBuilding(this);
                hasRegistered = true;
            }
        }

        public void Upgrade()
        {
            level++;
        }

        public float GetModifier()
        {
            if (!buildingData)
            {
                return 0;
            }

            return buildingData.IncomeModifier * level;
        }
    }
}
