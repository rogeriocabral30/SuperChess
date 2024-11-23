using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackMageManager : MonoBehaviour
{
    private static BlackMageManager selectedMage = null;

    private bool mouseOver = false;
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    public BlackPawnManager peaoPreto; 
    public BlackArcherManager arqueiroPreto; 
    public TabuleiroDamas tabuleiro;
    public AttackCircleManager attackCircleManager;

    private List<GameObject> casasDisponiveis = new List<GameObject>();
    private Dictionary<GameObject, Color> casaCoresOriginais = new Dictionary<GameObject, Color>();

    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Vector2Int posAtual;

    public int vidaAtual, vidaMaxima, movimento, ataqueOriginal, ataqueAtual;

    void Start()
    {
        vidaMaxima = 6;
        vidaAtual = vidaMaxima;
        movimento = 1;
        ataqueOriginal = 5;
        ataqueAtual = ataqueOriginal;

        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        tabuleiro = FindObjectOfType<TabuleiroDamas>();
        peaoPreto = FindObjectOfType<BlackPawnManager>();

        foreach (var casa in GameObject.FindGameObjectsWithTag("Casa"))
        {
            casaCoresOriginais[casa] = casa.GetComponent<Renderer>().material.color;
        }
    }

    void Update()
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
                DeselectMage();
            }
        }
    }

    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        if (selectedMage != this)
        {
            rend.material.color = startColor;
        }
        mouseOver = false;
    }

    private void OnMouseDown()
    {
        if (mouseOver)
        {
            if (selectedMage == this)
            {
                DeselectMage();
            }
            else
            {
                SelectMage();
            }
        }
    }

    private void SelectMage()
    {
        if (isMoving || selectedMage != null)
            return;

        selectedMage = this;
        rend.material.color = hoverColor;
        tentativa();
        MostrarAreaAtaque();
        StartCoroutine(WaitForClick());
    }

    public void DeselectMage()
    {
        selectedMage = null;
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
        while (selectedMage == this)
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
                        targetPos = new Vector3((int)targetPos.x, 1.1f, (int)targetPos.z);

                        if (IsPositionValid(targetPos))
                        {
                            tabuleiro.desocupaCasa((int)transform.position.x, (int)transform.position.z);
                            targetPosition = targetPos;
                            tabuleiro.ocupaCasa((int)targetPosition.x, (int)targetPosition.z);
                            isMoving = true;
                            DeselectMage();
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
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(3, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
            new Vector2Int(0, 3),
            new Vector2Int(0, -1),
            new Vector2Int(0, -2),
            new Vector2Int(0, -3),
            new Vector2Int(-1, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(-3, 0),
        };

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
            attackCircleManager.ShowAttackCircle(posicaoCírculo);
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
        return distance <= 1.0f;
    }

    private bool IsDirectionFree(Vector2Int currentPos, Vector3 targetPos)
    {
        int deltaX = (int)targetPos.x - currentPos.x;
        int deltaY = (int)targetPos.z - currentPos.y;

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
        return true;
    }

    public void tentativa()
    {
        casasDisponiveis.Clear();
        Vector3 currentPos = transform.position;
        posAtual = new Vector2Int((int)currentPos.x, (int)currentPos.z);

        VerificarCasa(1, 0);  // Frente
        VerificarCasa(-1, 0); // Atrás
        VerificarCasa(0, 1);  // Direita
        VerificarCasa(0, -1); // Esquerda
    }

    private void VerificarCasa(int deltaX, int deltaY)
    {
        int novaX = (int)transform.position.x + deltaX;
        int novaY = (int)transform.position.z + deltaY;
        if (!tabuleiro.checaCasa(novaX, novaY))
            AdicionarCasaSeVazia(novaX, novaY);
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
        foreach (var casa in casasDisponiveis)
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
