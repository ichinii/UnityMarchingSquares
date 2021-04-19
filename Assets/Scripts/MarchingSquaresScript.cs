using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquaresScript : MonoBehaviour
{
    public GameObject gridObject;

    void Start()
    {
        GetComponent<Renderer>().material.mainTexture = gridObject.GetComponent<GridScript>().texture;
        ComputeMesh();
    }

    void Update()
    {
    }

    float Interpolate(float a, float b, float threshold)
    {
        // smoothstep thanks to https://www.khronos.org/registry/OpenGL-Refpages/gl4/html/smoothstep.xhtml
        var t = Mathf.Clamp((threshold - a) / (b - a), 0.0f, 1.0f);
        return t * t * (3.0f - 2.0f * t);
    }

    int[] SquareIndicesLoopup(int squareIndex, Vector2Int offset, Vector2Int size, int horizontalEdgesBegin, int verticalEdgesBegin)
    {
        var horizontalEdgesSize = size - new Vector2Int(1, 0);
        var verticalEdgesSize = size - new Vector2Int(0, 1);

        var a = GridScript.CoordToIndex(offset, size);
        var b = GridScript.CoordToIndex(offset + new Vector2Int(1, 0), size);
        var c = GridScript.CoordToIndex(offset + new Vector2Int(1, 1), size);
        var d = GridScript.CoordToIndex(offset + new Vector2Int(0, 1), size);

        var h = horizontalEdgesBegin + GridScript.CoordToIndex(offset, size - new Vector2Int(1, 0));
        var i = horizontalEdgesBegin + GridScript.CoordToIndex(offset + new Vector2Int(0, 1), size - new Vector2Int(1, 0));

        var v = verticalEdgesBegin + GridScript.CoordToIndex(offset, size);
        var w = verticalEdgesBegin + GridScript.CoordToIndex(offset + new Vector2Int(1, 0), size);

        switch (squareIndex) {
            default: return new int[] { };
            case 1:  return new int[] { a, h, v };
            case 2:  return new int[] { b, w, h };
            case 3:  return new int[] { a, b, w, a, w, v };
            case 4:  return new int[] { c, i, w};
            case 5:  return new int[] { a, h, w, a, w, c, a, c, i, a, i, h};
            case 6:  return new int[] { b, c, i, b, i, h};
            case 7:  return new int[] { a, b, c, a, c, i, a, i, v};
            case 8:  return new int[] { d, v, i};
            case 9:  return new int[] { a, h, i, a, i, d};
            case 10: return new int[] { b, w, i, b, i, d, b, d, v, b, v, h};
            case 11: return new int[] { a, b, w, a, w, i, a, i, d};
            case 12: return new int[] { c, d, v, c, v, w};
            case 13: return new int[] { a, h, w, a, w, c, a, c, d};
            case 14: return new int[] { b, c, d, b, d, v, b, v, h};
            case 15: return new int[] { a, b, c, a, c, d };
        }
    }

    float Depth(float height, float threshold)
    {
        return -(height - threshold) / (1.0f - threshold);
    }

    void ComputeMesh()
    {
        var grid = gridObject.GetComponent<GridScript>();

        var cornersCount = grid.size.x * grid.size.y;
        var horizontalEdgesCount = (grid.size.x - 1) * grid.size.y;
        var verticalEdgesCount = grid.size.x * (grid.size.y - 1);
        var positionsCount = cornersCount + horizontalEdgesCount + verticalEdgesCount;

        var horizontalEdgesBegin = cornersCount;
        var verticalEdgesBegin = cornersCount + horizontalEdgesCount;

        var positions = new Vector3[positionsCount];
        var indices = new List<int>();
        indices.Capacity = 1024;

        for (var x = 0; x < grid.size.x; ++x)
        {
            for (var y = 0; y < grid.size.y; ++y)
            {
                var offset = new Vector2Int(x, y);
                var offsetf = offset - grid.size * new Vector2(0.5f, 0.5f);
                var index = grid.CoordToIndex(new Vector2Int(x, y));
                var a = grid.cells[x, y];
                var offsetf3 = new Vector3(offsetf.x, offsetf.y, Depth(a, grid.threshold));

                positions[index] = offsetf3;

                if (x < grid.size.x - 1)
                {
                    var b = grid.cells[x + 1, y];
                    var ratio = Interpolate(a, b, grid.threshold);
                    var horizontalEdgeIndex = GridScript.CoordToIndex(offset, grid.size - new Vector2Int(1, 0));
                    positions[horizontalEdgesBegin + horizontalEdgeIndex] = offsetf + new Vector2(ratio, 0.0f);
                    positions[horizontalEdgesBegin + horizontalEdgeIndex].z = Depth(Mathf.Lerp(a, b, ratio), grid.threshold);
                }

                if (y < grid.size.y - 1)
                {
                    var b = grid.cells[x, y + 1];
                    var ratio = Interpolate(a, b, grid.threshold);
                    positions[verticalEdgesBegin + index] = offsetf + new Vector2(0.0f, ratio);
                    positions[verticalEdgesBegin + index].z = Depth(Mathf.Lerp(a, b, ratio), grid.threshold);
                }

                if (x < grid.size.x - 1 && y < grid.size.y - 1)
                {
                    var b = grid.step(x, y);
                    var c = grid.step(x + 1, y);
                    var d = grid.step(x + 1, y + 1);
                    var e = grid.step(x, y + 1);

                    var squareIndex = b | c * 2 | d * 4 | e * 8;

                    indices.AddRange(SquareIndicesLoopup(squareIndex, offset, grid.size, horizontalEdgesBegin, verticalEdgesBegin));
                }
            }
        }

        var mesh = new Mesh();
        mesh.vertices = positions;
        mesh.triangles = indices.ToArray();
        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
