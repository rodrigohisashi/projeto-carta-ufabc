using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText;

    void Start()
    {
        // Recupere os dados de pontuação salvos usando o script ScoreManager
        string playerList = ScoreManager.LoadPlayerScores();
        
        if ((playerList.Equals("Nenhum registro encontrado")))
        {
            rankingText.text = "Nenhum registro encontrado!";
            Debug.Log("Nenhum registro encontrado.");
            return;
        }
         Debug.Log("Player: " + playerList );
        // Separe os dados de pontuação em uma lista de jogadores e pontuações
        string[] players = playerList.Split(',');
        
        // Classifique os dados de pontuação em ordem decrescente com base na pontuação
        System.Array.Sort(players, (player1, player2) => {
            string[] playerData1 = player1.Split(':');
            string[] playerData2 = player2.Split(':');
            if (playerData1.Length >= 2 && playerData2.Length >= 2)
            {
                int score1 = int.Parse(playerData1[1]);
                int score2 = int.Parse(playerData2[1]);
                return score2.CompareTo(score1);
            }
            return 0;
        });
        // Exiba os três principais resultados na cena "Ranking"
        rankingText.text = "Top 3 Ranking:\n";
        rankingText.text += "Nome - Score\n";
        for (int i = 0; i < Mathf.Min(3, players.Length-1); i++)
        {
            
            Debug.Log("ET: " +   players[0] );
            string[] playerData = players[i].Split(':');
            string playerName = playerData[0];
            Debug.Log("AAA: " +  playerData[0] );
    
            string playerScore = playerData[1];
            rankingText.text += playerName + " - " + playerScore + "\n";
        }
    }

    public void DeletarTudo()
    {
        print("deletou");
        ScoreManager.ClearPlayerData();
    }
}
