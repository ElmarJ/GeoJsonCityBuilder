using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PrefabOnGrid : MonoBehaviour
{
    public float maxRowHeight;
    public float maxColumnWidth;
    // public Face face;
    public GameObject prefab;

    public float rotation;
    public float zPosition;

    public float width;
    public float height;

    float[] rows;
    float[] columns;

    float m_rowHeight;
    float m_columnWidth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
//        RecalculateGrid();
//        DrawGrid();
    }

    public void RecalculateGrid() {
        var width = this.width;
        var height = this.height;

        var columnCount = (int)System.Math.Floor(width / maxColumnWidth);
        var rowCount =  (int)System.Math.Floor(height / maxRowHeight);

        m_columnWidth = width / columnCount;
        m_rowHeight = height / rowCount;

        columns = new float[columnCount];
        rows = new float[rowCount];

        for (int i = 0; i < columnCount; i++)
        {
            columns[i] = (i * m_columnWidth) + (m_columnWidth / 2);
        }

        for (int i = 0; i < rowCount; i++)
        {
            rows[i] = (i * m_rowHeight) + (m_rowHeight / 2);
        }
    }

    public void DrawGrid()
    {
        foreach (var column in columns)
        {
            foreach (var row in rows)
            {
                var go = Instantiate(prefab, transform);
                go.transform.localPosition = new Vector3(column, row, zPosition);
                go.transform.localScale = new Vector3(m_columnWidth, m_columnWidth, 1);
            }
        }
    }

    float GetWidth()
    {
        return 0f;
    }

    float GetHeight()
    {
        return 0f;
    }
}
