using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BakeAllChildren : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NavMeshSurface[] childNavmeshes = GetComponentsInChildren<NavMeshSurface>();
        foreach (NavMeshSurface childNavmesh in childNavmeshes)
        {
            childNavmesh.BuildNavMesh(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
