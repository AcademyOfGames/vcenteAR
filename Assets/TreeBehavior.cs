using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehavior : MonoBehaviour
{
    [SerializeField] private GameObject sprout;
    [SerializeField] private GameObject grownTree;
    [SerializeField] private GameObject growTreeVFX;
    [SerializeField] private GameObject growTreeGlowVFX;


    public void GrowTree()
    {
        growTreeGlowVFX.SetActive(true);
        sprout.SetActive(false);
        grownTree.SetActive(true);
        growTreeVFX.SetActive(true);
        Invoke(nameof(Poof), 1.0f);
    }

    private void Poof()
    {

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
