using UnityEngine;
using System.Collections;

public class RoomDetailsController : MonoBehaviour
{
    public GameObject RoomDetailsPane;

    private Animator Animator;
    
	// Use this for initialization
	void Start ()
	{
	    this.Animator = RoomDetailsPane.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetRoomPanelEnabled(bool enabled)
    {
        RoomDetailsPane.SetActive(enabled);
    }

    public void ShowRoomList()
    {
        Animator.SetBool("ShowBattleInfo", true);
    }

    public void HideRoomList()
    {
        Animator.SetBool("ShowBattleInfo", false);
    }



}
