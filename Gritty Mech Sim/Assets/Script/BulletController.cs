using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float Damage = 15f;
    private float SpeedInMetersPerSecond = 100f; // 1200f is a realistic tank shell speed
    private float MaxDistance = 1000f;
    private float totalDistTraveled = 0f;
    private GameObject attacker;

    [SerializeField] private GameObject HitPsPrefab;
    [SerializeField] private GameObject debugCube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float forwardDistance = SpeedInMetersPerSecond * Time.deltaTime;
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position,transform.forward, out hit, forwardDistance);
        Debug.DrawRay(transform.position,transform.forward*forwardDistance); // debugging raycast...
        GameObject target = null;
        if(isHit) {target = hit.collider.gameObject;}

        // if(isHit && target.transform.root != transform.root) {
        if(isHit && target != attacker) {
            GameObject cube = Instantiate(debugCube,hit.point,Quaternion.identity);
            Destroy(cube,1f);
            if(target.GetComponent<MechMovementController>()) {
                target.GetComponent<MechMovementController>().onReceiveDamage(Damage);
            }
            if(target.GetComponent<AIMechController>()) {
                target.GetComponent<AIMechController>().onReceiveDamage(Damage,attacker);
            }
            if(target.GetComponent<DamageReceiver>()) {
                target.GetComponent<DamageReceiver>().onReceiveDamage(Damage);
            }
            GameObject ps = Instantiate(HitPsPrefab, hit.point, Quaternion.identity);
            ps.GetComponent<HitPS>().setMaterial(target);
            Destroy(gameObject);
        }
        else {
            if(totalDistTraveled >= MaxDistance) {
                GameObject cube = Instantiate(debugCube,hit.point,Quaternion.identity);
                Destroy(cube,1f);
                Destroy(gameObject);
            }
            else {
                transform.Translate(Vector3.forward * forwardDistance);
                totalDistTraveled += forwardDistance;
            }
        }
    }

    public void setDamage(float val) {
        Damage = val;
    }

    public void setAttacker(GameObject atkr) {
        attacker = atkr;
    }

    public float getMaxDistance() {
        return MaxDistance;
    }
}