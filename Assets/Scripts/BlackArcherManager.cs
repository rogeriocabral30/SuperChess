using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackArcherManager : MonoBehaviour
{
    private static BlackArcherManager selectedArcher = null;

    private bool mouseOver = false;
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    public Vector2Int[] asd;
    public BlackPawnManager peaoPreto; 
    public TabuleiroDamas tabuleiro;
    private List<GameObject> casasDisponiveis = new List<GameObject>();
    private Dictionary<GameObject, Color> casaCoresOriginais = new Dictionary<GameObject, Color>();

    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    public float attackRange = 3f; // Distância do ataque
    public TurnManager turnManager; // Gerenciador de turnos
    public BattleManager battleManager; // Gerenciador de batalha
    public bool canAct = false;

    private Vector2Int posAtual;

    public AttackCircleManager attackCircleManager; // Gerenciador de círculos de ataque

    public int vidaAtual, vidaMaxima, movimento, ataqueOriginal, ataqueAtual;

    private void Start()
    {
        vidaMaxima = 10;
        vidaAtual = vidaMaxima;
        movimento = 2;
        ataqueOriginal = 4;
        ataqueAtual = ataqueOriginal;

        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        tabuleiro = FindObjectOfType<TabuleiroDamas>();
        peaoPreto = FindObjectOfType<BlackPawnManager>();
        attackCircleManager = FindObjectOfType<AttackCircleManager>();
        turnManager = FindObjectOfType<TurnManager>();
        battleManager = FindObjectOfType<BattleManager>();

        foreach (var casa in GameObject.FindGameObjectsWithTag("Casa"))
        {
            casaCoresOriginais[casa] = casa.GetComponent<Renderer>().material.color;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = targetPosition;
                isMoving = false;
                DeselectArcher();
            }
        }
    }

    public void EnableActions()
    {
        canAct = true; // Permite que o arqueiro faça ações
    }

    public void DisableActions()
    {
        canAct = false; // Impede que o arqueiro faça ações
    }

    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (selectedArcher != this)
        {
            rend.material.color = startColor;
        }
        mouseOver = false;
    }

    private void OnMouseDown()
    {
        if (mouseOver)
        {
            if (selectedArcher == this)
            {
                DeselectArcher();
            }
            else
            {
                SelectArcher();
            }
        }
    }

    private void SelectArcher()
    {
        if (isMoving || selectedArcher != null)
            return;

        selectedArcher = this;
        peaoPreto.DeselectPawn(); 
        rend.material.color = hoverColor;
        MostrarAreaAtaque();
        StartCoroutine(WaitForClick());
    }

    public void DeselectArcher()
    {
        selectedArcher = null;
        rend.material.color = startColor;
        RestaurarCasasDisponiveis();
        casasDisponiveis.Clear();
        foreach (GameObject c in GameObject.FindGameObjectsWithTag("circuloAtk"))
        {
            Destroy(c);
        }
    }

    private IEnumerator WaitForClick()
    {
        while (selectedArcher == this)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.CompareTag("Casa"))
                    {
                        Vector3 targetPos = hit.point;
                        targetPos = new Vector3(Mathf.Round(targetPos.x), 1.1f, Mathf.Round(targetPos.z));

                        if (IsPositionValid(targetPos))
                        {
                            tabuleiro.desocupaCasa((int)transform.position.x, (int)transform.position.z);
                            targetPosition = targetPos;
                            tabuleiro.ocupaCasa((int)targetPosition.x, (int)targetPosition.z);
                            isMoving = true;
                            DeselectArcher();
                            yield break;
                        }
                    }
                }
            }
            yield return null;
        }
    }

    public void MostrarAreaAtaque()
    {
        Vector2Int[] direcoes = new Vector2Int[]
        {
            new Vector2Int(3, 0),
            new Vector2Int(2, 0),
            new Vector2Int(2, -1),
            new Vector2Int(2, 1),
            new Vector2Int(-3, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(0, 3),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2),
            new Vector2Int(-1, 2),
            new Vector2Int(0, -3),
            new Vector2Int(0, -2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, -2),
        };

        asd = direcoes;
        foreach (var dir in direcoes)
        {
            Vector2Int novaPos = new Vector2Int(posAtual.x + dir.x, posAtual.y + dir.y);
            MostrarCírculoSeValido(novaPos);
        }
    }

    private void MostrarCírculoSeValido(Vector2Int novaPos)
    {
        if (novaPos.x >= 0 && novaPos.x < tabuleiro._casaOcupada.GetLength(0) &&
            novaPos.y >= 0 && novaPos.y < tabuleiro._casaOcupada.GetLength(1))
        {
            Vector3 posicaoCírculo = new Vector3(novaPos.x, 1f, novaPos.y);
            attackCircleManager.ShowAttackCircle(posicaoCírculo); // Exibe o círculo de ataque
        }
    }

    private bool IsPositionValid(Vector3 targetPos)
    {
        return tabuleiro.IsPositionEmpty(targetPos) && IsAdjacent(targetPos) && IsDirectionFree(posAtual, targetPos);
    }

    private bool IsAdjacent(Vector3 targetPos)
    {
        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(new Vector3(currentPos.x, 0, currentPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        return distance <= 2.0f; // Permite movimento nas diagonais
    }

    private bool IsDirectionFree(Vector2Int currentPos, Vector3 targetPos)
    {
        int deltaX = Mathf.RoundToInt(targetPos.x) - currentPos.x;
        int deltaY = Mathf.RoundToInt(targetPos.z) - currentPos.y;

        // Verifica se está movendo na horizontal
        if (deltaY == 0)
        {
            if (deltaX > 0)
            {
                return tabuleiro.IsPositionEmpty(new Vector3(currentPos.x + 1, 0, currentPos.y));
            }
            else if (deltaX < 0)
            {
                return tabuleiro.IsPositionEmpty(new Vector3(currentPos.x - 1, 0, currentPos.y));
            }
        }
        // Verifica se está movendo na vertical
        else if (deltaX == 0)
        {
            if (deltaY > 0)
            {
                return tabuleiro.IsPositionEmpty(new Vector3(currentPos.x, 0, currentPos.y + 1));
            }
            else if (deltaY < 0)
            {
                return tabuleiro.IsPositionEmpty(new Vector3(currentPos.x, 0, currentPos.y - 1));
            }
        }

        // Para movimentos diagonais 
        return true;
    }

    private void RestaurarCasasDisponiveis()
    {
        foreach (var casa in casasDisponiveis)
        {
            if (casaCoresOriginais.TryGetValue(casa, out Color originalColor))
            {
                casa.GetComponent<Renderer>().material.color = originalColor; // Restaura a cor original
            }
        }
    }

    internal bool IsInAttackRange(Vector3 targetPosition)
    {
        // verifica se o alvo está dentro do alcance
        return Vector3.Distance(transform.position, targetPosition) <= attackRange;
    }

    internal void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    internal bool CanMove(Vector3 targetPosition)
    {
        return IsPositionValid(targetPosition);
    }
}
