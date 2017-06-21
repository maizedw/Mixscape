using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class NpcCreature : Creature
{
    public string GreetParameter = "Greeting";
    public string RespondParameter = "Responding";
    public AkEvent GreetEvent;
    public AkEvent RespondEvent;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        InteractableObject interactableObject = GetComponent<InteractableObject>();
        if(interactableObject != null)
        {
            interactableObject.CustomInteractCallback = Respond;
        }
    }

    public void Greet(Vector3 talkDirection)
    {
        if(Animator != null && string.IsNullOrEmpty(GreetParameter) == false)
        {
            Animator.SetBool(GreetParameter, true);
        }
        GreetEvent.HandleEvent(null);
    }

    public void OnGreetingComplete()
    {
        if(Animator != null && string.IsNullOrEmpty(GreetParameter) == false)
        {
            Animator.SetBool(GreetParameter, false);
        }
    }

    public void Respond(Vector3 talkDirection)
    {
        TurnToFace(talkDirection);

        if(Animator != null && string.IsNullOrEmpty(RespondParameter) == false)
        {
            Animator.SetBool(RespondParameter, true);
        }
        RespondEvent.HandleEvent(null);
    }

    public void OnRespondComplete()
    {
        if(Animator != null && string.IsNullOrEmpty(RespondParameter) == false)
        {
            Animator.SetBool(RespondParameter, false);
        }
    }
}