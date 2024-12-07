using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using MoreMountains.Feedbacks;

public class AnimationTrigger : MonoBehaviour
{

    private AIBotController controller;
    private Blaster blaster;
    [SerializeField]private PowerLaser powerLaser;
    public MMF_Player laserFeedback;
    private void Awake()
    {
        controller = GetComponentInParent<AIBotController>();
        blaster = GetComponentInChildren<Blaster>();
    }

    public void TriggerPowerLaser()
    {
        if (powerLaser != null)
        {
            powerLaser.gameObject.SetActive(true);
        }
    }

    public void ActivateLaser()
    {
        laserFeedback.PlayFeedbacks();
    }

    
    public void DeactivateLaser()
    {
        powerLaser.gameObject.transform.localScale = new Vector3(1f,1f,0f);
    }

   

    public void TriggerBlasterFire()
    {
      

        // Use blaster if the bot has one
        if (blaster != null)
        {
            blaster.FireAtTarget(controller.target);
        }
    }



}
