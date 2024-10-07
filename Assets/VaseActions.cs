using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaseActions : MonoBehaviour
{
    [SerializeField] private ParticleSystem water;
    [SerializeField] private ParticleSystem mist;
    public void ActivateWater()
    {
        water.Play();
        mist.Play();
    }

    public void DeactivateWater()
    {
        water.Stop();
        mist.Stop();
    }
}
