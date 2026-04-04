using UnityEngine;

public class Turret : MonoBehaviour
{
[SerializeField] private Transform rotatingHead;
    [SerializeField] private float range = 10f;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float damagePerShot = 25f;
    [SerializeField] private LayerMask enemyLayer;

    private float _fireTimer;
    private Enemy _currentTarget;

    private void Update()
    {
        _currentTarget = FindNearestEnemy();

        if (_currentTarget == null) return;

        RotateToward(_currentTarget.transform.position);

        _fireTimer -= Time.deltaTime;
        if (_fireTimer <= 0f)
        {
            _fireTimer = fireInterval;
            Fire();
        }
    }

    private Enemy FindNearestEnemy()
    {
        // Overlap sphere to find all colliders in range on the enemy layer
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        Enemy nearest = null;
        float minDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void RotateToward(Vector3 targetPosition)
    {
        // Only rotate on Y axis — keeps the head level
        Vector3 direction = targetPosition - rotatingHead.position;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rotatingHead.rotation = Quaternion.Lerp(
            rotatingHead.rotation,
            targetRotation,
            Time.deltaTime * 10f  // rotation speed, tune to taste
        );
    }

    private void Fire()
    {
        if (_currentTarget != null)
            _currentTarget.TakeDamage(damagePerShot);
    }

    // Visualise range in editor so you can see it in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
