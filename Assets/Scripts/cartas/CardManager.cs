using UnityEngine;
using UnityEngine.UI;

public enum TipoCarta
{
    transformacao,
    HabilidadeEspecial,
    Campo
}


public class CardManager : MonoBehaviour
{
    public string nome; // Nome da carta
    public TipoCarta tipo; // Tipo de carta (Magia, Habilidade, transformaçao )
    public string descricao; // (explicação do efeito)
    public Sprite imagem; // Imagem da carta,

    // Efeitos específicos 
    public void UsarCarta(TabuleiroDamas tabuleiro, PieceManager pieceManager)
    {
        switch (tipo)
        {
            case TipoCarta.transformacao:
                AplicarMelhoria(tabuleiro, pieceManager);
                break;
            case TipoCarta.HabilidadeEspecial:
                AplicarHabilidadeEspecial(tabuleiro, pieceManager);
                break;
            case TipoCarta.Campo:
                AplicarEvento(tabuleiro, pieceManager);
                break;
        }
    }

    // Função para aplicar melhorias nas peças 
    private void AplicarMelhoria(TabuleiroDamas tabuleiro, PieceManager pieceManager)
    {
        // Implementar o efeito de melhoria 
        Debug.Log($"Melhoria aplicada: {nome}");
    }

    // Função para habilidades especiais 
    private void AplicarHabilidadeEspecial(TabuleiroDamas tabuleiro, PieceManager pieceManager)
    {
        // Exemplo: invocar um arqueiro ou mago
        Debug.Log($"Habilidade especial usada: {nome}");
    }

    // Função para eventos no tabuleiro ( criar buracos, alterar tabuleiro)
    private void AplicarEvento(TabuleiroDamas tabuleiro, PieceManager pieceManager)
    {
        // Exemplo: criar um buraco ou elevação no tabuleiro
        Debug.Log($"Evento aplicado: {nome}");
    }
}


// testando github