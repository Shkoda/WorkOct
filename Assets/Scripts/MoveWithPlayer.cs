using UnityEngine;
using System.Collections;

public class MoveWithPlayer2 : MonoBehaviour
{
    public GameObject target;
	
    // Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    this.transform.position = target.transform.position + new Vector3(0, 0, -10);
	}
}
