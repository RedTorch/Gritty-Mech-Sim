using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float weight_closerProximity = 1f; //in meters
    private float weight_isCurrTarget = 5f;
    private float weight_isMostRecentAttacker = 5f;
    private float weight_isMostRecentAttackerByDamage = 0.1f;

    private float attackRange = 200f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currTarget) {
            // destination = suggestPosition(currTarget.transform.position);
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

            if(!Physics.Linecast(transform.position, target.transform.position)) {
                score += weight_hasLineOfSight;
            }
            float dist = Vector3.Distance(transform.position, target.transform.position);
            score -=  dist * weight_closerProximity;
            if(target == currTarget) {
                score += weight_isCurrTarget;
            }
            if(target == mostRecentAttacker) {
                score += weight_isMostRecentAttacker;
                score += mostRecentAttackerDamage * weight_isMostRecentAttackerByDamage;
            }
            // other factors below...

            if(score > highestScore) {
                newTarget = target;
                highestScore = score;
            }
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
