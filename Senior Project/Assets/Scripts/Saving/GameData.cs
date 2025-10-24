using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float[] position;

    public GameData()
    {
        position = new float[3];
        position[0] = 0f;
        position[1] = 0f;
        position[2] = 0f;
    }

    public GameData(GameObject player)
    {
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
