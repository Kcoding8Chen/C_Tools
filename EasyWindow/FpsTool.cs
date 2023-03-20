using UnityEngine;

/// <summary>
/// 功能：显示FPS帧数 此处已整合到EasyWindow窗口中
/// </summary>
public class FpsTool : MonoBehaviour
{
    //fps更新间隔
    const float UpdateInterval = 0.5f;
    //上一次更新的时间
    float m_LastUpdateTime;
    //帧数
    float m_FrameCount;
    //帧率
    float m_FPS;
    string fps_string;
    string time_string;
    public static bool isshow = false;
    Rect fps_ShowRect;
    Rect time_ShowRect;

    GUIStyle m_GuiStyle = new GUIStyle();
    public static void initToScene()
    {
        GameObject fpsTool = GameObject.Find("FpsTool");//脚本工具根节点
        if(!fpsTool)
        {
            fpsTool = new GameObject("FpsTool");//将脚本工具根节点添加到场景
            fpsTool.AddComponent<FpsTool>(); //添加FPS插件
        }
        isshow = true;
    }

    private void Start()
    {
        m_LastUpdateTime = Time.realtimeSinceStartup;
        m_FPS = 0;
        InitStyle();
    }

    void InitStyle()
    {
        fps_ShowRect = new Rect(0, 50, 200, 40);
        time_ShowRect = new Rect(0, 100, 200, 40);
        m_GuiStyle.normal.background = null;
        m_GuiStyle.normal.textColor = Color.red;
        m_GuiStyle.fontSize = 60;
        m_GuiStyle.fontStyle = FontStyle.Bold;
    }

    private void OnGUI()
    {
        if(isshow){
            fps_string = "FPS: "+ string.Format("{0:f1}",m_FPS);
            time_string = "当前关卡耗时: "+ string.Format("{0:f1}",Time.timeSinceLevelLoad);
        }else{
            fps_string = "";
            time_string = "";
        }
        GUI.Label(fps_ShowRect, fps_string, m_GuiStyle);
        GUI.Label(time_ShowRect, time_string, m_GuiStyle);
    }

    private void Update()
    {
        m_FrameCount++;
        float curTime = Time.realtimeSinceStartup;
        if (curTime - m_LastUpdateTime >= UpdateInterval)
        {
            m_FPS = m_FrameCount / (curTime - m_LastUpdateTime);
            m_LastUpdateTime = Time.realtimeSinceStartup;
            m_FrameCount = 0;
        }
    }

    private void OnDestroy()
    {
        Destroy(this.gameObject);
    }
}
