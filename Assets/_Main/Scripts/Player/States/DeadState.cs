using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DeadState : PlayerState
{
    public DeadState(PlayerController player, PlayerParameters playerParameters) : base(player, playerParameters)
    {
        _state = PlayerStates.Dead;
    }

    public override void OnEnter()
    {
        _player.Mr.material = _playerParameters.DeadMaterial;
        WaitForEnd();
    }

    public override void Move(Vector3 direction, bool needDash)
    {
        _player.Rb.position += direction.normalized * _playerParameters.MoveSpeed * Time.deltaTime;
    }

    protected async void WaitForEnd()
    {
        await Task.Delay((int)(1000 * _playerParameters.DeadStateDuration));
        _player.SetNewState(new DefaultState(_player, _playerParameters));
    }
}
