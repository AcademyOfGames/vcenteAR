using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlButton : MonoBehaviour
{
    [SerializeField] string url;
    internal void GoToURL()
    {
        Application.OpenURL(url);
    }
}
