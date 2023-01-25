using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMechController : MonoBehaviour
{
    private Vector3 CurrVelocity = new Vector3(0f,0f,0f);
    [SerializeField] private float MoveSpeed = 20f;
    private Rigidbody rb;

    private GameObject target;
    [SerializeField] private float turnSpeed = 45f;

    private float health = 100f; // DISPLAY
    private float maxHealth = 100f;
    private bool isAlive = true;

    [SerializeField] private float attackRange = 100f;
    [SerializeField] private float currFireInterval = 0f;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private GameObject bulletPrefab;

    private Vector3 currRotation;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAlive) {
            return;
        }
        if(target) {
            Vector3 relativePos = target.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, 1 * Time.deltaTime );
            if(Vector3.Angle(transform.forward,target.transform.position-transform.position) < 10f && Vector3.Distance(transform.position,target.transform.position) <= attackRange) {
                tryFireGun();
            }
            if(Vector3.Distance(transform.position,target.transform.position) >= (attackRange*0.8f)) {
                transform.Translate(Vector3.forward*MoveSpeed*Time.deltaTime);
            }
        }
    }

    void tryFireGun() {
        currFireInterval -= Time.deltaTime;
        if(currFireInterval<=0f) {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            currFireInterval += fireInterval;
        }
    }

    public void receiveDamage(float dmg) {
        health -= dmg;
        if(health <= 0f) {
            death();
        }
    }

    public void death() {
        isAlive = false;
        Destroy(gameObject,0.5f);
    }
}
