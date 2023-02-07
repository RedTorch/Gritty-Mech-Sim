using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyTracker : MonoBehaviour
{
    [Tooltip("the player mech's center camera")]
    [SerializeField] private Camera mainCam;
    [Tooltip("the screen associated with the center camera, on which data is displayed")]
    [SerializeField] private RectTransform root;
    [SerializeField] private Canvas containingCanvas;
    [SerializeField] private GameObject statusFramePrefab;

    private int bufferSize = 200;
    private GameObject[] statFrames;
    private int prevEnemiesLength = 0;
    
    private float minScaleDistance = 50f; // the minimum distance at which the box will start to shrink
    private float pixWidth;
    private float pixHeight;

    // Start is called before the first frame update
    void Start()
    {
        statFrames = new GameObject[bufferSize];
        pixWidth = mainCam.pixelWidth;
        pixHeight = mainCam.pixelHeight;
        for(int i = 0; i < statFrames.Length; i++) {
            statFrames[i] = Instantiate(statusFramePrefab,containingCanvas.transform.position,containingCanvas.transform.rotation, root);
            statFrames[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // update boxes
        for(int i = enemies.Length; i < prevEnemiesLength; i++) {
            statFrames[i].SetActive(false);
        }
        prevEnemiesLength = enemies.Length;
        for(int i = 0; i < enemies.Length; i++) {
            Vector3 delta = enemies[i].transform.position-mainCam.transform.position;
            if(Vector3.Angle(mainCam.transform.forward, delta) > 60f) {
                continue;
            }
            statFrames[i].SetActive(true);
            float dist = Vector3.Distance(mainCam.transform.position, enemies[i].transform.position);
            float scaleFactor = Mathf.Clamp(minScaleDistance/dist,0.5f,1f) * 0.2f;
            (statFrames[i].GetComponent<BoxPrefabManager>()).getBox().GetComponent<RectTransform>().localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            Vector3 screenPos = mainCam.WorldToScreenPoint(enemies[i].transform.position);
            Vector2 newAnchoredPos = new Vector2((screenPos.x-0.5f*pixWidth)*150f/pixWidth, (screenPos.y-0.5f*pixHeight)*100f/pixHeight);
            (statFrames[i].GetComponent<RectTransform>()).anchoredPosition = newAnchoredPos;
            statFrames[i].GetComponent<BoxPrefabManager>().getText().GetComponent<TMP_Text>().text = $"({Mathf.Floor(newAnchoredPos.x)}, {Mathf.Floor(newAnchoredPos.x)})";
        }
    }
}