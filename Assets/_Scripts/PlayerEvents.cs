using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    delegate void OnGameOver();
    static event OnGameOver gameOver;
}
