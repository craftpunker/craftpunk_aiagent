using UnityEngine;

public class ReporterGUI : MonoBehaviour
{
    Reporter reporter;
    GUIContent content;
    GUIStyle btnStyle;

    public float updataInterval = 0.5f;
    private double lastInterval;
    private int frames = 0;//
    private float fps;
    void Awake()
    {
        reporter = gameObject.GetComponent<Reporter>();
    }
    private void Start()
    {
        content = new GUIContent(">", null, "Open log reporter");
        btnStyle = new GUIStyle();
        btnStyle.normal.background = reporter.images.barImage;
        btnStyle.border = new RectOffset(1, 1, 1, 1);
        btnStyle.alignment = TextAnchor.MiddleCenter;
        btnStyle.fontSize = (int)reporter.size.x;
        btnStyle.margin = new RectOffset(1, 1, 1, 1);
    }

    private void Update()
    {
        frames++;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updataInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }

    void OnGUI()
    {
        if (!reporter.show && GUILayout.Button(content, btnStyle, GUILayout.Width(reporter.size.x * 2), GUILayout.Height(reporter.size.y * 2)))
        {
            reporter.show = true;
        }
        GUI.skin.textField.fontSize = 60;
        GUILayout.TextField($"FPS:{fps}");
        reporter.OnGUIDraw();

    }
}
