using System;
using UnityEngine;

public class NpcVision : MonoBehaviour
{
    public string SeePlayerAnimTrigger;
    public bool SeePlayerTurnToFace;
    public bool SeePlayerGreet;

    private NpcCreature _npc;
    private MixscapeFirstPersonDrifter _player;

    // Use this for initialization
    void Start()
    {
        _npc = GetComponentInParent<NpcCreature>();
        _player = FindObjectOfType<MixscapeFirstPersonDrifter>();
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

    public bool CanSeePlayer()
    {
        return IsInBounds(_player.PrimaryCollider);
    }

    public bool IsInBounds(Collider collider)
    {
        foreach(var coll in GetComponents<BoxCollider>())
        {
            Collider[] colliderList = Physics.OverlapBox(coll.transform.TransformPoint(coll.center), coll.size * 0.5f, coll.transform.rotation);
            if(Array.IndexOf(colliderList, _player.PrimaryCollider) >= 0)
            {
                return true;
            }
        }

        return false;
    }
}