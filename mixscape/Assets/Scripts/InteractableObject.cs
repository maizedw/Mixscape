﻿using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public AkEvent InteractSound;
    public bool TestInteract;

    protected virtual void Update()
    {
        if(TestInteract)
        {
            Interact();
            TestInteract = false;
        }
    }

    public virtual void Interact(Vector3 interactDirection = new Vector3())
    {
        if(InteractSound != null)
        {
            InteractSound.HandleEvent(null);
        }
    }
}