using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileManager : MonoBehaviour
{
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;
    private bool mouseOver = false;

    public TabuleiroDamas tabuleiro; //variável para usar ao chamar objetos da classe tabuleiro
    public GameObject tile, circulo;

    // Start is called before the first frame update
    void Start()
    {
        tile = this.gameObject;
        tabuleiro = FindObjectOfType<TabuleiroDamas>();
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

   /*private void OnMouseOver()
    {
        rend.material.color = hoverColor;
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
        mouseOver = false;
    }*/

    private void OnMouseDown()
    {
        //circulo.SetActive(true);
    }

    public void AttackOn()
    {
        Vector3 teste = transform.position;
        circulo.SetActive(true);
    }

    public void AttackOff()
    {
        circulo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            if (tabuleiro.checaCasa((int)transform.position.x, (int)transform.position.z))
            {
                rend.material.color = Color.red;
            }
            else
            {
                rend.material.color = Color.green;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rend.material.color = startColor;
        }
    }
}
