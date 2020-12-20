using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Singleton { get; private set; }

    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private GameObject Board;
    [SerializeField] private SpriteRenderer boardO;
    [SerializeField] private SpriteRenderer boardX;
    private SpriteRenderer _boardSprite;
    private Animator _boardAnimator;

    private Camera _camera;

    public TicTacToeSpaceType Player { get; private set; }

    private TicTacToeSpaceScript[][] _grid;

    private TicTacToeSpaceScript[] _spaces;

    public decimal P1Score { get; private set; } = 0;

    public decimal P2Score { get; private set; } = 0;

    private bool _allowedToPlay = true;

    public TicTacToeSpaceType CurrentPlayer { get; private set; } = TicTacToeSpaceType.O;

    private static readonly int Zoomout = Animator.StringToHash("Zoomout");

    private void Awake()
    {
        Singleton = this;
        
        if (PhotonNetwork.IsMasterClient)
            Player = TicTacToeSpaceType.O;
        else
            Player = TicTacToeSpaceType.X;

        _boardSprite = Board.GetComponent<SpriteRenderer>();
        _boardAnimator = Board.GetComponent<Animator>();
        
        _camera = GetComponent<Camera>();
        _grid = new[]
        {
            new TicTacToeSpaceScript[] {getSpace(0), getSpace(1), getSpace(2)},
            new TicTacToeSpaceScript[] {getSpace(3), getSpace(4), getSpace(5)},
            new TicTacToeSpaceScript[] {getSpace(6), getSpace(7), getSpace(8)}
        };
        _spaces = new[]
        {
            getSpace(0),
            getSpace(1),
            getSpace(2),
            getSpace(3),
            getSpace(4),
            getSpace(5),
            getSpace(6),
            getSpace(7),
            getSpace(8)
        };
    }

    private TicTacToeSpaceScript getSpace(int num) =>
        GameObject.Find("grid_" + num).GetComponent<TicTacToeSpaceScript>();

    private void Update()
    {
        if (!_allowedToPlay) return;
        if (Player != CurrentPlayer) return;
        if (!Input.GetMouseButtonDown(0)) return;
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider == null) return;
        var space = hit.collider.GetComponent<TicTacToeSpaceScript>();

        if (!space) return;
        Debug.Log("hit");
        if (space.Type != TicTacToeSpaceType.Empty) return;
        Debug.Log("hit empty");
        space.photonView.RPC(nameof(space.SetValue), RpcTarget.All, Player, true);
        photonView.RPC(nameof(SpaceChanged), RpcTarget.MasterClient);
        switch (Player)
        {
            case TicTacToeSpaceType.O:
                photonView.RPC(nameof(SwitchTurn), RpcTarget.All, TicTacToeSpaceType.X);
                break;
            case TicTacToeSpaceType.X:
                photonView.RPC(nameof(SwitchTurn), RpcTarget.All, TicTacToeSpaceType.O);
                break;
        }
    }

    [PunRPC]
    private void SpaceChanged()
    {
        audioSource.Play();
        switch (OnSpaceChanged())
        {
            case TicTacToeSpaceType.Empty:
                break;
            case TicTacToeSpaceType.O:
                photonView.RPC(nameof(Win), RpcTarget.All, TicTacToeSpaceType.O);
                break;
            case TicTacToeSpaceType.X:
                photonView.RPC(nameof(Win), RpcTarget.All, TicTacToeSpaceType.X);
                break;
        }
    }

    [PunRPC]
    private void SwitchTurn(TicTacToeSpaceType newTurn)
    {
        CurrentPlayer = newTurn;
    }

    [PunRPC]
    private void Win(TicTacToeSpaceType winner)
    {
        _allowedToPlay = false;
        StartCoroutine(WinBoard(1.0f, 0f, 1f, winner));
        foreach (var space in _spaces)
        {
            space.StartFade();
        }
        
        switch (winner)
        {
            case TicTacToeSpaceType.O:
                if (P1Score >= 0)
                {
                    P1Score += 1;
                    P1Score /= 2;
                    if (P1Score >= 1)
                        P1Score = -1;
                }

                photonView.RPC(nameof(SetScore), RpcTarget.All, P1Score, TicTacToeSpaceType.O);
                break;
            case TicTacToeSpaceType.X:
                if (P2Score >= 0)
                {
                    P2Score += 1;
                    P2Score /= 2;
                    if (P2Score >= 0.99999999M)
                        P2Score = -1;
                }

                photonView.RPC(nameof(SetScore), RpcTarget.All, P2Score, TicTacToeSpaceType.X);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(winner), winner, null);
        }
    }

    private IEnumerator WinBoard(float startingValue, float endingValue, float time, TicTacToeSpaceType winner)
    {
        if (startingValue < endingValue)
            for (var i = startingValue; i < endingValue; i += Time.deltaTime/time)
            {
                var color = _boardSprite.color;
                color.a = i;
                _boardSprite.color = color;
                
                yield return null;
            }
        else
            for (var i = startingValue; i > endingValue; i -= Time.deltaTime/time)
            {
                var color = _boardSprite.color;
                color.a = i;
                _boardSprite.color = color;
                
                yield return null;
            }
        
        _boardAnimator.SetTrigger(Zoomout);
        
        yield return null;
        var bColor = _boardSprite.color;
        bColor.a = 1f;
        _boardSprite.color = bColor;
        if (winner == TicTacToeSpaceType.O)
            boardO.color = bColor;
        else
            boardX.color = bColor;
            
        yield return new WaitForSeconds(1.2f);
        bColor.a = 0f;
        boardO.color = bColor;
        boardX.color = bColor;
        
        _grid[0][0].SetValue(winner, false);
        _allowedToPlay = true;
    }

    [PunRPC]
    private void SetScore(decimal newScore, TicTacToeSpaceType player)
    {
        switch (player)
        {
            case TicTacToeSpaceType.O:
                P1Score = newScore;
                break;
            case TicTacToeSpaceType.X:
                P2Score = newScore;
                break;
        }
    } 

    private TicTacToeSpaceType OnSpaceChanged()
    {
        // check each row
        for (int i = 0; i < 3; i++)
        {
            var type = _grid[i][0].Type;
            if (_grid[i][1].Type != type) continue;
            if (_grid[i][2].Type != type) continue;
            return type;
        }
        
        // check each column
        for (int i = 0; i < 3; i++)
        {
            var type = _grid[0][i].Type;
            if (_grid[1][i].Type != type) continue;
            if (_grid[2][i].Type != type) continue;
            return type;
        }
        
        // check each diagonal
        // Top left
        var diagonalType = _grid[0][0].Type;
        if (_grid[1][1].Type != diagonalType) goto TopRight;
        if (_grid[2][2].Type != diagonalType) goto TopRight;
        return diagonalType;
            
        // Top right
        TopRight:
            diagonalType = _grid[0][2].Type;
            if (_grid[1][1].Type != diagonalType) return TicTacToeSpaceType.Empty;
            if (_grid[2][0].Type != diagonalType) return TicTacToeSpaceType.Empty;
            return diagonalType;
    }
}
