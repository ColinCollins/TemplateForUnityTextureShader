using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Drawing;

// https://www.youtube.com/watch?v=4066MndcyCk

public class Draw : MonoBehaviour
{
    public int Width = 400;
    public int Height = 400;
    public int ZOrder = 100;

    public int PCount = 6;
    public bool isUpdate = false;

    public float UpdateSpeed = 1f;

    private int col;
    private int row;
    private Texture2D texture;
    private Material mat;

    private List<Vector4> points = new List<Vector4>();
    private float[] distance;
    private float z = 0;

    private void Start()
    {
        Render();

        mat = GetComponent<Renderer>().material;
        mat.SetVectorArray("_Points", points.ToArray());
    }

    private void Init()
    {
        texture = new Texture2D(Width, Height);
        GetComponent<Renderer>().material.mainTexture = texture;

        col = texture.height / Height + 1;
        row = texture.width / Width + 1;
    }

    [ExecuteInEditMode]
    public void Render()
    {
        // Init();
        points.Clear();
        for (int i = 0; i < PCount; i++)
        {
            var pos = new Vector4(Random.Range(0, Width), Random.Range(0, Height), Random.Range(0, ZOrder));
            pos.x /= (float)Width;
            pos.y /= (float)Height;
            pos.z /= (float)ZOrder;

            points.Add(pos);
        }

        distance = new float[points.Count];
        z = 0;

        // UpdateRender();
    }

    private void UpdateRender() 
    {
        z += Time.deltaTime * UpdateSpeed;
        z %= ZOrder;

        for (int m = 0; m < Width; m++)
            for (int n = 0; n < Height; n++)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    distance[i] = Vector3.Distance(new Vector3(m, n, (int)z), points[i]);
                }

                float v = Mathf.Lerp(0, 255f, getMin(distance) / (Width / 4f)) / 255f;
                SetPixel(m, n, new UnityEngine.Color(v, v, v));
            }

        //points.ForEach(point =>
        //{
        //    SetPixel((int)point.x, (int)point.y, Color.green);
        //});

        texture.Apply();
    }

    private void Update()
    {
        // if you want cpu to draw more than one color in the texture
        if (!isUpdate)
            return;

        UpdateRender();
    }

    private void SetPixel(int x, int y, UnityEngine.Color color) 
    {
        x *= (Width / texture.width);
        y *= (Height / texture.height);

        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                texture.SetPixel(x + i, y + j, color);
            }
        }
    }

    private float getMin(float[] datas) 
    {
        float d = float.MaxValue;
        for (int i = 0; i < datas.Length; i++) 
        {
            if (d > datas[i])
                d = datas[i];
        }

        return d;
    }
}


[CustomEditor(typeof(Draw))]
public class ObjectBuilderEditor : Editor {
    public override void OnInspectorGUI() 
    { 
        DrawDefaultInspector();

        Draw myScript = (Draw)target;
        if(GUILayout.Button("创建对象")) { 
            myScript.Render();
        }
    }
}