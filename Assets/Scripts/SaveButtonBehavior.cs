using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButtonBehavior : MonoBehaviour
{
    public void OnSaveClicked()
    {
        SerializationManager.Save("test", SaveDataInfo.GenerateSaveData());
    }

    public void OnLoadClicked()
    {
        SaveDataInfo saveInfo = SerializationManager.Load("test") as SaveDataInfo;
        this.StartCoroutine(saveInfo.Apply());
    }
}
