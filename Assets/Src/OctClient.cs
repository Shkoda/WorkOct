using UnityEngine;
using System.Collections;

public class OctClient : MonoBehaviour {

	// Use this for initialization
	void Start () {

        if (!this.gameObject.GetComponent<Debugger>())
        {
            this.gameObject.AddComponent<Debugger>();
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void Reset()
    {
        throw new System.NotImplementedException();
    }
}
