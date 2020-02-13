using System.Collections;
using UnityEngine;

public class Atomic_Rotator : MonoBehaviour
{

    void Update()
    {
        transform.Rotate (new Vector3 (0,0,5) * Time.deltaTime);
    }
}