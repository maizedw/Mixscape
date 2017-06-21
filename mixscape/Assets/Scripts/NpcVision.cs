using UnityEngine;

public class NpcVision : MonoBehaviour
{
    public string SeePlayerAnimTrigger;
    public bool SeePlayerTurnToFace;
    public bool SeePlayerGreet;

    private NpcCreature _npc;

    // Use this for initialization
    void Start()
    {
        _npc = GetComponentInParent<NpcCreature>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        MixscapeFirstPersonDrifter player = other.GetComponent<MixscapeFirstPersonDrifter>();
        if(player != null)
        {
            if(_npc != null)
            {
                if(string.IsNullOrEmpty(SeePlayerAnimTrigger) == false)
                {
                    if(_npc.Animator != null)
                    {
                        _npc.Animator.SetTrigger(SeePlayerAnimTrigger);
                    }
                }

                if(SeePlayerTurnToFace)
                {
                    _npc.TurnToFace(player.transform.position - _npc.transform.position);
                }

                if(SeePlayerGreet)
                {
                    _npc.Greet(player.transform.position - _npc.transform.position);
                }
            }
        }
    }
}