using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTextBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateText());
    }

    public IEnumerator UpdateText()
    {
        while (true)
        {
            this.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            this.GetComponent<TextAnimatorPlayer>().ShowText("Loading...");
            yield return new WaitForSeconds(2.5f);
        }
    }
}
