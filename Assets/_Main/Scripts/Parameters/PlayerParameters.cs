using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameters", menuName = "Parameters/Player Parameters", order = 0)]
public class PlayerParameters : ScriptableObject
{
    public float MoveSpeed;
    public float DashDistance;
    public float DashSpeed;
    public float DeadStateDuration;
    public Material DefaultMaterial;
    public Material DashMaterial;
    public Material DeadMaterial;
}
