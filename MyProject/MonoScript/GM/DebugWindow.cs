using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugWindow : MonoBehaviour
{
    public Text fpsText;
    private static string windowText = "";
    public static void debug(string newString)
    {
        windowText = newString;
    }
    void Update()
    {
        if (GamePath.debug.debugMode)
        {
            fpsText.text  = windowText;
        }
    }
}