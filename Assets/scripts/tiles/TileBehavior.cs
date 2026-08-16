using System.Collections.Generic;
using builds;
using UnityEngine;

namespace tiles
{
    public abstract class TileBehavior : MonoBehaviour
    {
        private static readonly HashSet<TileBehavior> allTiles = new HashSet<TileBehavior>();

        public static IReadOnlyCollection<TileBehavior> AllTiles => allTiles;

        protected virtual void OnEnable()
        {
            allTiles.Add(this);
        }

        protected virtual void OnDisable()
        {
            allTiles.Remove(this);
        }

        [SerializeField]
        protected TileScriptable tileData;
        [SerializeField]
        protected TileStates tileState = TileStates.Empty;
        [SerializeField]
        protected Building tileBuilding = null;
        [SerializeField]
        protected ResourceData tileResourceData;
        [SerializeField]
        protected Material SelectedMaterial;
        [SerializeField]
        protected Material HoveredMaterial;
        [SerializeField]
        protected GameObject OutlinedPart;
        [SerializeField]
        protected GameObject SpawnAnchorPoint;
        [Tooltip("The tile's own surface/ground mesh renderer - assign the child that shows the tile's material (not the outline, not the building anchor). Used by WorldGenerator to apply a biome's material.")]
        [SerializeField]
        protected MeshRenderer MaterialRenderer;

        public bool isSelected { get; set; }
        public bool isHovered { get; set; }

        [SerializeField]
        public float Height;
        [SerializeField]
        public float Heat;
        [SerializeField]
        public float Moisture;
        [SerializeField]
        public TileBiomes Biome;
        [SerializeField]
        public TileSubBiomes SubBiome;

        private static readonly int BaseColorPropertyId = Shader.PropertyToID("_TileColor");
        private MaterialPropertyBlock materialPropertyBlock;

        // Expose read-only accessors so other systems can decide UI/logic without
        // changing the protected serialized fields directly.
        public TileScriptable TileData => tileData;
        public ResourceData TileResourceData => tileResourceData;
        public TileStates TileState
        {
            get => tileState;
            set => tileState = value;
        }
        public Building TileBuilding
        {
            get => tileBuilding;
            set => tileBuilding = value;
        }


        public abstract void OnTileSelected();
        public abstract void OnTileUnselected();
        public abstract void OnTileHovered();
        public abstract void OnTileUnhovered();
        public abstract void OnTilePlaced(Building building);
        public abstract bool OnTileRemoved();

        public void SetGeneratedMapData(float height, float heat, TileBiomes tileBiome, TileSubBiomes subBiome)
        {
            Height = height;
            Heat = heat;
            Biome = tileBiome;
            SubBiome = subBiome;

            switch (SubBiome)
            {
                case TileSubBiomes.Hills:
                    transform.position = new Vector3(transform.position.x, Height + 0.25f, transform.position.z);
                    break;
                default:
                    transform.position = new Vector3(transform.position.x, Height, transform.position.z);
                    break;
            }

            ApplyHeatColor(TileUtils.GetColor(height, tileBiome));
        }

        public void ApplyHeatColor(Color color)
        {
            if (!MaterialRenderer)
            {
                return;
            }

            materialPropertyBlock ??= new MaterialPropertyBlock();
            MaterialRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor(BaseColorPropertyId, color);
            MaterialRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void InitializeFromWorldGenerator(
            TileScriptable generatedTileData,
            ResourceData generatedResourceData,
            float height,
            float heat,
            float moisture,
            TileBiomes tileBiome,
            TileSubBiomes subBiome)
        {
            tileData = generatedTileData;
            tileResourceData = generatedResourceData;
            Moisture = moisture;
            SetGeneratedMapData(height, heat, tileBiome, subBiome);
        }

        public void AddEnvironment(GameObject obj)
        {
            if (obj)
            {
                GameObject objGen = Instantiate(obj, SpawnAnchorPoint.transform, false);
                objGen.transform.rotation = EmbientGenerator.ApplyRandomRotation();
                objGen.transform.localScale = EmbientGenerator.ApplyRandomScale(objGen.transform.localScale);
                objGen.transform.position = EmbientGenerator.ApplyRandomOffset(objGen.transform.position);
            }
        }

        public void RemoveEnvironment(GameObject obj)
        {
        }
    }
}