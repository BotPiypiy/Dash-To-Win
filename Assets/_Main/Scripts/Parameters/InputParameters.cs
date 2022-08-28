using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputParameters", menuName = "Parameters/Input Parameters", order = 2)]
public class InputParameters : ScriptableObject
{
    public KeyCode Dash;
    public KeyCode Pause;

    public float MouseSensevity;
}
