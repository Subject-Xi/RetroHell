 using System.Collections;
 using System.Collections.Generic;
 using Prime31;
 using UnityEngine;
 using UnityEngine.UI;

public class CounterText : MonoBehaviour
{

	private int GoldCount;
    public Text GoldCountText;
    private float AtomicMax;
    private float AtomicCount;
    public Text AtomicCountText;
    private int MaxHealth;
    private int HP;
    public Text HealthCountText;
    private int MaxSheild;
    private int SheildCount;
    public Text SheildCountText;

    void Start()
    {
    	GoldCount = 0;
  		AtomicMax = 1000000;
  	 	AtomicCount = 0;
  	 	MaxHealth = 100;
  	 	HP = 100;
  	 	MaxSheild = 100;
  	 	SheildCount = 100;
  	 	SetGoldCountText();
  	 	SetAtomicCountText();
  	 	SetHealthBarText();
  	 	SetSheildBarText();
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
     	if (other.gameObject.CompareTag("Health_Crystal"))
     	{
     		Destroy(other.gameObject);
     		HP = HP + 20; 
     		if(HP>MaxHealth)
     		{
     			HP = MaxHealth;
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
     	HealthCountText.text = "Health: " + HP.ToString();
     }
     void SetSheildBarText()
     {
     	SheildCountText.text = "Sheild: " + SheildCount.ToString();
     }
}

