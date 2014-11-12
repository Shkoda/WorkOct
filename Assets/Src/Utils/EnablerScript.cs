using UnityEngine;
using System.Collections;

public class EnablerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void EnableOwner()
    {
        this.enabled = true;
    }

    public void DisableOwner()
    {
        this.enabled = false;
    }

   
}
