 using System.Collections;
 using System.Collections.Generic;
 using Prime31;
 using UnityEngine;
 using UnityEngine.UI;
 using ND_VariaBULLET;

public class CounterText : MonoBehaviour
{

	private float GoldCount;
    public Text GoldCountText;
    private float AtomicMax;
    private float AtomicCount;
    public Text AtomicCountText;
    private float Health;
    private float MaxHealth;
    public Text HealthCountText;
    private float MaxSheild;
    private float SheildCount;
    public Text SheildCountText;
    private ShotCollisionDamage shotCollisionDamage;
    //private float Health = shotCollisionDamage.getHP();
   // private GameObject player;

    void Start()
    {
    	//Health = shotCollisionDamage.getHP();
    	GoldCount = 0;
  		AtomicMax = 1000000;
  	 	AtomicCount = 0;
  	 	MaxHealth = Health;
  	 	MaxSheild = 100;
  	 	SheildCount = 100;
  	 	SetGoldCountText();
  	 	SetAtomicCountText();
  	 	SetSheildBarText();
  	 	SetHealthBarText();

        //shotCollisionDamage = class.Find("ShotCollisionDamage");
//        player = GetComponent<Player>();
        //note: trying to pull the "HP" variable from ShotCollisionDamage to show it in my health counter so the player
        //knows how much health they have left
        //But I feel like I've been overthinking it and cant get it to work

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
     		if(AtomicCount > AtomicMax)
     		{
     			AtomicCount = AtomicMax;
     		}
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
     	if (other.gameObject.CompareTag("Health_Crystal"))
     	{
     		Destroy(other.gameObject);

     		Health = Health + 20; 
     		if(Health>MaxHealth)
     		{
     			Health = MaxHealth;
     		}
     		SetHealthBarText();
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
     void SetSheildBarText()
     {
     	SheildCountText.text = "Sheild: " + SheildCount.ToString();
     }
     void SetHealthBarText()
     {
     	Health = shotCollisionDamage.getHP();
     	//player.ShotCollisionDamage.getHP();
     	HealthCountText.text = "Health: " + Health.ToString();
     }
}

