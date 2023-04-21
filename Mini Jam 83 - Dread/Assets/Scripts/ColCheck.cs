using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColCheck : MonoBehaviour
{
    
    public Character character;
    public int index;
    public bool wall;

    void Start()
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(0.5f , 1f , 0.5f);   
    }
    
    void OnTriggerEnter(Collider col)
    {   
        if(col.gameObject.tag == "wall")
        {
            Debug.Log("wall enter");
            character.noCollider[index] = false;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "wall")
        {
            Debug.Log("wall leave");
            character.noCollider[index] = true;
        }
    }
    

}
