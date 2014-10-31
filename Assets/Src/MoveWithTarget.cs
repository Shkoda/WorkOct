using UnityEngine;
using System.Collections;

public class MoveWithTarget : MonoBehaviour
{
    public GameObject target;
    public double backlash;

    public bool userRalativeStartPosition;

    private Vector3 PreviousTargetPosition;
    // Use this for initialization
	void Start ()
	{
	    PreviousTargetPosition = target.transform.position;
	    if (userRalativeStartPosition)
	    {
            this.transform.position = RelativeCameraPosition(PreviousTargetPosition);
	    }

	}
	
	// Update is called once per frame
	void Update ()
	{
//        this.transform.position = target.transform.position + new Vector3(0, 0, -10);
	    Vector3 currentTargetPosition = target.transform.position;
	    Vector3 cameraPosition = this.transform.position;

        if (Vector3.Distance(cameraPosition, currentTargetPosition) > backlash)
	    {
	        Vector3 dif = currentTargetPosition - PreviousTargetPosition;
	        Vector3 nextPosition = this.transform.position + new Vector3(dif.x, dif.y, 0);

            this.transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime);

	    }

	    PreviousTargetPosition = currentTargetPosition;
	}

    private Vector3 RelativeCameraPosition(Vector3 playerPosition)
    {
        return new Vector3(playerPosition.x+1, playerPosition.y+2, playerPosition.z-10);
    }

   
}
