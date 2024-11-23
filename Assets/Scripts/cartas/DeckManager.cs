using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardManager> cartasDisponiveis; // Lista de todas as cartas possíveis
    public List<CardManager> cartasNoDeck; // Cartas atuais no deck
    public GameObject cartaCampo, cartaTransformacao, cartaHabilidade;
    private void Start()
    {
        // Preenche o deck com as cartas disponíveis
        CriarDeck();
    }

    // Cria um deck de cartas 
    private void CriarDeck()
    {
        cartasNoDeck = new List<CardManager>(cartasDisponiveis);
        EmbaralharDeck();

        foreach (var card in cartasNoDeck)
        {
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                Instantiate(cartaCampo);
            }
            if (i == 1)
            {
                Instantiate(cartaTransformacao);
            }
            if (i == 2)
            {
                Instantiate(cartaHabilidade);
            }
        }
    }

    // Função para embaralhar o deck
    public void EmbaralharDeck()
    {
        for (int i = 0; i < cartasNoDeck.Count; i++)
        {
            CardManager temp = cartasNoDeck[i];
            int randomIndex = Random.Range(i, cartasNoDeck.Count);
            cartasNoDeck[i] = cartasNoDeck[randomIndex];
            cartasNoDeck[randomIndex] = temp;
        }
    }

    // Comprar uma carta do deck
    public CardManager ComprarCarta()
    {
        if (cartasNoDeck.Count > 0)
        {
            CardManager cartaComprada = cartasNoDeck[0];
            cartasNoDeck.RemoveAt(0);
            return cartaComprada;
        }
        return null;
    }

    // Jogar uma carta
    public void JogarCarta(CardManager carta, TabuleiroDamas tabuleiro, PieceManager pieceManager)
    {
        carta.UsarCarta(tabuleiro, pieceManager);
    }
}
