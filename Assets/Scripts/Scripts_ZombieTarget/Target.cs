using UnityEngine;

public class Target : MonoBehaviour
{

    public int hitvalue = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
      Debug.Log("Entra en el trigger");
      
      if (other.CompareTag("Bullet"))
        {
                ARGMZombie gameManagerZombie = FindObjectOfType<ARGMZombie>();
            if (gameManagerZombie != null)
            {
                gameManagerZombie.CountCollision(hitvalue);
                
            }
             Destroy(other.gameObject);
             
             
        }
    }
}

