using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public Animator Animator { get; private set; }

    public float TurnSpeedDegrees = 180.0f;

    // Use this for initialization
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TurnToFace(Vector3 direction)
    {
        if(direction == Vector3.zero)
            return;

        Vector3 currForward = transform.forward;
        currForward.Normalize();
        Vector3 targetForward = direction;
        targetForward.y = 0.0f;
        targetForward.Normalize();
        float forwardDot = Vector3.Dot(currForward, targetForward);
        if(forwardDot < 1.0f)
        {
            float angleToTarget = Mathf.Rad2Deg * Mathf.Acos(forwardDot);
            float time = angleToTarget / TurnSpeedDegrees;
            Quaternion desiredFacingRotation = Quaternion.LookRotation(targetForward);
            LeanTween.rotate(gameObject, desiredFacingRotation.eulerAngles, time);
        }
    }
}
