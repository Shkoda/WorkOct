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
        TopX = FirstPanelPositionObject.transform.position.x;
        TopY = FirstPanelPositionObject.transform.position.y;
        TopZ = FirstPanelPositionObject.transform.position.z;
        InfoHeight = FirstPanelPositionObject.sizeDelta.y;
        OffsetBetweenInfos = 5;
        InstantinateTestRoomList();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void InstantinateTestRoomList()
    {
        Debugger.Log("------ call RoomListController.InstantinateTestRoomList() " +
                     DateTime.Now.ToString("HH:mm:ss.fff tt"));
        Random random = new Random();

        for (int i = 0; i < 10; i++)
        {
            int roomId = i + 1;
            int playersInside = random.Next(5);
            int capacity = playersInside + random.Next(1, 5);
            string status = "Waiting for " + (capacity - playersInside) + " players";


            AddRoomInfo(TopLeftCorner(i), roomId, playersInside, capacity, status);
        }
    }

    private Vector3 TopLeftCorner(int infoNumber)
    {
        float x = TopX;
        float y = TopY - infoNumber*(InfoHeight + OffsetBetweenInfos);
        float z = TopZ;
        return new Vector3(x, y, z);
    }

    private void AddRoomInfo(Vector3 topLeftCorner, int roomId, int playersInside, int capacity, string status)
    {
        Debugger.Log("Generated room #" + roomId + ", " + playersInside + "/" + capacity + ", status : " + status);
        var info = (GameObject) Instantiate(RoomInfoPrefab, topLeftCorner, Quaternion.identity);


        info.transform.parent = ParentPane.transform;

        var recTransform = info.GetComponent<RectTransform>();
        recTransform.anchoredPosition.Set(FirstPanelPositionObject.anchoredPosition.x,
            FirstPanelPositionObject.anchoredPosition.y);


//        recTransform.anchoredPosition.x

        var infoHolder = info.GetComponent<RoomInfoHolder>();
        infoHolder.RoomNameField.text = "#" + roomId;
        infoHolder.InsideField.text = playersInside + "/" + capacity;
        infoHolder.StatusField.text = status;
    }
}