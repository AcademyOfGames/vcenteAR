using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControls : MonoBehaviour
{
    [SerializeField] GameObject cubeUI1;
    [SerializeField] GameObject cubeUI2;

    private static UIControls _instance;

    public static UIControls Instance { get { return _instance; } }

    internal void ShowEndUI()
    {
        cubeUI2.SetActive(true);
        cubeUI1.SetActive(true);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
