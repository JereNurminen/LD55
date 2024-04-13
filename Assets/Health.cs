using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int hitPoints = 3;
    public UnityEvent onDeath;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TakeDamage(int damage) {
        hitPoints -= damage;
        if (hitPoints <= 0) {
            onDeath.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
