using UnityEngine;
using System.Collections;

public class BulletColliding : MonoBehaviour {
    public int damage;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 20); // 20sec
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
     
//            Destroy(gameObject); // Remember to always target the game object, otherwise you will just remove the script
     


    }
}
