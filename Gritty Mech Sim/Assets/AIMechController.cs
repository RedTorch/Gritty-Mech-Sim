using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class AIMechController : MonoBehaviour
{
    [SerializeField] private bool isFriendly = false;
    private bool isCommander = true;
    private string faction = "Enemy";
    private string targetedFaction = "Player";
    private Vector3 destination;
    private GameObject currTarget;

    private GameObject mostRecentAttacker;
    private float mostRecentAttackerDamage;

    private float weight_hasLineOfSight = 20f;
    private float weight_closerProximity = 100f; // by 10/(distance)
    private float weight_isCurrTarget = 5f;
    private float weight_isMostRecentAttacker = 5f;
    private float weight_isMostRecentAttackerByDamage = 0.1f;
    public Dictionary<Vector3, string> targetingTagsToDisplay = new Dictionary<Vector3, string>();

    private float attackRange = 200f;

    NavMeshAgent agent;

    [SerializeField] private MechMovementController mechMoveCon;

    private Transform camt;

    private bool isShowLines = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        camt = mechMoveCon.getLookRoot();
        if(isFriendly) {
            faction = "Player";
            targetedFaction = "Enemy";
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetingTagsToDisplay.Clear();
        evalNewTarget();
        mechMoveCon.setIsFiring(false);
        if(currTarget) {
            // destination = suggestPosition(currTarget.transform.position);
            if(isShowLines) { Debug.DrawLine(transform.position,currTarget.transform.position,Color.blue); }
            if(!hasLoS(currTarget)) {
                agent.destination = currTarget.transform.position;
                turnToFacePosition(currTarget.transform.position);
            }
            else {
                agent.destination = transform.position;
                turnToFacePosition(currTarget.transform.position);
                mechMoveCon.setIsFiring(true);
            }
        }
        else {
            agent.destination = transform.position;
        }
    }

    private Vector3 suggestPosition(Vector3 pos) {
        if(Vector3.Distance(pos, currTarget.transform.position) > attackRange) {
            // return closest position to currTarget that is in range
        }
        else if(Physics.Linecast(pos, currTarget.transform.position)) {
            // set destination
        }
        return transform.position;
    }

    private GameObject[] getValidTargets() {
        return GameObject.FindGameObjectsWithTag(targetedFaction);
    }

    void evalNewTarget() {
        float highestScore = -10000f; //minimum score; if no targets above this score are found, they will be ignored
        GameObject newTarget = null;
        foreach(GameObject target in getValidTargets()) {
            float score = 0f;
            string scoreDebugString = $"Object: {target.name}";

            float dist = Vector3.Distance(transform.position, target.transform.position);

            if(hasLoS(target)) {
                score += weight_hasLineOfSight;
                scoreDebugString += $"\nLoS: {weight_hasLineOfSight}";
            }
            score +=  (10f/dist) * weight_closerProximity;
            scoreDebugString += $"\nProximity: {Mathf.Floor((10f/dist) * weight_closerProximity)}";
            if(target == currTarget) {
                score += weight_isCurrTarget;
                scoreDebugString += $"\nIs Current Target: {weight_isCurrTarget}";
            }
            if(target == mostRecentAttacker) {
                score += weight_isMostRecentAttacker;
                score += mostRecentAttackerDamage * weight_isMostRecentAttackerByDamage;
                scoreDebugString += $"\nIs Most Recent Attacker: {weight_isMostRecentAttacker}\nIs Most Recent Attacker Damage: {mostRecentAttackerDamage * weight_isMostRecentAttackerByDamage}";
            }
            scoreDebugString += $"\nScore:{Mathf.Floor(score)}";

            targetingTagsToDisplay.Add(target.transform.position,scoreDebugString);

            // other factors below...

            if(score > highestScore) {
                newTarget = target;
                highestScore = score;
            }
        }
        currTarget = newTarget;
    }

    private bool hasLoS(GameObject target) {
        float dist = Vector3.Distance(camt.position, target.transform.position);
        RaycastHit[] rcHits = Physics.RaycastAll(new Ray(camt.position,target.transform.position-transform.position),dist);
        foreach(RaycastHit hitObj in rcHits) {
            if(hitObj.collider.transform.root != camt.root && hitObj.collider.transform.root != target.transform.root) {
                return false;
            }
        }
        return true;
    }

    private void turnToFacePosition(Vector3 pos) {
        Vector3 rotToPos = (Quaternion.LookRotation(pos-transform.position)).eulerAngles;
        // print($"desired rotation: {rotToPos.x},{rotToPos.y}");
        mechMoveCon.setTargetLook(new Vector2(rotToPos.y,-1f * parseRot(rotToPos.x)));
    }

    private float parseRot(float rot) {
        if(rot>180f) {
            rot -= 360f;
        }
        return rot;
    }  
    public void onReceiveDamage(float damage, GameObject attacker) {
        mostRecentAttacker = attacker;
        mostRecentAttackerDamage = damage;
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