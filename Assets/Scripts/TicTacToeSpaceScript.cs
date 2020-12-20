using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TicTacToeSpaceScript : MonoBehaviourPun
{
    [SerializeField] private GameObject xSprite;
    [SerializeField] private GameObject oSprite;

    private SpriteRenderer _xRenderer;
    private SpriteRenderer _oRenderer;
    //[SerializeField] private int row;
    //[SerializeField] private int column;
    
    public TicTacToeSpaceType Type { get; private set; }
    //public int Row => row;
    //public int Column => column;

    private void Awake()
    {
        _xRenderer = xSprite.GetComponent<SpriteRenderer>();
        _oRenderer = oSprite.GetComponent<SpriteRenderer>();
        
        SetValue(TicTacToeSpaceType.Empty);
    }

    [PunRPC]
    public void SetValue(TicTacToeSpaceType type, bool fade = true)
    {
        switch (type)
        {
            case TicTacToeSpaceType.Empty:
                Type = TicTacToeSpaceType.Empty;
                if (fade)
                {
                    StartCoroutine(FadeSprite(_oRenderer, 1f, 0f, 1f, oSprite));
                    StartCoroutine(FadeSprite(_xRenderer, 1f, 0f, 1f, xSprite));
                }
                else
                {
                    oSprite.SetActive(false);
                    xSprite.SetActive(false);
                }

                break;
            case TicTacToeSpaceType.O:
                Type = TicTacToeSpaceType.O;
                xSprite.SetActive(false);
                oSprite.SetActive(true);
                if (fade)
                    StartCoroutine(FadeSprite(_oRenderer, 0f, 1f, 0.5f));
                else
                {
                    var color = _oRenderer.color;
                    color.a = 1f;
                    _oRenderer.color = color;
                }

                break;
            case TicTacToeSpaceType.X:
                Type = TicTacToeSpaceType.X;
                xSprite.SetActive(true);
                oSprite.SetActive(false);
                if (fade)
                    StartCoroutine(FadeSprite(_xRenderer, 0f, 1f, 0.5f));
                else
                {
                    var color = _xRenderer.color;
                    color.a = 1f;
                    _xRenderer.color = color;
                }
                break;
        }
    }

    public void StartFade()
    {
        StopAllCoroutines();
        SetValue(TicTacToeSpaceType.Empty);
    }
    
    private static IEnumerator FadeSprite(SpriteRenderer sprite, float startingValue, float endingValue, float time, System.Object toDisable = null)
    {
        if (startingValue < endingValue)
            for (var i = startingValue; i < endingValue; i += Time.deltaTime/time)
            {
                var color = sprite.color;
                color.a = i;
                sprite.color = color;
                
                yield return null;
            }
        else
            for (var i = startingValue; i > endingValue; i -= Time.deltaTime/time)
            {
                var color = sprite.color;
                color.a = i;
                sprite.color = color;
                
                yield return null;
            }
        
        if (toDisable != null)
            ((GameObject) toDisable).SetActive(false);
    }
}
