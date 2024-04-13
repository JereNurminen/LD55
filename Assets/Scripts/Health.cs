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
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(int damage, Vector2? hitPosition = null) {
        hitPoints -= damage;
        onDamage.Invoke();
        if (hitPoints <= 0 && !isDead) {
            onDeath.Invoke();
            isDead = true;
        } else {
            if (onDamageEffectPrefab != null && hitPosition != null) {
                Instantiate(onDamageEffectPrefab, (Vector2)hitPosition, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
