using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSurfaceRendererScript : MonoBehaviour
{
    public GameObject gridObject;

    void Start()
    {
        ComputeMesh();
    }

    void Update()
    {
        GetComponent<Renderer>().material.SetFloat("Threshold", gridObject.GetComponent<GridScript>().threshold);
    }

    void ComputeMesh()
    {
        var grid = gridObject.GetComponent<GridScript>();

        var positions = new List<Vector3>();
        var uvs = new List<Vector2>();
        var indices = new List<int>();

        positions.Capacity = grid.Length();
        uvs.Capacity = grid.Length();
        indices.Capacity = grid.Length() * 6;

        for (var x = 0; x < grid.size.x; ++x)
        {
            for (var y = 0; y < grid.size.y; ++y)
            {
                float height = grid.cells[x, y];

                var xf = x - grid.size.x * 0.5f;
                var yf = y - grid.size.y * 0.5f;
                positions.Add(new Vector3(xf, yf, -height));

                var uv = new Vector2(x, y) / grid.size;
                uvs.Add(new Vector2(uv.x, uv.y));

                if (x < grid.size.x - 1 && y < grid.size.y - 1)
                {
                    if (x % 2 == 0 && y % 2 == 0 || x % 2 == 1 && y % 2 == 1)
                    {
                        indices.Add(grid.CoordToIndex(new Vector2Int(x, y)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x + 1, y)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x + 1, y + 1)));

                        indices.Add(grid.CoordToIndex(new Vector2Int(x, y)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x + 1, y + 1)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x, y + 1)));
                    }
                    else
                    {
                        indices.Add(grid.CoordToIndex(new Vector2Int(x + 1, y)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x + 1, y + 1)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x, y + 1)));

                        indices.Add(grid.CoordToIndex(new Vector2Int(x + 1, y)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x, y + 1)));
                        indices.Add(grid.CoordToIndex(new Vector2Int(x, y)));
                    }
                }
            }
        }

        var mesh = new Mesh();
        mesh.vertices = positions.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
