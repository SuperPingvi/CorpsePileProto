using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class CorpsePileManager : MonoBehaviour, IPileQuery
{
    public static CorpsePileManager Instance;
    
    public struct PileCell
        {
            public CorpsePile Pile;
            public CorpsePileInteractor Interactor;
        }
    
    [SerializeField] private GameObject corpsePilePrefab;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private float debrisScatterRadius = 2.5f;

    private Dictionary<Vector2Int, PileCell> _pileGrid = new Dictionary<Vector2Int, PileCell>();

    private void Awake()
    {
        Instance = this;
    }

    private Vector2Int GetCellKey(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.x / cellSize);
            int z = Mathf.FloorToInt(position.z / cellSize);
            return new Vector2Int(x, z);
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
        
        cell.Pile.OnDestroyed += (_, _) => _pileGrid.Remove(key);
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
    
    // ── Destruction ──────────────────────────────────────
    public List<PileCell> GetPilesInRadius(Vector3 worldPos, float radius)
    {
        List<PileCell> result = new List<PileCell>();
        int cellRadius = Mathf.CeilToInt(radius / cellSize);
        Vector2Int center = GetCellKey(worldPos);

        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int z = -cellRadius; z <= cellRadius; z++)
            {
                Vector2Int key = new Vector2Int(center.x + x, center.y + z);
                if (_pileGrid.TryGetValue(key, out PileCell cell))
                    result.Add(cell);
            }
        }
        return result;
    }
    
    public void SpawnDebrisOverTime(Vector3 position, int count, CorpseUnit corpseUnit)
    {
        StartCoroutine(DebrisCoroutine(position, count, corpseUnit));
    }

    private IEnumerator DebrisCoroutine(Vector3 position, int count, CorpseUnit corpseUnit)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * debrisScatterRadius;
            Vector3 scatterPos = position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            AddCorpse(scatterPos, corpseUnit);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
