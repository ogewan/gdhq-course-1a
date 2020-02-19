using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillToken : MonoBehaviour
{
    [SerializeField]
    GameObject yellowBar;
    [SerializeField]
    GameObject redBar;
    [SerializeField]
    GameObject greenBar;
    [SerializeField]
    GameObject[] yellow = new GameObject[3] { null, null, null };
    [SerializeField]
    GameObject[] red = new GameObject[3] { null, null, null };
    [SerializeField]
    GameObject[] green = new GameObject[3] { null, null, null };
    bool _yellowBarOn = false;
    bool _redBarOn = false;
    bool _greenBarOn = false;
    int _yellowTokens = 0;
    int _redTokens = 0;
    int _greenTokens = 0;

    private enum _typeColor { yellow, red, green};
    private void addToken(_typeColor type, int count = 1)
    {
        switch (type)
        {
            case _typeColor.yellow:
                if (_yellowTokens < 3)
                {
                    yellow[_yellowTokens].SetActive(true);
                }
                _yellowTokens += count;
                break;
            case _typeColor.red:
                if (_redTokens < 3)
                {
                    red[_redTokens].SetActive(true);
                }
                _redTokens += count;
                break;
            case _typeColor.green:
                if (_greenTokens < 3)
                {
                    green[_greenTokens].SetActive(true);
                }
                _greenTokens += count;
                break;
        }
    }

    public void yellowToken(int count)
    {
        addToken(_typeColor.yellow, count);
    }
    public void redToken(int count)
    {
        addToken(_typeColor.red, count);
    }
    public void greenToken(int count)
    {
        addToken(_typeColor.green, count);
    }
    public void toggleYellow()
    {
        _yellowBarOn = !_yellowBarOn;
        yellowBar.SetActive(true);
    }
    public void toggleRed()
    {
        _redBarOn = !_redBarOn;
        redBar.SetActive(true);
    }
    public void toggleGreen()
    {
        _greenBarOn = !_greenBarOn;
        greenBar.SetActive(true);
    }
}
