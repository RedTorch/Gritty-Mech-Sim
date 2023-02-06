using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] apartmentsPrefab;
    [SerializeField] private GameObject ground;
    [SerializeField] private Transform LevelGeoRoot;
    private int rad = 2;
    private float prefabWidth = 50f;
    private float prefabOffset = 70f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject pref;
        float rot;
        for(int i = -rad; i <= rad; i++) {
            for(int z = -rad; z <= rad; z++) {
                pref = apartmentsPrefab[(int)Mathf.Floor(Random.Range(0,apartmentsPrefab.Length - 0.01f))];
                rot = Mathf.Floor(Random.Range(0f,3.99f)) * 90f;
                GameObject obj = Instantiate(pref,new Vector3(i*prefabOffset,0f,z*prefabOffset),Quaternion.Euler(0f,rot,0f));
                obj.transform.parent = LevelGeoRoot;
            }
        }
        ground.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
