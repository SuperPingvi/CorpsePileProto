using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealt;
    
    public Action<Enemy> OnDeath;

    private CorpseUnit _corpseUnit;
    private bool _isDead = false;

    private void Awake()
    {
        _corpseUnit = GetComponent<CorpseUnit>();
        _currentHealt = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        _currentHealt -= damage;
        if (_currentHealt <= 0)
            Die();
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        
        if (_corpseUnit != null)
            CorpsePileManager.Instance.AddCorpse(transform.position, _corpseUnit);
        else
        {
            Debug.LogWarning($"[Enemy] {name} has no CorpseUnit component!");
        }
        
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }

    // ── Dev / debug only ───────────────────────────────────────────────
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
    }
}
