using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBehavior : MonoBehaviour
{
    static float waterLevel = 0;
    public float waterChangeRate = 0.01f;
    private void OnParticleCollision(GameObject other)
    {
        if(other.tag == "water")
        {
            FillWater();
        }
    }

    private void FillWater()
    {
        waterLevel += waterChangeRate;
        print(waterLevel); 
        if(waterLevel >= 1.0f)
        {
            FindObjectOfType<TreeBehavior>().GrowTree();
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
