using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPopup : MonoBehaviour
{
    public void OnCloseClicked()
    {
        GetComponent<UIPopup>().Hide();
    }
}
