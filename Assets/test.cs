using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Vector3 origin;
    public Vector3 direction;
    public Vector3 normal;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(origin, direction);
        Debug.DrawRay(origin + direction, normal, Color.cyan);
        Debug.DrawRay(origin + direction, Reflection(), Color.red);
    }

    Vector3 Reflection()
    {
        float dot = 2 * Vector3.Dot(direction.normalized, normal.normalized);
        return direction.normalized - (normal.normalized * dot);
    }
}
