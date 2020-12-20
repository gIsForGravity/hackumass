using System;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameVersion = "0";

    [RuntimeInitializeOnLoadMethod]
    private static void Startup()
    {
        SceneManager.LoadScene((int) Scenes.Menu);
    }
    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonPeer.RegisterType(typeof(Decimal), Byte.MaxValue,
            customObject => Encoding.ASCII.GetBytes(((decimal) customObject).ToString("N8")), 
            customObject => Convert.ToDecimal(Encoding.ASCII.GetString(customObject)));
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        // joined room
        if (PhotonNetwork.IsMasterClient)
            SceneManager.LoadScene((int) Scenes.Loading);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }
}
