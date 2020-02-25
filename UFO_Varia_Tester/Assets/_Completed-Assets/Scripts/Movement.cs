
using UnityEngine;
using System.Collections;
 
public class BasicMovement : MonoBehaviour {
 
    public float speed = 10f;
   
    void Update () {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
 
        gameObject.GetComponent<CharacterController>().Move(transform.TransformDirection(input * speed * Time.deltaTime));
    }
}