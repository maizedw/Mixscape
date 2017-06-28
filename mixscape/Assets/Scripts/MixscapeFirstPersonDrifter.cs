using UMA;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent (typeof (CharacterController))]
public class MixscapeFirstPersonDrifter: MonoBehaviour
{
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
    public float InteractMaxDistance = 10.0f;
    public bool LinkSpeedToScale;

    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    private bool limitDiagonalSpeed = true;
 
    public bool enableRunning = false;
 
    public float jumpSpeed = 4.0f;
    public float gravity = 10.0f;
 
    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    private float fallingDamageThreshold = 10.0f;
 
    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;
 
    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;
 
    public float slideSpeed = 5.0f;
 
    // If checked, then the player can change direction while in the air
    public bool airControl = true;
 
    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;
 
    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public int antiBunnyHopFactor = 1;

    public float FootstepDistance = 1.0f;
    public AkEvent FootstepEvent;
    public AkEvent JumpEvent;
    public float MinAirborneTimeToLand = 0.4f;
    public AkEvent LandEvent;
    public Collider PrimaryCollider;
    public bool EnableResizeControls = true;
    public float MaxScale = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private CharacterController controller;
    private Transform myTransform;
    private float speed;
    private RaycastHit hit;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;
    private bool playerControl = false;
    private int jumpTimer;
    private float _footstepMoveAmount = 0.0f;
    private bool _sliding;
    private float _airborneTimer = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        speed = walkSpeed;
        rayDistance = controller.height * .5f + controller.radius;
        slideLimit = controller.slopeLimit - .1f;
        jumpTimer = antiBunnyHopFactor;

        UMADynamicAvatar avatar = GetComponentInChildren<UMADynamicAvatar>();
        if(avatar != null)
        {
            avatar.Load(avatar.umaRecipe, avatar.umaAdditionalRecipes);
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if((Mathf.Approximately(horizontal, 0.0f) && Mathf.Approximately(vertical, 0.0f)))
        {
            _footstepMoveAmount = 0.0f;
        }
        else
        {
            if(_footstepMoveAmount > FootstepDistance)
            {
                if(FootstepEvent != null)
                {
                    FootstepEvent.HandleEvent(null);
                }
                _footstepMoveAmount = 0.0f;
            }
        }

        if(EnableResizeControls)
        {
            float currScale = transform.localScale.y;
            if(Input.GetKey(KeyCode.Q))
            {
                currScale = currScale - (currScale * Time.deltaTime);
            }
            else if(Input.GetKey(KeyCode.E))
            {
                currScale = currScale + (currScale * Time.deltaTime);
            }
            currScale = Mathf.Clamp(currScale, 0.1f, MaxScale);
            transform.localScale = new Vector3(currScale, currScale, currScale);
        }

        if(grounded == false)
        {
            _airborneTimer += Time.deltaTime;
        }
        else
        {
            _airborneTimer = 0.0f;
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Transform cameraTransform = GetComponentInChildren<Camera>().transform;
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            float interactMaxDistance = InteractMaxDistance * transform.lossyScale.y;
            if(!ProcessInteract(Physics.RaycastAll(ray, interactMaxDistance)))
            {
                float interactSphereSize = 0.25f * transform.lossyScale.y;
                float maxSphereSize = (interactSphereSize * 4.0f) + 0.01f;
                for(float sphereSize = interactSphereSize; sphereSize <= maxSphereSize; sphereSize += interactSphereSize)
                {
                    if(ProcessInteract(Physics.SphereCastAll(ray, sphereSize, interactMaxDistance)))
                    {
                        break;
                    }
                }
            }
        }

        AkSoundEngine.SetRTPCValue("player_scale", transform.lossyScale.y);
    }

    private void LateUpdate()
    {
        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            if(meshRenderer.shadowCastingMode != ShadowCastingMode.ShadowsOnly)
                meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            if(meshRenderer.receiveShadows)
                meshRenderer.receiveShadows = false;
        }
    }

    private bool ProcessInteract(RaycastHit[] raycastHits)
    {
        foreach(RaycastHit raycastHit in raycastHits)
        {
            if(raycastHit.distance <= 0.0f)
            {
                continue;
            }

            InteractableObject interactTarget = raycastHit.collider.GetComponent<InteractableObject>();
            if(interactTarget != null)
            {
                interactTarget.Interact(-transform.forward);
                return true;
            }
        }

        return false;
    }

    void FixedUpdate() {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;
 
        if (grounded) {
            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance)) {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
 
            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling) {
                falling = false;
                if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert (fallStartLevel - myTransform.position.y);
            }
 
            if( enableRunning )
            {
                speed = Input.GetButton("Run")? runSpeed : walkSpeed;
                if(LinkSpeedToScale)
                {
                    speed *= transform.lossyScale.y;
                }
            }
 
            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                _sliding = true;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else {
                moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                moveDirection = myTransform.TransformDirection(moveDirection) * speed;
                _sliding = false;
                playerControl = true;
            }
 
            // Jump! But only if the jump button has been released and player has been grounded for a given number of frames
            if (!Input.GetButton("Jump"))
                jumpTimer++;
            else if (jumpTimer >= antiBunnyHopFactor) {
                moveDirection.y = jumpSpeed;
                jumpTimer = 0;
                if(JumpEvent != null)
                {
                    JumpEvent.HandleEvent(null);
                }
            }
        }
        else {
            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling) {
                falling = true;
                fallStartLevel = myTransform.position.y;
            }
 
            // If air control is allowed, check movement but don't touch the y component
            if (airControl && playerControl) {
                moveDirection.x = inputX * speed * inputModifyFactor;
                moveDirection.z = inputY * speed * inputModifyFactor;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }
        }
 
        // Apply gravity
        moveDirection.y -= gravity * Time.fixedDeltaTime;

        bool wasGrounded = grounded;

        // Move the controller, and set grounded true or false depending on whether we're standing on something
        Vector3 movementThisFrame = moveDirection * Time.fixedDeltaTime;

        grounded = (controller.Move(movementThisFrame) & CollisionFlags.Below) != 0;
        if(grounded && !_sliding)
        {
            _footstepMoveAmount += movementThisFrame.magnitude / transform.lossyScale.y;
        }
        else
        {
            _footstepMoveAmount = 0.0f;
        }

        if(!wasGrounded && grounded && falling)
        {
            if(_airborneTimer >= MinAirborneTimeToLand)
            {
                if(LandEvent != null)
                {
                    LandEvent.HandleEvent(null);
                }
            }
        }
    }

    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit) {
        contactPoint = hit.point;
    }
 
    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance)
    {
        //print ("Ouch! Fell " + fallDistance + " units!");   
    }
}