using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(CorpsePile))]
[RequireComponent(typeof(NavMeshObstacle))]
public class CorpsePileNavMesh : MonoBehaviour
{
    [SerializeField] private float carveDelay = 0.5f;

    private CorpsePile _pile;
    private NavMeshObstacle _obstacle;
    private Coroutine _carveRoutine;

    private void Awake()
    {
        _pile = GetComponent<CorpsePile>();
        _obstacle = GetComponent<NavMeshObstacle>();

        _obstacle.carving = false;
        _obstacle.enabled = false;
    }

    private void OnEnable()
    {
        _pile.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(CorpsePile.PileState newState)
    {
        ScheduleCarve(newState);
    }

    private void OnDisable()
    {
        _pile.OnStateChanged -= OnStateChanged;
    }

    private void ScheduleCarve(CorpsePile.PileState state)
    {
        if (_carveRoutine != null)
            StopCoroutine(_carveRoutine);
            
        _carveRoutine = StartCoroutine(CarveAfterDelay(state));
    }

    private IEnumerator CarveAfterDelay(CorpsePile.PileState state)
    {
        yield return new WaitForSeconds(carveDelay);
        ApplyCarve(state);
    }

    private void ApplyCarve(CorpsePile.PileState state)
    {
        bool shouldBlock = state == CorpsePile.PileState.Blocked;
        _obstacle.enabled = shouldBlock;
        _obstacle.carving = shouldBlock;
    }
}
