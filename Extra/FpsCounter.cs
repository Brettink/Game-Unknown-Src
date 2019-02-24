using UnityEngine;

/// <summary>
/// ~  Fps counter for unity  ~
/// Brief : Calculate the FPS and display it on the screen 
/// HowTo : Create empty object at initial scene and attach this script!!!
/// </summary>
public class FpsCounter : MonoBehaviour
{
    // for ui.
    private int screenLongSide;
    private Rect boxRect;
    public Material textColor;
    private GUIStyle style = new GUIStyle();

    // for fps calculation.
    private int frameCount;
    private float elapsedTime;
    private double frameRate;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UpdateUISize();
    }

    /// <summary>
    /// Monitor changes in resolution and calcurate FPS
    /// </summary>
    private void Update()
    {
        // FPS calculation
        frameCount++;
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 0.5f)
        {
            frameRate = System.Math.Round(frameCount / elapsedTime, 1, System.MidpointRounding.AwayFromZero);
            frameCount = 0;
            elapsedTime = 0;

            // Update the UI size if the resolution has changed
            if (screenLongSide != Mathf.Max(Screen.width, Screen.height))
            {
                UpdateUISize();
            }
        }
    }

    /// <summary>
    /// Resize the UI according to the screen resolution
    /// </summary>
    private void UpdateUISize()
    {
        screenLongSide = Mathf.Max(Screen.width, Screen.height);
        var rectLongSide = screenLongSide / 10;
        style.fontSize = (int)(screenLongSide / 36.8);
		boxRect = new Rect(1, Screen.height - (style.fontSize * 6) - 1, rectLongSide, rectLongSide / 3);
        style.normal.textColor = Color.white;
    }

    /// <summary>
    /// Display FPS
    /// </summary>
    private void OnGUI()
    {
		Vector3 pos = GManager.playerLocation.position;
        GUI.Label(boxRect, " " + frameRate + "fps", style);
		GUI.matrix = Matrix4x4.Translate (Vector3.up*style.fontSize);
		GUI.Label (boxRect, " " + "Posistion X, Y, Z", style);
		GUI.matrix = Matrix4x4.Translate (Vector3.up*style.fontSize*2);
		GUI.Label (boxRect, " " + pos.ToString (), style);
    }
}