﻿using System.Collections.Generic;
using System.Linq;
using Assets.Src.Net.Envelopes.Client;
using UnityEngine;
using System.Collections;
using WorkOct.Protocol;

public class RoomManager
{
    private Dictionary<int, RoomInfo> rooms = new Dictionary<int, RoomInfo>();


    public RoomManager()
    {
        CNewPlayerEnvelope.HandleRoomList += setRooms;
        CGetRoomsEnvelope.HandleRoomList += setRooms;
    }

    public void setRooms(List<RoomInfo> list)
    {
        Debugger.Log("Setting new room list");
        rooms = list.ToDictionary(room => room.id, room => room);
        list.ForEach(room => Debugger.Log(room));
    }

    public Dictionary<int, RoomInfo> getRooms()
    {
        return rooms;
    }
}