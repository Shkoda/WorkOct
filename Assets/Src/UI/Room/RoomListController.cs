using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

    private float TopX, TopY, TopZ;
    private float InfoHeight, OffsetBetweenInfos;
    private float DefaultRoomPanelHeight;

    private Dictionary<int, GameObject> InstantinatedRooms = new Dictionary<int, GameObject>();

    public void SetRoomPanelEnabled(bool enabled)
    {
        RoomListPanel.SetActive(enabled);
        RoomListPanel.GetComponent<Animator>().SetBool("ShowRoomList", enabled);
    }

    // Use this for initialization
    private void Start()
    {
//        TopX = 5;
//        TopY = -12;
//        TopZ = 0;
//
//        InfoHeight = RoomInfoPrefab.GetComponent<RectTransform>().sizeDelta.y;
//        OffsetBetweenInfos = 5;
//        DefaultRoomPanelHeight = 331;

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
        RoomManager roomManager = OctClient.CurrentGame.RoomManager;
        if (roomManager.RoomsUpdated)
        {
            roomManager.RoomsUpdated = false;
            destroyInstantinatedRooms();
            List<RoomInfo> rooms = roomManager.getRooms().Values.ToList();
            InstantinateRoomList(rooms);
        }
      
    }


    private void destroyInstantinatedRooms()
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
        var info = (GameObject) Instantiate(RoomInfoPrefab);

        var rectTransform = info.GetComponent<RectTransform>();
        rectTransform.parent = ParentPane.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;

        var infoHolder = info.GetComponent<RoomInfoHolder>();
        infoHolder.RoomNameField.text = "#" + roomInfo.id;
        infoHolder.InsideField.text = roomInfo.players + "/" + roomInfo.capacity;
        infoHolder.StatusField.text = roomInfo.state.ToString();

        InstantinatedRooms.Add(roomInfo.id, info);
    }
}