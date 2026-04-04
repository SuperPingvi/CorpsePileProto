using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CorpsePile))]
public class CorpsePileVisual : MonoBehaviour
{
    [Tooltip("Max instances DrawMeshInstanced supports in one call.")]
    private const int MAX_INSTANCES = 1023;
    
    private CorpsePile _pile;

    private int _seed;
    
    // Per mesh type: the type definition + how many of this type are in the pile
    private List<CorpseMeshType> _meshTypes = new List<CorpseMeshType>();
    private List<int> _meshTypeCounts = new List<int>();
    
    // Reused every frame - avoids allocating a new array each Update
    private Matrix4x4[] _matrices = new Matrix4x4[MAX_INSTANCES];

    [SerializeField] private float spreadRadius = 0.8f;
    [SerializeField] private float bodyHeightOffset = 0.1f;
    [SerializeField] private float halfBodyHeight = 0.5f;

    private void Awake()
    {
        _pile = GetComponent<CorpsePile>();
        _seed = Random.Range(0, 999999); // unique per pile
    }

    private void OnEnable()
    {
        _pile.OnMassChanged += OnMassChanged;
    }

    private void OnDisable()
    {
        _pile.OnMassChanged -= OnMassChanged;
    }

    public void RegisterCorpseType(CorpseMeshType meshType)
    {
        for (int i = 0; i < _meshTypeCounts.Count; i++)
        {
            if (_meshTypes[i].mesh == meshType.mesh)
            {
                _meshTypeCounts[i]++;
                return;
            }
        }
        _meshTypes.Add(meshType);
        _meshTypeCounts.Add(1);
    }

    private void OnMassChanged(float mass)
    {
        // Mass changed — positions will be recalculated next Update automatically
        // Nothing to do here unless you want to trigger a specific effect 
    }

    private void Update()
    {
        if (_meshTypes.Count == 0) return;
        
        Random.InitState(_seed);

        int totalRendered = 0;

        for (int typeIndex = 0; typeIndex < _meshTypes.Count; typeIndex++)
        {
            CorpseMeshType meshType = _meshTypes[typeIndex];
            int count = Mathf.Min(_meshTypeCounts[typeIndex], MAX_INSTANCES);

            for (int i = 0; i < count; i++)
            {
                float stackProgress = (float)totalRendered / Mathf.Max(1, TotalCount() - 1);
                
                float yOffset = Mathf.Lerp(-0.25f, 0.4f, stackProgress);
                
                float xTilt = Mathf.Lerp(90f, 60f, stackProgress);
                float zTilt = Mathf.Lerp(0f, Random.Range(-30f, 30f), stackProgress);
                
                Vector2 circle = Random.insideUnitCircle * spreadRadius;
                Vector3 position = transform.position + new Vector3(circle.x, yOffset - halfBodyHeight, circle.y);
                Quaternion rotation = Quaternion.Euler(xTilt, Random.Range(0f, 360f),zTilt);
                _matrices[i] = Matrix4x4.TRS(position, rotation, meshType.scale);
                totalRendered++;
            }
            
            Graphics.DrawMeshInstanced
            (
                meshType.mesh,
                0,
                meshType.material,
                _matrices,
                count
            );    
        }
    }

    private int TotalCount()
    {
        int total = 0;
        foreach (int c in _meshTypeCounts) total += c;
        return total;
    }

}
