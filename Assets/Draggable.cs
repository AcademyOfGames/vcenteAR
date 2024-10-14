using System;
using System.Collections;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public bool isDragging { get; private set; }

    [SerializeField] private Animator anim;


    public void ActivateDragObject()
    {
        print("Powering anim");
        anim.Play("vasePouring");
        isDragging = true;
    }



    internal void DeactivateDragging()
    {
        anim.SetTrigger("straighten");
        isDragging = false;
    }
}