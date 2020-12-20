using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardScript : MonoBehaviour
{
    [SerializeField] private Text oText;
    [SerializeField] private Text xText;
    [SerializeField] private Text confirmation;

    private void Update()
    {
        OText();
        XText();

        if (GameManager.Singleton.Player == GameManager.Singleton.CurrentPlayer)
            confirmation.text = "It is your turn.";
        else
            confirmation.text = "It is your opponent's turn.";
    }

    private void OText()
    {
        if (GameManager.Singleton.P1Score >= 0)
            oText.text = GameManager.Singleton.P1Score.ToString("N8");
        else
            oText.text = "0.99999999...";
    }

    private void XText()
    {
        if (GameManager.Singleton.P2Score >= 0)
            xText.text = GameManager.Singleton.P2Score.ToString("N8");
        else
            xText.text = "0.99999999...";
    }
}
