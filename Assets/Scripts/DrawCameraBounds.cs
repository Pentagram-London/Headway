using UnityEngine;
using System.Collections;

public class DrawCameraBounds : MonoBehaviour {

    [SerializeField]
    FieldsManager fields;

    public Color color;

    void OnDrawGizmos()
    {
        Gizmos.color = color;

        if (fields != null)
            Gizmos.DrawWireSphere(transform.position, fields.viewMaxDistance);
    }
}
