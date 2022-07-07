using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    public GameObject[] players;
    [SerializeField] GameObject currentCharacter;

    bool change;
    int index=0;
    [SerializeField] float energy;

    // players[0] is the player, players[1] is the dark energy ghost.
    void Awake()
    {
        players[0].SetActive(false);
        players[1].SetActive(false);
    }

    void Start() // disables the 
    {
        players[index].SetActive(true);
        currentCharacter = players[0];
    }

    // Update is called once per frame
    void Update()
    {
        print(index);
        if(Input.GetKeyDown(KeyCode.Tab)) {     
            
            if(index ==0 && energy > 0)
            {
                ChangeCharacterToGhost();
                currentCharacter = players[1];
                index++;
            }

            else 
            {
                ChangeGhostToCharacter();
                currentCharacter = players[0];
                index--;
            }
            
    }
    }

    void ChangeCharacterToGhost(){ // index keeps the current player
        
        players[0].SetActive(false);
        players[1].SetActive(true);
    }
    void ChangeGhostToCharacter(){ // index keeps the current player
        
        players[1].SetActive(false);
        players[0].SetActive(true);
    }
}
