using System.Collections;
using UnityEngine;

public class UFO_Rotator : MonoBehaviour
{

    void Update()
    {
        transform.Rotate (new Vector3 (0,0,90) * (-Time.deltaTime));
    }
}