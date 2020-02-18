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
     private float AtomicMax;
     private float AtomicCount;
     public Text AtomicCountText;
     private int MaxHealth;
     private int HealthCount;
     public Text HealthCountText;
     private int MaxSheild;
     private int SheildCount;
     public Text SheildCountText;
  
  	 void Start()
  	 {
  	 	characterController = GetComponent<CharacterController>();
  	 	GoldCount = 0;
  	 	AtomicMax = 1000000;
  	 	AtomicCount = 0;
  	 	MaxHealth = 100;
  	 	HealthCount = 85;
  	 	MaxSheild = 100;
  	 	SheildCount = 85;
  	 	SetGoldCountText();
  	 	SetAtomicCountText();
  	 	SetHealthBarText();
  	 	SetSheildBarText();

  	 	//Enable Dev shenanigans
  	 	//JustForFun();
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
     		//AtomicCount = AtomicCount + 1;
     		AtomicCount++;
     		
     		SetAtomicCountText();
     	}
     	if (other.gameObject.CompareTag("Health_Crystal"))
     	{
     		Destroy(other.gameObject);
     		HealthCount = HealthCount + 20; 
     		if(HealthCount>MaxHealth)
     		{
     			HealthCount = MaxHealth;
     		}
     		SetHealthBarText();
     	}
     	if (other.gameObject.CompareTag("Sheild_Crystal"))
     	{
     		Destroy(other.gameObject);
     		SheildCount = SheildCount + 20;
     		if(SheildCount>MaxSheild)
     		{
     			SheildCount = MaxSheild;
     		}
     		SetSheildBarText();
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
     void SetHealthBarText()
     {
     	HealthCountText.text = "Health: " + HealthCount.ToString();
     }
     void SetSheildBarText()
     {
     	SheildCountText.text = "Sheild: " + SheildCount.ToString();
     }

     //Just for fun Dev settings
     private void JustForFun()
     {
     	//for (AtomicCount=0;AtomicCount<AtomicMax;AtomicCount++)
     	//	{
     	//		AtomicCount++;
     	//	}

     	for (float i=0;i<AtomicMax;i++)
     		{
     			AtomicCount = AtomicCount + (i * Time.deltaTime);
     			if (AtomicCount > AtomicMax)
     			{
     				AtomicCount = AtomicMax;
     			}
     		}
     }
 }