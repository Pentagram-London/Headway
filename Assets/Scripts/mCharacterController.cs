using UnityEngine;
using System.Collections;

public class mCharacterController : MonoBehaviour {

    [SerializeField]
    Vector3 velocity;
    [SerializeField]
    Vector3 velocityRun;
  

    float runSpeed = 2.2f;
    float runAccel = 1.0f;
    Rigidbody rigidBody;

	[Range(0, 1)]
	public float snowLevel;

    GameObject playerModel;
    Quaternion playerFacingQuaternion;

    Animator animator;
    public float walkAnimBlend;
	float walkAnimBlendVel;
	public float walkAnimBlendTime;

    float controllerDeadZone = 0.25f;
    float controllerStickPower = 2f;

	public float audioBasedRun;

    public Vector3 inputVector;
    public Vector3 inputVectorCameraSpace;
    public Vector3 processedInputVector;

    public float inputSmoothTime = 0.2f;
    Vector3 inputSmoothVelocity; // reference variable

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity; // reference variable

	public Material ownMaterial;

    public Transform icicle0;
    public Transform icicle1;


    void Awake ()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerModel = transform.GetChild(0).gameObject;
        animator = playerModel.GetComponent<Animator>();

        playerFacingQuaternion = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {

        // get the raw input axis
		inputVector = new Vector3(0, 0, audioBasedRun);

        // clamp the input vector to avoid hypotenuse boosts
        inputVector = Vector3.ClampMagnitude(inputVector, 1.0f);

        // deadzone the input and remap from the deadzone 0 - 1
        inputVector = inputVector.normalized * (Mathf.Clamp01(inputVector.magnitude - controllerDeadZone) / (1.0f - controllerDeadZone));

        // change the curve from linear to power of x
        inputVector = inputVector.normalized * Mathf.Pow(inputVector.magnitude, controllerStickPower);

        // give the input relative to the camera
		inputVectorCameraSpace = inputVector.x * Vector3.right + inputVector.z * Vector3.forward;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + inputVectorCameraSpace.normalized * 10f);
    }

    void FixedUpdate()
    {
        // turn the character smoothly based on input direction
        if (inputVectorCameraSpace != Vector3.zero)
        {
            float targetFacingAngle = Mathf.Atan2(inputVectorCameraSpace.x, inputVectorCameraSpace.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetFacingAngle, ref turnSmoothVelocity, turnSmoothTime);
        }

        // accelerate the character smoothly based on input
        processedInputVector = Vector3.SmoothDamp(processedInputVector, inputVectorCameraSpace, ref inputSmoothVelocity, inputSmoothTime);

        // reset velocity calculation
        velocity = Vector3.zero;

        // calculate and reproject run velocity
        velocityRun = processedInputVector * runSpeed;

		if (velocityRun.sqrMagnitude > 0.3f)
		{
			walkAnimBlend = Mathf.SmoothDamp(walkAnimBlend, 1, ref walkAnimBlendVel, walkAnimBlendTime);
			snowLevel += 0.01f;
		}	
		else
		{
			snowLevel -= 0.003f;
			walkAnimBlend = Mathf.SmoothDamp (walkAnimBlend, 0, ref walkAnimBlendVel, walkAnimBlendTime);
		}

		snowLevel = Mathf.Max(Mathf.Min(snowLevel, 1.0f), 0.0f);

		animator.SetFloat ("WalkBlend", walkAnimBlend);

        ownMaterial.SetFloat ("_Snow", (snowLevel * 2 - 1));

        float icicleScale = (1 - snowLevel) * 0.5f;

        icicle0.localScale = new Vector3(icicleScale, icicleScale, icicleScale);
        icicle1.localScale = new Vector3(icicleScale, icicleScale, icicleScale);

        velocity = velocityRun; // total velocities

        rigidBody.velocity = Vector3.zero;
        rigidBody.velocity = velocity;
        
    }
}
