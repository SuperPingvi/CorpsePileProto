using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform target;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, 1f);
    }

    void Spawn()
    {
        if (!EnemyManager.Instance.CanSpawn())
            return;
        
        GameObject e = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        
        var mover = e.GetComponent<EnemyMover>();
        if (mover  != null) 
            mover.target = target;

        var enemy = e.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnDeath += HandleEnemyDeath;
        }
        EnemyManager.Instance.RegisterEnemy();
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        EnemyManager.Instance.UnregisterEnemy();
    }

}
