using System;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using WorkOct.Protocol;
using Random = System.Random;


public class RoomListController : MonoBehaviour
{
    public GameObject ParentPane;
    public GameObject RoomInfoPrefab;
    public RectTransform FirstPanelPositionObject;

    private float TopX, TopY, TopZ;
    private float InfoHeight, OffsetBetweenInfos;


    // Use this for initialization
    private void Start()
    {
        TopX = 9;
        TopY = -12;
        TopZ = 0;

        InfoHeight = RoomInfoPrefab.GetComponent<RectTransform>().sizeDelta.y;
        OffsetBetweenInfos = 5;
        InstantinateTestRoomList();
    }


    // Update is called once per frame
    private void Update()
    {
    }

    public void InstantinateTestRoomList()
    {
        int roomNumber = 10;

        UpdateParentPanelSize(roomNumber);

        Random random = new Random();


        for (int i = 0; i < roomNumber; i++)
        {
            int roomId = i + 1;
            int playersInside = random.Next(5);
            int capacity = playersInside + random.Next(1, 5);
            string status = "Waiting for " + (capacity - playersInside) + " players";


            AddRoomInfo(AnchoredPosition(i), roomId, playersInside, capacity, status);
        }
    }

    private void UpdateParentPanelSize(int roomNumber)
    {
        var parentRectTransform = ParentPane.GetComponent<RectTransform>();
        parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x,
            roomNumber*(InfoHeight + OffsetBetweenInfos)-TopY);
    }

    private Vector3 AnchoredPosition(int infoNumber)
    {
        float x = TopX;
        float y = TopY - infoNumber*(InfoHeight + OffsetBetweenInfos);
        float z = TopZ;
        return new Vector3(x, y, z);
    }

    private void AddRoomInfo(Vector3 anchoredPosition, int roomId, int playersInside, int capacity, string status)
    {
        var info = (GameObject) Instantiate(RoomInfoPrefab);

        var rectTransform = info.GetComponent<RectTransform>();
        rectTransform.parent = ParentPane.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;

        var infoHolder = info.GetComponent<RoomInfoHolder>();
        infoHolder.RoomNameField.text = "#" + roomId;
        infoHolder.InsideField.text = playersInside + "/" + capacity;
        infoHolder.StatusField.text = status;
    }
}