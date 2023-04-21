using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSystem : MonoBehaviour
{
    Character character;
    public int diceMin = 1 , diceMx = 4;
    System.Random random;
    
    void Start()
    {
        System.Random random = new System.Random(); 
        character = gameObject.GetComponent<Character>();
        character.isPlayer = true;    
    }

    public void rollDice()
    {
        character.stepsCount = random.Next(diceMin, (diceMx+1));
        Debug.Log(character.stepsCount);
    }
}
