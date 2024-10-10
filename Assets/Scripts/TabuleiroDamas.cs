using UnityEngine;
using System.Collections.Generic;

public class TabuleiroDamas : MonoBehaviour
{
    [Header("Materiais das Casas")]
    [SerializeField] private Material materialCasaPreta; // Material para casas pretas
    [SerializeField] private Material materialCasaBranca; // Material para casas brancas

    [Header("Configurações de Buracos e Elevações")]
    [SerializeField] private Color corBuraco = Color.black; // Cor dos buracos
    [SerializeField] private Color corElevacao = Color.green; // Cor das casas com elevação
    [SerializeField] private float alturaElevacao = 0.5f; // Altura da elevação
    [SerializeField] private GameObject casa, pecaBranca, pecaPreta, mago;
    private GameObject[] _casa;


    private const int tamanhoTabuleiro = 10; // Tabuleiro 8x8 para damas
    private const int numeroBuracos = 4; // Número de buracos a serem criados (agora 2)
    public bool[,] _casaOcupada;

    private const int numeroElevacoes = 6; // Número de casas com elevação a serem criadas

    private List<Vector2Int> posicoesBuracos = new List<Vector2Int>();

    private void Start()
    {
        Instantiate(mago);
        _casaOcupada = new bool[tamanhoTabuleiro, tamanhoTabuleiro];
        CriarTabuleiroDamas();
        AdicionarBuracos();
        AdicionarElevacoes();
        GerarPecas();
    }

    private void Update() { }

    private void GerarPecas()
    {
        for (int x = 0; x < tamanhoTabuleiro; x++)
        {
            for (int y = 0; y < tamanhoTabuleiro; y++)
            {
                if (x == 0)
                {
                    Vector3 posicaoPeca = new Vector3(x, 0.3f, y);
                    Instantiate(pecaBranca, posicaoPeca, pecaBranca.transform.rotation);
                    pecaBranca.name = "peaoBranco";
                }
                if (x == 9)
                {
                    Vector3 posicaoPeca = new Vector3(x, 0.3f, y);
                    Instantiate(pecaPreta, posicaoPeca, pecaPreta.transform.rotation);
                    pecaPreta.name = "peaoPreto";
                }
                
            }
        }
        Debug.Log("Tabuleiro gerado");
    }

    public void ocupaCasa(int x, int y)
    {
        _casaOcupada[x, y] = true;
    }

    public void desocupaCasa(int x, int y)
    {
        _casaOcupada[x, y] = false;

    }

    private void CriarTabuleiroDamas()
    {
        float tamanhoCasa = 1.0f; // Tamanho fixo para as casas

        for (int x = 0; x < tamanhoTabuleiro; x++)
        {
            for (int z = 0; z < tamanhoTabuleiro; z++)
            {
                Vector3 posicao = new Vector3(x, 0, z);
                //casa = GameObject.CreatePrimitive(PrimitiveType.Cube);
                casa = GameObject.Instantiate(casa);
                casa.transform.position = posicao;
                casa.transform.localScale = new Vector3(tamanhoCasa, 0.2f, tamanhoCasa); // Define a escala das casas
                casa.tag = "Casa";

                Renderer renderer = casa.GetComponent<Renderer>();
                renderer.material = (x + z) % 2 == 0 ? materialCasaBranca : materialCasaPreta;
                casa.name = $"Casa {x},{z}";
            }
        }
    }

    private void AdicionarBuracos()
    {
        float tamanhoCasa = 1.0f;
        List<Vector2Int> posicoesDisponiveis = new List<Vector2Int>();

        // Definindo a área central do tabuleiro
        int margem = 2;

        for (int x = tamanhoTabuleiro / 2 - margem; x <= tamanhoTabuleiro / 2 + margem; x++)
        {
            for (int z = tamanhoTabuleiro / 2 - margem; z <= tamanhoTabuleiro / 2 + margem; z++)
            {
                if (x >= 0 && x < tamanhoTabuleiro && z >= 0 && z < tamanhoTabuleiro)
                {
                    posicoesDisponiveis.Add(new Vector2Int(x, z));
                }
            }
        }

        // Embaralha a lista de posições disponíveis
        for (int i = posicoesDisponiveis.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = posicoesDisponiveis[i];
            posicoesDisponiveis[i] = posicoesDisponiveis[j];
            posicoesDisponiveis[j] = temp;
        }

        // Garante que só sejam criados  buracos
        int buracosCriados = 0;
        for (int i = 0; i < posicoesDisponiveis.Count && buracosCriados < numeroBuracos; i++)
        {
            Vector2Int posicao = posicoesDisponiveis[i];
            Vector3 posicaoMundo = new Vector3(0, 0, 0);

            // Cria o buraco
            //GameObject buraco = GameObject.CreatePrimitive(PrimitiveType.Cube);
            /*GameObject buraco = new GameObject();
            buraco.transform.position = posicaoMundo;
            buraco.transform.localScale = new Vector3(tamanhoCasa, 0.1f, tamanhoCasa); // Define a escala dos buracos
            buraco.GetComponent<Renderer>().material.color = corBuraco;
            buraco.name = $"Buraco {posicao.x},{posicao.y}";
            */
            posicoesBuracos.Add(posicao);
            buracosCriados++;
        }

        _casa = GameObject.FindGameObjectsWithTag("Casa");
        foreach (Vector2 b in posicoesBuracos)
        {
            foreach (GameObject c in _casa)
            {
                if (c.transform.position.x == b.x && c.transform.position.z == b.y)
                {
                    c.SetActive(false);
                }
            }
        }
    }

    private void AdicionarElevacoes()
    {
        float tamanhoCasa = 1.0f;
        List<Vector2Int> posicoesDisponiveis = new List<Vector2Int>();

        for (int x = 2; x < tamanhoTabuleiro - 2; x++)
        {
            for (int z = 0; z < tamanhoTabuleiro; z++)
            {
                if (!posicoesBuracos.Contains(new Vector2Int(x, z)))
                {
                    posicoesDisponiveis.Add(new Vector2Int(x, z));
                }
            }
        }

        // Embaralha a lista de posições disponíveis
        for (int i = posicoesDisponiveis.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = posicoesDisponiveis[i];
            posicoesDisponiveis[i] = posicoesDisponiveis[j];
            posicoesDisponiveis[j] = temp;
        }

        for (int i = 0; i < numeroElevacoes; i++)
        {
            if (i >= posicoesDisponiveis.Count) break;

            Vector2Int posicao = posicoesDisponiveis[i];
            Vector3 posicaoMundo = new Vector3(posicao.x * tamanhoCasa, 0, posicao.y * tamanhoCasa);
            GameObject elevacao = GameObject.CreatePrimitive(PrimitiveType.Cube);
            elevacao.transform.position = posicaoMundo + new Vector3(0, alturaElevacao / 2, 0);
            elevacao.transform.localScale = new Vector3(tamanhoCasa, alturaElevacao, tamanhoCasa); // Define a escala da elevação
            elevacao.GetComponent<Renderer>().material.color = corElevacao;
            elevacao.name = $"Elevacao {posicao.x},{posicao.y}";
            _casaOcupada[posicao.x, posicao.y] = true;
        }
    }

    // Verifica se a posição está vazia
    public bool IsPositionEmpty(Vector3 targetPos)
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
