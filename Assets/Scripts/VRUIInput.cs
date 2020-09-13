using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VRUIInput : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;

    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        //laserPointer.PointerClick -= HandlePointerClick;
        //laserPointer.PointerClick += HandlePointerClick;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            button.Select();
            e.target.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            Debug.Log("HandlePointerIn", e.target.gameObject);
        }
    }
    /*private void HandlePointerClick(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if(button != null)
        {
            if (e.target.CompareTag("scene"))
            {
                if(e.target.name.CompareTo("Home") == 0)
                {
                    SceneManager.LoadScene(0);
                }
                else if(e.target.name.CompareTo("Garage") == 0)
                {
                    SceneManager.LoadScene(1);
                }
                else if(e.target.name.CompareTo("Game") == 0)
                {
                    SceneManager.LoadScene(2);
                }
                else if(e.target.name.CompareTo("Go") == 0)
                {
                    SceneManager.LoadScene(3);
                }
            }
            
        }
    }*/

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {

        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            e.target.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            Debug.Log("HandlePointerOut", e.target.gameObject);
        }
    }
}