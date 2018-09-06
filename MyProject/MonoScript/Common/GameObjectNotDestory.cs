using UnityEngine;
using System.Collections;

public class GameObjectNotDestory : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}