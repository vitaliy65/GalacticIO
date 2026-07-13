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

                // Buildings can be destroyed (tile removed) without going through
                // RemoveBuilding, so prune stale/null entries before summing income.
                // Doing this here avoids a MissingReferenceException from a destroyed
                // Building silently killing this coroutine forever.
                builtBuildings.RemoveAll(building => !building);

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

        public bool RemoveBuilding(Building building)
        {
            if (!building)
            {
                return false;
            }


            if (builtBuildings.Count == 1)
            {
                CurrencyManager.Instance.AddCoins(CurrencyManager.Instance.StartingCoinBalance);
            }
            else
            {
                CurrencyManager.Instance.AddCoins(building.GetSellValue());
            }

            return builtBuildings.Remove(building);
        }

        private void ApplyIncome(int totalIncome)
        {
            if (totalIncome <= 0)
            {
                return;
            }

            CurrencyManager.Instance.AddCoins(totalIncome);
        }

        public int CalculateIncome(Building building, ResourceData resourceData)
        {
            // A building can be placed on a tile that has no resource assigned
            // (e.g. resourceData was never wired up on that tile). Treat that as
            // zero income instead of throwing, so one misconfigured tile can't
            // permanently stop income for every other building in the game.
            if (!building || resourceData == null)
            {
                return 0;
            }

            return (int)(building.GetModifier() * resourceData.Income);
        }

        public bool UpgradeBuilding(Building building)
        {
            if (building == null || !builtBuildings.Contains(building))
            {
                return false;
            }

            return building.TryUpgrade();
        }
    }
}
