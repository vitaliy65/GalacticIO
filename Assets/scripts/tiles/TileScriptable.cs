using UnityEngine;

namespace tiles
{
    [CreateAssetMenu(fileName = "TileScriptable", menuName = "Scriptable Objects/TileScriptable")]
    public class TileScriptable : ScriptableObject
    {
        [SerializeField] private TileTypes tileType = TileTypes.Ground;
        [SerializeField] private TileResources tileResource = TileResources.None;
        [SerializeField] private bool _isWalkable = true;
        [SerializeField] private bool _isDestructible = false;
        [SerializeField] private bool _canBePlacedOn = false;
        [SerializeField] private int _health = 100;

        public TileTypes Type { get => tileType; set => tileType = value; }
        public TileResources Resource { get => tileResource; set => tileResource = value; }
        public bool isWalkable { get => _isWalkable; set => _isWalkable = value; }
        public bool isDestructible { get => _isDestructible; set => _isDestructible = value; }
        public int health { get => _health; set => _health = value; }
        public bool canBePlacedOn { get => _canBePlacedOn; set => _canBePlacedOn = value; }
    }
}