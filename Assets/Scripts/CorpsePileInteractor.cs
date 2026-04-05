using UnityEngine;
using System.Collections.Generic;
using PileCell = CorpsePileManager.PileCell;

public class CorpsePileInteractor : MonoBehaviour
{
    private CorpsePile _pile;
    private CorpsePileManager _manager;
    
    [SerializeField] private float debrisMassThreshold = 5f;
    [SerializeField] private float debrisConversionRatio = 0.5f;

    private void Awake()
    {
        _pile = GetComponent<CorpsePile>();
        _manager = CorpsePileManager.Instance;

        _pile.OnDestroyed += HandlePileDestroyed;
    }

    private void OnDestroy()
    {
        _pile.OnDestroyed -= HandlePileDestroyed;
    }

    private void HandlePileDestroyed(Vector3 position, float massAtDeath)
    {
        if (massAtDeath < debrisMassThreshold) return;

        float debrisMass = massAtDeath * debrisConversionRatio;
        int corpseCount = Mathf.FloorToInt(debrisMass / _pile.CorpseUnit.mass);
        
        CorpsePileManager.Instance.SpawnDebrisOverTime(position, corpseCount, _pile.CorpseUnit);

    }
    
    public void ApplyExplosion(Vector3 worldPos, float radius, float totalDamage, bool startsFire = false)
    {
        List<PileCell> affected = _manager.GetPilesInRadius(worldPos, radius);

        foreach (PileCell cell in affected)
        {
            float distance = Vector3.Distance(worldPos, cell.Pile.transform.position);
            float falloff = 1f - Mathf.Clamp01(distance / radius);
            float damage = totalDamage * falloff;

            cell.Pile.ReduceMass(damage);
        }
    }
}
