using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;

public class SocketIOManager : MonoBehaviour
{
    public static SocketIOManager Instance;

    [SerializeField] private string Address = "";
    private SocketIO socketManager;

    private void Awake()
    {
        Instance = this;
        UnityThread.initUnityThread();
    }

    private void Start()
    {
        socketManager = new SocketIO(Address);
        
        socketManager.OnAny((name, response) =>
        {
            Debug.Log(name);
            Debug.Log(response);
        });
        
        socketManager.On("OtherPlayerConnect", PlayerConnected);
        socketManager.On("OtherPlayerMove", OnOtherPlayerMove);
        socketManager.On("OtherPlayerDisconnected", (response => {}));
        socketManager.OnConnected += OnConnected;
        socketManager.OnDisconnected += (sender, s) => OnDisconnected();

        socketManager.ConnectAsync();
    }

    public void SendTransformAndAnimationData(Transform trans, int animNumber)
    {
        var copyTransData = new PacketTransformData();
        copyTransData.position = trans.position;
        copyTransData.rotation = trans.rotation;
        copyTransData.scale = trans.localScale;
        copyTransData.animationNumber = animNumber;

        string sendJson = JsonUtility.ToJson(copyTransData);
        socketManager.EmitAsync("PlayerMove", sendJson);
        print("Send Player Position");
    }

    private void OnOtherPlayerMove(SocketIOResponse obj)
    {
        UnityThread.executeInUpdate(() =>
        {
            ReadTransformData(obj.GetValue(0).ToString(), obj.GetValue(1).ToString());
        });
    }

    public void ReadTransformData(string playerId, string jsonData)
    {
        var data = JsonConvert.DeserializeObject<PacketTransformData>(jsonData);
        print(playerId+"이동함. "+jsonData);
        gameManager.SpawnPlayer(playerId);
        foreach (var gO in gameManager.playerCharacters)
        {
            if (gO.gameObject.name == playerId)
            {
                gO.transform.position = data.position;
                gO.transform.rotation = data.rotation;
                gO.transform.localScale = data.scale;
            }
        }
    }

    private void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected! Socket.IO");
    }

    private void OnDisconnected()
    {
        Debug.Log("Disconnected!");
    }

    public GameManager gameManager;
    private void PlayerConnected(SocketIOResponse obj)
    {
        UnityThread.executeInUpdate(() =>
        {
            var playerId = obj.GetValue(0).ToString();
            Debug.Log("Player Connected!! : " + playerId);
            gameManager.SpawnPlayer(playerId);
        });
    }

    private void OnDestroy()
    {
        if (socketManager != null)
        {
            socketManager.DisconnectAsync();
            socketManager = null;
        }
    }
}