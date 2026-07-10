using UnityEngine;
using tiles;
using builds;

public class Tile : TileBehavior
{
    private MeshRenderer outlineMeshRenderer;

    void Start()
    {
        outlineMeshRenderer = OutlinedPart.GetComponent<MeshRenderer>();
    }

    public override void OnTileSelected()
    {
        isSelected = true;
        OutlinedPart.SetActive(true);
        Material[] materials = outlineMeshRenderer.materials;
        materials[0] = SelectedMaterial;
        outlineMeshRenderer.materials = materials;
    }
    public override void OnTileUnselected()
    {
        isSelected = false;
        OutlinedPart.SetActive(false);
        Material[] materials = outlineMeshRenderer.materials;
        materials[0] = null;
        outlineMeshRenderer.materials = materials;
    }
    public override void OnTileHovered()
    {
        if (!isSelected)
        {
            isHovered = true;
            OutlinedPart.SetActive(true);
            Material[] materials = outlineMeshRenderer.materials;
            materials[0] = HoveredMaterial;
            outlineMeshRenderer.materials = materials;
        }
    }
    public override void OnTileUnhovered()
    {
        if (!isSelected)
        {
            isHovered = false;
            OutlinedPart.SetActive(false);
            Material[] materials = outlineMeshRenderer.materials;
            materials[0] = null;
            outlineMeshRenderer.materials = materials;
        }
    }
    public override void OnTilePlaced(BuildingData buildingData)
    {
        TileBuilding = buildingData;

        if (TileState == TileStates.Empty)
        {
            GameObject building = Instantiate(buildingData.BuildingPrefab, BuildAnchorPoint.transform, false);
            building.GetComponent<Building>().IsPlacedOnResource = resourceData;
        }

        TileState = TileStates.Occupied;
    }
    public override void OnTileRemoved()
    {
        TileBuilding = null;

        if (BuildAnchorPoint.transform.childCount > 0)
        {
            foreach (Transform child in BuildAnchorPoint.transform)
            {
                Destroy(child.gameObject);
            }
        }

        TileState = TileStates.Empty;
    }

    public void OnMouseEnter()
    {
        if (!isHovered)
        {
            OnTileHovered();
        }
    }

    public void OnMouseExit()
    {
        if (isHovered)
        {
            OnTileUnhovered();
        }
    }
}
