using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Container;
using System;

public class Line : RoomInfo
{
    [SerializeField] private float roomWidth;
    [SerializeField] private float pathWidth;
    [SerializeField] private TriggerObserver fallTriggerObserver;
    [SerializeField] private Transform fallObserverPivot;
    [SerializeField] private Transform Exit;
    [SerializeField] private Transform wall;
    [SerializeField] private GameObject path;
    [SerializeField] private GameObject grid;
    private float time;
    private float baseExitTime;

    // ルームプロパティ
    private float roomLength;
    private float lightTimeMulti;
    private int pathNumber;
    private float interval;
    private int turnNumber;
    private float exitTimeMulti;
    private float lightTime;
    private float lightLength;


    private enum PathKind
    {
        Block,
        Line
    }
    private PathKind pathKind;

    private enum LightMode
    {
        cyclically,
        interval
    }
    private LightMode lightMode;
    private List<GameObject> paths = new List<GameObject>();
    private protected override void SetUp()
    {
        fallObserverPivot.localScale = new Vector3(1, 1, roomLength);
        time = 0;
        Exit.localPosition = new Vector3(0, 0, roomLength - 10);
        wall.localScale = new Vector3(1, 1, (roomLength + 10f) / 20f);
        exitPoint = new Vector3(0, 0, roomLength + 10);

        Mesh mesh;
        PathInfo pathInfo;
        switch (pathKind)
        {
            case PathKind.Block:
                BlockPathInfo blockPathInfo = GeneratePathBlock();
                mesh = GenerateMesh(blockPathInfo);
                pathInfo = blockPathInfo;
                break;
            case PathKind.Line:
                LinePathInfo linePathInfo = GeneratePath();
                mesh = GenerateMesh(linePathInfo);
                pathInfo = linePathInfo;
                break;
            default:
                BlockPathInfo tmpBlockPathInfo = GeneratePathBlock();
                mesh = GenerateMesh(tmpBlockPathInfo);
                pathInfo = tmpBlockPathInfo;
                break;
        }
        baseExitTime = pathInfo.totalDistance / 7;
        exitTime = baseExitTime * exitTimeMulti + 12f / 7f;
        path.GetComponent<MeshFilter>().mesh = mesh;
        path.GetComponent<MeshCollider>().sharedMesh = mesh;
        lightLength = pathInfo.totalDistance / 12f;
        for (int i = 0; i < pathNumber + 1; i++) 
        {
            GameObject go = Instantiate(path, transform);
            go.transform.localPosition = new Vector3(0, 0, 5);
            paths.Add(go);
            go.GetComponent<Renderer>().material.SetFloat("_Length", lightLength / pathInfo.totalDistance);
        }
        lightTime = pathInfo.totalDistance / 7 * lightTimeMulti;
        mesh = GenerateGridMesh();
        grid.GetComponent<MeshFilter>().mesh = mesh;
    }

    private protected override void First()
    {

    }

    private protected override void Every()
    {
        time += Time.deltaTime;

        switch (lightMode)
        {
            case LightMode.cyclically:
                for (int i = 0; i < pathNumber + 1; i++)
                {
                    paths[i].GetComponent<Renderer>().material.SetFloat("_Offset", (time + lightTime * i / pathNumber) % (lightTime * (pathNumber + 1) / pathNumber) / lightTime);
                }
                break;
            case LightMode.interval:
                paths[0].GetComponent<Renderer>().material.SetFloat("_Offset", time / lightTime);
                if (time - lightTime >= interval)
                {
                    time = 0;
                }
                break;
        }

        if (fallTriggerObserver.Enter)
        {
            gameManager.MusicPlayer.PlayOneShot(GameManager.Audio_C.GetAudioClip(NameSE.Fall));
            gameManager.GameOverInitiation(GameManager.Text_C.FailReason.LineRoom[0].GetString());
        }
    }

    public override void Signal(Signal signal) 
    {
        if (signal == Container.Signal.True)
        {
            gameManager.ActiveNextRoom();
            roomClearedFrag = true;
            doorManager.SetActive(false);
            backDoorManager.SetActive(false);
        }

        if (signal == Container.Signal.False)
        {
            gameManager.GameOverInitiation(GameManager.Text_C.FailReason.Utility[1].GetString());
        }
    }

    private protected override void ForceParam(int number)
    {
        switch (number)
        {
            case 3:
                pathKind = PathKind.Block;
                lightMode = LightMode.cyclically;
                pathNumber = 1;
                lightTimeMulti = 1f;
                exitTimeMulti = 1f;
                roomLength = 20;
                turnNumber = 3;
                wrongRoomFrag = false;
                break;
            case 4:
                pathKind = PathKind.Line;
                lightMode = LightMode.interval;
                interval = 1;
                pathWidth = 3;
                pathNumber = 1;
                lightTimeMulti = 0.85f;
                exitTimeMulti = 1f;
                roomLength = 30;
                turnNumber = 4;
                wrongRoomFrag = false;
                break;
        }
    }

    [ParameterDecision(30, 1)]
    public void SetPathKind(float intensity)
    {
        pathKind = PathKind.Block;
        if (intensity >= 10 && UnityEngine.Random.Range(0, 2) == 0)
        {
            pathKind = PathKind.Line;
            pathWidth = 3f - (intensity - 10) / 20;
        }
    }

    [ParameterDecision(50, 0)]
    public void SetLightTime(float intensity)
    {
        if (intensity <= 10)
        {
            lightMode = LightMode.cyclically;
            pathNumber = 1;
            lightTimeMulti = 1 - intensity / 50;
            exitTimeMulti += 1f;
        }
        else if (intensity <= 30)
        {
            switch (UnityEngine.Random.Range(0, 2))
            {
                case 0:
                    if (pathKind == PathKind.Block)
                    {
                        lightMode = LightMode.interval;
                        pathNumber = 1;
                        interval = 1;
                        lightTimeMulti = 0.5f - 0.1f * (intensity - 10) / 20;
                        exitTimeMulti += 1.5f;
                    }
                    else
                    {
                        lightMode = LightMode.cyclically;
                        pathNumber = 3;
                        lightTimeMulti = 2 + 0.5f * (intensity - 10) / 20;
                        exitTimeMulti += 1.5f;
                    }
                    break;
                case 1:
                    if (pathKind == PathKind.Block)
                    {
                        lightMode = LightMode.interval;
                        pathNumber = 10;
                        lightTimeMulti = 0.8f - 0.1f * (intensity - 10) / 20;
                        exitTimeMulti += 1f;
                    }
                    else
                    {
                        lightMode = LightMode.interval;
                        pathNumber = 10;
                        lightTimeMulti = 0.85f - 0.1f * (intensity - 10) / 20;
                        exitTimeMulti += 1f;
                    }
                    break;
            }
        }
        else
        {
            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    if (pathKind == PathKind.Block)
                    {
                        lightMode = LightMode.cyclically;
                        pathNumber = 3;
                        lightTimeMulti = 2.5f + 0.5f * (intensity - 30) / 20;
                        exitTimeMulti += 1.66f;
                    }
                    else
                    {
                        lightMode = LightMode.interval;
                        pathNumber = 1;
                        interval = 1;
                        lightTimeMulti = 0.5f - 0.1f * (intensity - 30) / 20;
                        exitTimeMulti += 1.5f - 0.4f * (intensity - 30) / 20;
                    }
                    break;
                case 1:
                    if (pathKind == PathKind.Block)
                    {
                        lightMode = LightMode.interval;
                        pathNumber = 1;
                        interval = 1f;
                        lightTimeMulti = 0.2f + 0.15f * (intensity - 30) / 20;
                        exitTimeMulti += 1.2f;
                    }
                    else
                    {
                        lightMode = LightMode.cyclically;
                        pathNumber = 2;
                        lightTimeMulti = 2f + 0.2f * (intensity - 30) / 20;
                        exitTimeMulti = 1.3f;
                    }
                    break;
                case 2:
                    if (pathKind == PathKind.Block)
                    {
                        lightMode = LightMode.cyclically;
                        pathNumber = 1;
                        interval = 1;
                        lightTimeMulti = 0.7f - 0.1f * (intensity - 30) / 20;
                        exitTimeMulti += 1f;
                    }
                    else
                    {
                        lightMode = LightMode.cyclically;
                        pathNumber = 1;
                        lightTimeMulti = 0.75f - 0.1f * (intensity - 30) / 20;
                        exitTimeMulti += 1f;
                    }
                    break;
            }
        }
    }

    [ParameterDecision(40, 0)]
    public void SetPathLength(float intensity)
    {
        roomLength = (int)(intensity / 4) / 2 * 2 + 20;
    }

    [ParameterDecision(40, 0)]
    public void SetPathTrunNumber(float intensity)
    {
        turnNumber = 2 + (int)intensity / 10;
    }

    private LinePathInfo GeneratePath()
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(new Vector3(0, 0, 0));

        List<float> turnPoint = new List<float>();
        for (int i = 1; i <= turnNumber; i++)
        {
            turnPoint.Add((float)i / (turnNumber + 1));
        }

        float blur = 1f / (turnNumber + 1) / 4;
        turnPoint = turnPoint.Select(a => a + UnityEngine.Random.Range(-blur, blur)).ToList();

        foreach (float i in turnPoint)
        {
            path.Add(new Vector3(UnityEngine.Random.Range(0.1f, 0.9f) * 20 - 10, 0, i * roomLength));
        }
        path.Add(new Vector3(0, 0, roomLength));

        List<float> pathDistance = new List<float>();
        float totalDistance = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            float disatance = (path[i + 1] - path[i]).magnitude;
            pathDistance.Add(disatance);
            totalDistance += disatance;
        }

        return new LinePathInfo(path, pathDistance, totalDistance);
    }

    private BlockPathInfo GeneratePathBlock()
    {
        List<Vector3> list = new List<Vector3>();
        list.Add(new Vector3(0, 0, 0));
        int distance = 2;
        int length = (int)roomLength / 2 - 1;
        int width = ((int)roomWidth / 2 - (1 - (int)roomWidth / 2 % 2)) / 2;
        float odd = 1;
        float oddDec = (float)turnNumber / length;
        while (list[list.Count - 1].z < length)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= odd)
            {
                distance += 2;
                list.Add(list[list.Count - 1] + new Vector3(0, 0, 1));
                odd -= oddDec;
            }
            else
            {
                odd += 1f;
                int right = width - (int)list[list.Count - 1].x;
                int left = width + (int)list[list.Count - 1].x;
                if (UnityEngine.Random.Range(0f, 1f) < (float)right / (right + left))
                {
                    int num = UnityEngine.Random.Range(0, right);
                    for (int i = 0; i < num; i++)
                    {
                        distance += 2;
                        list.Add(list[list.Count - 1] + new Vector3(1, 0, 0));
                    }
                }
                else
                {
                    int num = UnityEngine.Random.Range(0, left);
                    for (int i = 0; i < num; i++)
                    {
                        distance += 2;
                        list.Add(list[list.Count - 1] + new Vector3(-1, 0, 0));
                    }
                }
                distance += 2;
                list.Add(list[list.Count - 1] + new Vector3(0, 0, 1));
            }
        }

        list = list.Select(a => Vector3.Scale(a, new Vector3(2, 2, 2)) + new Vector3(0, 0, 1)).ToList();

        return new BlockPathInfo(list, distance);
    }

    private Mesh GenerateGridMesh()
    {
        List<Vector3> varticles = new List<Vector3>();
        int width = (int)roomWidth / 2 - (1 - (int)roomWidth / 2 % 2) + 1;
        int length = (int)roomLength / 2 - 1;
        float gridBoldness = 0.01f;

        // 頂点生成
        for (int i = 0; i < width; i++)
        {
            varticles.Add(new Vector3(9 + gridBoldness - i * 2, 0, 0));
            varticles.Add(new Vector3(9 - gridBoldness - i * 2, 0, 0));
            varticles.Add(new Vector3(9 + gridBoldness - i * 2, 0, roomLength));
            varticles.Add(new Vector3(9 - gridBoldness - i * 2, 0, roomLength));
        }

        for (int i = 0; i < length; i++)
        {
            varticles.Add(new Vector3(roomWidth / 2, 0, 2 - gridBoldness + i * 2));
            varticles.Add(new Vector3(-roomWidth / 2, 0, 2 - gridBoldness + i * 2));
            varticles.Add(new Vector3(roomWidth / 2, 0, 2 + gridBoldness + i * 2));
            varticles.Add(new Vector3(-roomWidth / 2, 0, 2 + gridBoldness + i * 2));
        }

        Mesh mesh = new Mesh();
        mesh.vertices = varticles.ToArray();

        // ポリゴン定義
        List<int> triangle = new List<int>();
        for (int i = 0; i < width + length; i++)
        {
            triangle.AddRange(new[] { 0, 1, 2, 2, 1, 3 }.Select(a => a + i * 4));
        }

        mesh.triangles = triangle.ToArray();

        // UV定義
        Vector2[] uvs = new Vector2[varticles.Count];
        for (int i = 0; i < width + length; i++)
        {
            uvs[i * 4 + 0] = new Vector2(1, 0);
            uvs[i * 4 + 1] = new Vector2(0, 0);
            uvs[i * 4 + 2] = new Vector2(1, 1);
            uvs[i * 4 + 3] = new Vector2(0, 1);
        }

        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private Mesh GenerateMesh(LinePathInfo pathInfo)
    {
        IReadOnlyList<Vector3> path = pathInfo.path;
        List<Vector3> varticles = new List<Vector3>();

        // 頂点生成
        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0)
            {
                CalculateVerts(Vector3.back, path[i], path[i + 1], varticles);
                continue;
            }
            else if (i == path.Count - 1)
            {
                CalculateVerts(path[i - 1], path[i], Vector3.back, varticles);
                continue;
            }

            CalculateVerts(path[i - 1], path[i], path[i + 1], varticles);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = varticles.ToArray();

        // ポリゴン定義
        List<int> triangle = new List<int>();
        for (int i = 0; i < path.Count - 1; i++)
        {
            triangle.AddRange(new[] { 0, 1, 2, 2, 1, 3 }.Select(a => a + i * 2));
        }

        mesh.triangles = triangle.ToArray();

        // UV定義
        Vector2[] uvs = new Vector2[varticles.Count];
        uvs[0] = new Vector2(1, 0);
        uvs[1] = new Vector2(0, 0);
        float sum = 0;
        for (int i = 0; i < pathInfo.pathDistance.Count; i++)
        {
            sum += pathInfo.pathDistance[i];
            uvs[i * 2 + 2] = new Vector2(1, sum / pathInfo.totalDistance);
            uvs[i * 2 + 3] = new Vector2(0, sum / pathInfo.totalDistance);
        }
        
        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private Mesh GenerateMesh(BlockPathInfo pathInfo)
    {
        IReadOnlyList<Vector3> path = pathInfo.path;
        List<Vector3> varticles = new List<Vector3>();

        // 頂点生成
        for (int i = 0; i < path.Count; i++)
        {
            varticles.Add(path[i] + new Vector3(1, 0, -1));
            varticles.Add(path[i] + new Vector3(-1, 0, -1));
            varticles.Add(path[i] + new Vector3(1, 0, 1));
            varticles.Add(path[i] + new Vector3(-1, 0, 1));
        }

        Mesh mesh = new Mesh();
        mesh.vertices = varticles.ToArray();

        // ポリゴン定義
        List<int> triangle = new List<int>();
        for (int i = 0; i < path.Count; i++)
        {
            triangle.AddRange(new[] { 0, 1, 2, 2, 1, 3 }.Select(a => a + i * 4));
        }

        mesh.triangles = triangle.ToArray();

        // UV定義
        Vector2[] uvs = new Vector2[varticles.Count];
        float div = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            uvs[i * 4 + 0] = new Vector2(1, i / div);
            uvs[i * 4 + 1] = new Vector2(0, i / div);
            uvs[i * 4 + 2] = new Vector2(1, i / div);
            uvs[i * 4 + 3] = new Vector2(0, i / div);
        }

        mesh.uv = uvs;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void CalculateVerts(Vector3 back, Vector3 middle, Vector3 front, List<Vector3> verts)
    {
        List<Vector3> direction = new List<Vector3>();

        if (back != Vector3.back)
        {
            direction.Add(middle - back);
        }

        if (front != Vector3.back)
        {
            direction.Add(middle - front);
        }

        float[] gradient = direction.Select(a => a.z / a.x).ToArray();

        if (gradient.Length == 2)
        {
            if (gradient[0] == gradient[1])
            {
                float multi = Mathf.Sqrt(Mathf.Pow(-1 / gradient[0], 2) + 1);
                verts.Add(middle + new Vector3(1 / multi, 0, -1 / gradient[0] / multi));
                verts.Add(middle - new Vector3(1 / multi, 0, -1 / gradient[0] / multi));
                return;
            }
        }

        float[] intercept = new float[4];
        for (int j = 0; j < gradient.Length; j++)
        {
            float multi = Mathf.Sqrt(Mathf.Pow(-1 / gradient[j], 2) + 1);
            Vector3 vector = middle + new Vector3(1 / multi, 0, -1 / gradient[j] / multi) * pathWidth / 2;
            intercept[j * 2] = vector.z - vector.x * gradient[j];
            vector = middle - new Vector3(1 / multi, 0, -1 / gradient[j] / multi);
            intercept[j * 2 + 1] = vector.z - vector.x * gradient[j];
        }

        if (gradient.Length == 1)
        {
            verts.Add(new Vector3((middle.z - intercept[0]) / gradient[0], 0, middle.z));
            verts.Add(new Vector3((middle.z - intercept[1]) / gradient[0], 0, middle.z));
        }
        else
        {
            float intercectionX = (intercept[2] - intercept[0]) / (gradient[0] - gradient[1]);
            verts.Add(new Vector3(intercectionX, 0, gradient[0] * intercectionX + intercept[0]));
            intercectionX = (intercept[3] - intercept[1]) / (gradient[0] - gradient[1]);
            verts.Add(new Vector3(intercectionX, 0, gradient[0] * intercectionX + intercept[1]));
        }
    }

    private class PathInfo
    {
        public float totalDistance;
    }

    private class BlockPathInfo : PathInfo
    {
        public IReadOnlyList<Vector3> path;

        public BlockPathInfo(IReadOnlyList<Vector3> path, float totalDistance)
        {
            this.path = path;
            this.totalDistance = totalDistance;
        }
    }

    private class LinePathInfo : PathInfo
    {
        public IReadOnlyList<Vector3> path;
        public IReadOnlyList<float> pathDistance;

        public LinePathInfo(IReadOnlyList<Vector3> path, IReadOnlyList<float> pathDistance, float totalDistance)
        {
            this.path = path;
            this.pathDistance = pathDistance;
            this.totalDistance = totalDistance;
        }
    }
}
