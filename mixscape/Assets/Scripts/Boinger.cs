using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Boinger : MonoBehaviour
{
    public AkEvent BoingSound;
    public float BoingTime = 0.8f;
    public float BoingSpeed = 14.0f;
    public float MaxRotation = 5.0f;
    public float BoingStartScaleDown = 0.075f;
    public float BoingEndExtraScaleUp = 0.05f;
    public bool BoingIsAtomic;
    
    public bool TestBoing;

    private Quaternion _baseRotation;

    public bool IsBoinging { get { return _boingTimer >= 0.0f; } }

    private float _boingTimer = -1.0f;
    private Vector3 _boingAroundVec;
    private float _baseYScale;

    // Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if(TestBoing)
	    {
	        Boing();
	        TestBoing = false;

	    }
	    if(IsBoinging)
	    {
	        _boingTimer += Time.deltaTime;

	        float percentage = _boingTimer / BoingTime;
	        float boingMultiplier = Mathf.Lerp(MaxRotation, 0.0f, percentage * percentage);

	        float currZRot = Mathf.Sin(_boingTimer * BoingSpeed * Mathf.PI) * boingMultiplier;
            Quaternion currRotation = Quaternion.AngleAxis(currZRot, _boingAroundVec);

	        transform.localRotation = _baseRotation * currRotation;

	        if(_boingTimer >= BoingTime)
	        {
	            _boingTimer = -1.0f;
	            transform.localRotation = _baseRotation;
	        }
	    }
	}

    public void Boing(Vector3 rotateAroundVec = new Vector3())
    {
        if(!isActiveAndEnabled)
            return;

        if(BoingIsAtomic && IsBoinging)
            return;

        BoingSound.HandleEvent(null);

        if(rotateAroundVec == Vector3.zero)
        {
            rotateAroundVec = transform.forward;
        }

        if(!IsBoinging)
        {
            _baseRotation = transform.localRotation;
            _baseYScale = transform.localScale.y;
        }

        _boingTimer = 0.0f;

        // move boingaroundvec into local space
        _boingAroundVec = transform.InverseTransformDirection(rotateAroundVec);
        
        LTSeq seq = LeanTween.sequence();
        seq.append(LeanTween.scaleY(gameObject, _baseYScale * (1.0f - BoingStartScaleDown), BoingTime * 0.2f).setEaseOutCubic());
        seq.append(0.2f);
        seq.append(LeanTween.scaleY(gameObject, _baseYScale * (1.0f + BoingEndExtraScaleUp), BoingTime * 0.3f).setEaseOutCubic());
        seq.append(LeanTween.scaleY(gameObject, _baseYScale, BoingTime * 0.25f).setEaseInCubic());
    }
}
