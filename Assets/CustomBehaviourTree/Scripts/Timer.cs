using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float _time;
    float _elaspedTime;

    #region Timer
    public Timer(float time)
        => SetTimer(time);
    public void SetTimer(float time)
        => _time = time;

    public bool IsTimeUp()
    {
        _elaspedTime += Time.deltaTime;
        return _elaspedTime >= _time;
    }

    public void ResetTimer()
    => _elaspedTime = 0f;

    #endregion
}
