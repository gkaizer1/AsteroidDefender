using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Globalization;

[ExecuteInEditMode]
public class BuildCostTextBehavior : MonoBehaviour
{
    public TileCost tileCost;

    [Header("Prefabs")]
    public TextMeshProUGUI textObjectPrefab;
    public Image imagePrefab;
    public Image DividerPrefab;

    [HideInInspector]
    public Canvas container;

    public ResourceDefinitions resourceDefinitions;

    public bool Initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        container = GetComponent<Canvas>();
        foreach (Transform child in this.transform)
        {
            Destroy(child);
        }

        Canvas.ForceUpdateCanvases();

        foreach (Transform child in this.transform)
            DestroyImmediate(child.gameObject, true);

        foreach (var cost in tileCost.tileCost)
        {
            Instantiate(DividerPrefab, this.transform);
            var resourceDefinition = resourceDefinitions.resources.FirstOrDefault(x => x.resource == cost.resource);

            var image = Instantiate(imagePrefab, this.transform);
            image.GetComponent<Image>().sprite = resourceDefinition.Image;

            var text = Instantiate(textObjectPrefab, this.transform);
            text.GetComponent<TextMeshProUGUI>().text = cost.Cost.ToString("N0", CultureInfo.CurrentCulture);
            //textObject.text = cost.Cost + " " + cost.resource.ToString();
        }
        Instantiate(DividerPrefab, this.transform);

        Canvas.ForceUpdateCanvases();
    }

    // Update is called once per frame
    void Start()
    {
        // On start - we disable/re-enable to force recomputation of children sizes
        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
    }
}
