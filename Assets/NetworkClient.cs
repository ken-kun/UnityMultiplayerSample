﻿using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkMessages;
using NetworkObjects;
using System;
using System.Text;

public class NetworkClient : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public string serverIP;
    public ushort serverPort;
    //public GameObject playerCube;
    NetworkObjects.NetworkPlayer myCube;


    void Start ()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndPoint.Parse(serverIP,serverPort);
        m_Connection = m_Driver.Connect(endpoint);
        myCube = new NetworkObjects.NetworkPlayer();

        //myCube.cubePosition = new Vector3(0, 9, 0);

        //myCube.playerCube.transform.position = myCube.cubePosition();



        //InvokeRepeating("Position", 0.5f, 0.5f);
    }
    
    void SendToServer(string message){
        var writer = m_Driver.BeginSend(m_Connection);
        NativeArray<byte> bytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(message),Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    void OnConnect(){
        Debug.Log("We are now connected to the server");

        //// Example to send a handshake message:
        //HandshakeMsg m = new HandshakeMsg();
        //m.player.id = m_Connection.InternalId.ToString();
        //SendToServer(JsonUtility.ToJson(m));

        //PlayerUpdateMsg m = new PlayerUpdateMsg();
        //m.player.id = m_Connection.InternalId.ToString();
        //SendToServer(JsonUtility.ToJson(m));

        //Position();
        //myCube.playerCube.GetComponent<Transform>().position = new Vector3(myCube.cubePosition.x, myCube.cubePosition.y, myCube.cubePosition.z);

        
        myCube.playerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        myCube.playerCube.transform.position = new Vector3(0, 0, 0);

    }


    //[Serializable]
    //public class PositionMessage
    //{
    //    public string cmd;
    //    public Vector3 position;
    //}

    void Position()
    {
        PlayerUpdateMsg msg = new PlayerUpdateMsg();
        //msg.cmd = "posUpdate";
        msg.player.cubePosition.x = GetComponent<Transform>().position.x;
        msg.player.cubePosition.y = GetComponent<Transform>().position.y;
        msg.player.cubePosition.z = GetComponent<Transform>().position.z;
        SendToServer(JsonUtility.ToJson(msg));

    }

    
    void OnData(DataStreamReader stream){
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length,Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.cmd){
            case Commands.HANDSHAKE:
            HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
            Debug.Log("Handshake message received!");
            break;
            case Commands.PLAYER_UPDATE:
            PlayerUpdateMsg puMsg = JsonUtility.FromJson<PlayerUpdateMsg>(recMsg);
            Debug.Log("Player update message received!");
            break;
            case Commands.SERVER_UPDATE:
            ServerUpdateMsg suMsg = JsonUtility.FromJson<ServerUpdateMsg>(recMsg);
            Debug.Log("Server update message received!");
            break;
            default:
            Debug.Log("Unrecognized message received!");
            break;
        }
    }

    void Disconnect(){
        m_Connection.Disconnect(m_Driver);
        m_Connection = default(NetworkConnection);
    }

    void OnDisconnect(){
        Debug.Log("Client got disconnected from server");
        m_Connection = default(NetworkConnection);

        Destroy(myCube.playerCube);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    public void Movement()
    {
        

        ///Cube movement
        if (Input.GetKey("a"))
        {
            PlayerUpdateMsg mes = new PlayerUpdateMsg();
            mes.cmd = Commands.PLAYER_UPDATE;
            mes.player.id = m_Connection.InternalId.ToString();
            mes.player.cubePosition.x -= 1;
            mes.player.cubePosition = GetComponent<Transform>().position = new Vector3(myCube.cubePosition.x, myCube.cubePosition.y, myCube.cubePosition.z);
            SendToServer(JsonUtility.ToJson(mes));
            //playerCube.transform.Translate(-1,0,0);

            myCube.cubePosition.x -= 1;
            myCube.playerCube.transform.Translate(-1, 0, 0);
            

        }

        if (Input.GetKey("d"))
        {
            PlayerUpdateMsg mes = new PlayerUpdateMsg();
            mes.cmd = Commands.PLAYER_UPDATE;
            mes.player.id = m_Connection.InternalId.ToString();
            mes.player.cubePosition.x += 1;
            mes.player.cubePosition = GetComponent<Transform>().position = new Vector3(myCube.cubePosition.x, myCube.cubePosition.y, myCube.cubePosition.z);
            SendToServer(JsonUtility.ToJson(mes));
            //playerCube.transform.Translate(1, 0, 0);

            myCube.cubePosition.x += 1;
            myCube.playerCube.transform.Translate(1, 0, 0);
            

        }

        if (Input.GetKey("w"))
        {
            PlayerUpdateMsg mes = new PlayerUpdateMsg();
            mes.cmd = Commands.PLAYER_UPDATE;
            mes.player.id = m_Connection.InternalId.ToString();
            mes.player.cubePosition.y += 1;
            mes.player.cubePosition = GetComponent<Transform>().position = new Vector3(myCube.cubePosition.x, myCube.cubePosition.y, myCube.cubePosition.z);
            SendToServer(JsonUtility.ToJson(mes));
            //playerCube.transform.Translate(0, 1, 0);

            myCube.cubePosition.y += 1;
            myCube.playerCube.transform.Translate(0, 1, 0);
            

        }

        if (Input.GetKey("s"))
        {
            PlayerUpdateMsg mes = new PlayerUpdateMsg();
            mes.cmd = Commands.PLAYER_UPDATE;
            mes.player.id = m_Connection.InternalId.ToString();
            mes.player.cubePosition.y -= 1;
            mes.player.cubePosition = GetComponent<Transform>().position = new Vector3(myCube.cubePosition.x, myCube.cubePosition.y);
            SendToServer(JsonUtility.ToJson(mes));
            //playerCube.transform.Translate(0, -1, 0);

            myCube.cubePosition.y -= 1;
            myCube.playerCube.transform.Translate(0, -1, 0);
            

        }

        //PlayerUpdateMsg msg = new PlayerUpdateMsg();
        //msg.cmd = Commands.PLAYER_UPDATE;
        //msg.player.id = m_Connection.InternalId.ToString();
        //msg.player.cubePosition.x = myCube.cubePosition.x;
        //msg.player.cubePosition.y = myCube.cubePosition.y;
        //msg.player.cubePosition.z = myCube.cubePosition.z;
        //SendToServer(JsonUtility.ToJson(msg));

        Debug.Log(myCube.cubePosition);


    }


    void Update()
    {
        Movement();
        



        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        cmd = m_Connection.PopEvent(m_Driver, out stream);
        while (cmd != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                OnConnect();
                    
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                OnDisconnect();
            }

            cmd = m_Connection.PopEvent(m_Driver, out stream);
        }
    }
}