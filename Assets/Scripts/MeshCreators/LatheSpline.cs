using UnityEngine;
using Handout;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class LatheSpline : MeshCreator {

	public int NumCurves = 10; //number of segments around the mesh
	public bool ModifySharedMesh = false;

	//helper function to map the x,y location of a vertex to an index in a 1D array
	private int getIndex(int x, int y, int height) {
		return y + x * height;
	}


	public override void RecalculateMesh() {
		Curve curve = GetComponent<Curve>();
		if (curve==null)
			return;
		List<Vector3> _vertices = curve.points;

		MeshBuilder meshBuilder = new MeshBuilder();

		int vertexCount = _vertices.Count;
		int curveCount = NumCurves;
		float verticalTotalLength = 0;
		Vector2 previousPoint = Vector2.zero;
		float objectSurface = 0;
        for (int vertexIndex = 0; vertexIndex <= vertexCount; vertexIndex++)
        {
/*			int pVertIndex = vertexIndex;
			if (vertexIndex == vertexCount)
			{
				pVertIndex = 0;
            }
            Vector2 pVerticleDist = (new Vector2(_vertices[pVertIndex].x, _vertices[pVertIndex].y) - previousPoint);
            pVerticleDist.x *= 2;
            float verticalLength = pVerticleDist.magnitude;
			verticalTotalLength += verticalLength;
			

            previousPoint = new Vector2(_vertices[pVertIndex].x, _vertices[pVertIndex].y);
            Debug.Log(pVerticleDist);
            Debug.Log(verticalTotalLength);

			objectSurface += verticalLength * Mathf.PI;
			Debug.Log(objectSurface);*/
        }

        //Go through all curves (vertical lines around mesh)
        for (int curveIndex = 0; curveIndex <= curveCount; curveIndex++) {
            int baseCurveIndex = curveIndex % curveCount; // wraps back to 0 for final ring

            //Create quaternion for rotating around y-axis (the curveIndex is used to determine the angle in degrees):
            Quaternion rotation = Quaternion.Euler(0, baseCurveIndex * 360.0f / curveCount, 0);

            //Go through all vertices (all vertices per spline):
            for (int vertexIndex = 0; vertexIndex<vertexCount; vertexIndex++) {
				//create a Vector3 from a Vector2 (or: set the z-coordinate of the curve point to zero):
				Vector3 vertex = new Vector3(_vertices[vertexIndex].x, _vertices[vertexIndex].y, 0);
                // TODO: add correct uvs
                float uvX = (float)curveIndex / (float)(curveCount - 1); // instead of / curveCount
                float uvY = (float)vertexIndex / (float)(vertexCount - 1);
                Vector2 uv = new Vector2(uvX, uvY);
                //use quaternion to rotate the vertex into position:
                vertex = rotation * vertex;
				//add it to the mesh:
				meshBuilder.AddVertex(vertex, uv);
			}
		}

		//Generate quads:
		// TODO: fix the normals along the "stitch", by using the same (shared) vertices
		//  for the quads on both sides of the stitch:
		for (int curveIndex = 0; curveIndex < curveCount; curveIndex++) { //start at 1, because we need to access spline at splineIndex-1
			for (int vertexIndex = 0; vertexIndex < vertexCount - 1; vertexIndex++) { //start at 1, because we need to access vertex at vertexIndex-1
				int nextCurveIndex = curveIndex + 1;

                //generate 4 vertices (quad):
                int v0 = getIndex(curveIndex, vertexIndex, vertexCount);
                int v1 = getIndex(nextCurveIndex, vertexIndex, vertexCount);
                int v2 = getIndex(nextCurveIndex, vertexIndex + 1, vertexCount);
                int v3 = getIndex(curveIndex, vertexIndex + 1, vertexCount);
                //Debug.Log(v0 + " and " + v1 + " and " + v2 + " and " + v3);
                // Add two triangles (quad):
                meshBuilder.AddTriangle(v0, v1, v2);
                meshBuilder.AddTriangle(v0, v2, v3);
            }
		}

		// Generate mesh and apply it to the meshfilter component:
		ReplaceMesh(meshBuilder.CreateMesh(), ModifySharedMesh);
	}
}
