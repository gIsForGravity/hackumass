using Photon.Pun;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private bool started = false;
    
    private void Update()
    {
        if (started) return;
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount != 2) return;
        PhotonNetwork.LoadLevel((int) Scenes.Game);
        started = true;
    }
}
