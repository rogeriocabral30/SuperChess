using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private bool mouseOver = false;
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    // Start is called before the first frame update
    void Start()
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
        //if (selectedPawn != this) // Não altera a cor se a peça estiver selecionada
        //{
            rend.material.color = startColor;
        //}
        mouseOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
