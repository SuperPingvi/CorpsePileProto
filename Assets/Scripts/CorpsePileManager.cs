using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CorpsePileManager : MonoBehaviour, IPileQuery
{
    public static CorpsePileManager Instance;
    
    struct PileCell
        {
            public CorpsePile Pile;
            public CorpsePileInteractor Interactor;
        }
    
    [SerializeField] private GameObject corpsePilePrefab;
    [SerializeField] private float cellSize = 2f;

    private Dictionary<Vector2Int, PileCell> _pileGrid = new Dictionary<Vector2Int, PileCell>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddCorpse(Vector3 position, CorpseUnit corpseUnit)
    {
        Vector2Int key = GetCellKey(position);

        if (_pileGrid.TryGetValue(key, out PileCell cell))
        {
            Debug.Log($"[PileManager] Corpse added to existing pile at cell {key}");
            cell.Pile.AddCorpse(corpseUnit);
        }
        else
        {
            Debug.Log($"[PileManager] New pile created at cell {key}");
            CreateNewCorpsePile(key, position, corpseUnit);
        }
    }
    
    // ── IPileQuery implementation ──────────────────────────────────────
    public CorpsePile.PileState GetStateAtPosition(Vector3 position)
    {
        Vector2Int key = GetCellKey(position);

        if (_pileGrid.TryGetValue(key, out PileCell cell))
            return cell.Pile.State;
        
        return CorpsePile.PileState.Free;
    }

    public float GetSlowMultiplierAtPosition(Vector3 position)
    {
        Vector2Int key = GetCellKey(position);
        if (_pileGrid.TryGetValue(key, out PileCell cell))
            return cell.Pile.SlowMultiplier;
        
        return 1f; // No pile here — full speed
    }
    private void CreateNewCorpsePile(Vector2Int key, Vector3 pos, CorpseUnit corpseUnit)
    {
        Vector3 snappedPos = new Vector3(key.x * cellSize + cellSize / 2f, pos.y, key.y * cellSize + cellSize / 2f);
        
        GameObject obj = Instantiate(corpsePilePrefab, snappedPos, Quaternion.identity);
        CorpsePile pile = obj.GetComponent<CorpsePile>();
        CorpsePileInteractor interactor = obj.GetComponent<CorpsePileInteractor>();
        
        PileCell cell = new PileCell
        {
            Pile = pile,
            Interactor = interactor
        };
        
        _pileGrid[key] = cell;
        cell.Pile.AddCorpse(corpseUnit);
    }

    private Vector2Int GetCellKey(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / cellSize);
        int z = Mathf.FloorToInt(position.z / cellSize);
        return new Vector2Int(x, z);
    }
}
