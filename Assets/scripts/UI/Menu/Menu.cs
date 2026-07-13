using tiles;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected TileBehavior selectedTile;
    protected bool isSubscribed;

    protected virtual void OnEnable()
    {
        SubscribeToSelectionManager();
    }

    protected virtual void Start()
    {
        SubscribeToSelectionManager();
        SyncSelection(TileSelectionManager.SelectedTileGlobal);
    }

    protected virtual void OnDisable()
    {
        UnsubscribeFromSelectionManager();
    }

    protected virtual void OnDestroy() { }

    protected virtual void Update()
    {
        SyncSelection(TileSelectionManager.SelectedTileGlobal);

        if (Input.GetMouseButtonDown(1) && selectedTile != null)
        {
            TileSelectionManager.Instance?.ClearSelection();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnLeftClick();
        }
    }

    protected virtual void OnLeftClick()
    {
        // derived classes handle left click
    }

    protected void SubscribeToSelectionManager()
    {
        if (isSubscribed || TileSelectionManager.Instance == null)
            return;

        TileSelectionManager.Instance.MoveToPositionRequested += MoveToPosition;
        TileSelectionManager.Instance.HideMenuRequested += HideMenu;
        TileSelectionManager.Instance.ShowMenuRequested += ShowMenu;
        isSubscribed = true;
    }

    protected void UnsubscribeFromSelectionManager()
    {
        if (!isSubscribed || TileSelectionManager.Instance == null)
            return;

        TileSelectionManager.Instance.MoveToPositionRequested -= MoveToPosition;
        TileSelectionManager.Instance.HideMenuRequested -= HideMenu;
        TileSelectionManager.Instance.ShowMenuRequested -= ShowMenu;
        isSubscribed = false;
    }

    protected void SyncSelection(TileBehavior tile)
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

    public abstract void ShowMenu(TileBehavior tile);
    public abstract void HideMenu();
    public abstract void MoveToPosition(Vector3 position);
    protected abstract bool IsMenuHit(Collider collider);
}
