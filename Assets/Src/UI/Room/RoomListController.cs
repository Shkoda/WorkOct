using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Assets.Src.Utils;
using UnityEngine;
using System.Collections;
using WorkOct.Protocol;
using Random = System.Random;


public class RoomListController : MonoBehaviour
{
    public GameObject RoomListPanel;
    public GameObject ParentPane;
    public GameObject RoomInfoPrefab;
    public RectTransform FirstPanelPositionObject;

    public RoomDetailsController RoomDetailsController;

    private float TopX, TopY, TopZ;
    private float InfoHeight, OffsetBetweenInfos;
    private float DefaultRoomPanelHeight;

    private Dictionary<int, GameObject> InstantinatedRooms = new Dictionary<int, GameObject>();

    private Animator Animator;


    [System.ComponentModel.DefaultValue(Constants.Undefined)] private int selectedRoomId;

    public int SelectedRoomId
    {
        get { return selectedRoomId; }
        set
        {
            selectedRoomId = value;
            RoomDetailsController.SelectedRoomId = value;
        }
    }


    public void Enable()
    {
        RoomListPanel.SetActive(true);
    }

    public void Disable()
    {
        RoomListPanel.SetActive(true);
    }


    public void ShowRoomList()
    {
        Animator = RoomListPanel.GetComponent<Animator>();
        Animator.SetBool("ShowRoomList", true);
    }

    public void HideRoomList()
    {
        Animator.SetBool("ShowRoomList", false);
    }

    public void ShowRoomDetails()
    {
        RoomDetailsController.Enable();
        RoomDetailsController.Show();
    }

    public void DisableAndShowRoomDetails()
    {
        Disable();
        ShowRoomDetails();
    }


    // Use this for initialization
    private void Start()
    {
        Animator = RoomListPanel.GetComponent<Animator>();
    }

    private void Awake()
    {
        TopX = 5;
        TopY = -12;
        TopZ = 0;

        InfoHeight = RoomInfoPrefab.GetComponent<RectTransform>().sizeDelta.y;
        OffsetBetweenInfos = 5;
        DefaultRoomPanelHeight = 331;
    }


    // Update is called once per frame
    private void Update()
    {
        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        RoomManager roomManager = OctClient.CurrentGame.RoomManager;
//        Debugger.Log("RoomListController.RefreshRoomList() roomManager == " + roomManager);
//        Debugger.Log("RoomListController.RefreshRoomList() roomManager.RoomsUpdated == " + roomManager.RoomsUpdated);
        List<RoomInfo> rooms = roomManager.getRooms().Values.ToList();

//        rooms.ForEach(room => Debugger.Log("RoomListController.RefreshRoomList() room " + room.id));

        if (roomManager.RoomsUpdated)
        {
            roomManager.RoomsUpdated = false;
            DestroyInstantinatedRooms();

            Debugger.Log("Refreshing room list..." + rooms);
            InstantinateRoomList(rooms);
        }
    }


    private void DestroyInstantinatedRooms()
    {
        InstantinatedRooms.Values.ToList().ForEach(Destroy);
        InstantinatedRooms.Clear();
    }


    public void InstantinateRoomList(List<RoomInfo> rooms)
    {
        int roomNumber = rooms.Count;
        UpdateParentPanelSize(roomNumber);
        for (int i = 0; i < roomNumber; i++)
        {
            AddRoomInfo(AnchoredPosition(i), rooms[i]);
        }
    }

    private void UpdateParentPanelSize(int roomNumber)
    {
        var parentRectTransform = ParentPane.GetComponent<RectTransform>();
        float height = Math.Max(DefaultRoomPanelHeight, roomNumber*(InfoHeight + OffsetBetweenInfos) - TopY);
        parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x,
            height);
    }

    private Vector3 AnchoredPosition(int infoNumber)
    {
        float x = TopX;
        float y = TopY - infoNumber*(InfoHeight + OffsetBetweenInfos);
        float z = TopZ;
        return new Vector3(x, y, z);
    }

    private void AddRoomInfo(Vector3 anchoredPosition, RoomInfo roomInfo)
    {
//        Debugger.Log("RoomListController.AddRoomInfo() room " + roomInfo.id);
        var info = (GameObject) Instantiate(RoomInfoPrefab);

        var rectTransform = info.GetComponent<RectTransform>();
        rectTransform.parent = ParentPane.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;

        var infoHolder = info.GetComponent<RoomInfoHolder>();
        infoHolder.Id = roomInfo.id;
        infoHolder.RoomNameField.text = "#" + roomInfo.id;
        infoHolder.InsideField.text = roomInfo.players.Count + "/" + roomInfo.capacity;
        infoHolder.StatusField.text = roomInfo.state.ToString();
        infoHolder.RoomListController = this;

        info.SetActive(true);
        InstantinatedRooms.Add(roomInfo.id, info);
    }
}