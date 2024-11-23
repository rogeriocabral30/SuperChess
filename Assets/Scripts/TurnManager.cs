using System;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    //  para representar os turnos
    public enum Turn { player1, player2 }

    // Turno atual
    public Turn currentTurn { get; private set; }

    // Texto para exibir o turno atual
    public Text turnText;

    // Gerenciador de peças
    public PieceManager pieceManager;

    private void Start()
    {
        // Inicia com o jogador 1
        currentTurn = Turn.player1;
        StartTurn();
    }

    // Inicia o turno do jogador atual
    public void StartTurn()
    {
        NotifyCurrentTurn();
        pieceManager.NotifyPieceManager(currentTurn);
    }

    // Notifica os jogadores sobre o turno atual
    public void NotifyCurrentTurn()
    {
        turnText.text = $"Turno atual: {currentTurn}";
        Debug.Log($"Turno atual: {currentTurn}");
    }

    // Troca o turno
    public void SwitchTurn()
    {
        currentTurn = (currentTurn == Turn.player1) ? Turn.player2 : Turn.player1;
        StartTurn();
    }

    internal void EndTurn()
    {
        throw new NotImplementedException();
    }
}
