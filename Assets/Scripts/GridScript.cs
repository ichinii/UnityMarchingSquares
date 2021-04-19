using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public Vector2 phase = new Vector2(5, 5);
    public Vector2Int size = new Vector2Int(10 * 16 / 9, 10);
    public float density = 1.0f;
    public float threshold = 0.5f;

    public float[,] cells;
    public Texture2D texture;

    void Awake()
    {
        ComputeGrid(density);
        GenTexture();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private void ComputeGrid(float density)
    {
        cells = new float[size.x, size.y];

        for (var x = 0; x < size.x; ++x)
        {
            for (var y = 0; y < size.y; ++y)
            {
                var xf = (float)x / size.x * density + phase.x;
                var yf = (float)y / size.y * density + phase.y;
                if (y == 0)
                    cells[x, y] = .8f;
                else
                {
                    float height = Mathf.PerlinNoise(xf, yf);
                    cells[x, y] = height;
                    //cells[x, y] = 1.0f - Mathf.Min(1.0f, (new Vector2(x, y) - new Vector2(size.x, size.y) / 2.0f).magnitude * 0.1f);
                }
            }
        }
    }

    public int step(int x, int y)
    {
        return cells[x, y] >= threshold ? 1 : 0;
    }

    public int Length()
    {
        return size.x * size.y;
    }

    public static Vector2Int IndexToCoord(int i, Vector2Int size)
    {
        return new Vector2Int(i % size.x, i / size.x);
    }

    public Vector2Int IndexToCoord(int i)
    {
        return IndexToCoord(i, size);
    }

    public static int CoordToIndex(Vector2Int coord, Vector2Int size)
    {
        return coord.x + coord.y * size.x;
    }

    public int CoordToIndex(Vector2Int coord)
    {
        return CoordToIndex(coord, size);
    }

    private void GenTexture()
    {
        var pixels = new float[Length()];

        for (var i = 0; i < Length(); ++i)
        {
            var coord = IndexToCoord(i);
            var height = cells[coord.x, coord.y];
            pixels[i] = height;
        }

        texture = new Texture2D(size.x, size.y, TextureFormat.RFloat, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixelData(pixels, 0);
        texture.Apply();
    }
}
