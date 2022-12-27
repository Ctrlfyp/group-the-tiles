using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameTheme
{
    public string id;
    public string name;
    public int price;

    // public List<Color> colors;

    private List<string> _stringColors;
    public List<string> stringColors
    {
        set
        {
            // convert string to color
            _stringColors = value;
            _colors = new List<Color>();
            foreach (string colorString in value)
            {
                Color newCol = new Color(0, 0, 0);
                ColorUtility.TryParseHtmlString(colorString, out newCol);
                _colors.Add(newCol);
            }
        }
        get
        {
            return _stringColors;
        }
    }

    private List<Color> _colors;
    public List<Color> colors
    {
        get
        {
            return _colors;
        }
    }

    public GameTheme()
    {
        _colors = new List<Color>();
    }
}
