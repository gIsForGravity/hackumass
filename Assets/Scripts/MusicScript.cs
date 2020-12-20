using UnityEngine;

public class MusicScript : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void CreateMusic()
    {
        Instantiate(Resources.Load<GameObject>("Music"));
    }
}
