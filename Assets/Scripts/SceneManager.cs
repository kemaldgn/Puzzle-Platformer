using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("R"))   
            ReloadScene();
    }

    void ReloadScene(){
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
