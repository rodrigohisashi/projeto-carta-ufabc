using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Garimpeiro : MonoBehaviour
{

    static public Garimpeiro S;

    [Header("Set in Inspector")]
    public TextAsset layoutXML;

    public float xOffset = 0f;

    public float yOffset = 0f;

    public Vector3 layoutCenter;

    public bool startFaceUp = true;

    public Sprite cartaBack;

    public GameObject prefabCarta;

    public GameObject prefabSprite;

    [Header("Set Dynamically")]

    public Baralho baralho; 

    public Layout layout;


    public List<CartaGarimpeiro> monte;

    public List<string> nomesCartas;

    public List<Carta> cartasBaralho;

    public Transform pivoBaralho;

    public CartaGarimpeiro target;

    public List<CartaGarimpeiro> tablado;

    public List<CartaGarimpeiro> descarte;

    static public GameObject PanelGanhar; // Painel a ser exibido quando o jogador ganha

    static public GameObject PanelPerder; // Painel a ser exibido quando o jogador ganha

   public AudioClip musicaDeVitoria; // Adicione a música de vitória aqui no Editor do Unity
   public AudioClip musicaDerrota; // Adicione a música de vitória aqui no Editor do Unity

    private AudioSource audioSource; // Referência ao componente AudioSource
    

    public Button botaoEnviar;
    public TMP_InputField inputField;

    void Awake() {
        S = this;
        PanelGanhar = GameObject.Find("PanelGanhar");
        PanelPerder = GameObject.Find("PanelPerder"); 
        audioSource = GetComponent<AudioSource>(); // Obtém o componente AudioSource do objeto atual
     
        // Encontre o botão pelo nome e adicione um ouvinte de clique
        botaoEnviar = GameObject.Find("Enviar").GetComponent<Button>();
        botaoEnviar.onClick.AddListener(EnviarNomeDoJogador);

        // Encontre o InputField pelo nome
        inputField = GameObject.Find("ColocarNome").GetComponent<TMP_InputField>();
   
    }

    void Start() {
        baralho = GetComponent<Baralho>();
        Baralho.Embaralha(ref cartasBaralho);
        PanelGanhar.SetActive(false);
        PanelPerder.SetActive(false);
        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);
        monte = ConverteListCartasToListCartasGarimpeiro(baralho.cartasBaralho);
        LayoutGame();
        // if (target == null) {
        //     MoveParaTarget(Draw());
        //     MoveParaDescarte(target);
        //     UpdateMonte();

        // }

    }

    void LayoutGame() {
        if (pivoBaralho == null) {
            GameObject tGO = new GameObject("_pivoBaralho");
            pivoBaralho = tGO.transform;
            pivoBaralho.transform.position = layoutCenter;
        }
        CartaGarimpeiro cp;
        foreach (SlotDef tSD in layout.slotDefs) {
            cp = Draw();
            cp.faceUp = tSD.faceUp;
            cp.transform.parent = pivoBaralho;
            cp.transform.localPosition = new Vector3(
                layout.multiplicador.x * tSD.x,
                layout.multiplicador.y * tSD.y + 5,
                -tSD.layerID);
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            cp.state = eCartaState.tablado;
            cp.SetSortingLayerName(tSD.layerName);
            tablado.Add(cp);
        }
        foreach (CartaGarimpeiro tCP in tablado) {
            foreach( int hid in tCP.slotDef.hiddenBy) {
                cp = BuscaCartaPeloLayoutID(hid);
                tCP.hiddenBy.Add(cp);
            }
        }
        MoveParaTarget(Draw());
        UpdateMonte();

    }


    CartaGarimpeiro Draw() {
        if (monte.Count > 0) {
            CartaGarimpeiro cd = monte[0];
            monte.RemoveAt(0);
            return cd;
        } else {
            Debug.LogWarning("Tentativa de desenhar de uma monte vazia.");
            return null; // ou faça outra ação apropriada quando o monte estiver vazio
        }
    }
    
    void MoveParaDescarte(CartaGarimpeiro ct) {
        ct.state = eCartaState.descarte;
        descarte.Add(ct);
        ct.transform.parent = pivoBaralho;
        ct.transform.localPosition = new Vector3(
            layout.multiplicador.x * layout.descarte.x,
            layout.multiplicador.y * layout.descarte.y,
            -layout.descarte.layerID+0.5f);
        ct.faceUp = true;
        ct.SetSortingLayerName(layout.descarte.layerName);
        ct.SetSortOrder(-100+descarte.Count);
   
    }

    void MoveParaTarget(CartaGarimpeiro ct) {
        if (target != null) MoveParaDescarte(target);
        target = ct;
        ct.state = eCartaState.target;
        ct.transform.parent = pivoBaralho;
        ct.transform.localPosition = new Vector3(
            layout.multiplicador.x * layout.descarte.x,
            layout.multiplicador.y * layout.descarte.y,
            -layout.descarte.layerID
        );
        ct.faceUp = true;
        ct.SetSortingLayerName(layout.descarte.layerName);
        ct.SetSortOrder(0);

    }

    void UpdateMonte() {
        CartaGarimpeiro ct;
        for (int i=0; i<monte.Count; i++) {
            ct = monte[i];
            ct.transform.parent = pivoBaralho;
            Vector2 dpSepara = layout.monte.espaco;
            ct.transform.localPosition = new Vector3(
                layout.multiplicador.x * (layout.monte.x + i*dpSepara.x),
                layout.multiplicador.y * (layout.monte.y + i*dpSepara.y),
                -layout.monte.layerID+0.1f*i
            );
            ct.faceUp = false;
            ct.state = eCartaState.monte;
            ct.SetSortingLayerName(layout.monte.layerName);
            ct.SetSortOrder(-10*i);
        }
    }

    public void CartaClicada(CartaGarimpeiro ct) {
   
        print(ct);
        print("caso " + ct.state);
        switch(ct.state) {
            case eCartaState.target:
                break;
            case eCartaState.monte:
                   if (monte.Count > 0) {
                    
                MoveParaDescarte(target);
                MoveParaTarget(Draw());
                UpdateMonte();
                ScoreManager.EVENT(eScoreEvent.monte);
                }
                break;
            case eCartaState.tablado:
                print("AAA");
                bool jogadaValida = true;
                if (!ct.faceUp) {
                    jogadaValida = false;
                }
                if (!ValorAdjacente(ct, target)) {
                    jogadaValida = false;
                }
                if (!jogadaValida) return;
                print("DEU ERRADO");
                tablado.Remove(ct);
                MoveParaTarget(ct);
                SetFacesTablado();
                ScoreManager.EVENT(eScoreEvent.mina);
                break;
        }
        VerificaGameOver();
    }

    void VerificaGameOver() {
        if (tablado.Count==0) {
            GameOver(true);
            return;
        }
        if (monte.Count>0) {
            return;
        }
        foreach (CartaGarimpeiro ct in tablado) {
            if (ValorAdjacente (ct, target)) {
                return;
            }
        }
        GameOver(false);
    }

    void GameOver(bool won) {
        if (won) {
           // print ("Game Over. Você VENCEU! :)");
           ScoreManager.EVENT(eScoreEvent.gameVitoria);
            if (PanelGanhar != null)
            {
                PanelGanhar.SetActive(true); // Ative o painel quando o jogador ganhar
            }
                // Tocar música de vitória
            if (musicaDeVitoria != null)
            {
                audioSource.Stop(); // Para a música de fundo atual, se houver alguma
                audioSource.clip = musicaDeVitoria; // Define a música de vitória como o clipe de áudio
                audioSource.Play(); // Toca a música de vitória
            }
        } else {
            // print ("Game Over. Derrota ... :(");
            ScoreManager.EVENT(eScoreEvent.gameDerrota);
            if (PanelPerder != null)
            {
                PanelPerder.SetActive(true); // Ative o painel quando o jogador ganhar
            }
             if (musicaDerrota != null)
            {
                audioSource.Stop(); // Para a música de fundo atual, se houver alguma
                audioSource.clip = musicaDerrota; // Define a música de vitória como o clipe de áudio
                audioSource.Play(); // Toca a música de vitória
            }
        }
    }

    public bool ValorAdjacente(CartaGarimpeiro c0, CartaGarimpeiro c1) {
        if (!c0.faceUp || !c1.faceUp) return (false);
        if (Math.Abs(c0.valor - c1.valor) == 1) {
            return(true);
        }
        if (c0.valor == 1 && c1.valor == 13) return (true);

        if (c0.valor == 13 && c1.valor == 1) return (true);
        return (false);


    }

    CartaGarimpeiro BuscaCartaPeloLayoutID(int layoutID) {
        foreach(CartaGarimpeiro tCP in tablado) {
            if (tCP.layoutID == layoutID) {
                return (tCP);
            }
        }
        return (null);
    }

    void SetFacesTablado() {
        foreach(CartaGarimpeiro ct in tablado) {
            bool faceUp = true;
            foreach (CartaGarimpeiro cover in ct.hiddenBy ) {
                if (cover.state == eCartaState.tablado) {
                    faceUp = false;
                }
            }
            ct.faceUp = faceUp;
        }
    }


    List<CartaGarimpeiro> ConverteListCartasToListCartasGarimpeiro(List<Carta> lCD) {
        List<CartaGarimpeiro> lCP = new List<CartaGarimpeiro>();
        CartaGarimpeiro tCP;
        foreach (Carta tCD in lCD) {
            tCP = tCD as CartaGarimpeiro;
            lCP.Add(tCP);

        }
        return (lCP);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

        // Crie um método para enviar o nome do jogador
    void EnviarNomeDoJogador() {
        // Atribua o nome do jogador à variável
        string nomeDoJogador = inputField.text; // Acesse o valor do texto do InputField

        // Acesse o script ScoreManager e use a função SavePlayerScore para salvar o nome do jogador e a pontuação
        ScoreManager.SavePlayerScore(nomeDoJogador, ScoreManager.SCORE_DA_PARTIDA_ANTERIOR);
        print("Enviou");
            // Desative os botões após o envio do nome do jogador
        if (botaoEnviar != null) {
            botaoEnviar.gameObject.SetActive(false);
        }

        if (inputField != null) {
            inputField.gameObject.SetActive(false);
        }
    }
}
