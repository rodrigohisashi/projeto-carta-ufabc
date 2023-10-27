using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCartaState {
    monte,
    tablado,
    target,
    descarte
}


public class CartaGarimpeiro : Carta
{
    [Header("Set Dynamically: CartaGarimpeiro")]

    public eCartaState state = eCartaState.monte;

    public List<CartaGarimpeiro> hiddenBy = new List<CartaGarimpeiro>();

    public int layoutID;

    public SlotDef slotDef;

    override public void OnMouseDown() {
        Garimpeiro.S.CartaClicada(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
