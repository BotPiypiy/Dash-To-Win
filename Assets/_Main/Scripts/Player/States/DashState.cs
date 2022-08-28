
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DashState : PlayerState
{
    protected Vector3 _direction;
    protected bool _cancelToken;

    public DashState(PlayerController player, PlayerParameters playerParameters, Vector3 direction) : base(player, playerParameters)
    {
        _state = PlayerStates.Dash;
        _direction = direction;
    }

    public override void OnEnter()
    {
        _player.Mr.material = _playerParameters.DashMaterial;

        if (_player.isClient && _player.isLocalPlayer)
        {
            if (_direction != Vector3.zero)
            {
                Dash();
            }
        }
    }

    public override void OnExit()
    {
        _cancelToken = true;
    }

    protected async void Dash()
    {
        float deltaTime;
        float dashTime = _playerParameters.DashDistance / _playerParameters.DashSpeed;
        
        while (dashTime > 0f)
        {
            if (_cancelToken)
            {
                 return;
            }
            deltaTime = Time.time;
            await Task.Yield();
            deltaTime = Time.time - deltaTime;

            _player.Rb.position += _direction.normalized * _playerParameters.DashSpeed * deltaTime;

            dashTime -= deltaTime;
        }

        _player.SetNewState(new DefaultState(_player, _playerParameters));
    }

    public override void OnCollisionEnter(Collision collision)
    {
        PlayerController enemy;
        if (collision.transform.TryGetComponent(out enemy))
        {
            if (enemy.TryDie())
            {
                _player.IncScore();
            }

            if(enemy.State is DashState)
            {
                _player.TryDie();
            }
        }
    }

    public override Vector3 TryGetDirection()
    {
        return _direction;
    }
}
