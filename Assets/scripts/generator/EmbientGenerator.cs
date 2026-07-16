using tiles;
using UnityEngine;

public class EmbientGenerator : Generator
{
    [SerializeField]
    public EnvironmentList ForestEnvironmentList;

    public static EmbientGenerator Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public GameObject GenerateEnvironment(TileBiomes biome, bool isOre)
    {
        switch (biome)
        {
            case TileBiomes.Desert:
                break;
            case TileBiomes.Forest:
                return CalculateSpawn(isOre, ForestEnvironmentList);
            case TileBiomes.Plains:
                break;
            case TileBiomes.Mountain:
                break;
            case TileBiomes.Ocean:
                break;
            case TileBiomes.Tundra:
                break;
            default: return null;
        }

        return null;
    }

    private GameObject CalculateSpawn(bool isOre, EnvironmentList list)
    {
        if (isOre)
            return list.OrePrefab;

        if (list.EnvironmentMaping == null || list.EnvironmentMaping.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (var option in list.EnvironmentMaping)
            totalWeight += option.Key;

        float roll = Random.value * totalWeight;
        float current = 0f;

        foreach (var option in list.EnvironmentMaping)
        {
            current += option.Key;
            if (roll <= current)
                return option.Value;
        }

        return null;
    }

    public static Quaternion ApplyRandomRotation()
    {
        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

}
