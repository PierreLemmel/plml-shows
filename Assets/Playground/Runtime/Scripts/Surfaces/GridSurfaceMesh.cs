using Plml.Jace;
using System;
using UnityEngine;

namespace Plml.Playground.Surfaces
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class GridSurfaceMesh : MonoBehaviour
    {
        public GridSurfaceParameters surfaceParameters = new GridSurfaceParameters();

        public SurfaceFormula formula = new();


        public Vector2 Speed { get; set; } = new Vector2(0.0f, 0.0f);

        private Mesh mesh;

        private Vector3[] vertices;
        private Vector2[] uvs;
        private int[] points;

        private Vector2 accumulatedOffset = Vector2.zero;

        private Func<float, float, float, float> surfaceFunction = (x, y, t) => 0.0f;

        private readonly ISpy spy = Spy.CreateNew();
        private readonly CalculationEngine mathEngine = new();

        private void Awake()
        {
            spy
                .When(surfaceParameters)
                .HasChangesOn(
                    p => p.rows,
                    p => p.columns
                )
                .Do(SetupMesh);

            spy
                .When(formula)
                .HasChangesOn(p => p.text)
                .Do(SetupFunction);

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            mesh = meshFilter.mesh;
        }

        private void Update()
        {
            spy.DetectChanges();

            accumulatedOffset += Time.deltaTime * Speed;

            float width = surfaceParameters.width;
            float height = surfaceParameters.height;

            float halfWidth = width / 2.0f;
            float halfHeight = height / 2.0f;

            vertices.MapTo(uvs, vertex => new Vector2((vertex.x + halfWidth) / width, (vertex.z + halfHeight) / height));

            int nbOfCols = surfaceParameters.columns;
            int nbOfRows = surfaceParameters.rows;

            int pointsPerRow = nbOfCols + 2;
            int pointsPerCol = nbOfRows + 2;

            int quadsPerRow = nbOfCols + 1;
            int quadsPerCol = nbOfRows + 1;

            int k = 0;
            for (int row = 0; row < quadsPerCol; row++)
            {
                for (int col = 0; col < quadsPerRow; col++)
                {
                    points[k++] = row * pointsPerRow + col;
                    points[k++] = (row + 1) * pointsPerRow + col;
                    points[k++] = (row + 1) * pointsPerRow + col + 1;
                    points[k++] = row * pointsPerRow + col + 1;
                }
            }

            UpdatePoints();
            UpdateMesh();
        }

        private void SetupMesh()
        {
            mesh.Clear();

            int nbOfCols = surfaceParameters.columns;
            int nbOfRows = surfaceParameters.rows;

            int pointsPerRow = nbOfCols + 2;
            int pointsPerCol = nbOfRows + 2;

            int quadsPerRow = nbOfCols + 1;
            int quadsPerCol = nbOfRows + 1;

            vertices = new Vector3[pointsPerRow * pointsPerCol];
            uvs = new Vector2[pointsPerRow * pointsPerCol];
            points = new int[4 * quadsPerRow * quadsPerCol];

            UpdateMesh();
        }

        private void UpdatePoints()
        {
            int cols = surfaceParameters.columns;
            int rows = surfaceParameters.rows;

            float t = Time.time;

            float width = surfaceParameters.width;
            float height = surfaceParameters.height;

            int pointsPerRow = cols + 2;
            int pointsPerCol = rows + 2;

            float colMaxf = cols - 1.0f;
            float rowMaxf = rows - 1.0f;

            float halfWidth = width / 2.0f;
            float halfHeight = height / 2.0f;

            float spacingX = width / cols;
            float spacingZ = height / rows;

            float originX = accumulatedOffset.x;
            float originZ = accumulatedOffset.y;

            float relOffsetX = ((accumulatedOffset.x % spacingX / spacingX) + 0.5f) % 1.0f - 0.5f;
            float offsetX = -relOffsetX * spacingX;
            float relOffsetZ = ((accumulatedOffset.y % spacingZ / spacingZ) + 0.5f) % 1.0f - 0.5f;
            float offsetZ = -relOffsetZ * spacingZ;

            float rowRange = height * (rows - 1) / rows;
            float colRange = width * (cols - 1) / cols;

            for (int row = 0; row < rows; row++)
            {
                int y = row + 1;
                float zCoord = rowRange * (row / rowMaxf - 0.5f) + offsetZ;
                for (int col = 0; col < cols; col++)
                {
                    int x = col + 1;
                    float xCoord = colRange * (col / colMaxf - 0.5f) + offsetX;
                    SetPoint(x, y, xCoord, zCoord);
                }

                SetPoint(0, y, -halfWidth, zCoord);
                SetPoint(pointsPerRow - 1, y, halfWidth, zCoord);
            }

            for (int col = 0; col < cols; col++)
            {
                int x = col + 1;
                float xCoord = colRange * (col / colMaxf - 0.5f) + offsetX;
                SetPoint(x, 0, xCoord, -halfHeight);
                SetPoint(x, pointsPerCol - 1, xCoord, halfHeight);
            }

            SetPoint(0, 0, -halfWidth, -halfHeight);
            SetPoint(0, pointsPerCol - 1, -halfWidth, halfHeight);
            SetPoint(pointsPerRow - 1, 0, halfWidth, -halfHeight);
            SetPoint(pointsPerRow - 1, pointsPerCol - 1, halfWidth, halfHeight);

            void SetPoint(int x, int y, float xCoord, float zCoord)
            {
                float yCoord = surfaceFunction(xCoord + originX, zCoord + originZ, t);
                Vector3 point = new(xCoord, yCoord, zCoord);

                vertices[y * pointsPerRow + x] = point;
            }
        }

        private void SetupFunction()
        {
            try
            {
                Func<float, float, float, float> result = mathEngine.Formula(formula.text)
                        .Parameter("x")
                        .Parameter("y")
                        .Parameter("t")
                        .Build();

                formula.error = "";

                surfaceFunction = result;
            }
            catch (Exception ex)
            {
                formula.error = ex.Message;
            }
        }

        private void UpdateMesh()
        {
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.SetIndices(points, MeshTopology.Quads, 0);
        }
    }
}
