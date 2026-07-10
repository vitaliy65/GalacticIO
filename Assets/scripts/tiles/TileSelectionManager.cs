
using UnityEngine;
using tiles;

public class TileSelectionManager : MonoBehaviour
{
    public delegate void MoveToPositionDelegate(Vector3 position);
    public delegate void HideMenuDelegate();
    public delegate void ShowMenuDelegate(TileBehavior tile);

    public static TileSelectionManager Instance { get; private set; }
    public static TileBehavior SelectedTileGlobal => Instance != null ? Instance.selectedTile : null;

    public event MoveToPositionDelegate MoveToPositionRequested;
    public event HideMenuDelegate HideMenuRequested;
    public event ShowMenuDelegate ShowMenuRequested;

    private TileBehavior selectedTile;

    public TileBehavior SelectedTile => selectedTile;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectTileUnderCursor();
        }

        if (Input.GetMouseButtonDown(1) && selectedTile)
        {
            ClearSelection();
        }
    }

    private void SelectTileUnderCursor()
    {
        if (!Camera.main)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (IsMenuHit(hit.collider))
                return;

            TileBehavior tile = hit.collider.GetComponent<TileBehavior>();
            if (tile)
            {
                SetSelection(tile);
                return;
            }
        }
    }

    private void SetSelection(TileBehavior tile)
    {
        if (selectedTile == tile)
        {
            ClearSelection();
            return;
        }

        ClearSelection();
        selectedTile = tile;

        if (selectedTile)
        {
            selectedTile.OnTileSelected();
            MoveToPositionRequested?.Invoke(selectedTile.transform.position);
            ShowMenuRequested?.Invoke(selectedTile);
        }
    }

    public void ClearSelection()
    {
        if (!selectedTile)
            return;

        HideMenuRequested?.Invoke();
        selectedTile.OnTileUnselected();
        selectedTile = null;
    }

    public void RequestMenuHide()
    {
        HideMenuRequested?.Invoke();
    }

    public void RequestMenuShow(TileBehavior tile)
    {
        ShowMenuRequested?.Invoke(tile);
    }

    public void RequestMoveToPosition(Vector3 position)
    {
        MoveToPositionRequested?.Invoke(position);
    }

    private bool IsMenuHit(Collider collider)
    {
        if (!collider || !BuildingSelectorWheel.Instance)
            return false;

        return collider.transform.IsChildOf(BuildingSelectorWheel.Instance.transform) ||
               collider.gameObject == BuildingSelectorWheel.Instance.gameObject;
    }
}