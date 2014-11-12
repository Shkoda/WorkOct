using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomInfoHolder : MonoBehaviour
{
    public Text RoomNameField;
    public Text InsideField;
    public Text StatusField;

    public int Id { get; set; }
    public RoomListController RoomListController { get; set; }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void JoinRoom()
    {
        Debugger.Log("joining room "+Id);
//        RoomListController.SetRoomPanelEnabled(false);
        RoomListController.HideRoomList();
    }
}