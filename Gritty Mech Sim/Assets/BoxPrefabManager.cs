using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPrefabManager : MonoBehaviour
{
    [SerializeField] private GameObject box;
    [SerializeField] private GameObject text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject getBox() {
        return box;
    }

    public GameObject getText() {
        return text;
    }
}
