using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _workbench, _newGame, _manual;

    private void Awake()
    {
        _workbench.SetActive(false);
        _manual.SetActive(false);
        _newGame.SetActive(true);
    }
}
