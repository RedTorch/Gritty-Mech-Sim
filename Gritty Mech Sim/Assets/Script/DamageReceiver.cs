using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    private float hp = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onReceiveDamage(float dmg) {
        hp -= dmg;
        if(hp <= 0f) {
            // instantiate "broken" model and/or particles
            Destroy(gameObject);
        }
    }
}
