using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using System;

public enum ClientToServerID
{
    name = 1,
    towerSpawn,
    mobSpawn,
    points
}

public enum ServerToClientID
{
    name = 1,
    towerSpawn,
    mobSpawn,
    points,
    resourceSpawn
}

public class NetworkManager : MonoBehaviour
{
    /*
    We want to make sure there can be only ONE instance of our network manager
    We are creating a private static instance of our NetworkManager and a public static Property to control it
    */
    private static NetworkManager _networkmanagerInstance;
    public static NetworkManager NetworkManagerInstance
    {
        //Property Read is the instance, public by default
        get => _networkmanagerInstance;
        //private means only this instance of the class can access set
        private set
        {
            //set the instance to the value if the instance is null
            if (_networkmanagerInstance == null)
            {
                _networkmanagerInstance = value;
            }
            //if it is not null, check if the value is stored as the static instance
            else if (_networkmanagerInstance != value)
            {
                //if not, throw a warning and destroy that instance

                //$ is to identify the string as containing an interpolated value
                Debug.LogWarning($"{nameof(NetworkManager)} instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    public Client GameClient { get; private set; }

    [SerializeField] private ushort s_port;
    [SerializeField] private string s_ip;

    private void Awake()
    {
        //when the object that this is attached to in game initialises, try to set the instance to this
        NetworkManagerInstance = this;
    }

    private void Start()
    {
        //Logs what the network is doing
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        //Create new client 
        GameClient = new Client();
        //connect to a server
        GameClient.Connected += DidConnect;
        //display if connection failed
        GameClient.ConnectionFailed += FailedToConnect;
        //When the client disconnects from the server
        GameClient.Disconnected += DidDisconnect;
    }

    void DidConnect(object sender, EventArgs e)
    {
        //UIManager.UIManagerInstance.SendName();
    }

    void FailedToConnect(object sender, EventArgs e)
    {
        //UIManager.UIManagerInstance.BackToMain();
    }

    void DidDisconnect(object sender, EventArgs e)
    {
        //UIManager.UIManagerInstance.BackToMain();
    }

    void FixedUpdate()
    {
        GameClient.Tick();
    }

    private void OnApplicationQuit()
    {
        GameClient.Disconnect();
    }

    public void Connect()
    {
        //Connect to the server
        GameClient.Connect($"{s_ip}:{s_port}");
    }
}
