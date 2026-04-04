using UnityEngine;
using System;

public class CorpsePile : MonoBehaviour
{
    [SerializeField] private float slowThreshold = 5f;
    [SerializeField] private float blockThreshold = 15f;
    [SerializeField] private float slowMultiplier = 0.4f;

    //States
    public enum PileState { Free, Slow, Blocked }
    public PileState State { get; private set; }
    
    public float Mass { get; private set; }

    public float SlowMultiplier => State == PileState.Slow ? slowMultiplier : 1f;

    public event Action<float> OnMassChanged;
    public event Action<PileState> OnStateChanged;
    
    
    private CorpsePileVisual _visual;

    private void Awake()
    {
        _visual = GetComponent<CorpsePileVisual>();
    }

    public void AddCorpse(CorpseUnit corpseUnit)
    {
        Mass += corpseUnit.mass;
        Debug.Log($"[CorpsePile] Mass is now {Mass} | State: {State}");
        
        if (_visual != null)
        {
            CorpseMeshType meshType = new CorpseMeshType
            {
                mesh = corpseUnit.corpseMesh,
                material = corpseUnit.corpseMaterial,
                scale = corpseUnit.meshScale
            };
            _visual.RegisterCorpseType(meshType);
        }
        
        OnMassChanged?.Invoke(Mass);
        EvaluateState();
    }

    private void EvaluateState()
    {
        PileState newState;
        
        if (Mass >= blockThreshold)
            newState = PileState.Blocked;
        else if (Mass >= slowThreshold)
            newState = PileState.Slow;
        else
            newState = PileState.Free;

        if (newState != State)
        {
            State = newState;
            Debug.Log($"[CorpsePile] State changed to {State}");
            OnStateChanged?.Invoke(State);
        }
    }

    public void ReduceMass(float amount)
    {
        Mass = Mathf.Max(0f, Mass - amount);
        OnMassChanged?.Invoke(Mass);
        EvaluateState();
    }
}
