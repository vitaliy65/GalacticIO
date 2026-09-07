using currency;
using tiles;
using UnityEngine;

namespace builds
{
    public class Building : MonoBehaviour, IIncomeProvider
    {
        [SerializeField] protected BuildingData buildingData;
        [SerializeField] protected int level = 1;
        [SerializeField] protected ResourceData isPlacedOnResource;

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

            // Payment already happened at the moment of purchase (see SocketData.OnClick),
            // so by the time this MonoBehaviour exists it just needs to register itself.
            BuildingManager.Instance.AddBuilding(this);
            hasRegistered = true;
        }

        public bool TryUpgrade()
        {
            if (CurrencyManager.Instance.TrySpendCoins(buildingData.BaseCost * level))
            {
                level++;
                return true;
            }

            Debug.Log("Not enough coins to upgrade building.");
            return false;
        }

        public int GetSellValue()
        {
            if (!buildingData)
                return 0;

            // Sell value is half of the total cost spent on this building.
            int totalCost = buildingData.BaseCost * level;
            return totalCost / 2;
        }

        public virtual bool CanSell() { return true; }

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
