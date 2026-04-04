using UnityEngine;

public class GameManager : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Game Quit");
            Application.Quit();
        }
    }
}
