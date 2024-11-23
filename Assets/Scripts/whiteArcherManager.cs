using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class whiteArcherManager : MonoBehaviour
{
    private static whiteArcherManager selectedArcher = null;

    private bool mouseOver = false;
    public Color hoverColor;
    private Renderer rend;
    private Color startColor;

    public Vector2Int[] asd;
    public WhitePawnManager peaoBranco;
    public TabuleiroDamas tabuleiro; 
    private List<GameObject> casasDisponiveis = new List<GameObject>();
    private Dictionary<GameObject, Color> casaCoresOriginais = new Dictionary<GameObject, Color>();

    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;
  
    public float attackRange = 3f; // Defina a distância do ataque
    public TurnManager turnManager; // Adiciona o TurnManager
    public BattleManager battleManager; // Adiciona o BattleManager
    public bool canAct = false;

    private Vector2Int posAtual;

    public AttackCircleManager attackCircleManager; // Referência ao AttackCircleManager

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
        peaoBranco = FindObjectOfType<WhitePawnManager>();
        attackCircleManager = FindObjectOfType<AttackCircleManager>(); // Inicia area de ataque ( ate que fim , progresso)
        turnManager = FindObjectOfType<TurnManager>(); // Encontra o TurnManager
        battleManager = FindObjectOfType<BattleManager>(); // Encontra o BattleManager

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
        canAct = true; // Permite que o arqueiro faca acoes
        
    }

    public void DisableActions()
    {
        canAct = false; // Impede que o arqueiro faca acoes
        
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
        peaoBranco.DeselectPawn();
        rend.material.color = hoverColor;
        tentativa();
        //MostrarCasasDisponiveisArqueiro();        
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


            //attackCircleManager.HideAttackCircle();
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

    public Array MostrarAreaAtaque()
    {
        
        /*Vector2Int[] direcoes = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // Direita
            new Vector2Int(-1, 0),  // Esquerda
            new Vector2Int(0, 1),   // Cima
            new Vector2Int(0, -1),  // Baixo
            new Vector2Int(1, 1),   // Diagonal direita-cima
            new Vector2Int(1, -1),  // Diagonal direita-baixo
            new Vector2Int(-1, 1),  // Diagonal esquerda-cima
            new Vector2Int(-1, -1)  // Diagonal esquerda-baixo
        };*/

        Vector2Int[] direcoes = new Vector2Int[]
        {
            new Vector2Int(3, 0),
            new Vector2Int(2, 0),
            new Vector2Int(2, -1),
            new Vector2Int(2, 1),

            new Vector2Int(-3, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(-2, 1),
            new Vector2Int(-2,- 1),

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
        return direcoes;
    }

    private void MostrarCírculoSeValido(Vector2Int novaPos)
    {
        if (novaPos.x >= 0 && novaPos.x < tabuleiro._casaOcupada.GetLength(0) &&
            novaPos.y >= 0 && novaPos.y < tabuleiro._casaOcupada.GetLength(1))
        {
            Vector3 posicaoCírculo = new Vector3(novaPos.x, 1f, novaPos.y);
            attackCircleManager.ShowAttackCircle(posicaoCírculo); // Exibe a merda do circulo
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
        AdicionarCasaSeVaziaSeVazia(posAtual.x + 2, posAtual.y);
        AdicionarCasaSeVaziaSeVazia(posAtual.x - 2, posAtual.y);
        AdicionarCasaSeVaziaSeVazia(posAtual.x, posAtual.y + 2);
        AdicionarCasaSeVaziaSeVazia(posAtual.x, posAtual.y - 2);

        // Adiciona casas adjacentes 
        AdicionarCasaSeVaziaSeVazia(posAtual.x + 1, posAtual.y);
        AdicionarCasaSeVaziaSeVazia(posAtual.x - 1, posAtual.y);
        AdicionarCasaSeVaziaSeVazia(posAtual.x, posAtual.y + 1);
        AdicionarCasaSeVaziaSeVazia(posAtual.x, posAtual.y - 1);
    }

    public void tentativa()
    {
        casasDisponiveis.Clear();
        Vector3 currentPos = transform.position;
        posAtual = new Vector2Int((int)currentPos.x, (int)currentPos.z);

        if (tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z) == false) //checa a casa da frente
        {
            AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z);
            
            if (tabuleiro.checaCasa((int)currentPos.x + 2, (int)currentPos.z) == false) //checa 2 casas a frente
            {
                AdicionarCasaSeVazia((int)currentPos.x + 2, (int)currentPos.z);
            }

            if (tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z + 1) == false) //checa a casa diagonal da esquerda
            {
                AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z + 1);
            }

            if (tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z - 1) == false) //checa a casa diagonal da direita
            {
                AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z - 1);
            }
        }

        if (tabuleiro.checaCasa((int)currentPos.x - 1, (int)currentPos.z) == false) //checa a casa de trás
        {
            AdicionarCasaSeVazia((int)currentPos.x - 1, (int)currentPos.z);
            
            if (tabuleiro.checaCasa((int)currentPos.x - 2, (int)currentPos.z) == false) //checa 2 casas atrás
            {
                AdicionarCasaSeVazia((int)currentPos.x - 2, (int)currentPos.z);
            }

            if (tabuleiro.checaCasa((int)currentPos.x - 1, (int)currentPos.z - 1) == false) //checa diagonal atrás direita
            {
                AdicionarCasaSeVazia((int)currentPos.x - 1, (int)currentPos.z -1);
            }

            if (tabuleiro.checaCasa((int)currentPos.x - 1, (int)currentPos.z + 1) == false) // checa diagonal atrás esquerda
            {
                AdicionarCasaSeVazia((int)currentPos.x - 1, (int)currentPos.z + 1);
            }
        }

        if (tabuleiro.checaCasa((int)currentPos.x, (int)currentPos.z + 1) == false) //checa casa da esquerda
        {
            AdicionarCasaSeVazia((int)currentPos.x, (int)currentPos.z + 1);

            if (tabuleiro.checaCasa((int)currentPos.x, (int)currentPos.z + 2) == false) //checa 2 casas para esquerda
            {
                AdicionarCasaSeVazia((int)currentPos.x, (int)currentPos.z + 2);
            }

            if (tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z + 1) == false) //checa diagonal esquerda/frente
            {
                AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z + 1);
            }

            if (tabuleiro.checaCasa((int)currentPos.x - 1, (int)currentPos.z + 1) == false) // checa diagonal esquerda/trás
            {
                AdicionarCasaSeVazia((int)currentPos.x - 1, (int)currentPos.z + 1);
            }
        }

        if (tabuleiro.checaCasa((int)currentPos.x, (int)currentPos.z - 1) == false) //checa casa da direita
        {
            AdicionarCasaSeVazia((int)currentPos.x, (int)currentPos.z - 1);

            if (tabuleiro.checaCasa((int)currentPos.x, (int)currentPos.z - 2) == false) //checa 2 casas para direita
            {
                AdicionarCasaSeVazia((int)currentPos.x, (int)currentPos.z - 2);
            }

            if (tabuleiro.checaCasa((int)currentPos.x - 1, (int)currentPos.z - 1) == false) //checa diagonal direita/atrás
            {
                AdicionarCasaSeVazia((int)currentPos.x - 1, (int)currentPos.z - 1);
            }

            if (tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z - 1) == false) //checa diagonal direita/frente
            {
                AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z - 1);
            }

        }

        if (tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z + 1) == false)
        {
            AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z + 1);
        }

        if (tabuleiro.checaCasa((int)currentPos.x + 1, (int)currentPos.z - 1) == false)
        {
            AdicionarCasaSeVazia((int)currentPos.x + 1, (int)currentPos.z - 1);
        }

        if (tabuleiro.checaCasa((int)currentPos.x - 1, (int)currentPos.z + 1) == false)
        {
            AdicionarCasaSeVazia((int)currentPos.x -1 , (int)currentPos.z + 1);
        }

        if (tabuleiro.checaCasa((int)currentPos.x - 1, (int)currentPos.z - 1) == false)
        {
            AdicionarCasaSeVazia((int)currentPos.x - 1, (int)currentPos.z - 1);
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
                    casa.GetComponent<Renderer>().material.color = Color.yellow; // Muda a cor da casa
                    casasDisponiveis.Add(casa);
                }
            }
        }
    }

    private void AdicionarCasaSeVaziaSeVazia(int x, int y)
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
        throw new NotImplementedException();
    }

    internal void UpdatePosition(Vector3 newPosition)
    {
        throw new NotImplementedException();
    }

    internal bool CanMove(Vector3 targetPosition)
    {
        throw new NotImplementedException();
    }
}
