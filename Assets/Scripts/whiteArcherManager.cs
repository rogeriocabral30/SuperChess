using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whiteArcherManager : MonoBehaviour
{
    private static whiteArcherManager selectedArcher = null;

    private bool mouseOver = false; 
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    public TabuleiroDamas tabuleiro;
    private List<GameObject> casasDisponiveis = new List<GameObject>();
    private Dictionary<GameObject, Color> casaCoresOriginais = new Dictionary<GameObject, Color>();

    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private Vector2Int posAtual;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        tabuleiro = FindObjectOfType<TabuleiroDamas>();

        
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
        rend.material.color = hoverColor;
        MostrarCasasDisponiveisArqueiro();

        StartCoroutine(WaitForClick());
    }

    private void DeselectArcher()
    {
        selectedArcher = null;
        rend.material.color = startColor;
        RestaurarCasasDisponiveis();
        casasDisponiveis.Clear();
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
                            targetPosition = targetPos;
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

    private bool IsPositionValid(Vector3 targetPos)
    {
        return tabuleiro.IsPositionEmpty(targetPos) && IsAdjacent(targetPos);
    }

    private bool IsAdjacent(Vector3 targetPos)
    {
        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(new Vector3(currentPos.x, 0, currentPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        return distance <= 2.0f; // Permite movimento nas diagonais
    }

    private void MostrarCasasDisponiveisArqueiro()
    {
        casasDisponiveis.Clear();
        Vector3 currentPos = transform.position;
        posAtual = new Vector2Int(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.z));

        // Adiciona casas diagonais
        AdicionarCasaSeVazia(posAtual.x + 1, posAtual.y + 1);
        AdicionarCasaSeVazia(posAtual.x + 1, posAtual.y - 1);
        AdicionarCasaSeVazia(posAtual.x - 1, posAtual.y + 1);
        AdicionarCasaSeVazia(posAtual.x - 1, posAtual.y - 1);

        // Adiciona casas em linha reta 
        AdicionarCasaSeVazia(posAtual.x + 2, posAtual.y);
        AdicionarCasaSeVazia(posAtual.x - 2, posAtual.y);
        AdicionarCasaSeVazia(posAtual.x, posAtual.y + 2);
        AdicionarCasaSeVazia(posAtual.x, posAtual.y - 2);

        // Adiciona casas adjacentes 
        AdicionarCasaSeVazia(posAtual.x + 1, posAtual.y);
        AdicionarCasaSeVazia(posAtual.x - 1, posAtual.y);
        AdicionarCasaSeVazia(posAtual.x, posAtual.y + 1);
        AdicionarCasaSeVazia(posAtual.x, posAtual.y - 1);
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
                    casa.GetComponent<Renderer>().material.color = Color.yellow; // Muda a cor da casa
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
                casa.GetComponent<Renderer>().material.color = originalColor; // Restaura a cor original
            }
        }
    }
}
