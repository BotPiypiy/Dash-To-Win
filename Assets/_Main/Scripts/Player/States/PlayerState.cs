using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PlayerState
{
    protected PlayerParameters _playerParameters;
    protected PlayerController _player;
    protected PlayerStates _state;

    public PlayerState(PlayerController player, PlayerParameters playerParameters)
    {
        _playerParameters = playerParameters;
        _player = player;
    }

    public static void CreateState(out PlayerState newState, PlayerController player, PlayerParameters playerParameters, PlayerStates state, Vector3 direction)
    {
        switch (state)
        {
            case PlayerStates.Default:
                {
                    newState = new DefaultState(player, playerParameters);
                    break;
                }
            case PlayerStates.Dash:
                {
                    newState = new DashState(player, playerParameters, direction);
                    break;
                }
            case PlayerStates.Dead:
                {
                    newState = new DeadState(player, playerParameters);
                    break;
                }
            default:
                {
                    newState = new DefaultState(player, playerParameters);
                    break;
                }
        }
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnCollisionEnter(Collision collision) { }
    public virtual void Move(Vector3 direction, bool needDash) { }
    public PlayerStates GetState()
    {
        return _state;
    }

    public virtual Vector3 TryGetDirection()
    {
        return Vector3.zero;
    }
}

public enum PlayerStates
{
    Default,
    Dash,
    Dead,
}
