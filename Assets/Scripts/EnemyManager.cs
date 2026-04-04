using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    
    public int maxEnemies = 20;
    public int currentEnemies;
    
    void Awake()
    {
        Instance = this;
    }

    public bool CanSpawn()
    {
        return currentEnemies < maxEnemies;
    }

    public void RegisterEnemy()
    {
        currentEnemies++;
    }

    public void UnregisterEnemy()
    {
        currentEnemies--;
    }
}
