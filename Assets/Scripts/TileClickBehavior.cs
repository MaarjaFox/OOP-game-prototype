using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClickBehavior : MonoBehaviour
{
    // Start is called before the first frame update


    public MapManager _manager;

    public Light light;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    void OnMouseDown()
    {
        Console.WriteLine("Clicked");
       _manager.TileClicked(this.gameObject.GetComponentInParent<Hex>());
       
    }


    private void OnMouseEnter()
    {
     //    this.GetComponent<Renderer>().material.SetColor("_Color", _manager.hoverColor);
   //     Console.WriteLine("Hovering C" +this.gameObject.GetComponentInParent<Hex>().Q.ToString() + " R"+ this.gameObject.GetComponentInParent<Hex>().Q.ToString() );
        UIManager.Instance.SetHexText(this.gameObject.GetComponentInParent<Hex>().Id.ToString());
        light.gameObject.SetActive(true);

        _manager.TileHoverEntered(this.gameObject.GetComponentInParent<Hex>());
    }

    private void OnMouseExit()
    {
     //   this.GetComponent<Renderer>().material.SetColor("_Color", _manager.defaultColor);  
        light.gameObject.SetActive( false);
        _manager.TileHoverExited(this.gameObject.GetComponentInParent<Hex>());
    }

   


}
