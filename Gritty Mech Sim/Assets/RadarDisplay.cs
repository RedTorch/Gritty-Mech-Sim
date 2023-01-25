using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarDisplay : MonoBehaviour
{
    [SerializeField] private Transform mechPos;
    [SerializeField] private GameObject enemyIconPrefab;
    [SerializeField] private RectTransform iconRoot;
    [SerializeField] private GameObject detectionCircleSprite;
    private GameObject[] enemyLocs;
    private GameObject[] enemyIcons;
    private float detectionDistance = 100f;
    private float scaleFactor = 1f;
    // Start is called before the first frame update
    void Start()
    {
        enemyLocs = GameObject.FindGameObjectsWithTag("Enemy");
        enemyIcons = new GameObject[enemyLocs.Length];
        for(int i = 0; i < enemyLocs.Length; i++) {
            enemyIcons[i] = Instantiate(enemyIconPrefab,iconRoot.position,iconRoot.rotation,iconRoot);
            enemyIcons[i].gameObject.SetActive(false);
        }
        detectionCircleSprite.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, detectionDistance*0.4f*scaleFactor);
        detectionCircleSprite.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, detectionDistance*0.4f*scaleFactor);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < enemyLocs.Length; i++) {
            print("distance = " + Vector3.Distance(enemyLocs[i].transform.position, mechPos.position) + " and enemyLocs[i] == null is " + (enemyLocs == null));
            if(enemyLocs[i] != null && Vector3.Distance(enemyLocs[i].transform.position, mechPos.position) <= detectionDistance) {
                Vector3 relPos = mechPos.InverseTransformPoint(enemyLocs[i].transform.position) * scaleFactor;
                Vector2 newAnchorPos = new Vector2(relPos.x,relPos.z);
                enemyIcons[i].GetComponent<RectTransform>().anchoredPosition = newAnchorPos;
                enemyIcons[i].gameObject.SetActive(true);
            }
            else {
                enemyIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
