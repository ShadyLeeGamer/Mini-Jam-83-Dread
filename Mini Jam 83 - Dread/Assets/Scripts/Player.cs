using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //public bool camShouldFollowPlayer = true;
    public Character character;
    
    [SerializeField] bool yourTurn = true; 
    [SerializeField] DiceSystem diceSys;

    GameScreen gameScreen;

    void Start()
    {
        diceSys = gameObject.GetComponent<DiceSystem>();
        character = gameObject.GetComponent<Character>();

        gameScreen = GameScreen.Instance;
    }

    void Update()
    {

        //#fff movement
        //it is 0 2 1 3 because the clooiders are added clockwise to the noCollider array like this  {forward  , right , backward , left}
        if (gameScreen.PlayerIsOn)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && !character.getShouldMove() && character.noCollider[0] && yourTurn)//forward
            {
                character.setSteps(new Vector2(0f, 1f));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && !character.getShouldMove() && character.noCollider[2] && yourTurn)//backward
            {
                character.setSteps(new Vector2(0f, -1f));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && !character.getShouldMove() && character.noCollider[1] && yourTurn)//right
            {
                character.setSteps(new Vector2(1f, 0f));
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && !character.getShouldMove() && character.noCollider[3] && yourTurn)//left
            {
                character.setSteps(new Vector2(-1f, 0f));
            }
        }
        //#fff movement
        
    }

    public void changeTurns()
    {   
        yourTurn = !yourTurn;
        if(yourTurn)
        {
            diceSys.rollDice();
        }
    }
}