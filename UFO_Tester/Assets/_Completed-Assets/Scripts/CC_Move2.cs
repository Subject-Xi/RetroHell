 using System.Collections;
 using System.Collections.Generic;
 using Prime31;
 using UnityEngine;
  
 public class CC_Move2 : MonoBehaviour {
  
     CharacterController characterController;
     bool canPlayerMove = true;
     public float speed = 0.0f;
  
  	 void Start()
  	 {
  	 	characterController = GetComponent<CharacterController>();
  	 }

     void FixedUpdate()
     {
         float moveHorizontal = Input.GetAxisRaw("Horizontal");
         float moveVertical = Input.GetAxisRaw("Vertical");

         Vector3 LeftRight = new Vector3(moveHorizontal, 0.0f);
         	LeftRight *= speed;
         Vector3 UpDown = new Vector3(0.0f, moveVertical);
         	UpDown *= speed;
  
         if (canPlayerMove)
         {
             characterController.Move(LeftRight * Time.deltaTime);

             characterController.Move(UpDown * Time.deltaTime);
         }
     }

     void OnControllerCollisionHit(ControllerColliderHit hit)
     {
     	Rigidbody body = hit.collider.attachedRigidbody;

     	if(body == null || body.isKinematic)
     	{
     		return;
     	}
     }
 }