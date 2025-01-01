/// <summary>
/// Author: Lele Feng 
/// </summary>

using UnityEngine;
using System.Collections;

public class ColorPicker : MonoBehaviour
{

    public BoxCollider pickerCollider;

    private bool m_grab;//是否正在进行颜色拾取操作
    private Camera m_camera;
    private Texture2D m_screenRenderTexture;//存储从屏幕渲染的纹理
    private static Texture2D m_staticRectTexture;//绘制表示所拾取颜色的矩形
    private static GUIStyle m_staticRectStyle;

    private static Vector3 m_pixelPosition = Vector3.zero;
    private Color m_pickedColor = Color.white;

    void Awake()
    {
        // Get the Camera component
        m_camera = GetComponent<Camera>();
        if (m_camera == null)
        {
            Debug.LogError("You need to dray this script to a camera!");
            return;
        }

        // Attach a BoxCollider to this camera
        // 为了接收鼠标事件
        if (pickerCollider == null)
        {
            pickerCollider = gameObject.AddComponent<BoxCollider>();
            //确保碰撞器在摄像机的截锥体中
            pickerCollider.center = Vector3.zero;
            pickerCollider.center += m_camera.transform.worldToLocalMatrix.MultiplyVector(m_camera.transform.forward) * (m_camera.nearClipPlane + 0.2f);
            pickerCollider.size = new Vector3(Screen.width, Screen.height, 0.1f);//大小设置为屏幕大小
        }
    }

    // Draw the color we picked
    public static void GUIDrawRect(Rect position, Color color)
    {
        if (m_staticRectTexture == null)
        {
            m_staticRectTexture = new Texture2D(1, 1);
        }

        if (m_staticRectStyle == null)
        {
            m_staticRectStyle = new GUIStyle();
        }

        m_staticRectTexture.SetPixel(0, 0, color);//设置像素颜色为传入的颜色值
        m_staticRectTexture.Apply();

        m_staticRectStyle.normal.background = m_staticRectTexture;

        GUI.Box(position, GUIContent.none, m_staticRectStyle);//应用到GUI中
    }

    //在相机完成渲染场景后每一帧都被调用
    void OnPostRender()
    {
        if (m_grab)
        {
            m_screenRenderTexture = new Texture2D(Screen.width, Screen.height);//创建一个与屏幕大小相同的纹理
            m_screenRenderTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);//读取像素信息到创建的渲染纹理中
            m_screenRenderTexture.Apply();//并应用
            m_pickedColor = m_screenRenderTexture.GetPixel(Mathf.FloorToInt(m_pixelPosition.x), Mathf.FloorToInt(m_pixelPosition.y));//获取点击位置的像素颜色
            m_grab = false;
        }
    }
    //鼠标在挂载了该脚本的游戏对象上按下时，这个函数会被调用
    void OnMouseDown()
    {
        m_grab = true;
        // Record the mouse position to pick pixel
        m_pixelPosition = Input.mousePosition;
    }

    void OnGUI()//UI 系统中绘制界面元素，每帧
    {
        GUI.Box(new Rect(0, 0, 120, 200), "Color Picker");
        GUIDrawRect(new Rect(20, 30, 80, 80), m_pickedColor);
        //指定double的小数位
        GUI.Label(new Rect(10, 120, 100, 20), "R: " + System.Math.Round((double)m_pickedColor.r, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.r * 255) + ")");
        GUI.Label(new Rect(10, 140, 100, 20), "G: " + System.Math.Round((double)m_pickedColor.g, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.g * 255) + ")");
        GUI.Label(new Rect(10, 160, 100, 20), "B: " + System.Math.Round((double)m_pickedColor.b, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.b * 255) + ")");
        GUI.Label(new Rect(10, 180, 100, 20), "A: " + System.Math.Round((double)m_pickedColor.a, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.a * 255) + ")");
    }
}