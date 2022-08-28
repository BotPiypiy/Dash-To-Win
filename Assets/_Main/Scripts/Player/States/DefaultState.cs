using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : PlayerState
{
    public DefaultState(PlayerController player, PlayerParameters playerParameters) : base(player, playerParameters)
    {
        _state = PlayerStates.Default;
    }

    public override void OnEnter()
    {
        _player.Mr.material = _playerParameters.DefaultMaterial;
    }

    public override void Move(Vector3 direction, bool needDash)
    {
        if(needDash)
        {
            _player.SetNewState(new DashState(_player, _playerParameters, direction));
        }
        else
        {
            _player.Rb.position += direction.normalized * _playerParameters.MoveSpeed * Time.deltaTime;
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        PlayerController enemy;

        if(collision.transform.TryGetComponent(out enemy))
        {
            if(enemy.State is DashState)
            {
                _player.TryDie();
            }
        }
    }
}
