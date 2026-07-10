using tiles;
using UnityEngine;

public class BuildingSelectorWheel : MonoBehaviour
{
    [SerializeField]
    private GameObject buildingSelectionMenu;
    [SerializeField]
    private GameObject[] buildingSelectionMenuSockets = new GameObject[6];
    public static BuildingSelectorWheel Instance { get; private set; }

    private TileBehavior selectedTile;
    private bool isSubscribed;

    private void OnEnable()
    {
        SubscribeToSelectionManager();
    }

    private void Start()
    {
        SubscribeToSelectionManager();
        SyncSelection(TileSelectionManager.SelectedTileGlobal);
    }

    private void OnDisable()
    {
        UnsubscribeFromSelectionManager();
    }

    private void Update()
    {
        SyncSelection(TileSelectionManager.SelectedTileGlobal);

        if (Input.GetMouseButtonDown(1) && selectedTile != null)
        {
            TileSelectionManager.Instance?.ClearSelection();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SelectBuildingUnderCursor();
        }
    }

    private void SubscribeToSelectionManager()
    {
        if (isSubscribed || TileSelectionManager.Instance == null)
            return;

        TileSelectionManager.Instance.MoveToPositionRequested += MoveToPosition;
        TileSelectionManager.Instance.HideMenuRequested += HideMenu;
        TileSelectionManager.Instance.ShowMenuRequested += ShowMenu;
        isSubscribed = true;
    }

    private void UnsubscribeFromSelectionManager()
    {
        if (!isSubscribed || TileSelectionManager.Instance == null)
            return;

        TileSelectionManager.Instance.MoveToPositionRequested -= MoveToPosition;
        TileSelectionManager.Instance.HideMenuRequested -= HideMenu;
        TileSelectionManager.Instance.ShowMenuRequested -= ShowMenu;
        isSubscribed = false;
    }

    private void SyncSelection(TileBehavior tile)
    {
        if (selectedTile == tile)
            return;

        selectedTile = tile;
        if (selectedTile != null)
        {
            ShowMenu(selectedTile);
        }
        else
        {
            HideMenu();
        }
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

    public void ShowMenu(TileBehavior tile)
    {
        selectedTile = tile;
        ConfigureSockets(tile);

        if (tile == null || tile.StaticTileData == null ||
            tile.StaticTileData.Type == TileTypes.Ground ||
            tile.TileState == TileStates.Occupied)
        {
            HideMenu();
            return;
        }

        if (buildingSelectionMenu == null)
            return;

        buildingSelectionMenu.SetActive(true);
    }

    public void HideMenu()
    {
        selectedTile = null;
        if (buildingSelectionMenu != null)
        {
            buildingSelectionMenu.SetActive(false);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        if (buildingSelectionMenu != null)
        {
            buildingSelectionMenu.transform.position = position;
        }
    }

    private bool IsMenuHit(Collider collider)
    {
        if (!collider || !buildingSelectionMenu)
            return false;

        return collider.transform.IsChildOf(buildingSelectionMenu.transform) ||
               collider.gameObject == buildingSelectionMenu;
    }

    private void ConfigureSockets(TileBehavior tile)
    {
        int activeCount = 6;

        if (tile == null || tile.StaticTileData == null)
        {
            activeCount = 0;
        }
        else if (tile.StaticTileData.Type == TileTypes.Ore)
        {
            switch (tile.StaticTileData.Resource)
            {
                case TileResources.copper:
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
