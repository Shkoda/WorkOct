using UnityEngine;
using System.Collections;

public class SkylineManager : MonoBehaviour {
    public Transform prefab;
    public int numberOfObjects;
    public Vector3 startPosition;

    private Vector3 nextPosition;

    void Start()
    {
        nextPosition = startPosition;
        for (int i = 0; i < numberOfObjects; i++)
        {
            Transform o = (Transform)Instantiate(prefab);
            o.localPosition = nextPosition;
            nextPosition.x += o.localScale.x;
        }
    }
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
