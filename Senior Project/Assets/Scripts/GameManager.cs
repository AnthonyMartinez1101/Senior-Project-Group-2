using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TileManager tileManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (tileManager == null)
        {
            tileManager = Object.FindFirstObjectByType<TileManager>();
        }
    }
}
