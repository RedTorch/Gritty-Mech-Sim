using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float Damage = 15f;
    private float SpeedInMetersPerSecond = 1200f;
    private float MaxDistance = 2400f;
    private float totalDistTraveled = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float forwardDistance = SpeedInMetersPerSecond * Time.deltaTime;
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward, out hit, forwardDistance)) {
            GameObject target = hit.collider.gameObject;
            target.SendMessage("receiveDamage", Damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
        else if(totalDistTraveled >= MaxDistance) {
            Destroy(gameObject);
        }
        else {
            transform.Translate(Vector3.forward * forwardDistance);
            totalDistTraveled += forwardDistance;
        }
    }

    public void SetDamage(float val) {
        Damage = val;
    }
}