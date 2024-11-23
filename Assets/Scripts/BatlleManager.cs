using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Gerenciador de turno
    public TurnManager turnManager;

    // Gerenciador de peças
    public PieceManager pieceManager;

    private void Update()
    {
        // Verifica se o jogador 1 clicou no tabuleiro
        if (Input.GetMouseButtonDown(0) && turnManager.currentTurn == TurnManager.Turn.player1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verifica se o clique atingiu uma casa no tabuleiro
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Casa"))
                {
                    Vector3 targetPosition = hit.point;
                    Attack(targetPosition);
                }
            }
        }
    }

    public void Attack(Vector3 targetPosition)
    {
        Attack(targetPosition, pieceManager);
    }

    // Ataca uma posição no tabuleiro
    public void Attack(Vector3 targetPosition, PieceManager pieceManager)
    {
        // Verifica se a posição está dentro do alcance de ataque
        if (pieceManager.CanMovePiece("WhitePawn", targetPosition))
        {
            // Implementa o ataque
            // ...

            // Troca o turno após o ataque
            turnManager.SwitchTurn();
        }
    }
}

class pieceManager
{
    internal bool CanMovePiece(string v, Vector3 targetPosition)
    {
        throw new NotImplementedException();
    }
}