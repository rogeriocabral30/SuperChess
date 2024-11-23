using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WhitePawnManager : MonoBehaviour
{
    private static WhitePawnManager selectedPawn = null;

    private bool mouseOver = false;
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private Vector3 posicaoOriginal;
    private bool isMoving = false;
    public bool canAct = false;
    public TabuleiroDamas tabuleiro;
    public whiteArcherManager arqueiroBranco;
    private List<GameObject> casasDisponiveis = new List<GameObject>();
    private Dictionary<GameObject, Color> casaCoresOriginais = new Dictionary<GameObject, Color>();
    public GameObject whiteArcher, peao, whiteMage;

    public TurnManager turnManager;
    public BattleManager battleManager;

    public Vector2Int posAtual, novaPos;

    private void Start()
    {
        peao = this.gameObject;
        tabuleiro = FindObjectOfType<TabuleiroDamas>();
        arqueiroBranco = FindObjectOfType<whiteArcherManager>();
        battleManager = FindObjectOfType<BattleManager>(); // Adicionada inicialização do BattleManager

        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        // Armazena as cores originais das casas
        foreach (var casa in GameObject.FindGameObjectsWithTag("Casa"))
        {
            casaCoresOriginais[casa] = casa.GetComponent<Renderer>().material.color;
        }
    }

    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (selectedPawn != this)
        {
            rend.material.color = startColor;
        }
        mouseOver = false;
    }

    public void OnMouseDown()
    {
        if (mouseOver)
        {
            if (selectedPawn == this)
            {
                DeselectPawn();
            }
            else
            {
                posicaoOriginal = transform.position;
                SelectPawn();

                if (canAct && battleManager != null) // Verifica se battleManager não é nulo
                {
                    battleManager.Attack(transform.position);
                    return;
                }
                else if (battleManager == null)
                {
                    Debug.LogError("battleManager is null! Please assign it in the inspector or initialize properly.");
                }
            }
        }
    }

    public void promoteArcher()
    {
        Vector3 promotePosition = this.transform.position;
        promotePosition.y += 0.8f;
        Instantiate(whiteArcher, promotePosition, whiteArcher.transform.rotation);
        DeselectPawn();
        Destroy(peao);
    }

    public void promoteMage()
    {
        Vector3 promotePosition = this.transform.position;
        promotePosition.y += 1.1f;
        Instantiate(whiteMage, promotePosition, whiteMage.transform.rotation);
        DeselectPawn();
        Destroy(peao);
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
                tabuleiro.ocupaCasa((int)targetPosition.x, (int)targetPosition.z);
                isMoving = false;
            }
        }
    }

    public void EnableActions()
    {
        canAct = true;
    }

    public void DisableActions()
    {
        canAct = false;
    }

    public void Move(Vector3 targetPosition)
    {
        if (canAct)
        {
            transform.position = targetPosition;
            DisableActions();
            turnManager.EndTurn();
        }
    }

    private void SelectPawn()
    {
        if (isMoving || selectedPawn != null)
            return;

        selectedPawn = this;
        rend.material.color = hoverColor;
        tentativa();
        StartCoroutine(WaitForClick());
    }

    public void DeselectPawn()
    {
        selectedPawn = null;
        rend.material.color = startColor;
        RestaurarCasasDisponiveis();
    }

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
                    if (hit.collider.CompareTag("Casa"))
                    {
                        Vector3 targetPos = hit.point;
                        targetPos = new Vector3((int)(targetPos.x), transform.position.y, transform.position.z);

                        if (IsPositionEmpty(targetPos))
                        {
                            targetPosition = targetPos;

                            foreach (GameObject c in casasDisponiveis)
                            {
                                if (targetPos.x == c.transform.position.x && targetPos.y - 0.3f == c.transform.position.y)
                                {
                                    tabuleiro.desocupaCasa((int)posicaoOriginal.x, (int)posicaoOriginal.z);
                                    isMoving = true;
                                    DeselectPawn();
                                    yield break;
                                }
                            }
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(2))
            {
                promoteArcher();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                promoteMage();
            }
            yield return null;
        }
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        Collider[] hitColliders = Physics.OverlapBox(targetPos, new Vector3(0.5f, 0.1f, 0.5f));
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("ChessPawn"))
            {
                return false;
            }
        }
        return true;
    }

    public void tentativa()
    {
        casasDisponiveis.Clear();
        Vector3 currentPos = transform.position;
        posAtual = new Vector2Int((int)currentPos.x, (int)currentPos.z);

        if (!tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z))
        {
            AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z);

            if (!tabuleiro.checaCasa((int)currentPos.x + 2, (int)currentPos.z) && transform.position.x == 0)
            {
                AdicionarCasaSeVazia((int)currentPos.x + 2, (int)currentPos.z);
            }
        }
    }

    private void AdicionarCasaSeVazia(int x, int y)
    {
        if (x >= 0 && x < tabuleiro._casaOcupada.GetLength(0) && y >= 0 && y < tabuleiro._casaOcupada.GetLength(1))
        {
            if (tabuleiro.IsPositionEmpty(new Vector3(x, 0, y)))
            {
                GameObject casa = GameObject.Find($"Casa {x},{y}");
                if (casa != null)
                {
                    casa.GetComponent<Renderer>().material.color = Color.yellow;
                    casasDisponiveis.Add(casa);
                }
            }
        }
    }

    private void RestaurarCasasDisponiveis()
    {
        foreach (GameObject casa in casasDisponiveis)
        {
            if (casaCoresOriginais.TryGetValue(casa, out Color originalColor))
            {
                casa.GetComponent<Renderer>().material.color = originalColor;
            }
        }
    }

    internal bool CanMove(Vector3 targetPosition)
    {
        throw new NotImplementedException();
    }

    internal void UpdatePosition(Vector3 newPosition)
    {
        throw new NotImplementedException();
    }
}
