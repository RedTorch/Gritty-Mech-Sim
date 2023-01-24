using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassRotation : MonoBehaviour
{
    private Animator anim;
    private float rotationPercent;
    [SerializeField] private MechMovementController mechmc;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rotationPercent = mechmc.getCompassPercent();
        anim.PlayInFixedTime("Compass", -1, rotationPercent);
    }
}
