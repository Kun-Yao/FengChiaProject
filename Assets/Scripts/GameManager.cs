using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public string CarName = null;
    public int relifePoint;
    private bool[] isEmpty = new bool[6];

    public static AudioClip BackSound;
    public static AudioClip FF;
    static AudioSource audioSrc;
    private bool Begin = false;



    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            if (!Begin)
            {
                BackSound = Resources.Load<AudioClip>("Sounds/back");
                audioSrc = GetComponent<AudioSource>();
                audioSrc.clip = BackSound;
                audioSrc.loop = true;
                audioSrc.volume = 0.5f;
                audioSrc.Play();
                Begin = true;
            }
            DontDestroyOnLoad(this);
        }
        else if(this != instance)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game" && Begin)
        {
            audioSrc.Stop();
            Begin = false;
        }
        else if(SceneManager.GetActiveScene().name != "Game" && !Begin)
        {
            audioSrc.clip = BackSound;
            audioSrc.loop = true;
            audioSrc.volume = 0.7f;
            audioSrc.Play();
            Begin = true;
        }
        
    }

    public void setName(string Name)
    {
        CarName = Name;
    }

    public string getName()
    {
        return CarName;
    }

    public void setEmpty(int index, bool empty)
    {
        isEmpty[index] = empty;
    }

    public bool getEmpty(int index)
    {
        return isEmpty[index];
    }

    //public Vector3 getLocation(int index)
    //{
    //    return coordinate[index];
    //}

    public bool canMove = false;

    public void ResetCar(string CarName)
    {
        print("relife in GM: "+relifePoint);
        string name = CarName;
        GameObject.Destroy(GameObject.Find(CarName));
        Transform relife = GameObject.Find("CheckPoints").transform.GetChild(relifePoint);
        GameObject newG = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/" + name), relife.position, Quaternion.Euler(0, 0, 0));
        newG.transform.localRotation = Quaternion.FromToRotation(newG.transform.forward, relife.right);
        newG.name = name;
        newG.GetComponent<CarController>().enabled = true;
        newG.GetComponent<Rigidbody>().useGravity = true;
        canMove = true;
    }

    public static void PlayFF7()
    {
        FF = Resources.Load<AudioClip>("Sounds/FF7");
        audioSrc.clip = FF;
        audioSrc.volume = 1.0f;
        audioSrc.loop = false;
        audioSrc.Play();
    }
}
