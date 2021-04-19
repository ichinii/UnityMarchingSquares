using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPointsRendererScript : MonoBehaviour
{
    public GameObject gridObject;

    private void Start()
    {
        ComputeMesh();
    }

    void Update()
    {
    }

    void ComputeMesh()
    {
        var grid = gridObject.GetComponent<GridScript>();

        var positions = new List<Vector3>();
        var colors = new List<Color>();
        var uvs = new List<Vector2>();
        var indices = new List<int>();

        positions.Capacity = 2048;
        uvs.Capacity = 2048;
        indices.Capacity = 2048;

        for (var x = 0; x < grid.size.x; ++x)
        {
            for (var y = 0; y < grid.size.y; ++y)
            {
                var height = grid.cells[x, y];
                var indexOffset = positions.Count;

                var sz = 0.2f;
                var offset = sz / -2.0f;
                var xf = x - grid.size.x * 0.5f + offset;
                var yf = y - grid.size.y * 0.5f + offset;

                positions.Add(new Vector3(xf, yf));
                positions.Add(new Vector3(xf + sz, yf));
                positions.Add(new Vector3(xf + sz, yf + sz));
                positions.Add(new Vector3(xf, yf + sz));

                var color = height >= grid.threshold ? 1.0f : height / grid.threshold;
                var alpha = height >= grid.threshold ? 1.0f : 0.2f;
                colors.Add(new Color(height, height, height, alpha));
                colors.Add(new Color(height, height, height, alpha));
                colors.Add(new Color(height, height, height, alpha));
                colors.Add(new Color(height, height, height, alpha));

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));

                indices.Add(indexOffset);
                indices.Add(indexOffset + 1);
                indices.Add(indexOffset + 2);
                indices.Add(indexOffset);
                indices.Add(indexOffset + 2);
                indices.Add(indexOffset + 3);
            }
        }

        var mesh = new Mesh();
        mesh.vertices = positions.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.colors = colors.ToArray();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
