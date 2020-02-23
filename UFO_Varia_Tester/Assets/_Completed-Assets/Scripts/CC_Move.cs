 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
  
 public class CC_Move : MonoBehaviour {
  
     bool canPlayerMove = true;
  
     public float speed = 0.0f;
  
     void FixedUpdate()
     {
         float moveHorizontal = Input.GetAxisRaw("Horizontal");
         float moveVertical = Input.GetAxisRaw("Vertical");

         Vector3 LeftRight = new Vector3(moveHorizontal, 0.0f);
         Vector3 UpDown = new Vector3(0.0f, moveVertical);
  
         if (canPlayerMove)
         {
             transform.Translate(LeftRight * speed * Time.deltaTime, Space.World);

             transform.Translate(UpDown * speed * Time.deltaTime, Space.World);
         }
     }
 }