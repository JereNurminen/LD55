using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int hitPoints = 3;
    public UnityEvent onDeath;
    public UnityEvent onDamage;
    public GameObject onDamageEffectPrefab;
    public bool isDead = false;
    public bool isInvulnerable = false;

    // Start is called before the first frame update
    void Start() { }

    public void TakeDamage(int damage, Vector2? hitPosition = null)
    {
        if (!isInvulnerable)
        {
            SetHealth(hitPoints - damage);
            if (onDamageEffectPrefab != null && hitPosition != null)
            {
                Instantiate(onDamageEffectPrefab, (Vector2)hitPosition, Quaternion.identity);
            }
        }

        onDamage.Invoke();
    }

    public void SetHealth(int health)
    {
        hitPoints = health;

        if (hitPoints <= 0 && !isDead)
        {
            onDeath.Invoke();
            isDead = true;
        }
    }

    // Update is called once per frame
    void Update() { }
}
