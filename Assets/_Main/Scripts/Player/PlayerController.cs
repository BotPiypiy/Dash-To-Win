using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private PlayerParameters _playerParameters;
    [SerializeField] private InputParameters _inputParameters;
    [SerializeField] private Camera _playerCamera;

    private Camera _mainCamera;
    private PlayerState _state;
    private Vector3 _moveDirection;
    private bool canMove;

    public PlayerState State { get; private set; }
    public Rigidbody Rb { get; private set; }
    public MeshRenderer Mr { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Mr = GetComponent<MeshRenderer>();
        _state = new DefaultState(this, _playerParameters);
    }

    private void Start()
    {
        if (isClient && isLocalPlayer)
        {
            NetManager.LocalPlayer = this;
            gameObject.name = NetManager.Instance.UIManager.GetNickname();
            _mainCamera = Camera.main;
            OnGameStart();
            NetManager.GameOver += GameOverHandler;
        }
    }

    private void OnGameStart()
    {
        canMove = true;
        NetManager.Instance.ScoreBoard.CmdAddPlayer(gameObject.name);
        _mainCamera.gameObject.SetActive(false);
        _playerCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDestroy()
    {
        if (isClient && isLocalPlayer)
        {
            OnGameOver();

            NetManager.GameOver -= GameOverHandler;
        }
    }

    private void OnGameOver()
    {
        canMove = false;
        _playerCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private async void GameOverHandler(string name)
    {
        transform.position = NetManager.Instance.GetStartPosition().position;
        OnGameOver();
        await Task.Delay((int)(NetManager.Instance.GameParameters.GameOverDelay * 1000f));
        OnGameStart();
    }

    public void SetNewState(PlayerState newState)
    {
        if (isClient && isLocalPlayer)
        {
            CmdSetNewState(newState.GetState(), newState.TryGetDirection());
            if (_state.GetState() != newState.GetState())
            {
                _state.OnExit();
                _state = newState;
                _state.OnEnter();
            }
        }
    }

    public void Disconnect()
    {
        if (isClient && isLocalPlayer)
        {
            NetManager.Instance.ScoreBoard.CmdDeletePlayer(gameObject.name);
            Destroy(gameObject);
        }
    }

    [Command]
    private void CmdSetNewState(PlayerStates state, Vector3 dir)
    {
        PlayerState newState;
        PlayerState.CreateState(out newState, this, _playerParameters, state, dir);
        RpcSetNewState(state, newState.TryGetDirection());

        if (_state.GetState() != newState.GetState())
        {
            _state.OnExit();
            _state = newState;
            _state.OnEnter();
        }
    }

    [ClientRpc]
    private void RpcSetNewState(PlayerStates state, Vector3 dir)
    {
        PlayerState newState;
        PlayerState.CreateState(out newState, this, _playerParameters, state, dir);

        if (_state.GetState() != newState.GetState())
        {
            _state.OnExit();
            _state = newState;
            _state.OnEnter();
        }
    }

    public void IncScore()
    {
        if (isClient && isLocalPlayer)
        {
            NetManager.Instance.ScoreBoard.IncScore(gameObject.name);
        }
    }

    private void Update()
    {
        if (isClient && isLocalPlayer)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(_inputParameters.Pause))
        {
            SwitchCursor();
        }

        if (Cursor.lockState == CursorLockMode.Locked && canMove)
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            transform.Rotate(Vector3.up, mouseX * _inputParameters.MouseSensevity * Time.deltaTime);

            _moveDirection = Vector3.zero;
            _moveDirection = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;

            if (_moveDirection != Vector3.zero)
            {
                _state.Move(_moveDirection, Input.GetKeyDown(_inputParameters.Dash));
            }
        }
    }

    private void SwitchCursor()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public bool TryDie()
    {
        if (_state is DeadState)
        {
            return false;
        }

        Die();
        return true;
    }

    private void Die()
    {
        SetNewState(new DeadState(this, _playerParameters));
    }

    private void OnCollisionEnter(Collision collision)
    {
        _state.OnCollisionEnter(collision);
    }
}
