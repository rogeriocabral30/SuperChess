using UnityEngine;
using System.Collections; // Certifique-se de incluir este namespace

public class PawnManager : MonoBehaviour
{
    private bool mouseOver = false;
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    public float moveSpeed = 2f; // Velocidade de movimentação
    private Vector3 targetPosition; // Posição alvo para movimentação
    private bool isMoving = false; // Flag para verificar se a peça está se movendo

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

    private void OnMouseDown()
    {
        // Se a peça está sob o cursor e é clicada, marca como selecionada
        if (mouseOver)
        {
            SelectPawn();
        }
    }

    private void Update()
    {
        // Mova a peça em direção ao alvo
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = targetPosition;
                isMoving = false; // Interrompe o movimento quando chega ao alvo
            }
        }
    }

    // Seleciona a peça e define a posição alvo
    private void SelectPawn()
    {
        // Se já houver outra peça se movendo, não faz nada
        if (isMoving)
            return;

        // Inicia o processo de seleção e movimentação
        StartCoroutine(WaitForClick());
    }

    //  aguarda um clique em uma posição do tabuleiro
    private IEnumerator WaitForClick()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Verifica se o clique foi sobre uma casa do tabuleiro
                    if (hit.collider.CompareTag("Casa"))
                    {
                        targetPosition = hit.point;
                        isMoving = true;
                        yield break; // move quando a posição alvo é definida
                    }
                }
            }
            yield return null; // Espera para verificar novamente
        }
    }
}
