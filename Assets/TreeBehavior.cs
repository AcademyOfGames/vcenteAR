using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeBehavior : MonoBehaviour
{
    [SerializeField] private GameObject sprout;
    [SerializeField] private GameObject grownTree;
    [SerializeField] private GameObject growTreeVFX;
    [SerializeField] private GameObject growTreeGlowVFX;

    List<ParticleSystem> treeParticles;

    public void GrowTree()
    {
        growTreeGlowVFX.SetActive(true);
        sprout.SetActive(false);
        grownTree.SetActive(true);
        growTreeVFX.SetActive(true);
        StartCoroutine(nameof(HideVFX));
    }

    private IEnumerator HideVFX()
    {
        yield return new WaitForSeconds(2);
        UIControls.Instance.ShowEndUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        treeParticles = new List<ParticleSystem>();

        treeParticles.AddRange(growTreeVFX.GetComponentsInChildren<ParticleSystem>().ToList());
        treeParticles.AddRange(growTreeGlowVFX.GetComponentsInChildren<ParticleSystem>().ToList());

    }

    // Update is called once per frame
    void Update()
    {

    }
}
