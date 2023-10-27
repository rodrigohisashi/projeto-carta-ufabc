using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carta : MonoBehaviour
{
    [Header("Set Dynamically")]

    public Sprite sprite;

    public string naipe;

    public int valor;

    public GameObject back = null;

    public SpriteRenderer[] spriteRenderers;
//    public bool faceUp = true;

    public bool faceUp {
        get {return(!back.activeSelf);
        }
        set{back.SetActive(!value);}
    }
    // Start is called before the first frame update

    SpriteRenderer _tSR = null;

   virtual public void OnMouseDown() {

        print(valor+naipe);
        // _tSR = GetComponent<SpriteRenderer>();
        // if (_tSR.sortingOrder==3) _tSR.sortingOrder=1;
        // else {
        //     Destroy(gameObject);
        // }
    }

    public void PopulateSpriteRenderers() {
        if (spriteRenderers == null || spriteRenderers.Length == 0) {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }

    public void SetSortingLayerName(string tSLN) {
        PopulateSpriteRenderers();
        foreach(SpriteRenderer tSR in spriteRenderers) {
            tSR.sortingLayerName = tSLN;
        }
    }

    public void SetSortOrder(int sOrd) {
        PopulateSpriteRenderers();
        foreach(SpriteRenderer tSR in spriteRenderers) {
            switch(tSR.gameObject.name) {
                case "back":
                    tSR.sortingOrder = sOrd + 1;
                    break;
                case "face":
                defaul:
                    tSR.sortingOrder = sOrd;    
                    break;
                
            }
        }
    }

    void Start()
    {
       SetSortOrder(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
