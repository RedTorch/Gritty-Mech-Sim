using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeManager : MonoBehaviour
{
    // Start is called before the first frame update
    private ParticleSystem _ps;
    void Start()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_ps && !_ps.IsAlive())
         {
             Destroy(gameObject);
         }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.root.GetComponent<MechMovementController>()) {
            other.transform.root.GetComponent<MechMovementController>().setIsSmoked(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.transform.root.GetComponent<MechMovementController>()) {
            other.transform.root.GetComponent<MechMovementController>().setIsSmoked(false);
        }
    }

    void deactivateCollider() {
        //
    }
}
