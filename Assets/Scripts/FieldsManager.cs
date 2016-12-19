using UnityEngine;
using System.Collections;

public class FieldsManager : MonoBehaviour
{

    // camera
    public Camera cam;

    // particle fields
    public GameObject[,] fields = new GameObject[2, 2];
    public Vector3[,] fieldPositions = new Vector3[2, 2];

    [Range(1, 50)]
    public float fieldScale = 30;
    public float fieldHeight = 5;
    [Range(0.1f, 25)]
    public float viewMaxDistance = 15;

    public Vector3 fieldOrigin;
    public Vector3 currentField; // [x , y , z]

    
    
    // init
    void Awake()
    {
        fields[0, 0] = GameObject.Find("Field_00");
        fields[1, 0] = GameObject.Find("Field_10");
        fields[0, 1] = GameObject.Find("Field_01");
        fields[1, 1] = GameObject.Find("Field_11");

        fieldOrigin = new Vector3(0.0f, 0.0f, 0.0f);

        fieldPositions[0, 0] = fieldOrigin;
        fieldPositions[1, 0] = fieldPositions[0, 0] + new Vector3(fieldScale, 0.0f, 0.0f);
        fieldPositions[0, 1] = fieldPositions[0, 0] + new Vector3(0.0f, 0.0f, fieldScale);
        fieldPositions[1, 1] = fieldPositions[0, 1] + new Vector3(fieldScale, 0.0f, 0.0f);

        fields[0, 0].transform.position = fieldPositions[0, 0];
        fields[1, 0].transform.position = fieldPositions[1, 0];
        fields[0, 1].transform.position = fieldPositions[0, 1];
        fields[1, 1].transform.position = fieldPositions[1, 1];

        GameObject[] particleSystemObjects = new GameObject[4];

        particleSystemObjects[0] = GameObject.Find("Field_00_Particles");
        particleSystemObjects[1] = GameObject.Find("Field_10_Particles");
        particleSystemObjects[2] = GameObject.Find("Field_01_Particles");
        particleSystemObjects[3] = GameObject.Find("Field_11_Particles");

        ParticleSystem[] particleSystems = new ParticleSystem[4];

        particleSystems[0] = particleSystemObjects[0].GetComponent<ParticleSystem>();
        particleSystems[1] = particleSystemObjects[1].GetComponent<ParticleSystem>();
        particleSystems[2] = particleSystemObjects[2].GetComponent<ParticleSystem>();
        particleSystems[3] = particleSystemObjects[3].GetComponent<ParticleSystem>();
        
        for (int i = 0; i < particleSystemObjects.Length; i++)
        {

            // make sure particle systems are centered at ground level
            particleSystemObjects[i].transform.position = new Vector3(particleSystemObjects[i].transform.position.x, fieldHeight * 0.5f, particleSystemObjects[i].transform.position.z); ; 

            // set particle system emitter box size to match field
            var shape = particleSystems[i].shape;
            shape.box = new Vector3(fieldScale, fieldHeight, fieldScale);
        }

        SkinnedMeshRenderer[] groundMeshes = new SkinnedMeshRenderer[4];

        groundMeshes[0] = GameObject.Find("Field_00_Ground").GetComponent<SkinnedMeshRenderer>();
        groundMeshes[1] = GameObject.Find("Field_10_Ground").GetComponent<SkinnedMeshRenderer>();
        groundMeshes[2] = GameObject.Find("Field_01_Ground").GetComponent<SkinnedMeshRenderer>();
        groundMeshes[3] = GameObject.Find("Field_11_Ground").GetComponent<SkinnedMeshRenderer>();

        Vector3 groundMeshBounds = new Vector3(fieldScale, fieldScale, fieldScale);
        for (int i = 0; i < groundMeshes.Length; i++)
        {
            Vector3 groundMeshCenter = new Vector3(0.0f, -groundMeshes[i].transform.position.y, 0.0f);
            groundMeshes[i].localBounds = new Bounds(groundMeshCenter, groundMeshBounds);
        }


    }

    void Update()
    {
        ManageFields();
    }

    // dynamically position the fields so that the camera is continuously surrounded
    void ManageFields()
    {

        /*

        [00] . . . . [10] . . . X
         .             .
         .             .
         .      V      .
         .             .
         .             .
        [01] . . . . [11]
         .
         .
         .
         Z

         */

		Vector3 camPos = cam.transform.position;

        bool escapedfields = false;

        // determine which x field the camera is in
        if ((camPos.x > fieldPositions[0, 0].x - fieldScale * 0.5f) && (camPos.x < fieldPositions[0, 0].x + fieldScale * 0.5f))
        {
            currentField.x = 0;
        }
        else if ((camPos.x > fieldPositions[1, 0].x - fieldScale * 0.5f) && (camPos.x < fieldPositions[1, 0].x + fieldScale * 0.5f))
        {
            currentField.x = 1;
        }
        else
        {
            escapedfields = true;
        }

        // determine which z field the camera is in
        if ((camPos.z > fieldPositions[0, 0].z - fieldScale * 0.5f) && (camPos.z < fieldPositions[0, 0].z + fieldScale * 0.5f))
        {
            currentField.z = 0;
        }
        else if ((camPos.z > fieldPositions[0, 1].z - fieldScale * 0.5f) && (camPos.z < fieldPositions[0, 1].z + fieldScale * 0.5f))
        {
            currentField.z = 1;
        }
        else
        {
            escapedfields = true;
        }

        if (!escapedfields)
        {
            CheckAndSort_X(camPos);
            CheckAndSort_Z(camPos);
        }
    }

    void CheckAndSort_X(Vector3 camPos)
    {
        if (currentField.x == 0)
        {
            if (camPos.x < fieldPositions[0, 0].x)
            {
                fieldPositions[1, 0].x = fieldPositions[0, 0].x - fieldScale;
                fieldPositions[1, 1].x = fieldPositions[0, 1].x - fieldScale;
                fields[1, 0].transform.position = fieldPositions[1, 0];
                fields[1, 1].transform.position = fieldPositions[1, 1];
                return;
            }
            if (camPos.x > fieldPositions[0, 0].x)
            {
                fieldPositions[1, 0].x = fieldPositions[0, 0].x + fieldScale;
                fieldPositions[1, 1].x = fieldPositions[0, 1].x + fieldScale;
                fields[1, 0].transform.position = fieldPositions[1, 0];
                fields[1, 1].transform.position = fieldPositions[1, 1];
                return;
            }
            else
            {
                return;
            }
        }

        if (currentField.x == 1)
        {
            if (camPos.x < fieldPositions[1, 0].x)
            {
                fieldPositions[0, 0].x = fieldPositions[1, 0].x - fieldScale;
                fieldPositions[0, 1].x = fieldPositions[1, 1].x - fieldScale;
                fields[0, 0].transform.position = fieldPositions[0, 0];
                fields[0, 1].transform.position = fieldPositions[0, 1];
                return;
            }
            if (camPos.x > fieldPositions[1, 0].x)
            {
                fieldPositions[0, 0].x = fieldPositions[1, 0].x + fieldScale;
                fieldPositions[0, 1].x = fieldPositions[1, 1].x + fieldScale;
                fields[0, 0].transform.position = fieldPositions[0, 0];
                fields[0, 1].transform.position = fieldPositions[0, 1];
                return;
            }
            else
            {
                return;
            }
        }
    }

    void CheckAndSort_Z(Vector3 camPos)
    {
        if (currentField.z == 0)
        {
            if (camPos.z < fieldPositions[0, 0].z)
            {
                fieldPositions[0, 1].z = fieldPositions[0, 0].z - fieldScale;
                fieldPositions[1, 1].z = fieldPositions[1, 0].z - fieldScale;
                fields[0, 1].transform.position = fieldPositions[0, 1];
                fields[1, 1].transform.position = fieldPositions[1, 1];
                return;
            }
            if (camPos.z > fieldPositions[0, 0].z)
            {
                fieldPositions[0, 1].z = fieldPositions[0, 0].z + fieldScale;
                fieldPositions[1, 1].z = fieldPositions[1, 0].z + fieldScale;
                fields[0, 1].transform.position = fieldPositions[0, 1];
                fields[1, 1].transform.position = fieldPositions[1, 1];
                return;
            }
            else
            {
                return;
            }
        }

        if (currentField.z == 1)
        {
            if (camPos.z < fieldPositions[0, 1].z)
            {
                fieldPositions[0, 0].z = fieldPositions[0, 1].z - fieldScale;
                fieldPositions[1, 0].z = fieldPositions[1, 1].z - fieldScale;
                fields[0, 0].transform.position = fieldPositions[0, 0];
                fields[1, 0].transform.position = fieldPositions[1, 0];
                return;
            }
            if (camPos.z > fieldPositions[0, 0].z)
            {
                fieldPositions[0, 0].z = fieldPositions[0, 1].z + fieldScale;
                fieldPositions[1, 0].z = fieldPositions[1, 1].z + fieldScale;
                fields[0, 0].transform.position = fieldPositions[0, 0];
                fields[1, 0].transform.position = fieldPositions[1, 0];
                return;
            }
            else
            {
                return;
            }
        }
    }

}

 