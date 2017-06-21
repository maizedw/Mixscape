using UnityEngine;

public class AnimTriggerInteractable : InteractableObject
{
    public string Trigger = "shout";

    public bool TurnToFace = true;
    public float TurnSpeedDegrees = 180.0f;
    private Animator _animator;


    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public override void Interact(Vector3 interactDirection = new Vector3())
    {
        base.Interact(interactDirection);

        if(TurnToFace)
        {
            Vector3 currForward = transform.forward;
            Vector3 targetForward = interactDirection;
            targetForward.y = 0.0f;
            float angleToTarget = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(currForward, targetForward));
            float time = angleToTarget / TurnSpeedDegrees;
            Quaternion desiredFacingRotation = Quaternion.LookRotation(interactDirection);
            LeanTween.rotate(gameObject, desiredFacingRotation.eulerAngles, time);
        }

        if(_animator != null)
        {
            _animator.SetTrigger(Trigger);
        }
    }
}