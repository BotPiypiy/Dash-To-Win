using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParameters", menuName = "Parameters/Game Parameters", order = 1)]
public class GameParameters : ScriptableObject
{
    public float GameOverDelay;
    public int ScoreForWin;
}
