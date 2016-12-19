using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class CameraSequence : MonoBehaviour {

	public Transform player;
    public mCharacterController playerController;
	public Transform cam;
	public Transform camTilt;
	public AnimationCurve heightCurve;
	public AnimationCurve angleCurve;

	public float distance;

	float maxDistance = 10.0f;
	float minDistance = 2.5f;
	float orbitSpeed = 0.2f;

    public AudioMixer audioMixer;

    public AudioAnalyser audioAnalyser;
    public float audioTriggerCount;

    public enum GameState { Opening, Running, Ending };
    public GameState gameState = GameState.Opening;

    // Opening animation
    Vector3 openingCamPos;
    Vector3 openingEndCamPos;
    public AnimationCurve openingCurve;

    Vector3 openingCamOffsetPos;
    Quaternion openingCamOffsetRot;

    Vector3 openingEndCamOffsetPos;
    Quaternion openingEndCamOffsetRot;

    public Material grainAndTint;

    void Start()
    {
        distance = minDistance;

        // define opening state start and end points.

        audioMixer.SetFloat("SFXVol", -10.0f);

        openingCamPos = Vector3.zero;
        openingEndCamPos = player.position;

        openingCamOffsetPos = new Vector3(0.0f, 1.0f, -distance);
        openingCamOffsetRot = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        openingEndCamOffsetPos = new Vector3(0.0f, heightCurve.Evaluate(distance / (maxDistance - minDistance)) * 5.0f, -distance);
        openingEndCamOffsetRot = Quaternion.Euler((angleCurve.Evaluate(distance / 8.0f) - 1.5f) * -20.0f, 0.0f, 0.0f);


        if (gameState == GameState.Opening)
        {
            StartCoroutine("Opening");
        }

    }

	void FixedUpdate ()
	{
		distance = Mathf.Min(Mathf.Max(minDistance, distance), maxDistance);

        if (gameState == GameState.Running)
        {

            if (audioAnalyser.audioInLevel > 20.0f)
                audioTriggerCount = 5.0f;
 

            // depending on the audio trigger count, notify the player, and increment the camera distance
            if (audioTriggerCount > 0.0f)
            {
                if (gameState == GameState.Running)
                {
                    playerController.audioBasedRun = 1.0f;
                    distance += 0.01f;
                }

            }
            else
            {
                if (gameState == GameState.Running)
                {
                    playerController.audioBasedRun = 0.0f;
                    distance -= 0.01f;
                }
            }

            // increment and limit the audio trigger count
            audioTriggerCount -= 1.0f;
            audioTriggerCount = Mathf.Max(0.0f, audioTriggerCount);

            // camera orbiting, follow, distance-based height, and tilt
            transform.rotation *= Quaternion.Euler(0.0f, orbitSpeed, 0.0f);
            transform.position = player.position;
            camTilt.localPosition = Vector3.Lerp(camTilt.localPosition, new Vector3(0.0f, heightCurve.Evaluate(distance / (maxDistance - minDistance)) * 5.0f, -distance), Time.deltaTime * 5.0f);
            camTilt.localRotation = Quaternion.Euler((angleCurve.Evaluate(distance / 8.0f) - 1.5f) * -20.0f, 0.0f, 0.0f);
        }		    
		
	}

    IEnumerator Opening()
    {        
        float sfxVol = -10.0f;
        for (float l = 0f; l < 1; l += 0.002f)
        {
           sfxVol = Mathf.Lerp(-30.0f, 0.0f, l);
           audioMixer.SetFloat("SFXVol", sfxVol);

           grainAndTint.SetFloat("_WhiteOut", 1 - l);

           transform.position = Vector3.Lerp(openingCamPos, openingEndCamPos, openingCurve.Evaluate(l));
           camTilt.localPosition = Vector3.Lerp(openingCamOffsetPos, openingEndCamOffsetPos, openingCurve.Evaluate(l));
           camTilt.localRotation = Quaternion.Slerp(openingCamOffsetRot, openingEndCamOffsetRot, openingCurve.Evaluate(l));
           yield return null;
        }

        //audioMixer.SetFloat("SFXVol", 0.0f);
        //transform.position = player.position;
        //camTilt.localPosition = openingEndCamOffsetPos;
        //camTilt.localRotation = openingEndCamOffsetRot; 

        gameState = GameState.Running;
    }
}
