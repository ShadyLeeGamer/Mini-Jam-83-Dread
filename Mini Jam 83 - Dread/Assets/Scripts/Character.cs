using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    //Ref = refrence
    //Pos = position
    //Ts = transform
    //Go = Game Object
    //Rb = Rigidbody
    //temp = temporary
    //Val = value

    /*
    //#ff0 
    //make sure you add a CharacterController in order for the script to work 

    //and if it isn't a player uncheck the isPlayer box from the inspector

    //the variable shouldMove is true when the character is moving you can use it in the animator check something near line 100 down
    //in the Update function //#84f

    //it is 0 2 1 3 because the clooiders are added clockwise to the noCollider array like this  {forward  , right , backward , left}

    //setSteps takes a Vector2 as a parameter and it represents the ovement on the x axis and the z axis 

    //Note that make sure you keep the values that you give to setSteps no bigger value than 1 or smaller than -1 because it 
    //will only increase the speed of the character if you want the character to move for more than one grid adjust the gridSize variable
    
    // if you want to change the speed change the characterSpeed variable

    EXAMPLE on how to implement movement
    if(Input.GetKeyDown(KeyCode.UpArrow) && !character.getShouldMove() && character.noCollider[0])//forward
    {
        character.setSteps(new Vector2(0f , 1f)); 
    }
    else if(Input.GetKeyDown(KeyCode.DownArrow) && !character.getShouldMove() && character.noCollider[2])//backward
    {
        character.setSteps(new Vector2(0f ,-1f));
    }
    else if(Input.GetKeyDown(KeyCode.RightArrow) && !character.getShouldMove() && character.noCollider[1])//right
    {
        character.setSteps(new Vector2(1f , 0f));
    }
    else if(Input.GetKeyDown(KeyCode.LeftArrow) && !character.getShouldMove() && character.noCollider[3])//left
    {
        character.setSteps(new Vector2(-1f, 0f));
    }            
    //#ff0
    */

    public GameObject collidersParent;
    public GameObject[] colliders = new GameObject[4];
    public bool[] noCollider = new bool[4];
    public float gridSize = 1.0f , characterSpeed = 2.5f , stepsCount = 0;
    public bool isPlayer = false;
    
    [SerializeField]ColCheck[] collidersRef = new ColCheck[4];
    [SerializeField]CharacterController controller;
    [SerializeField]Vector3 characterVelocity , startPos;
    [SerializeField]bool groundedCharacter , shouldMove = false, isStanding = true ;
    [SerializeField]float  gravityValue = -9.81f , standingAnimationDuration = 1.0f , crouchingAnimationDuration = 1.0f;
    [SerializeField]Vector2 steps = new Vector2(0f,0f);
    [SerializeField]Animator animator;
    
    bool  b = false , k = false , l = false;
    float f = 0.0f , gridSize_one , standing_endTime , crouching_endTime;
    void Start()
    {

        animator = gameObject.GetComponent<Animator>(); 

        gridSize_one = gridSize - 0.1f;
        f = transform.position.y;

        for(int i = 0 ; i < 4 ; i++)
        {
            noCollider[i] = true;
        }
        createColliders();
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.C) && !shouldMove && isPlayer && animator != null)
        {
            
            isStanding = !isStanding;
            if(isStanding)
            {
                l = true;
                standing_endTime = Time.time + standingAnimationDuration;
            }
            else
            {
                crouching_endTime = Time.time + crouchingAnimationDuration;
            }
            
        }

        if(animator != null)//#84f
        {
            animator.SetBool("shouldMove" , shouldMove);
            animator.SetBool("isStanding" , isStanding);
        }//#84f

        if(collidersParent != null)
        {
            collidersParent.transform.position = transform.position;
        }

        if( Time.time >=  standing_endTime && l )
        {
            l = false;
        }
            
        if(shouldMove && Time.time > standing_endTime && Time.time > crouching_endTime)
        {
            if(b)
            {
                startPos = transform.position;
                b = false; 
            }

            Move(steps);
            
            if(Vector3.Distance(startPos , transform.position) >= gridSize_one)
            {
                transform.position = Vector3.MoveTowards(transform.position ,startPos + new Vector3(steps.x , 0.0f, steps.y ) , 1f);
                if(!k)
                {
                    transform.position = new Vector3(transform.position.x , f , transform.position.z);
                    k = true;
                }
                b = true;
                shouldMove = false;
                steps = new Vector2(0f , 0f);
            }
        }
        
    }
    // axis.x is the horizontal movement (on the X axis)
    // axis.y i sthe vertical movement (on the Z axis)
    public void Move(Vector2 axis)
    {
        groundedCharacter = controller.isGrounded;
        if (groundedCharacter && characterVelocity.y < 0)
        {
            characterVelocity.y = 0f;
        }

        Vector3 move = new Vector3(isStanding ? axis.x:axis.x/2.0f , 0 , isStanding ? axis.y:axis.y/2.0f);
        controller.Move(move * Time.deltaTime * characterSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        characterVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(characterVelocity * Time.deltaTime);
    }

    public void createColliders()
    {
        collidersParent = new GameObject();
        collidersParent.transform.parent = transform.parent;
        collidersParent.transform.position = transform.position;

        for(int i = 0 ; i < 4 ; i++)
        {
            colliders[i] = new GameObject();
            colliders[i].transform.parent = collidersParent.transform;
            Rigidbody tempRb = colliders[i].AddComponent<Rigidbody>();
            tempRb.isKinematic = true;
            colliders[i].AddComponent<BoxCollider>();
            collidersRef[i] = colliders[i].AddComponent<ColCheck>();
            collidersRef[i].character = this;
            collidersRef[i].index = i;

            if(i == 3)
            {
                colliders[0].transform.position = new Vector3(0f , 2f , 0.8f);
                colliders[1].transform.position = new Vector3(0.8f , 2f , 0f);
                colliders[2].transform.position = new Vector3(0f , 2f ,-0.8f);
                colliders[3].transform.position = new Vector3(-0.8f, 2f , 0f);
            }
        }

    }


    //steps getter & setters
    public void setSteps(Vector2 val)
    {
        shouldMove = true;
        steps = val;
        stepsCount -= Mathf.Abs(val.x == 0.0f ? val.y : val.x ) * ( isStanding ? 1.0f : 2.0f ) ;
    }

    // in case you wanted to change the "steps" without the player moving which means not changing "shouldMove"
    public void just_setSteps(Vector2 val)
    {
        steps = val;
    }

    public Vector2 getSteps()
    {
        return steps;
    }
    //steps getter & setter

    //shouldMove getter and setter
    public void setShouldMove(bool val)
    {
        shouldMove = val;
    }

    public bool getShouldMove()
    {
        return shouldMove;
    }
    //shouldMove getter and setter
}
