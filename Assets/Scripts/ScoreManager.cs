using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public enum eScoreEvent {
    monte, 
    mina,
    minaOuro,
    gameVitoria,
    gameDerrota
}
public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S;
    static public int SCORE_DA_PARTIDA_ANTERIOR = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dynamically")]

    public int serie = 0;
    public int scoreRodada = 0;
    public int score = 0;

    TextMeshProUGUI scoreText;

    void Awake() {
        if (S == null) {
            S = this;
        } else {
            Debug.LogError("ScoreManager.Awake(): S já existe!");
        }
        if (PlayerPrefs.HasKey("GarimpeiroHighScore")) {
            HIGH_SCORE = PlayerPrefs.GetInt("GarimpeiroHighScore");
        }
        score += SCORE_DA_PARTIDA_ANTERIOR;
        SCORE_DA_PARTIDA_ANTERIOR = 0;
        S.scoreText = GetComponent<TextMeshProUGUI>();
    }

    static public void EVENT (eScoreEvent evt) {
        try {
            S.Event(evt);
        } catch (System.NullReferenceException nre) {
            Debug.LogError("ScoreManager:EVENT() Chamado enquanto S = null. \n"+nre);
        }
    }

    void Event(eScoreEvent evt) {

        switch(evt) {
            case eScoreEvent.monte:
            case eScoreEvent.gameVitoria:
            
            case eScoreEvent.gameDerrota:
                serie = 0;
                score += scoreRodada;
                scoreRodada = 0;
                break;
            case eScoreEvent.mina:
                serie++;
                scoreRodada += serie;
                break;
        }

        switch(evt) {
            case eScoreEvent.gameVitoria:
                SCORE_DA_PARTIDA_ANTERIOR = score;
                if (SCORE_DA_PARTIDA_ANTERIOR > HIGH_SCORE) {
                    HIGH_SCORE = SCORE_DA_PARTIDA_ANTERIOR;
                    PlayerPrefs.SetInt("GarimpeiroHighScore", score);
                }
                print(Garimpeiro.PanelGanhar);
       
                Transform textoDoPainelTransform = Garimpeiro.PanelGanhar.transform.Find("PontuacaoFinal");
                print(textoDoPainelTransform);
     
                TextMeshProUGUI textoMeshDoPainel = textoDoPainelTransform.GetComponent<TextMeshProUGUI>();
                if (textoMeshDoPainel != null)
                {
                    textoMeshDoPainel.text = "PONTUAÇÃO FINAL: " + score.ToString();
                }
            
                 
                print("VITÓRIA! PONTOS DESTA PARTIDA: " + score);
                break;
            case eScoreEvent.gameDerrota:
                if (HIGH_SCORE <= score) {
                    print("Você teve uma pontuação alta! High score: " +score);
                } else {
                    print("Sua pontuação no game foi: "+ score);
                }
                break;
            default:
                scoreText.text = "Total: " + score.ToString() + " Rodada:" + scoreRodada.ToString() + " Série: " + serie.ToString();
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        S.scoreText.text = "Início, Record Atual = " + HIGH_SCORE;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    static public void SavePlayerScore(string playerName, int playerScore) {
        // Crie uma lista de jogadores e pontuações se não existir
        if (!PlayerPrefs.HasKey("PlayerList")) {
            PlayerPrefs.SetString("PlayerList", "");
        }

        // Adicione o novo jogador e sua pontuação à lista existente
        string playerList = PlayerPrefs.GetString("PlayerList");
        playerList += playerName + ":" + playerScore.ToString() + ",";
        PlayerPrefs.SetString("PlayerList", playerList);
    }

    // Adicione essa função para carregar o ranking de jogadores
    static public string LoadPlayerScores() {
        if (!PlayerPrefs.HasKey("PlayerList")) {
            return "Nenhum registro encontrado";
        }

        // Obtenha a lista de jogadores e pontuações
        string playerList = PlayerPrefs.GetString("PlayerList");
        return playerList;
    }

      public static void ClearPlayerData()
    {
        // Lógica para limpar os dados de jogador, como excluir arquivos, limpar bancos de dados, ou reinicializar variáveis de armazenamento de dados.
        // Por exemplo, se você estiver usando PlayerPrefs, pode usar o seguinte código para limpar os dados:
        PlayerPrefs.DeleteAll();
    }
}
