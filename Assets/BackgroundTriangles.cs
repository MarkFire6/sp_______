using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FlatLandscape : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float spacing = 1f;
    public float jitter = 0.4f;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // 1. Pre-calculate the "jittered" grid points so triangles share the same corner positions
        Vector3[,] grid = new Vector3[width + 1, height + 1];
        float hW = (width * spacing) / 2f;
        float hH = (height * spacing) / 2f;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                grid[x, y] = new Vector3(
                    x * spacing - hW + Random.Range(-jitter, jitter),
                    y * spacing - hH + Random.Range(-jitter, jitter),
                    10 // Your Z position
                );
            }
        }

        // 2. Build the mesh with 6 unique vertices per "square" (cell) to allow flat coloring
        int triCount = width * height * 2;
        Vector3[] vertices = new Vector3[triCount * 3];
        int[] triangles = new int[triCount * 3];
        Color[] colors = new Color[triCount * 3];

        int v = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Get the 4 corners from our jittered grid
                Vector3 bl = grid[x, y];
                Vector3 tl = grid[x, y + 1];
                Vector3 tr = grid[x + 1, y + 1];
                Vector3 br = grid[x + 1, y];

                // Triangle 1
                Color c1 = GetBlueColor();
                vertices[v] = bl; vertices[v + 1] = tl; vertices[v + 2] = tr;
                colors[v] = c1; colors[v + 1] = c1; colors[v + 2] = c1;
                triangles[v] = v; triangles[v + 1] = v + 1; triangles[v + 2] = v + 2;
                v += 3;

                // Triangle 2
                Color c2 = GetBlueColor();
                vertices[v] = bl; vertices[v + 1] = tr; vertices[v + 2] = br;
                colors[v] = c2; colors[v + 1] = c2; colors[v + 2] = c2;
                triangles[v] = v; triangles[v + 1] = v + 1; triangles[v + 2] = v + 2;
                v += 3;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    Color GetBlueColor()
    {
        float b = Random.Range(0.05f, 0.1f);
        float s = Random.Range(0.0f, 0.05f);
        return new Color(s, s, s + b);
    }
}
