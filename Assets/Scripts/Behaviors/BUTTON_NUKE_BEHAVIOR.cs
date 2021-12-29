using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BUTTON_NUKE_BEHAVIOR : MonoBehaviour
{
    public Button yourButton;

    public GameObject Nuke;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(OnClicked);
    }
    void OnClicked()
    {
        SelectionManager.SelectedGameObject = Nuke;
    }
}
