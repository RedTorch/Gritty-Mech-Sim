using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenadeManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject smokeCloudPrefab;
    private float fuse = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(fuse < 0f || rb.velocity.magnitude <= 0.001f) {
            Instantiate(smokeCloudPrefab,transform.position,Quaternion.identity);
            Destroy(gameObject);
        } else {
            fuse -= Time.deltaTime;
        }
    }
}
