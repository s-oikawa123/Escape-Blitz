using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class MeshGenerater
{
    private Vector3 center;
    private List<Vector3> verts;
    private List<Surface> surfaces;
    public MeshGenerater()
    {
        verts = new List<Vector3>();
        surfaces = new List<Surface>();
    }

    public void CreateHall(Vector3 from, Vector3 to, float thickness)
    {
        // 重心定義
        center = (from + to) / 2;

        // 内面定義
        Vector3 innerFrom = new Vector3(Mathf.Min(from.x, to.x), Mathf.Min(from.y, to.y), Mathf.Min(from.z, to.z));
        Vector3 innerTo = new Vector3(Mathf.Max(from.x, to.x), Mathf.Max(from.y, to.y), Mathf.Max(from.z, to.z));

        List<Vector3> tempVerts = new List<Vector3>
        {
            innerFrom,
            new Vector3(innerTo.x, innerFrom.y, innerFrom.z),
            new Vector3(innerFrom.x, innerTo.y, innerFrom.z),
            new Vector3(innerFrom.x, innerFrom.y, innerTo.z),
            new Vector3(innerTo.x, innerTo.y, innerFrom.z),
            new Vector3(innerFrom.x, innerTo.y, innerTo.z),
            new Vector3(innerTo.x, innerFrom.y, innerTo.z),
            innerTo
        };

        int[] tempIndex = new int[]
        {
            0, 1, 6, 3,
            2, 4, 1, 0,
            5, 7, 4, 2,
            3, 6, 7, 5,
            4, 7, 6, 1,
            5, 2, 0, 3
        };

        foreach (int i in tempIndex)
        {
            verts.Add(tempVerts[i]);
        }

        // 外面定義
        Vector3 outerFrom = innerFrom - Vector3.one * thickness;
        Vector3 outerTo = innerTo + Vector3.one * thickness;

        tempVerts = new List<Vector3>
        {
            outerFrom,
            new Vector3(outerTo.x, outerFrom.y, outerFrom.z),
            new Vector3(outerFrom.x, outerTo.y, outerFrom.z),
            new Vector3(outerFrom.x, outerFrom.y, outerTo.z),
            new Vector3(outerTo.x, outerTo.y, outerFrom.z),
            new Vector3(outerFrom.x, outerTo.y, outerTo.z),
            new Vector3(outerTo.x, outerFrom.y, outerTo.z),
            outerTo
        };

        foreach (int i in tempIndex)
        {
            verts.Add(tempVerts[i]);
        }

        List<Surface> tempSurfaces = new List<Surface>();

        for(int i = 0; i < 12; i++)
        {
            Surface surface = new Surface(Enumerable.Range(0 + i * 4, 3 + i * 4).ToArray());
            int[] add = new int[] { 0, 1, 2 };
            int[] add2 = new int[] { 2, 3, 0 };
            if (i < 6)
            {
                surface.poligons.Add(add.Select(a => a + i * 4).Reverse().ToArray());
                surface.poligons.Add(add2.Select(a => a + i * 4).Reverse().ToArray());
                surface.invertNormal = true;
            }
            else
            {
                surface.poligons.Add(add.Select(a => a + i * 4).ToArray());
                surface.poligons.Add(add2.Select(a => a + i * 4).ToArray());
            }
            tempSurfaces.Add(surface);
        }

        tempSurfaces.ForEach(a => CalculateSurfaceFormura(a));

        tempSurfaces.ForEach(a => Debug.Log($"{a.normal.x}, {a.normal.y}, {a.normal.z}, {a.d}"));

        surfaces.AddRange(tempSurfaces);
    }

    public void CalculateSurfaceFormura(Surface surface)
    {
        Vector3 baseVec = verts[surface.vertIndex[0]];
        Vector3 a = verts[surface.vertIndex[1]] - verts[surface.vertIndex[0]];
        Vector3 b = verts[surface.vertIndex[2]] - verts[surface.vertIndex[0]];
        Vector3 cross = Vector3.Cross(a, b).normalized;
        if (Vector3.Dot(cross, baseVec - center) > 0)
        {
            surface.normal = cross;
            surface.d = cross.x * -baseVec.x + cross.y * -baseVec.y + cross.z * -baseVec.z;
        }
        else
        {
            surface.normal = -cross;
            surface.d = -cross.x * -baseVec.x + -cross.y * -baseVec.y + -cross.z * -baseVec.z;
        }
    }

    public Mesh GenerateMesh()
    {
        Mesh resultMesh = new Mesh();
        resultMesh.vertices = verts.ToArray();
        List<int> triangle = new List<int>();
        foreach (var surface in surfaces) 
        {
            foreach (var poligon in surface.poligons)
            {
                triangle.AddRange(poligon);
            }
        }

        resultMesh.triangles = triangle.ToArray();

        resultMesh.RecalculateBounds();

        return resultMesh;
    }

    public class Line
    {
        public int[] verts;

        public Line(int a, int b)
        {
            verts = new int[] { a, b };
        }
    }

    public class Surface
    {
        public Vector3 normal;
        public float d;
        public int[] vertIndex;
        public List<int[]> poligons;
        public bool invertNormal;

        public Surface(int[] vertIndex)
        {
            this.vertIndex = vertIndex;
            poligons = new List<int[]>();
        }

        public Surface(Vector3 normal, float d, int[] vertIndex, bool invertNormal)
        {
            this.normal = normal;
            this.d = d;
            this.vertIndex = vertIndex;
            this.invertNormal = invertNormal;
            poligons = new List<int[]>();
        }
    }
}
