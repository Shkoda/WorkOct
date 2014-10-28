using UnityEngine;
using System.Collections;

public class BGScroller : MonoBehaviour {

//    public GameObject Background;
//    public GameObject MovingTarget;


    public float scrollSpeed ;
    public float tileSizeZ ;

    private Vector3 startPosition;

    void Start()
    {

        startPosition =this. transform.position;

    }

    void Update()
    {

//
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
        this.transform.position = startPosition + new Vector3(1,0,0) * newPosition;
        Debugger.Log("forvard " + Vector3.forward * newPosition);
//
        Debugger.Log("new position = " + newPosition + ",  this.transform.position = "+this.transform.position);

//        this.transform.position+= new Vector3(0.01f,0,0);
    }
}
