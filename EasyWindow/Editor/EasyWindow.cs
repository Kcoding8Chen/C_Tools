using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

/// <summary>
/// 功能：虚拟按键输入
/// </summary>
public class VirtualKeyboard : MonoBehaviour
{
    [DllImport("user32.dll",EntryPoint = "keybd_event")]
    public static extern void keybd_event(
        byte bvk , //虚拟按键
        byte bScna , //0
        int dwFlags , //0按下，1按住，2释放
        int dwExtraInfo //0
    );
}

/// <summary>
/// 功能：场景运行暂停跳转  游戏速度调整    展开/折叠层级
/// </summary>
public class EasyWindow : EditorWindow
{
    bool fpsInit = true;
    bool fpsOpen = false;
    float gameSpeed = 1f;
    int sceneCount = 1;
    int sceneIndex = 0;

    [MenuItem("EasyWindow/测试助手")]
    private static void ShowWindow()
    {
        Debug.Log("测试助手已开启");
        var window = GetWindow<EasyWindow>();
        window.titleContent = new GUIContent("测试助手");
        window.Show();
    }

    [MenuItem("EasyWindow/当前时间")]
    private static void ShowTimeNow()
    {
        System.DateTime NowTime = System.DateTime.Now;
        Debug.Log($"当前时间：{NowTime}");
    }

    [MenuItem("EasyWindow/展开所有层级")]
    static void foldSelection()
    {
        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        var hierarchyWindow = EditorWindow.focusedWindow;
        var expandMethodInfo = hierarchyWindow.GetType().GetMethod("SetExpandedRecursive");
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            expandMethodInfo.Invoke(hierarchyWindow, new object[] { root.GetInstanceID(), true });
        }
    }

    [MenuItem("EasyWindow/折叠所有层级")]
    static void UnfoldSelection()
    {
        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        var hierarchyWindow = EditorWindow.focusedWindow;
        var expandMethodInfo = hierarchyWindow.GetType().GetMethod("SetExpandedRecursive");
        foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            expandMethodInfo.Invoke(hierarchyWindow, new object[] { root.GetInstanceID(), false });
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("快捷按钮",EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("开始/结束"))
        {
            //先按下再释放  【Ctrl + P】
            VirtualKeyboard.keybd_event(17,0,0,0);
            VirtualKeyboard.keybd_event(80,0,0,0);
            VirtualKeyboard.keybd_event(17,0,2,0);
            VirtualKeyboard.keybd_event(80,0,2,0);
            AllReset();
            Debug.Log("开始/结束");
        }

        if(GUILayout.Button("暂停/继续"))
        {
            //先按下再释放  【Ctrl + Shift + P】
            VirtualKeyboard.keybd_event(17,0,0,0);
            VirtualKeyboard.keybd_event(16,0,0,0);
            VirtualKeyboard.keybd_event(80,0,0,0);
            VirtualKeyboard.keybd_event(17,0,2,0);
            VirtualKeyboard.keybd_event(16,0,2,0);
            VirtualKeyboard.keybd_event(80,0,2,0);
            Debug.Log("暂停/继续");
        }

        if(GUILayout.Button("随机跳关"))
        {
            sceneCount = SceneManager.sceneCountInBuildSettings;
            sceneIndex = Random.Range(0,sceneCount);
            Debug.Log(sceneIndex);
            AllReset();
            SceneManager.LoadScene(sceneIndex);
            Debug.Log("随机跳关:    "+SceneManager.GetSceneByBuildIndex(sceneIndex).name);
        }

        if(GUILayout.Button("重玩这关"))
        {
            AllReset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("重玩:    "+SceneManager.GetActiveScene().name);
        }




        gameSpeed = EditorGUILayout.Slider("变速齿轮",gameSpeed,0f,4f);
        Time.timeScale = gameSpeed;
        // if(Time.timeScale!=1f)
        // {
        //     Debug.Log("当前游戏运行速度:    "+gameSpeed+"x");
        // }
        EditorGUILayout.Space();


        if(GUILayout.Toggle(fpsOpen,"FPS开启"))
        {
            if(fpsInit){
                fpsInit = false;
                FpsTool.initToScene();
            }
            fpsOpen = true;
        }
        if(GUILayout.Toggle(!fpsOpen,"FPS关闭"))
        {
            FpsTool.isshow = false;
            FpsBoolReset();
        }
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("Label");
        GUILayout.Label("使用说明：  ");
        GUILayout.Label("随机跳关: 随机加载BuildSetting内的一个场景");
        GUILayout.Label("重玩这关: 重新加载当前游戏场景");
        GUILayout.Label("变速齿轮: 拖动滑条控制游戏运行速度");
        EditorGUILayout.EndVertical();

        GUILayout.Label("----------------------------------------");
        GUILayout.Label("场景跳转Tips:");
        GUILayout.Label("自动清除控制台日志,打印输出当前场景名称");
        GUILayout.Label("自动关闭FPS,恢复游戏运行速度为1");
        GUILayout.Label("----------------------------------------");
        EditorGUILayout.Space();

    }

    //总重置
    private void AllReset()
    {
        TimescaleReset();
        FpsBoolReset();
        ConsoleCleaner();
    }

    //重置FPS相关参数设置
    private void FpsBoolReset()
    {
        fpsInit = true;
        fpsOpen = false;
    }

    //重置游戏速度
    private void TimescaleReset()
    {
        gameSpeed = 1f;
    }

    //控制台日志清除
    private void ConsoleCleaner()
    {
        #if UNITY_EDITOR
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
        System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
        System.Reflection.MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
        clearConsoleMethod.Invoke(new object(), null);
        #endif
    }
}