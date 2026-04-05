using UnityEngine;
using System.Collections.Generic;

public class ExplosionTester : MonoBehaviour
{
    [SerializeField] private float radius = 4f;
    [SerializeField] private float damage = 50f;
    [SerializeField] private Camera testCamera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = testCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 clickPos = hit.point;
                List<CorpsePileManager.PileCell> affected = 
                    CorpsePileManager.Instance.GetPilesInRadius(clickPos, radius);

                foreach (var cell in affected)
                {
                    float distance = Vector3.Distance(clickPos, cell.Pile.transform.position);
                    float falloff = 1f - Mathf.Clamp01(distance / radius);
                    cell.Pile.ReduceMass(damage * falloff);
                }

                Debug.Log($"[ExplosionTester] Clicked {clickPos}, hit {affected.Count} piles");
            }
        }
    }
}
