using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StretchToPoint : MonoBehaviour
{
    public Vector2 endPoint;
    public Vector2 StartPoint { get => new Vector2(this.transform.localPosition.x, transform.localPosition.z); }
    public Vector2 Vector { get => this.endPoint - this.StartPoint; }
    public double DirectionRad { get => (Math.Atan2(Vector.x, Vector.y) - 0.5 * Math.PI); }
    public double DirectionDeg { get => (DirectionRad * 180 / Math.PI); }
    public float Length { get => Vector.magnitude; }

    // Start is called before the first frame update
    void Start()
    {
        PerformStretch();
    }

    // Update is called once per frame
    void Update()
    {
        PerformStretch();       
    }

    public void PerformStretch() {
        // rotate wall into given direction:
        transform.rotation = Quaternion.AngleAxis((float)this.DirectionDeg +180, Vector3.up);
        // transform.Rotate(0, (float)this.DirectionDeg, 0, Space.Self);

        // stretch to given length and height:            
        transform.localScale = new Vector3(this.Length, transform.localScale.y, 1);
    }
}
