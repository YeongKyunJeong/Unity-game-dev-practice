//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class Portal : Collidable
{
    public string[] sceneNames;
    public int connectedPlaceNumber = 1;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            //Teleport the player, 랜덤 던전으로 이동
            GameManager.instance.SaveState();
            //string sceneName = sceneNames[Random.Range(0, sceneNames.Length)];
            string sceneName = sceneNames[1];
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}