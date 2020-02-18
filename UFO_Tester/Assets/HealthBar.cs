 using System.Collections;
 using System.Collections.Generic;
 using Prime31;
 using UnityEngine;
 using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	 private int MaxHealth;
     private int HealthCount;
     private float HealthBarLength;
    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = 100;
  	 	HealthCount = 85;
  	 	HealthBarLength = Screen.width/5;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustHealthCount(0);
    }

    void OnGUI()
    {
     	GUI.Box(new Rect(10, 10, HealthBarLength, 20), HealthCount + "/" + MaxHealth);
    }

    public void AdjustHealthCount (int adj)
    {

   HealthCount += adj;
   if(HealthCount < 0)
   {
        HealthCount = 0;
   }
   if(HealthCount > MaxHealth)
   {
        HealthCount = MaxHealth;
   }
   if(MaxHealth < 1)
   {
        MaxHealth = 1;
   }
   HealthBarLength = (Screen.width / 2) * (HealthCount / (float)MaxHealth);
	}
}
