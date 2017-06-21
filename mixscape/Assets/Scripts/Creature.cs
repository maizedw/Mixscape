using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float TurnSpeedDegrees = 180.0f;

    private Animator _animator;
    public Animator Animator
    {
        get
        {
            if(_animator == null)
            {
                _animator = GetComponent<Animator>();
                if(_animator == null)
                    _animator = GetComponentInChildren<Animator>();
            }
            return _animator;
        }
    }

    // Use this for initialization
    protected virtual void Start()
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
