using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour
{
    private NavMeshAgent _agent;
    private float _baseSpeed;
    private float _pileCheckTimer;

    [SerializeField] private float pileCheckInterval = 0.3f;
    
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _baseSpeed = _agent.speed; // store original speed
    }
    private void MoveTowardsTarget()
    {
        if (target != null)
            _agent.SetDestination(target.position);
    }

    private void Update()
    {
        MoveTowardsTarget();
        
        _pileCheckTimer -= Time.deltaTime;
        if (_pileCheckTimer <= 0)
        {
            _pileCheckTimer = pileCheckInterval;
            CheckPileUnderfoot();
        }
    }

    private void CheckPileUnderfoot()
    {
        IPileQuery pileQuery = CorpsePileManager.Instance;
        CorpsePile.PileState state = pileQuery.GetStateAtPosition(transform.position);

        switch (state)
        {
            case CorpsePile.PileState.Free:
                _agent.speed = _baseSpeed;
                break;
            
            case CorpsePile.PileState.Slow:
                float multiplier = pileQuery.GetSlowMultiplierAtPosition(transform.position);
                _agent.speed = _baseSpeed * multiplier;
                break;
            
            case CorpsePile.PileState.Blocked:
                _agent.speed = _baseSpeed;
                break;
        }
    }
    
}
