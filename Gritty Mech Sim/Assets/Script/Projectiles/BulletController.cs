using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float Damage = 15f;
    [SerializeField] private float SpeedInMetersPerSecond = 1200f; // 1200f is a realistic tank shell speed -- 1400 at max? Btw, an atgm will be 8x slower
    [SerializeField] private float MaxDistance = 500f;
    private float totalDistTraveled = 0f;
    private GameObject firedBy;

    [SerializeField] private GameObject HitPsPrefab;
    [SerializeField] private GameObject ExplosionPsPrefab;
    [SerializeField] private GameObject debugCube;

    [SerializeField] private UIManager uiman;

    [SerializeField] private GameObject myParticleSystemObject;
    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        float forwardDistance = SpeedInMetersPerSecond * Time.deltaTime * 1f;
        // RaycastHit hit;
        // if(Physics.Raycast(transform.position,transform.forward, out hit, forwardDistance)) {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, forwardDistance);
        foreach(RaycastHit hit in hits) {
            // print($"{gameObject.name} hit {hit.collider.gameObject.name}");
            if(hit.collider.gameObject != firedBy && hit.collider.gameObject.tag != "BulletPassthrough") {
            // i.e. if the bullet hits a target that it should be blocked by
                GameObject target = hit.collider.gameObject;
                GameObject targetRoot = hit.collider.transform.root.gameObject;
                if(targetRoot.GetComponent<MechMovementController>()) {
                    if(targetRoot.GetComponent<MechMovementController>().onReceiveDamage(Damage) && uiman) {
                        uiman.showDestroyedMarker();
                    }
                    else if(uiman) {
                        uiman.showHitMarker();
                    }
                }
                if(targetRoot.GetComponent<AIMechController>()) {
                    target.GetComponent<AIMechController>().onReceiveDamage(Damage,firedBy);
                }
                if(target.GetComponent<DamageReceiver>()) {
                    target.GetComponent<DamageReceiver>().onReceiveDamage(Damage);
                }
                if(HitPsPrefab) {
                    GameObject ps = Instantiate(HitPsPrefab, hit.point, Quaternion.identity);
                    ps.GetComponent<HitPS>().setMaterial(target);
                }
                if(ExplosionPsPrefab) {
                    GameObject ps = Instantiate(ExplosionPsPrefab, hit.point, Quaternion.identity);
                }
                if(myParticleSystemObject) {
                    myParticleSystemObject.transform.SetParent(null);
                }
                transform.position = hit.point;
                selfDestroy();
                // return;
            }
        }
        if(totalDistTraveled >= MaxDistance) {
            selfDestroy();
        }
        else {
            transform.Translate(Vector3.forward * forwardDistance);
            totalDistTraveled += forwardDistance;
            // Debug.DrawRay(transform.position,transform.forward*forwardDistance); // debugging raycast...
        }
    }

    private void selfDestroy() {
        if(debugCube) {
            GameObject cube = Instantiate(debugCube,transform.position,Quaternion.identity);
            Destroy(cube,1f);
        }
        Destroy(gameObject);
    }

    public void setFiredBy(GameObject atkr) {
        firedBy = atkr;
        if(atkr.name == "Player") {
            setUiman(atkr.GetComponent<MechMovementController>().getMyUiMan());
        }
    }

    public void setUiman(UIManager newman) {
        uiman = newman;
    }

    public float getMaxDistance() {
        return MaxDistance;
    }
}