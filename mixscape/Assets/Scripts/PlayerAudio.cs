using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public float FootstepInterval = 0.25f;
    public AkEvent FootstepEvent;
    private float _timer = 0.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if(Mathf.Approximately(horizontal, 0.0f) && Mathf.Approximately(vertical, 0.0f))
        {
            _timer = 0.0f;
        }
        else
        {
            _timer += Time.deltaTime;
            if(_timer > FootstepInterval)
            {
                FootstepEvent.HandleEvent(null);
                _timer -= FootstepInterval;
            }
        }
    }
}
