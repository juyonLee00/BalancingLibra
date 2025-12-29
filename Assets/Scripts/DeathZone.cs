using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Ball>() != null)
        {
            GameManager.Instance.GameOver("Ball fell into the death zone");
        }
    }
}
