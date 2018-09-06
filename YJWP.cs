using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YJWP : MonoBehaviour
{
    public List<GameObject> pages = new List<GameObject>();
    public int currentPageIndex = 0;
    public GameObject currentPage;
    public CanvasGroup cg;

    public bool fadingOut = false;
    public bool fadingIn = false;
    public float fadeRate = 8f;
    public float growRate = 0.5f;
    public Texture butTexture;

    void Update()
    {
        if (fadingOut)
            cg.alpha -= fadeRate * Time.deltaTime;
        if (fadingIn)
            cg.alpha += fadeRate * Time.deltaTime;
        currentPage.transform.localScale += Vector3.one * growRate * Time.deltaTime;
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 30, 0, 30, 30),butTexture,GUIStyle.none))
        {
            Application.Quit();
        }
    }
    public void newPage(int newpage)
    {
        if(newpage!=currentPageIndex)
        {

        }
    }
    public IEnumerator ChangePage(int newPage)
    {
        cg = currentPage.GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        fadingIn = false;
        fadingOut = true;

        while(cg.alpha>0)
        {
            yield return 0;
        }

        currentPage.SetActive(false);

        fadingIn = true;
        fadingOut = false;
        currentPageIndex = newPage;
        currentPage = pages[currentPageIndex];
        currentPage.SetActive(true);
        cg = currentPage.GetComponent<CanvasGroup>();
        cg.alpha = 0.0f;
        currentPage.transform.localScale = Vector3.one * 0.95f;
        while (cg.alpha < 1f || currentPage.transform.localScale.x < 1f)
            yield return 0;
        cg.alpha = 1f;
        currentPage.transform.localScale = Vector3.one;
        fadingIn = false;
//        yield return 0;

    } 
}
