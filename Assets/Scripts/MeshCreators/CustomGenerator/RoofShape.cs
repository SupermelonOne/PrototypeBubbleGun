using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RoofShape : MeshCreator
{

    [Range(0.0f, 100.0f)]
    public float cornerRoofPercentage = 85;
    [Range(0.0f, 100.0f)]
    public float flatRoofPercentage = 90;


    public bool ModifySharedMesh = false;
    public float roofHeight = 1;
    public float roofMiddleHeight = 0.9f;

    private int amountOfChecks = 0;

    public void CopyFrom(RoofShape copy)
    {
        cornerRoofPercentage = copy.cornerRoofPercentage;
        flatRoofPercentage = copy.flatRoofPercentage;
        ModifySharedMesh = copy.ModifySharedMesh;
        roofHeight = copy.roofHeight;
        roofMiddleHeight = copy.roofMiddleHeight;
    } 


    public override void RecalculateMesh()
    {
        //convert the percentage to actual usable numbers
        cornerRoofPercentage /= 100;
        flatRoofPercentage /= 100;
        if (flatRoofPercentage < cornerRoofPercentage)
        {
            flatRoofPercentage = cornerRoofPercentage;
        }



        Curve curve = GetComponent<Curve>();
        if (curve == null)
            return;

        List<Vector2> points = new List<Vector2>();
        foreach (Vector3 point in curve.points)
        {
            points.Add(new Vector2(point.x, point.z));
        }
        MeshBuilder builder = new MeshBuilder();
        Vector2 center = Vector2.zero;
        float totalWeight = 0;
        for(int i = 0; i < points.Count; i++)
        {
            int nextI = (i+1) % points.Count;
            int prevI = (i-1) % points.Count;
            if (prevI < 0)
            {
                prevI += points.Count;
            }

            Vector2 a = points[prevI] - points[i];
            Vector2 b = points[nextI] - points[i];

            float angle = Vector2.Angle(a, b);

            Debug.Log(angle + " on point: " + points[i]);
            float pointWeight = angle * angle * angle;
            totalWeight += pointWeight;

            center += points[i] * pointWeight;
        }
        center /= totalWeight;

        List<Vector2> centerPoints = new List<Vector2>();
        for (int i = 0; i< points.Count; i++)
        {
            Vector2 whereToGo = points[i];
            whereToGo += (center - points[i]) * cornerRoofPercentage;
            centerPoints.Add(whereToGo);
        }
        List<Vector2> innerPoints = new List<Vector2>();
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 whereToGo = points[i];
            whereToGo += (center - points[i]) * flatRoofPercentage;
            innerPoints.Add(whereToGo);
        }

        #region makeSmallTop
        if (cornerRoofPercentage < 1)
        {
            // Create a list of indices 0..n-1:
            List<int> indices = new List<int>();
            for (int i = 0; i < innerPoints.Count; i++)
            {
                indices.Add(i);
            }
            // This list is going to contain the vertex indices of the triangles: (3 integers per triangle)
            List<int> triangles = new List<int>();
            List<Vector2> polygon = new List<Vector2>();
            for (int i = 0; i < innerPoints.Count; i++)
            {
                polygon.Add(innerPoints[i]);
            }

            // Compute the triangulation of [polygon], store it in [triangles]:
            TriangulatePolygon(triangles, polygon, indices);
            // Add front face:
            for (int i = 0; i < innerPoints.Count; i++)
            {
                // TODO: Add uvs
                // you can do this just by giving their in world x and y position right?
                builder.AddVertex(new Vector3(innerPoints[i].x, roofMiddleHeight, innerPoints[i].y), new Vector2(innerPoints[i].x, innerPoints[i].y));
                //Debug.Log(innerPoints.Count);
                //Debug.Log(innerPoints[i].ToString());
            }
            for (int t = 0; t < triangles.Count; t += 3)
            {
                builder.AddTriangle(triangles[t], triangles[t + 1], triangles[t + 2]);
                //Debug.Log ("Adding triangle " + triangles [t] + "," + triangles [t + 1] + "," + triangles [t + 2]);
            }
        }
        #endregion

        // TODO add underside polygon so the bottom also renders on buildings with overhang

        float totalLength = 0;

        for (int i = 0; i< points.Count; i++)
        {
            int nextI = (i + 1) % points.Count;
            totalLength += (points[i] - points[nextI]).magnitude;
        }
        float fullTexLength = MathF.Floor(totalLength);

        float startHorTex = 0;

        for (int i = 0; i < points.Count; i++)
        {
            int nextI = (i + 1) % points.Count;
            // TODO add UV's
            float botHorLength = (points[i] - points[nextI]).magnitude;
            float botHorMin = startHorTex;
            float botHorLengthTex = ((botHorLength / totalLength) * fullTexLength);
            float botHorMax = botHorMin + botHorLengthTex;
            startHorTex += botHorLengthTex;
            float topHorLength = (centerPoints[i] - centerPoints[nextI]).magnitude;
            float topMin = botHorMin + ((botHorLengthTex - topHorLength) / 2);
            float topMax = topHorLength + topMin;

            float tBotHorLength = (innerPoints[i] - innerPoints[nextI]).magnitude;
            float tBotHorLengthTex = ((tBotHorLength / totalLength) * fullTexLength);
            float tBotMin = ((topMax - topMin) - tBotHorLengthTex)/2 + topMin;
            float tBotMax = tBotHorLengthTex + tBotMin;
            //Debug.Log(tBotHorLength / botHorLength);


            // TODO: hier berekend je de afstand tussen 2 lijnen via vector2's, wat dus niet helemaal accuraat is
            Vector3 bottomLeft = new Vector3(points[i].x, 0, points[i].y);
            Vector3 bottomRight = new Vector3(points[nextI].x, 0, points[nextI].y);
            Vector3 topLeft = new Vector3(centerPoints[i].x, roofHeight, centerPoints[i].y);
            Vector3 topRight = new Vector3(centerPoints[nextI].x, roofHeight, centerPoints[nextI].y);
            Vector3 middleLeft = new Vector3(innerPoints[i].x, roofMiddleHeight, innerPoints[i].y);
            Vector3 middleRight = new Vector3(innerPoints[nextI].x, roofMiddleHeight, innerPoints[nextI].y);
            float verticalDistance = 
                (Vector3.Cross((topLeft - bottomLeft), (bottomLeft - bottomRight))).magnitude
                / 
                (bottomLeft - bottomRight).magnitude;
            //Debug.Log(verticalDistance);
            float topVerticalDistance =
                (Vector3.Cross((middleLeft - topLeft), (topLeft - topRight))).magnitude
                /
                (topLeft - topRight).magnitude;
            topVerticalDistance += verticalDistance;
            //Debug.Log(topVerticalDistance);
            


            int v1 = builder.AddVertex(new Vector3(points[i].x, 0, points[i].y), new Vector2(botHorMax, 0));
            int v2 = builder.AddVertex(new Vector3(points[nextI].x, 0, points[nextI].y), new Vector2(botHorMin, 0));
            int v3 = builder.AddVertex(new Vector3(centerPoints[i].x, roofHeight, centerPoints[i].y), new Vector2(topMax, verticalDistance));
            int v4 = builder.AddVertex(new  Vector3(centerPoints[nextI].x, roofHeight, centerPoints[nextI].y), new Vector2(topMin, verticalDistance));
            int v3v2 = builder.AddVertex(new Vector3(centerPoints[i].x, roofHeight, centerPoints[i].y), new Vector2(topMax, verticalDistance));
            int v4v2 = builder.AddVertex(new Vector3(centerPoints[nextI].x, roofHeight, centerPoints[nextI].y), new Vector2(topMin, verticalDistance));
            int v5 = builder.AddVertex(new Vector3(innerPoints[i].x, roofMiddleHeight, innerPoints[i].y), new Vector2(tBotMax, topVerticalDistance));
            int v6 = builder.AddVertex(new Vector3(innerPoints[nextI].x, roofMiddleHeight, innerPoints[nextI].y), new Vector2(tBotMin, topVerticalDistance));



            builder.AddTriangle(v1, v2, v4);
            builder.AddTriangle(v4, v3, v1);
            builder.AddTriangle(v3v2, v4v2, v6);
            builder.AddTriangle(v6, v5, v3v2);
        }


        ReplaceMesh(builder.CreateMesh(), ModifySharedMesh);

        //set the percentages back to normal
        cornerRoofPercentage *= 100;
        flatRoofPercentage *= 100;
    }



    void TriangulatePolygon(List<int> triangles, List<Vector2> polygon, List<int> indices)
    {
        for (int i = 0; i < polygon.Count; i++)
        {
            int i2 = (i + 1) % polygon.Count;
            int i3 = (i + 2) % polygon.Count;
            Vector2 u = polygon[i];
            Vector2 v = polygon[i2];
            Vector2 w = polygon[i3];

            // TODO: Check whether the polygon corner at point v is less than 180 degrees - if not continue the for loop (with the next value for i)
            if (Over180Degrees(u, v, w))
            {
                bool pointInside = false;
                foreach (Vector2 point in polygon)
                {
                    if (point != u && point != v && point != w && !pointInside)
                    {
                        InsideTriangle(u, v, w, point);
                    }

                }
                // TODO: Check whether there are no other points of the polygon inside the triangle u,v,w - if not continue the for loop (with the next value for i)
                if (!pointInside)
                {
                    // (Hint: see the methods below!!! :-)  )

                    // Add a triangle on u,v,w:
                    triangles.Add(indices[i]);
                    triangles.Add(indices[i2]);
                    triangles.Add(indices[i3]);

                    polygon.RemoveAt(i2); // remove v from point list (keep u and w!)
                    indices.RemoveAt(i2); // also remove the corresponding index from the index list
                    if (polygon.Count < 3)
                        return; // The polygon is now fully triangulated

                    // continue with a smaller polygon - restart the for loop:
                    i = -1;
                }
            }
        }

        if (amountOfChecks <= 0)
        {
            amountOfChecks++;

            RecalculateMesh();
            return;
        }
        else
            throw new Exception("No suitable triangulation found - is the polygon simple and clockwise?");
    }


    bool Over180Degrees(Vector2 u, Vector2 v, Vector2 w)
    {
        Vector2 A = u - v;
        Vector2 B = v - w;
        A.Normalize();
        B.Normalize();

        float dot = Vector2.Dot(A, B);
        float cross = A.x * B.y - A.y * B.x;

        float angle = Mathf.Atan2(cross, dot) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360;

        return angle > 180f; ;
    }

    // Returns true if p1,p2 and p3 form a clockwise triangle (returns false if anticlockwise, or all three on the same line)
    bool Clockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Vector2 difference1 = (p2 - p1);
        Vector2 difference2 = (p3 - p2);
        // Take the dot product of the (normal of difference1) and (difference2):
        return (-difference1.y * difference2.x + difference1.x * difference2.y) < 0;
    }

    // Returns true if [testPoint] lies inside, or on the boundary, of the triangle given by the points p1,p2 and p3.
    bool InsideTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 testPoint)
    {
        if (Clockwise(p1, p2, p3))
            return !Clockwise(p2, p1, testPoint) && !Clockwise(p3, p2, testPoint) && !Clockwise(p1, p3, testPoint);
        else
            return !Clockwise(p1, p2, testPoint) && !Clockwise(p2, p3, testPoint) && !Clockwise(p3, p1, testPoint);
    }
}
