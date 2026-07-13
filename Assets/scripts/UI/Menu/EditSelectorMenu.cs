using tiles;
using UnityEngine;

public class EditSelectorMenu : Menu
{
    [SerializeField]
    private GameObject EditSelectionMenu;
    public static EditSelectorMenu Instance { get; private set; }
    // selectedTile and isSubscribed are provided by base Menu

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
    }

    protected override void OnLeftClick()
    {
        if (Camera.main == null || selectedTile == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        if (!IsMenuHit(hit.collider))
            return;

        Socket socket = null;
        if (hit.collider.TryGetComponent<EditSocket>(out var edit)) socket = edit;
        else if (hit.collider.TryGetComponent<DeleteSocket>(out var del)) socket = del;

        if (socket != null)
            socket.OnClick();
    }

    public override void ShowMenu(TileBehavior tile)
    {
        selectedTile = tile;

        if (!tile || !tile.TileData ||
            tile.TileData.Type == TileTypes.Ground ||
            tile.TileState == TileStates.Empty)
        {
            HideMenu();
            return;
        }

        if (EditSelectionMenu == null)
            return;

        EditSelectionMenu.SetActive(true);
    }

    public override void HideMenu()
    {
        selectedTile = null;
        if (EditSelectionMenu != null)
        {
            EditSelectionMenu.SetActive(false);
        }
    }

    public override void MoveToPosition(Vector3 position)
    {
        if (EditSelectionMenu != null)
        {
            EditSelectionMenu.transform.position = position;
        }
    }

    protected override bool IsMenuHit(Collider collider)
    {
        if (!collider || !EditSelectionMenu)
            return false;

        return collider.transform.IsChildOf(EditSelectionMenu.transform) ||
               collider.gameObject == EditSelectionMenu;
    }
}
