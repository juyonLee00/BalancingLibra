using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ball fallenBall = other.GetComponentInParent<Ball>();
        
        if(fallenBall != null)
        {
            GameManager.Instance.LoseLife("Ball fell off the scale (Death Zone)");
            Destroy(fallenBall.gameObject);
        }
    }
}