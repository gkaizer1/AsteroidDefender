using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGridBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> VerticalLines = new List<GameObject>();
    public List<GameObject> HorizontalLines = new List<GameObject>();

    public int VerticalLineCount = 10;
    public int HorizontalLineCount = 10; 

    public static readonly float LINE_WIDTH = 2.0f;
    void Start()
    {
        EventManager.OnCameraOrthograficSizeChange += OnCameraOrthograficSizeChange;
        EventManager.OnWorldSizeChanged += OnWorldSizeChanged;
    }

    private void OnWorldSizeChanged()
    {
        HorizontalLineCount = (int)Mathf.Ceil(Utils.AspecRatio * (float)VerticalLineCount);
        GenerateHorizontalLines();
    }

    private void GenerateHorizontalLines()
    {
        //float leftX = 0.0f;// -ZoomBehavior.MAX_WORLD_WIDTH / 2.0f;

        VerticalLines.ForEach(x => Destroy(x.gameObject));
        VerticalLines.Clear();

        HorizontalLines.ForEach(x => Destroy(x.gameObject));
        HorizontalLines.Clear();

        for (int i = 1; i < VerticalLineCount; i++)
        {
            var obj = Instantiate(Resources.Load("grid_line_prefab")) as GameObject;
            VerticalLines.Add(obj);

            obj.transform.parent = this.gameObject.transform;
            obj.name = $"grid_vertical_{i}";
            var line = obj.GetComponent<LineRenderer>();
            line.startWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;
            line.endWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;

            //float xPosition = leftX + ZoomBehavior.MAX_WORLD_WIDTH * ((float)i / (float)VerticalLineCount);
            //line.SetPosition(0, new Vector3(xPosition, -ZoomBehavior.MAX_WORLD_HEIGHT, 1.0f));
            //line.SetPosition(1, new Vector3(xPosition, ZoomBehavior.MAX_WORLD_HEIGHT, 1.0f));
        }

        for (int i = 1; i < HorizontalLineCount; i++)
        {
            var obj = Instantiate(Resources.Load("grid_line_prefab")) as GameObject;
            HorizontalLines.Add(obj);

            obj.transform.parent = this.gameObject.transform;
            obj.name = $"grid_horizontal_{i}";
            var line = obj.GetComponent<LineRenderer>();
            line.startWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;
            line.endWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;

            //float yPosition = ZoomBehavior.MAX_WORLD_HEIGHT / 2.0f  - ZoomBehavior.MAX_WORLD_HEIGHT * ((float)i / (float)VerticalLineCount);
            //line.SetPosition(0, new Vector3(ZoomBehavior.MAX_WORLD_WIDTH / 2.0f, yPosition, 1.0f));
            //line.SetPosition(1, new Vector3(-ZoomBehavior.MAX_WORLD_WIDTH / 2.0f, yPosition, 1.0f));
        }

    }

    private void OnCameraOrthograficSizeChange(float orthoSize)
    {
        // Update all the lines widths
        VerticalLines.ForEach(x =>
        {
            var line = x.GetComponent<LineRenderer>();
            line.startWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;
            line.endWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;
        });
        // Update all the lines widths
        HorizontalLines.ForEach(x =>
        {
            var line = x.GetComponent<LineRenderer>();
            line.startWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;
            line.endWidth = Utils.ScreenToWorldWidthPixel * LINE_WIDTH;
        });
    }
}
