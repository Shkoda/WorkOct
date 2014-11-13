using Assets.Src.Utils;
using UnityEngine;
using System.Collections;

public class RoomDetailsController : MonoBehaviour
{
    public GameObject RoomDetailsPane;

    private Animator Animator;

    [System.ComponentModel.DefaultValue(Constants.Undefined)]
    public int SelectedRoomId { get; set; }
    
	// Use this for initialization
	void Start ()
	{
	    this.Animator = RoomDetailsPane.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Enable()
    {
        RoomDetailsPane.SetActive(true);
    }

    public void Disable()
    {
        RoomDetailsPane.SetActive(true);
    }


    public void SetRoomPanelEnabled(bool enabled)
    {
        RoomDetailsPane.SetActive(enabled);
    }



    public void Show()
    {
        Animator.SetBool("ShowBattleInfo", true);
    }

    public void Hide()
    {
        Animator.SetBool("ShowBattleInfo", false);
    }



}
