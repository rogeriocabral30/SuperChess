using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    // Dicionário para armazenar todas as peças
    private Dictionary<string, GameObject> pieces = new Dictionary<string, GameObject>();

    // Gerenciadores de peças 
    public WhitePawnManager whitePawnManager;
    public whiteArcherManager whiteArcherManager;
    public whiteMageManager whiteMageManager;

    // Gerenciador de turno
    public TurnManager turnManager;

    private void Start()
    {
        // Inicializa o dicionário de peças
        InitializePieces();
    }

    // Inicializa o dicionário de peças
    private void InitializePieces()
    {
        // Adiciona as peças brancas
        pieces.Add("WhitePawn", whitePawnManager.gameObject);
        pieces.Add("WhiteArcher", whiteArcherManager.gameObject);
        pieces.Add("WhiteMage", whiteMageManager.gameObject);

        // Adiciona as peças pretas (futuramente você pode adicionar)
        // pieces.Add("BlackPawn", blackPawnManager.gameObject);
        // pieces.Add("BlackArcher", blackArcherManager.gameObject);
    }

    // Notifica as peças sobre o turno atual
    public void NotifyPieceManager(TurnManager.Turn turn)
    {
        // Aqui você pode habilitar ou desabilitar peças com base no turno atual
        Debug.Log($"Turno atual: {turn}");

        // Adicione lógica para ativar/desativar peças aqui, se necessário
    }

    // Retorna uma peça específica
    public GameObject GetPiece(string pieceName)
    {
        return pieces[pieceName];
    }

    // Verifica se uma peça pode se mover para uma posição
    public bool CanMovePiece(string pieceName, Vector3 targetPosition)
    {
        // Verifica se a peça existe
        if (!pieces.ContainsKey(pieceName))
        {
            return false;
        }

        // Verificação para o gerenciador 
        switch (pieceName)
        {
            case "WhiteMage":
                return whiteMageManager.CanMove(targetPosition);
            case "WhitePawn":
                return whitePawnManager.CanMove(targetPosition);
            case "WhiteArcher":
                return whiteArcherManager.CanMove(targetPosition);
            default:
                return false;
        }
    }

    // Atualiza o estado de uma peça
    public void UpdatePieceState(string pieceName, Vector3 newPosition)
    {
        // Verifica se a peça existe
        if (!pieces.ContainsKey(pieceName))
        {
            return;
        }

        // Atualização para o gerenciador 
        switch (pieceName)
        {
            case "WhiteMage":
                whiteMageManager.UpdatePosition(newPosition);
                break;
            case "WhitePawn":
                whitePawnManager.UpdatePosition(newPosition);
                break;
            case "WhiteArcher":
                whiteArcherManager.UpdatePosition(newPosition);
                break;
        }
    }
}
