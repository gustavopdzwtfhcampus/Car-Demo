using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Global static instance of game manager
    public static GameManager instance { get; private set; }
    private void Awake()
    {
        //If there is already an instance of a game manager, remove oneself
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            //Makes sure that this instance cannot be destroyed anymore
            DontDestroyOnLoad(this);
        }
    }

    public void UnloadObject(GameObject gameObject)
    {
        //If the object has a collider, deactivate it
        //Meaning the object cannot be interacted with via collisions anymore
        if (gameObject.GetComponent<Collider>())
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }

        //If the object has a mesh renderer, deactivate it
        //Meaning the object does not get visualized/rendered anymore
        if (gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void PlaySound(GameObject gameObject)
    {
        //If the object has an audiosource on it, play it
        if (gameObject.GetComponent<AudioSource>())
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
