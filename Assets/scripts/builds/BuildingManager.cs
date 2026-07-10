using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using currency;
using UnityEngine;

namespace builds
{
    public class BuildingManager : MonoBehaviour
    {
        [SerializeField] private float incomeRefreshInterval = 1f;

        private readonly List<Building> builtBuildings = new List<Building>();
        public static BuildingManager Instance { get; private set; }

        public delegate void IncomeUpdatedHandler();
        public event IncomeUpdatedHandler IncomeUpdated;

        public IReadOnlyList<Building> BuiltBuildings => builtBuildings;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            StartCoroutine(IncomeRefreshRoutine());
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private IEnumerator IncomeRefreshRoutine()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(incomeRefreshInterval);
                int totalIncome = 0;

                foreach (var building in builtBuildings)
                {
                    totalIncome += CalculateIncome(building, building.IsPlacedOnResource);
                }

                ApplyIncome(totalIncome);
                IncomeUpdated?.Invoke();
            }
        }

        public void AddBuilding(Building building)
        {
            if (!building)
            {
                return;
            }

            if (!builtBuildings.Contains(building))
            {
                builtBuildings.Add(building);
            }
        }

        private void ApplyIncome(int totalIncome)
        {
            CurrencyManager.Instance.AddCoins(totalIncome);
        }

        public int CalculateIncome(Building building, ResourceData resourceData)
        {
            return (int)(building.GetModifier() * resourceData.Income);
        }

        public void UpgradeBuilding(Building building)
        {
            if (building == null || !builtBuildings.Contains(building))
            {
                return;
            }

            building.Upgrade();
        }
    }
}
