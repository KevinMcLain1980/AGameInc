//using UnityEngine;
//using UnityEngine.UI;

//public class DrownPlayer : MonoBehaviour
//{
  //  [Header("Water UI Overlay")]
  //  [SerializeField] private Image waterOverlay;

 //   [Header("Oxygen Settings")]
  //  [SerializeField] private PlayerStat oxygenStat;
 //   [SerializeField] private float oxygenDrainRate = 5f;

 //   private bool isUnderwater = false;
//    private bool isDrowning = false;

 //   private void Update()
 //   {
    //    if (isUnderwater && oxygenStat != null)
      //  {
            // Drain oxygen over time
  //         float amount = oxygenDrainRate * Time.deltaTime;
   //         oxygenStat.SetValue(oxygenStat.CurrentValue);

            // Trigger drowning when oxygen hits zero
        //    if (oxygenStat.CurrentValue <= 0f && !isDrowning)
         //  {
           //     StartDrowning();
           // }
      //  }
  //  }

   // public void SetWaterOverlay(bool state)
  //  {
    //    if (waterOverlay != null)
       //     waterOverlay.enabled = state;

      //  isUnderwater = state;

     //   if (!state && oxygenStat != null)
     //   {
      //      oxygenStat.ResetStat(); // Restore oxygen when exiting water
       //     Debug.Log("Exited water: oxygen reset.");
      //  }
      //  else
     //   {
     //       Debug.Log("Entered water: starting oxygen drain.");
     //   }
   // }

   // private void StartDrowning()
   // {
    //    isDrowning = true;

    //    Animator animator = GetComponent<Animator>();
    //    if (animator != null)
   //     {
    //        animator.SetTrigger("Drowning");
   //     }

    //    Debug.Log("Drowning started.");
  //  }

    // Called via Animation Event at end of Drowning animation
  //  public void OnDrowningAnimationComplete()
 //   {
  //      Animator animator = GetComponent<Animator>();
  //      if (animator != null)
   //     {
  //          animator.SetTrigger("Death");
   //     }
    
    //    Debug.Log("Drowning animation complete. Death triggered.");
 //   }
//