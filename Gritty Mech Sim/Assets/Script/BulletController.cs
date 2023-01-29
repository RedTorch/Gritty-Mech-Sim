using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float Damage = 15f;
    private float SpeedInMetersPerSecond = 1200f;
    private float MaxDistance = 2400f;
    private float totalDistTraveled = 0f;
    private GameObject attacker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // float forwardDistance = SpeedInMetersPerSecond * Time.deltaTime;
        // RaycastHit hit;
        // if(Physics.Raycast(transform.position,transform.forward, out hit, forwardDistance)) {
        //     GameObject target = hit.collider.gameObject;
        //     if(target.GetComponent<MechMovementController>() && hit.collider.gameObject != gameObject) {
        //         target.GetComponent<MechMovementController>().onReceiveDamage(Damage);
        //     }
        //     if(target.GetComponent<AIMechController>()) {
        //         target.GetComponent<AIMechController>().onReceiveDamage(Damage,attacker);
        //     }
        //     Destroy(gameObject);
        // }
        // if(totalDistTraveled >= MaxDistance) {
        //     Destroy(gameObject);
        // }
        // else {
        //     transform.Translate(Vector3.forward * forwardDistance);
        //     totalDistTraveled += forwardDistance;
        // }
        float forwardDistance = SpeedInMetersPerSecond * Time.deltaTime;
        RaycastHit hit;
        if(!Physics.Raycast(transform.position,transform.forward, out hit, forwardDistance)) {
            if(totalDistTraveled >= MaxDistance) {
                Destroy(gameObject);
            }
            else {
                transform.Translate(Vector3.forward * forwardDistance);
                totalDistTraveled += forwardDistance;
            }
        }
        else {
            GameObject target = hit.collider.gameObject;
            if(target == attacker) {
                if(totalDistTraveled >= MaxDistance) {
                    Destroy(gameObject);
                }
                else {
                    transform.Translate(Vector3.forward * forwardDistance);
                    totalDistTraveled += forwardDistance;
                }
            }
            else {
                if(target.GetComponent<MechMovementController>()) {
                    target.GetComponent<MechMovementController>().onReceiveDamage(Damage);
                }
                if(target.GetComponent<AIMechController>()) {
                    target.GetComponent<AIMechController>().onReceiveDamage(Damage,attacker);
                }
                Destroy(gameObject);
            }
        }
    }

    public void setDamage(float val) {
        Damage = val;
    }

    public void setAttacker(GameObject atkr) {
        attacker = atkr;
    }
}