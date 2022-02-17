using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePopupBehavior : MonoBehaviour
{
    public void OnCloseClicked()
    {
        GetComponent<UIPopup>().Hide();
    }
}
