 using System.Collections;
 using System.Collections.Generic;
 using Prime31;
 using UnityEngine;
 using UnityEngine.UI;
  
 public class CC_Move2 : MonoBehaviour {
  
     CharacterController characterController;
     bool canPlayerMove = true;
     public float speed = 0.0f;
     private int GoldCount;
     public Text GoldCountText;
     private int AtomicCount;
     public Text AtomicCountText;
  
  	 void Start()
  	 {
  	 	characterController = GetComponent<CharacterController>();
  	 	GoldCount = 0;
  	 	AtomicCount = 0;
  	 	SetGoldCountText();
  	 	SetAtomicCountText();
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

     void OnTriggerEnter(Collider other)
     {
     	if (other.gameObject.CompareTag("Gold"))
     	{
     		Destroy(other.gameObject);
     		GoldCount = GoldCount + 1;
     		SetGoldCountText();
     	}
     	if (other.gameObject.CompareTag("Atomic_Crystal"))
     	{
     		Destroy(other.gameObject);
     		AtomicCount = AtomicCount + 1;
     		SetAtomicCountText();
     	}
     }

     void SetGoldCountText()
     {
     	GoldCountText.text = "Gold: " + GoldCount.ToString();
     }
     void SetAtomicCountText()
     {
     	AtomicCountText.text = "Atomic Crystals: " + AtomicCount.ToString();
     }
 }