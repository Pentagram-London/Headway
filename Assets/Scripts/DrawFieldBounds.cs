using UnityEngine;
using System.Collections;

public class DrawFieldBounds : MonoBehaviour {

    [SerializeField]
    FieldsManager fields;

    public Color color;

    void OnDrawGizmos()
    {

        Gizmos.color = color;

        if (fields != null)
            Gizmos.DrawWireCube(transform.position, Vector3.one * fields.fieldScale);

    }
}
