using UnityEngine;
using System.Collections;

public class PawnManager : MonoBehaviour
{
    private static PawnManager selectedPawn = null; // Peça atualmente selecionada

    private bool mouseOver = false;
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    public float moveSpeed = 2f; // Velocidade de movimentação
    private Vector3 targetPosition; // Posição  da peça para movimentação
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
        if (selectedPawn != this) // Não altera a cor se a peça estiver selecionada
        {
            rend.material.color = startColor;
        }
        mouseOver = false;
    }

    private void OnMouseDown()
    {
        // Se a peça está sob o cursor e é clicada, verifica o status de seleção
        if (mouseOver)
        {
            if (selectedPawn == this)
            {
                DeselectPawn();
            }
            else
            {
                SelectPawn();
            }
        }
    }

    private void Update()
    {
        // Mova a peça em direção a casa
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = targetPosition;
                isMoving = false; // Interrompe o movimento quando chega a casa
            }
        }
    }

    // Seleciona a peça e define a posição da casa
    private void SelectPawn()
    {
        // Se já houver outra peça se movendo, não faz nada
        if (isMoving || selectedPawn != null)
            return;

        // Marca a peça como selecionada
        selectedPawn = this;
        rend.material.color = hoverColor; // Mantém a cor de seleção

        // Inicia a movimentação
        StartCoroutine(WaitForClick());
    }

    private void DeselectPawn()
    {
        selectedPawn = null;
        rend.material.color = startColor; // Restaura a cor original
    }

    // Aguarda um clique em uma posição do tabuleiro
    private IEnumerator WaitForClick()
    {
        while (selectedPawn == this)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Verifica se o clique foi sobre uma casaa
                    if (hit.collider.CompareTag("Casa"))
                    {
                        Vector3 targetPos = hit.point;

                        // Ajusta a posição da peça para o centro da casa
                        targetPos = new Vector3(Mathf.Round(targetPos.x), transform.position.y, Mathf.Round(targetPos.z));

                        // Verifica se a posição  é uma casa adjacente e se está vazia
                        if (IsAdjacent(targetPos) && IsPositionEmpty(targetPos))
                        {
                            targetPosition = targetPos;
                            isMoving = true;
                            DeselectPawn(); // Deseleciona a peça após o movimento
                            yield break; // Move quando a posição da casa é definida
                        }
                    }
                }
            }
            yield return null; // Espera para verificar novamente
        }
    }

    // Verifica se a posição da casaé adjacente à posição atual
    private bool IsAdjacent(Vector3 targetPos)
    {
        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(new Vector3(currentPos.x, 0, currentPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        return distance == 1.0f; // Verifica se a distância é exatamente 1 unidade (uma casa)
    }

    // Verifica se a posição da casa está vazia
    private bool IsPositionEmpty(Vector3 targetPos)
    {
        Collider[] hitColliders = Physics.OverlapBox(targetPos, new Vector3(0.5f, 0.1f, 0.5f));
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("ChessPawn")) // Verifica se há uma peça na posição
            {
                return false; // A posição está ocupada
            }
        }
        return true; // A posição está vazia
    }
}
