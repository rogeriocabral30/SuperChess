using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class pawnManager : MonoBehaviour
{
    private bool mouseOver = false;

    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;
        mouseOver = true;
    }
    
    private void OnMouseExit() 
    {
        rend.material.color = startColor;
        mouseOver = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && mouseOver == true)
        {
           transform.position += new Vector3(1, 0, 0);
        }
    }
}
