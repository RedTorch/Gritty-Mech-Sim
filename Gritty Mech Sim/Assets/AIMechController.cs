using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AIMechController : MonoBehaviour
{
    private bool isCommander = true;
    private string faction = "Enemy";
    private string targetedFaction = "Player";
    private Vector3 destination;
    private GameObject currTarget;

    private GameObject mostRecentAttacker;
    private float mostRecentAttackerDamage;

    private float weight_hasLineOfSight = 10f;
    private float weight_closerProximity = 100f; // by 10/(distance)
    private float weight_isCurrTarget = 5f;
    private float weight_isMostRecentAttacker = 5f;
    private float weight_isMostRecentAttackerByDamage = 0.1f;
    public Dictionary<Vector3, string> targetingTagsToDisplay = new Dictionary<Vector3, string>();

    private float attackRange = 200f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetingTagsToDisplay.Clear();
        evalNewTarget();
        if(currTarget) {
            // destination = suggestPosition(currTarget.transform.position);
            Debug.DrawLine(transform.position,currTarget.transform.position,Color.red);
        }
        else {
            // defend the area
        }
    }

    private Vector3 suggestPosition(Vector3 pos) {
        if(Vector3.Distance(pos, currTarget.transform.position) > attackRange) {
            // return closest position that is in range
        }
        else if(Physics.Linecast(pos, currTarget.transform.position)) {
            // set destination
        }
        return transform.position;
    }

    void evalNewTarget() {
        float highestScore = -10000f; //minimum score; if no targets above this score are found, they will be ignored
        GameObject newTarget = null;
        foreach(GameObject target in GameObject.FindGameObjectsWithTag(targetedFaction)) {
            float score = 0f;
            string scoreDebugString = $"Object: {target.name}";

            float dist = Vector3.Distance(transform.position, target.transform.position);
            bool hasLoS = true;
            RaycastHit[] rcHits = Physics.RaycastAll(transform.position,target.transform.position,dist);
            foreach(RaycastHit hitObj in rcHits) {
                if(hitObj.collider.gameObject != gameObject && hitObj.collider.gameObject != target) {
                    hasLoS = false;
                }
            }

            if(hasLoS) {
                score += weight_hasLineOfSight;
            }
            score +=  (10f/dist) * weight_closerProximity;
            if(target == currTarget) {
                score += weight_isCurrTarget;
            }
            if(target == mostRecentAttacker) {
                score += weight_isMostRecentAttacker;
                score += mostRecentAttackerDamage * weight_isMostRecentAttackerByDamage;
            }
            scoreDebugString += $"\nLoS: {hasLoS}-{weight_hasLineOfSight}\nProximity: {(10f/dist) * weight_closerProximity}\nIs Current Target: {weight_isCurrTarget}\nIs Most Recent Attacker: {weight_isMostRecentAttacker}\nIs Most Recent Attacker Damage: {mostRecentAttackerDamage * weight_isMostRecentAttackerByDamage}\nScore:{score}";

            targetingTagsToDisplay.Add(target.transform.position,scoreDebugString);

            // other factors below...

            if(score > highestScore) {
                newTarget = target;
                highestScore = score;
            }

            print(scoreDebugString);
        }
        currTarget = newTarget;
    }

    public void onReceiveDamage(float damage, GameObject attacker) {
        mostRecentAttacker = attacker;
        mostRecentAttackerDamage = damage;
    }

    void NavigateToTarget() {
        //
    }
}

[CustomEditor(typeof(AIMechController))]
public class AIMechExaminer : Editor
{
    void OnSceneGUI() {
        var t = target as AIMechController;
        foreach(KeyValuePair<Vector3, string> kvp in t.targetingTagsToDisplay) {
            Handles.Label(kvp.Key, kvp.Value);
        }
    }
}