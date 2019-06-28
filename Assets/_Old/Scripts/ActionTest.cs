using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActionTest : MonoBehaviour
{
    public GameManager _GameManager;

    void Start()
    {
        _GameManager._GameState
        .DistinctUntilChanged()
        .Where(x => x == GameState.Playing)
        .Subscribe(_ => Debug.Log("Reset"));
    }

    void Update()
    {
    }
}
