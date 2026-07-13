using tiles;
using UnityEngine;

public class BuildingSelectorMenu : Menu
{
    [SerializeField]
    private GameObject buildingSelectionMenu;
    [SerializeField]
    private GameObject[] buildingSelectionMenuSockets = new GameObject[6];
    public static BuildingSelectorMenu Instance { get; private set; }

    // selectedTile and isSubscribed provided by base Menu

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    protected override void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        base.OnDestroy();
    }

    protected override void Update() { base.Update(); }

    protected override void OnLeftClick()
    {
        SelectBuildingUnderCursor();
    }

    private void SelectBuildingUnderCursor()
    {
        if (Camera.main == null || selectedTile == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        if (!IsMenuHit(hit.collider))
            return;

        SocketData socketData = hit.collider.GetComponent<SocketData>();
        if (socketData != null)
        {
            socketData.OnClick();
        }
    }

    public override void ShowMenu(TileBehavior tile)
    {
        selectedTile = tile;
        ConfigureSockets(tile);

        if (tile == null || tile.TileData == null ||
            tile.TileData.Type == TileTypes.Ground ||
            tile.TileState == TileStates.Occupied)
        {
            HideMenu();
            return;
        }

        if (buildingSelectionMenu == null)
            return;

        buildingSelectionMenu.SetActive(true);
    }

    public override void HideMenu()
    {
        selectedTile = null;
        if (buildingSelectionMenu != null)
        {
            buildingSelectionMenu.SetActive(false);
        }
    }

    public override void MoveToPosition(Vector3 position)
    {
        if (buildingSelectionMenu != null)
        {
            buildingSelectionMenu.transform.position = position;
        }
    }

    protected override bool IsMenuHit(Collider collider)
    {
        if (!collider || !buildingSelectionMenu)
            return false;

        return collider.transform.IsChildOf(buildingSelectionMenu.transform) ||
               collider.gameObject == buildingSelectionMenu;
    }

    private void ConfigureSockets(TileBehavior tile)
    {
        int activeCount = 6;

        if (tile == null || tile.TileData == null)
        {
            activeCount = 0;
        }
        else if (tile.TileData.Type == TileTypes.Ore)
        {
            switch (tile.TileResourceData.TileResource)
            {
                case TileResources.Copper:
                    activeCount = 1;
                    break;
                case TileResources.Iron:
                    activeCount = 2;
                    break;
                case TileResources.Gold:
                    activeCount = 3;
                    break;
                case TileResources.Silver:
                    activeCount = 4;
                    break;
                case TileResources.Diamond:
                    activeCount = 5;
                    break;
                default:
                    activeCount = 6;
                    break;
            }
        }
        else
        {
            activeCount = 6;
        }

        activeCount = Mathf.Clamp(activeCount, 0, buildingSelectionMenuSockets.Length);
        for (int i = 0; i < buildingSelectionMenuSockets.Length; i++)
        {
            if (buildingSelectionMenuSockets[i] != null)
                buildingSelectionMenuSockets[i].SetActive(i < activeCount);
        }
    }
}
