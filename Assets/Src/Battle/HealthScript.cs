using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour
{
    public int HP;
    public bool IsDestroyable;
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Damage(int damage)
    {
        if (IsDestroyable)
        {
            HP -= damage;
            if (HP <= 0)
                Destroy(gameObject);
        }    
    }
   
    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        BulletColliding shot = otherCollider.gameObject.GetComponent<BulletColliding>();
        if (shot != null)
        {
            // Avoid friendly fire
//            if (shot.isEnemyShot != isEnemy)
//            {
                Damage(shot.damage);

                // Destroy the shot
                Destroy(shot.gameObject); // Remember to always target the game object, otherwise you will just remove the script
//            }
        }


    }
}