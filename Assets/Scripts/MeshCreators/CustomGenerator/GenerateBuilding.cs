using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Curve))]
//[RequireComponent (typeof(VerticalExtrude))]
//[RequireComponent (typeof(RoofShape))]
public class GenerateBuilding : MeshCreator
{
    [Range(-100.0f, 100.0f)]
    [SerializeField] private float roofOverhang = 0.0f;
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private Material roofMaterial;
    private GameObject oldBuilding;
    private GameObject oldRoof;


    public override void RecalculateMesh()
    {
        if (oldBuilding != null)
            DestroyImmediate(oldBuilding);
        if (oldRoof != null)
            DestroyImmediate(oldRoof);

        Curve curve = GetComponent<Curve>();
        VerticalExtrude buildingExtrude = GetComponent<VerticalExtrude>();
        RoofShape roofShape = GetComponent<RoofShape>();

        #region Generate Building
        if (buildingExtrude != null)
        {
            GameObject building = new GameObject("main Building");
            building.transform.parent = transform;
            building.transform.position = transform.position;
            Curve buildingCurve = building.AddComponent<Curve>();
            List<Vector3> buildingPoints = new List<Vector3>();
            for (int i = 0; i < curve.points.Count; i++)
            {
                buildingPoints.Add(curve.points[i]);
            }
            buildingCurve.points = buildingPoints;
            VerticalExtrude pBuildingExtrude = building.AddComponent<VerticalExtrude>();
            pBuildingExtrude.CopyFrom(buildingExtrude);
            building.GetComponent<MeshRenderer>().material = buildingMaterial;
            pBuildingExtrude.RecalculateMesh();

            oldBuilding = building;
        }
        #endregion


        #region Generate Rooftop
        if (roofShape != null)
        {
            Vector3 center = Vector3.zero;
            for (int i = 0; i < curve.points.Count; i++)
            {
                center += curve.points[i] / (float)curve.points.Count;
            }
            List<Vector3> roofPoints = new List<Vector3>();
            for (int i = 0; i < curve.points.Count; i++)
            {
                Vector3 whereToGo = curve.points[i];
                whereToGo += (curve.points[i] - center).normalized * roofOverhang;
                roofPoints.Add(whereToGo);
            }
            GameObject rooftop = new GameObject("building Roof");
            rooftop.transform.parent = transform;
            rooftop.transform.position = new Vector3(transform.position.x, transform.position.y + buildingExtrude.height, transform.position.z);
            Curve rooftopCurve = rooftop.AddComponent<Curve>();
            rooftopCurve.points = roofPoints;
            RoofShape pRoofShape = rooftop.AddComponent<RoofShape>();
            pRoofShape.CopyFrom(roofShape);
            rooftop.GetComponent<MeshRenderer>().material = roofMaterial;
            pRoofShape.RecalculateMesh();

            oldRoof = rooftop;
        }
        #endregion

        Debug.Log("I build a building");
    }
}
