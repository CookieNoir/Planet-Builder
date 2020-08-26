using UnityEngine;
public class PlanetTest : MonoBehaviour
{
    [Header("Main Values")]
    [Range(0, 65535)] public int key;
    [Range(6, 16)] public int verticesPerSegment = 10;
    private int verticesCount;

    [Space(20, order = 0)]
    [Header("Game", order = 1)]
    [Min(0.2f)] public float blockScale = 0.5f;
    [Min(0f)] public float blockOffset = 0.02f;
    [Space(15)]
    [Range(0f, 0.9f)] public float minObstaclesRatio = 0.3f;
    [Range(0f, 0.9f)] public float maxObstaclesRatio = 0.7f;
    [Space(15)]
    [Range(1, 5)] public int minBuildingHeight = 1;
    [Range(1, 5)] public int maxBuildingHeight = 5;

    [Space(20, order = 0)]
    [Header("Relief", order = 1)]
    public MeshFilter reliefMesh;
    [Space(15)]
    [Min(0.5f)] public float minCoreRadius = 0.8f;
    [Min(0.5f)] public float maxCoreRadius = 1.2f;
    private float coreRadius = 1f;
    [Space(8)]
    [Min(0f)] public float minGroundRadius = 0.2f;
    [Min(0f)] public float maxGroundRadius = 0.3f;
    private float groundRadius = 0.5f;
    private float coreGroundRadius = 1.5f;
    [Space(8)]
    [Min(0f)] public float lowestGroundRadiusAllowed = 0f;
    [Space(15)]
    [Range(0f, 1f)] public float heightDepthRatio = 0.5f; // 0 - только горы, 1 - только ямы
    [Space(8)]
    [Min(0f)] public float minHeight = 0.15f;
    [Min(0f)] public float maxHeight = 0.3f;
    [Space(8)]
    [Min(0f)] public float minDepth = 0.1f;
    [Min(0f)] public float maxDepth = 0.18f;
    [Space(15)]
    [Range(0f, 0.5f)] public float reliefBlendingOffset = 0.225f;
    [Range(0f, 1f)] public float coreOffsetMultiplier = 0.34f;
    public bool rotateRelief;
    [Space(15)]
    public Color reliefColorFree;
    public Color reliefColorHigher;
    public Color reliefColorLower;
    [Space(8)]
    [Min(0f)] public float reliefColorThreshold = 0.01f;
    private float highestCoreRadius;
    private float lowestGroundRadius;

    [Space(20, order = 0)]
    [Header("Decorative Objects", order = 1)]
    public bool turnOnDecorativeObjects;
    public Transform decorativeObjectsContainer;
    [Space(15)]
    [Range(0f, 1f)] public float decorativeObjectsDensity;
    private int[] decorativeObjectsIndices;
    [Space(8)]
    public GameObject[] freeSegmentObjects;
    public GameObject[] higherSegmentObjects;
    public GameObject[] lowerSegmentObjects;
    private IInteractable[] groundObjects;

    [Space(20, order = 0)]
    [Header("Shadow", order = 1)]
    public bool turnOnShadow;
    public MeshFilter shadowMesh;
    [Space(15)]
    [Range(0f, 360f)] public float minShadowAngle = 0f;
    [Range(0f, 360f)] public float shadowAngleRange = 360f;
    private float shadowAngle;
    [Space(8)]
    [Range(-1f, 1f)] public float minShadowLineRelativePosition = -1f;
    [Range(-1f, 1f)] public float maxShadowLineRelativePosition = 1f;
    private float shadowLineRelativePosition;
    [Space(8)]
    [Range(-1f, 1f)] public float minShadowArcRelativePosition = -1f;
    [Range(-1f, 1f)] public float maxShadowArcRelativePosition = 1f;
    private float shadowArcRelativePosition;
    [Space(15)]
    [Min(5f)] public float maxShadowLength = 8f;
    [Range(15, 31)] public int arcVerticesCount = 23;
    private const float _EPS = 0.01f;

    [Space(20, order = 0)]
    [Header("Water", order = 1)]
    public bool turnOnWater;
    public MeshFilter waterMesh;
    public MeshRenderer waterRenderer;
    private Material waterMaterial;
    [Space(15)]
    public bool fillWholePlanet;
    [Min(0f)] public float waterOffset = 0.01f; // Может быть отрицательным значением
    [Space(15)]
    public Color waterColor;
    public Color waterDepthColor;
    [Space(8)]
    [Range(0f, 0.002f)] public float waterWavesAmplitude;
    [Range(0f, 60f)] public float waterWavesFrequency;
    [Range(0f, 4f)] public float waterWavesSpeed;

    [Space(20, order = 0)]
    [Header("Atmosphere", order = 1)]
    public bool turnOnAtmosphere;
    public MeshFilter atmosphereMesh;
    public MeshRenderer atmosphereRenderer;
    private Material atmosphereMaterial;
    [Space(15)]
    public Color atmosphereColorBottom;
    public Color atmosphereColorMiddle;
    public Color atmosphereColorTop;
    [Space(8)]
    [Range(0.01f, 0.99f)] public float atmosphereColorBorder;
    [Space(8)]
    public Color atmosphereColorNight;

    [Space(20, order = 0)]
    [Header("Border Ring", order = 1)]
    public bool turnOnBorderRing;
    public MeshFilter borderRingMesh;
    [Space(15)]
    [Min(0f)] public float bottomBorderOffset = 0f;
    [Min(0.01f)] public float topBorderOffset = 0.05f;
    [Space(15)]
    public Color borderRingColorFree;
    public Color borderRingColorHigher;
    public Color borderRingColorLower;
    [Space(8)]
    [Range(0f, 0.5f)] public float borderRingColorBlendingOffset;

    [Space(20, order = 0)]
    [Header("Planet Properties", order = 1)]
    [ReadOnly] public float planetRadius = 4f;
    [ReadOnly] public int segments = 15;
    [ReadOnly] public float segmentAngle = 24f;
    [ReadOnly] public float reliefRotation;
    [ReadOnly] public int buildingHeight;
    [ReadOnly] public bool[] segmentsType; // false - сегмент досягаем, true - сегмент содержит препятствие
    [ReadOnly] public float[] segmentsHeight;
    private float[] segmentsCoreHeight;
    private float halfSegmentAngle;

    public int GetSegmentByAngle(float angle)
    {
        angle += halfSegmentAngle - reliefRotation;
        angle -= 360f * Mathf.Floor(angle / 360f);
        return Mathf.FloorToInt(angle / segmentAngle) % segments;
    }

    private void Start()
    {
        waterMaterial = waterRenderer.material;
        atmosphereMaterial = atmosphereRenderer.material;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            key = Random.Range(0, 65535);
            GetPlanet();
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            GetPlanet();
        }
    }

    private void GetPlanet()
    {
        RandomizeValues();

        GetRelief();
        ClearDecorativeObjects();
        if (turnOnDecorativeObjects) GetDecorativeObjects();
        waterMesh.gameObject.SetActive(turnOnWater);
        if (turnOnWater) GetWater();
        shadowMesh.gameObject.SetActive(turnOnShadow);
        if (turnOnShadow) GetShadow();
        atmosphereMesh.gameObject.SetActive(turnOnAtmosphere);
        if (turnOnAtmosphere) GetAtmosphere();
        borderRingMesh.gameObject.SetActive(turnOnBorderRing);
        if (turnOnBorderRing) GetBorderRing();
    }

    private void RandomizeValues()
    {
        Random.InitState(key);
        coreRadius = Random.Range(minCoreRadius, maxCoreRadius);
        groundRadius = Random.Range(minGroundRadius, maxGroundRadius);
        coreGroundRadius = coreRadius + groundRadius;
        segments = Mathf.FloorToInt(2 * Mathf.PI * coreGroundRadius / (blockScale + blockOffset));
        segmentAngle = 360f / segments;
        halfSegmentAngle = 180f / segments;
        verticesCount = verticesPerSegment * segments;

        int obstaclesCount = Mathf.FloorToInt(segments * Random.Range(minObstaclesRatio, maxObstaclesRatio));
        if (obstaclesCount > segments - 3) obstaclesCount = segments - 3;
        if (obstaclesCount < 0) obstaclesCount = 0;

        buildingHeight = Random.Range(minBuildingHeight, maxBuildingHeight + 1);
        planetRadius = coreGroundRadius + buildingHeight * blockScale;

        lowestGroundRadius = coreGroundRadius;

        segmentsType = new bool[segments];
        segmentsHeight = new float[segments];
        segmentsCoreHeight = new float[segments];
        decorativeObjectsIndices = new int[segments];

        int obstaclePosition = Random.Range(0, segments);
        highestCoreRadius = coreRadius;
        float heightValue,
              depthMultiplier = 1f / heightDepthRatio,
              heightMultiplier = 1f / (1f - heightDepthRatio);

        for (int i = 0; i < segments; ++i)
        {
            segmentsType[i] = false;
            segmentsHeight[i] = coreGroundRadius;
            segmentsCoreHeight[i] = coreRadius;
            decorativeObjectsIndices[i] = -1;
        }

        while (obstaclesCount > 0)
        {
            segmentsType[obstaclePosition] = true;
            heightValue = Random.value;
            if (heightDepthRatio == 1)
            {
                heightValue = -Mathf.Lerp(maxDepth, minDepth, heightValue);
            }
            else
            {
                if (heightValue >= heightDepthRatio)
                {
                    heightValue = Mathf.Lerp(minHeight, maxHeight, (heightValue - heightDepthRatio) * heightMultiplier);
                }
                else
                {
                    heightValue = -Mathf.Lerp(maxDepth, minDepth, heightValue * depthMultiplier);
                }
            }
            segmentsHeight[obstaclePosition] += heightValue;
            if (segmentsHeight[obstaclePosition] < lowestGroundRadius) lowestGroundRadius = segmentsHeight[obstaclePosition];
            segmentsCoreHeight[obstaclePosition] += coreOffsetMultiplier * heightValue;
            if (segmentsHeight[obstaclePosition] - segmentsCoreHeight[obstaclePosition] < lowestGroundRadiusAllowed)
                segmentsCoreHeight[obstaclePosition] = segmentsHeight[obstaclePosition] - lowestGroundRadiusAllowed;
            if (segmentsCoreHeight[obstaclePosition] > highestCoreRadius) highestCoreRadius = segmentsCoreHeight[obstaclePosition];
            obstaclePosition = (obstaclePosition + (7 * obstaclePosition + 23) / 11 + 37 * obstaclesCount) % segments;
            while (segmentsType[obstaclePosition])
            {
                obstaclePosition = (obstaclePosition + 1) % segments;
            }
            obstaclesCount--;
        }

        reliefRotation = Random.Range(0f, segmentAngle);

        shadowAngle = Random.Range(minShadowAngle, minShadowAngle + shadowAngleRange);
        shadowLineRelativePosition = Random.Range(minShadowLineRelativePosition, maxShadowLineRelativePosition);
        shadowArcRelativePosition = Random.Range(minShadowArcRelativePosition, maxShadowArcRelativePosition);

        obstaclesCount = Mathf.FloorToInt(segments * decorativeObjectsDensity);

        if (obstaclesCount == segments)
        {
            for (int i = 0; i < segments; ++i)
            {
                SetDecorativeObjectIndex(i);
            }
        }
        else {
            obstaclePosition = Random.Range(0, segments);
            while (obstaclesCount > 0)
            {
                SetDecorativeObjectIndex(obstaclePosition);

                obstaclePosition = (obstaclePosition + (31 * obstaclePosition + 87) / 17 + 41 * obstaclesCount) % segments;
                while (decorativeObjectsIndices[obstaclePosition] != -1)
                {
                    obstaclePosition = (obstaclePosition + 1) % segments;
                }
                obstaclesCount--;
            }
        }
    }

    private void SetDecorativeObjectIndex(in int segment)
    {
        int upperBound;
        if (segmentsType[segment])
        {
            if (segmentsHeight[segment] >= coreGroundRadius)
                upperBound = higherSegmentObjects.Length;
            else
                upperBound = lowerSegmentObjects.Length;
        }
        else upperBound = freeSegmentObjects.Length;

        if (upperBound == 0) decorativeObjectsIndices[segment] = -2;
        else decorativeObjectsIndices[segment] = Random.Range(0, upperBound);
    }

    private void GetRelief()
    {
        int totalVerticesCount = verticesCount * 3 + 3;
        int[] tris0 = new int[3 * verticesCount];
        int[] tris1 = new int[6 * verticesCount];

        Vector3[] vertices = new Vector3[totalVerticesCount];
        Color[] colors = new Color[totalVerticesCount];
        Vector2[] uvs = new Vector2[totalVerticesCount];

        vertices[totalVerticesCount - 1] = Vector3.zero;
        uvs[totalVerticesCount - 1] = Vector2.zero;

        int index = 0, index2, triIndex;
        float angle = 0f, offset = 1f,
              verticesCountInverted = 1f / verticesCount,
              highestCoreRadiusInverted = 0.5f / highestCoreRadius; // Используется в UV-развертке внутренней окружности

        Vector2 uvCenter = new Vector2(0.5f, 0.5f);
        int p0, p1;

        index = 2 * verticesCount;
        vertices[index] = Vector3.right * segmentsCoreHeight[0];
        if (segmentsType[0])
        {
            if (segmentsHeight[0] >= coreGroundRadius) colors[index] = reliefColorHigher;
            else colors[index] = reliefColorLower;
        }
        else colors[index] = reliefColorFree;
        uvs[index] = new Vector2(offset, 0f);
        p0 = index;

        index2 = 3 * verticesCount + 1;
        vertices[index2] = Vector3.right * segmentsHeight[0];
        colors[index2] = colors[index];
        uvs[index2] = new Vector2(offset, 1f);
        p1 = index2;

        float verticesPerSegmentInverted = 1f / verticesPerSegment;
        int vs, vn;
        Vector3 angularOne;
        float prevHeight = segmentsHeight[0], curHeight, adjacentHeight, blendFunctionResult,
              prevCoreHeight = segmentsCoreHeight[0], curCoreHeight, adjacentCoreHeight, adjacentPosition;
        bool prevType = segmentsType[0], curType;
        for (int s = segments - 1; s > -1; --s)
        {
            curHeight = segmentsHeight[s];
            curCoreHeight = segmentsCoreHeight[s];
            curType = segmentsType[s];
            vs = s * verticesPerSegment;
            for (int v = verticesPerSegment - 1; v > -1; --v)
            {
                vn = vs + v;
                offset = vn * verticesCountInverted;
                angle = offset * 360f;
                adjacentPosition = 1f - v * verticesPerSegmentInverted;
                blendFunctionResult = BlendFunction(adjacentPosition, reliefBlendingOffset);
                adjacentHeight = Mathf.Lerp(prevHeight, curHeight, blendFunctionResult);

                adjacentCoreHeight = Mathf.Lerp(prevCoreHeight, curCoreHeight, blendFunctionResult);
                angularOne = PolarSystem.Position(angle, 1f, Vector3.zero);

                vertices[vn] = angularOne * adjacentCoreHeight;
                uvs[vn] = angularOne * adjacentCoreHeight * highestCoreRadiusInverted;

                index = verticesCount + vn;
                vertices[index] = vertices[vn];
                if (prevType == curType)
                {
                    if (curType)
                    {
                        if (adjacentHeight >= coreGroundRadius) colors[index] = reliefColorHigher;
                        else colors[index] = reliefColorLower;
                    }
                    else colors[index] = reliefColorFree;
                }
                else
                {
                    if (adjacentHeight - coreGroundRadius > reliefColorThreshold)
                    {
                        colors[index] = reliefColorHigher;
                    }
                    else if (coreGroundRadius - adjacentHeight > reliefColorThreshold)
                    {
                        colors[index] = reliefColorLower;
                    }
                    else colors[index] = reliefColorFree;
                }
                uvs[index] = new Vector2(offset, 0f);

                triIndex = 6 * vn;
                tris1[triIndex] = p0;
                tris1[triIndex + 1] = p1;
                tris1[triIndex + 2] = index;
                p0 = index;

                index2 = 2 * verticesCount + vn + 1;
                vertices[index2] = angularOne * adjacentHeight;
                colors[index2] = colors[index];
                uvs[index2] = new Vector2(offset, 1f);

                tris1[triIndex + 3] = p0;
                tris1[triIndex + 4] = p1;
                tris1[triIndex + 5] = index2;
                p1 = index2;
            }
            prevHeight = curHeight;
            prevCoreHeight = curCoreHeight;
            prevType = curType;
        }

        for (int i = 0; i < verticesCount - 1; ++i)
        {
            triIndex = 3 * i;
            tris0[triIndex] = i + 1;
            tris0[triIndex + 1] = i;
            tris0[triIndex + 2] = totalVerticesCount - 1;
        }
        triIndex = 3 * verticesCount - 3;
        tris0[triIndex] = 0;
        tris0[triIndex + 1] = verticesCount - 1;
        tris0[triIndex + 2] = totalVerticesCount - 1;

        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uvs;
        mesh.SetTriangles(tris0, 0);
        mesh.SetTriangles(tris1, 1);
        reliefMesh.mesh = mesh;

        if (rotateRelief) reliefMesh.transform.rotation = Quaternion.Euler(0f, 0f, reliefRotation);
        else reliefMesh.transform.rotation = Quaternion.identity;
    }

    private void ClearDecorativeObjects()
    {
        foreach (Transform child in decorativeObjectsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void GetDecorativeObjects()
    {
        groundObjects = new IInteractable[segments];
        for (int i = 0; i < segments; ++i)
        {
            if (decorativeObjectsIndices[i] > -1)
            {
                GameObject newDecorativeObject;
                float angle = segmentAngle * i;
                if (segmentsType[i])
                {
                    if (segmentsHeight[i] >= coreGroundRadius)
                        newDecorativeObject = higherSegmentObjects[decorativeObjectsIndices[i]];
                    else
                        newDecorativeObject = lowerSegmentObjects[decorativeObjectsIndices[i]];
                }
                else newDecorativeObject = freeSegmentObjects[decorativeObjectsIndices[i]];

                newDecorativeObject = Instantiate(newDecorativeObject, decorativeObjectsContainer);
                newDecorativeObject.transform.localPosition = PolarSystem.Position(angle, segmentsHeight[i], Vector3.zero);
                newDecorativeObject.transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
                newDecorativeObject.transform.localScale *= blockScale;
                groundObjects[i] = newDecorativeObject.GetComponent<IInteractable>();
                if (groundObjects[i] != null) groundObjects[i].Deploy(this, angle);
            }
        }
    }

    private void GetWater()
    {
        Vector3[] vertices = new Vector3[verticesCount];
        Color[] colors = new Color[verticesCount];
        Vector2[] uvs = new Vector2[verticesCount];
        int trianglesCount = verticesCount - 2;
        int[] triangles = new int[3 * trianglesCount];
        float angle,
              verticesCountInverted = 1f / verticesCount,
              targetRadius;
        if (fillWholePlanet) targetRadius = planetRadius;
        else targetRadius = coreGroundRadius;
        Vector3 angularOne;

        for (int i = verticesCount - 1; i >= 0; --i)
        {
            angle = i * verticesCountInverted * 360f;
            angularOne = PolarSystem.Position(angle, 1f, Vector3.zero);
            vertices[i] = angularOne * (targetRadius - waterOffset);
            colors[i] = waterColor;
            uvs[i] = angularOne * 0.5f;
        }

        int p0, p1 = 0, p2 = 1, triIndex;
        bool isForward = false;
        for (int i = 0; i < trianglesCount; ++i)
        {
            p0 = p1;
            p1 = p2;
            triIndex = 3 * i;
            if (isForward)
            {
                p2 = 2 + i / 2;
                triangles[triIndex] = p0;
                triangles[triIndex + 1] = p1;
                triangles[triIndex + 2] = p2;
            }
            else
            {
                p2 = verticesCount - 1 - i / 2;
                triangles[triIndex] = p0;
                triangles[triIndex + 1] = p2;
                triangles[triIndex + 2] = p1;
            }
            isForward = !isForward;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        waterMesh.mesh = mesh;

        waterMaterial.SetColor("_Color", waterDepthColor);
        waterMaterial.SetFloat("_Amplitude", waterWavesAmplitude);
        waterMaterial.SetFloat("_Frequency", waterWavesFrequency);
        waterMaterial.SetFloat("_Speed", waterWavesSpeed);
    }

    private void GetAtmosphere()
    {
        Vector3[] vertices = new Vector3[3 * verticesCount + 3];
        Color[] colors = new Color[3 * verticesCount + 3];
        Vector2[] uvs = new Vector2[3 * verticesCount + 3];
        int[] triangles = new int[12 * verticesCount];
        int index = verticesCount, p0 = index, p1, p2, triIndex;
        float angle = 0f, offset = 1f,
              verticesCountInverted = 1f / verticesCount,
              midRadius = Mathf.Lerp(lowestGroundRadius, planetRadius, atmosphereColorBorder);
        Vector3 angularOne;
        vertices[index] = new Vector3(lowestGroundRadius, 0f, 0f);
        colors[index] = atmosphereColorBottom;
        uvs[index] = new Vector2(offset, 0f);

        index += verticesCount + 1;
        vertices[index] = new Vector3(midRadius, 0f, 0f);
        colors[index] = atmosphereColorMiddle;
        uvs[index] = new Vector2(offset, atmosphereColorBorder);
        p1 = index;

        index += verticesCount + 1;
        vertices[index] = new Vector3(planetRadius, 0f, 0f);
        colors[index] = atmosphereColorTop;
        uvs[index] = new Vector2(offset, 1f);
        p2 = index;

        for (int i = verticesCount - 1; i > -1; --i)
        {
            offset = i * verticesCountInverted;
            angle = offset * 360f;
            angularOne = PolarSystem.Position(angle, 1f, Vector3.zero);

            index = i;
            vertices[index] = angularOne * lowestGroundRadius;
            colors[index] = atmosphereColorBottom;
            uvs[index] = new Vector2(offset, 0f);

            triIndex = 12 * index;
            triangles[triIndex] = p0;
            triangles[triIndex + 1] = p1;
            triangles[triIndex + 2] = index;
            p0 = index;

            index += verticesCount + 1;
            vertices[index] = angularOne * midRadius;
            colors[index] = atmosphereColorMiddle;
            uvs[index] = new Vector2(offset, atmosphereColorBorder);

            triangles[triIndex + 3] = p0;
            triangles[triIndex + 4] = p1;
            triangles[triIndex + 5] = index;

            triangles[triIndex + 6] = p1;
            triangles[triIndex + 7] = p2;
            triangles[triIndex + 8] = index;
            p1 = index;

            index += verticesCount + 1;
            vertices[index] = angularOne * planetRadius;
            colors[index] = atmosphereColorTop;
            uvs[index] = new Vector2(offset, 1f);

            triangles[triIndex + 9] = p1;
            triangles[triIndex + 10] = p2;
            triangles[triIndex + 11] = index;
            p2 = index;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        atmosphereMesh.mesh = mesh;

        atmosphereMaterial.SetColor("_Color", atmosphereColorNight);
    }

    private void GetBorderRing()
    {
        Vector3[] vertices = new Vector3[2 * verticesCount + 2];
        Color[] colors = new Color[2 * verticesCount + 2];
        Vector2[] uvs = new Vector2[2 * verticesCount + 2];
        int[] triangles = new int[6 * verticesCount];
        int index = verticesCount, p0 = index, p1, triIndex;
        float angle = 0f, offset = 1f,
              verticesCountInverted = 1f / verticesCount,
              minRadius = planetRadius - bottomBorderOffset,
              maxRadius = planetRadius + topBorderOffset;
        Vector3 angularOne;
        vertices[index] = new Vector3(minRadius, 0f, 0f);
        if (segmentsType[0])
        {
            if (segmentsHeight[0] >= coreGroundRadius) colors[index] = borderRingColorHigher;
            else colors[index] = borderRingColorLower;
        }
        else colors[index] = borderRingColorFree;
        uvs[index] = new Vector2(offset, 0f);

        index += index + 1;
        vertices[index] = new Vector3(maxRadius, 0f, 0f);
        colors[index] = colors[verticesCount];
        uvs[index] = new Vector2(offset, 1f);
        p1 = index;

        float verticesPerSegmentInverted = 1f / verticesPerSegment;
        int vs, vn;
        Color prevColor = colors[index], curColor;
        float adjacentPosition;

        for (int s = segments - 1; s > -1; --s)
        {
            if (segmentsType[s])
            {
                if (segmentsHeight[s] >= coreGroundRadius) curColor = borderRingColorHigher;
                else curColor = borderRingColorLower;
            }
            else curColor = borderRingColorFree;
            vs = s * verticesPerSegment;
            for (int v = verticesPerSegment - 1; v > -1; --v)
            {
                vn = vs + v;
                offset = vn * verticesCountInverted;
                angle = offset * 360f;
                adjacentPosition = 1f - v * verticesPerSegmentInverted;
                angularOne = PolarSystem.Position(angle, 1f, Vector3.zero);

                vertices[vn] = angularOne * minRadius;
                colors[vn] = Color.Lerp(prevColor, curColor, BlendFunction(adjacentPosition, borderRingColorBlendingOffset));
                uvs[vn] = new Vector2(offset, 0f);

                triIndex = 6 * vn;
                triangles[triIndex] = p0;
                triangles[triIndex + 1] = p1;
                triangles[triIndex + 2] = vn;
                p0 = vn;

                index = verticesCount + vn + 1;
                vertices[index] = angularOne * maxRadius;
                colors[index] = colors[vn];
                uvs[index] = new Vector2(offset, 1f);

                triangles[triIndex + 3] = p0;
                triangles[triIndex + 4] = p1;
                triangles[triIndex + 5] = index;
                p1 = index;
            }
            prevColor = curColor;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        borderRingMesh.mesh = mesh;
    }

    private void GetRings(ref MeshFilter meshFilter1, in float minRadius1, in float maxRadius1, in float colorOffset1,
                          ref MeshFilter meshFilter2, in float minRadius2, in float maxRadius2, in float colorOffset2)
    {
        int vs = 2 * verticesCount + 2;
        Vector3[] vertices1 = new Vector3[vs];
        Color[] colors1 = new Color[vs];
        Vector3[] vertices2 = new Vector3[vs];
        Color[] colors2 = new Color[vs];
        Vector2[] uvs = new Vector2[vs];
        int[] triangles = new int[6 * verticesCount];
        int index = verticesCount, p0 = index, p1, triIndex;
        float angle = 0f, offset = 1f,
              verticesCountInverted = 1f / verticesCount;
        Vector3 angularOne;
        vertices1[index] = new Vector3(minRadius1, 0f, 0f);
        vertices2[index] = new Vector3(minRadius2, 0f, 0f);
        if (segmentsType[0])
        {
            if (segmentsHeight[0] >= coreGroundRadius) colors1[index] = Color.red;
            else colors1[index] = Color.green;
        }
        else colors1[index] = Color.black;
        colors2[index] = colors1[index];
        uvs[index] = new Vector2(offset, 0f);

        index += index + 1;
        vertices1[index] = new Vector3(maxRadius1, 0f, 0f);
        vertices2[index] = new Vector3(maxRadius2, 0f, 0f);
        colors1[index] = colors1[verticesCount];
        colors2[index] = colors1[index];
        uvs[index] = new Vector2(offset, 1f);
        p1 = index;

        float verticesPerSegmentInverted = 1f / verticesPerSegment;
        int vn;
        Color prevColor = colors1[index], curColor;
        float adjacentPosition;

        for (int s = segments - 1; s > -1; --s)
        {
            if (segmentsType[s])
            {
                if (segmentsHeight[s] >= coreGroundRadius) curColor = Color.red;
                else curColor = Color.green;
            }
            else curColor = Color.black;
            vs = s * verticesPerSegment;
            for (int v = verticesPerSegment - 1; v > -1; --v)
            {
                vn = vs + v;
                offset = vn * verticesCountInverted;
                angle = offset * 360f;
                adjacentPosition = 1f - v * verticesPerSegmentInverted;
                angularOne = PolarSystem.Position(angle, 1f, Vector3.zero);

                vertices1[vn] = angularOne * minRadius1;
                vertices2[vn] = angularOne * minRadius2;
                colors1[vn] = Color.Lerp(prevColor, curColor, BlendFunction(adjacentPosition, colorOffset1));
                colors2[vn] = Color.Lerp(prevColor, curColor, BlendFunction(adjacentPosition, colorOffset2));
                uvs[vn] = new Vector2(offset, 0f);

                triIndex = 6 * vn;
                triangles[triIndex] = p0;
                triangles[triIndex + 1] = p1;
                triangles[triIndex + 2] = vn;
                p0 = vn;

                index = verticesCount + vn + 1;
                vertices1[index] = angularOne * maxRadius1;
                vertices2[index] = angularOne * maxRadius2;
                colors1[index] = colors1[vn];
                colors2[index] = colors2[vn];
                uvs[index] = new Vector2(offset, 1f);

                triangles[triIndex + 3] = p0;
                triangles[triIndex + 4] = p1;
                triangles[triIndex + 5] = index;
                p1 = index;
            }
            prevColor = curColor;
        }

        Mesh mesh1 = new Mesh();
        mesh1.vertices = vertices1;
        mesh1.colors = colors1;
        mesh1.uv = uvs;
        mesh1.triangles = triangles;
        meshFilter1.mesh = mesh1;
        Mesh mesh2 = new Mesh();
        mesh2.vertices = vertices2;
        mesh2.colors = colors2;
        mesh2.uv = uvs;
        mesh2.triangles = triangles;
        meshFilter2.mesh = mesh2;
    }

    private void GetShadow()
    {
        Vector3 lightDirection = PolarSystem.Position(shadowAngle, 1f, Vector3.zero),
                lightDirectionPerpendicular = new Vector3(-lightDirection.y, lightDirection.x, 0),
                midPoint = lightDirection * shadowLineRelativePosition * coreRadius;
        Vector3[] vertices;
        int arcEndIndex = 1 + arcVerticesCount, lastVertexIndex;
        bool addTriangle = false;

        if (shadowLineRelativePosition < -_EPS)
        {
            float a = midPoint.magnitude,
            b = Mathf.Sqrt(planetRadius * planetRadius - a * a),
            shadowLength = b * b / a;
            if (shadowLength > maxShadowLength)
            {
                lastVertexIndex = 3 + arcVerticesCount;
                addTriangle = true;
                vertices = new Vector3[lastVertexIndex + 1];
                vertices[0] = midPoint + b * lightDirectionPerpendicular;
                vertices[arcEndIndex] = midPoint - b * lightDirectionPerpendicular;

                vertices[lastVertexIndex] = midPoint - lightDirection * maxShadowLength;
                b = b - maxShadowLength * a / b;
                vertices[lastVertexIndex - 1] = vertices[lastVertexIndex] - b * lightDirectionPerpendicular;
                vertices[lastVertexIndex] = vertices[lastVertexIndex] + b * lightDirectionPerpendicular;
            }
            else
            {
                lastVertexIndex = 2 + arcVerticesCount;
                vertices = new Vector3[lastVertexIndex + 1];
                vertices[0] = midPoint + b * lightDirectionPerpendicular;
                vertices[arcEndIndex] = midPoint - b * lightDirectionPerpendicular;
                vertices[lastVertexIndex] = midPoint - lightDirection * shadowLength;
            }
        }
        else if (shadowLineRelativePosition > _EPS)
        {
            float a = midPoint.magnitude,
            b = Mathf.Sqrt(planetRadius * planetRadius - a * a);
            lastVertexIndex = 3 + arcVerticesCount;
            addTriangle = true;
            vertices = new Vector3[lastVertexIndex + 1];
            vertices[0] = midPoint + b * lightDirectionPerpendicular;
            vertices[arcEndIndex] = midPoint - b * lightDirectionPerpendicular;

            vertices[lastVertexIndex] = -lightDirection * (maxShadowLength - a);
            b = maxShadowLength * a / b + b;
            vertices[lastVertexIndex - 1] = vertices[lastVertexIndex] - b * lightDirectionPerpendicular;
            vertices[lastVertexIndex] = vertices[lastVertexIndex] + b * lightDirectionPerpendicular;
        }
        else
        {
            lastVertexIndex = 3 + arcVerticesCount;
            addTriangle = true;
            vertices = new Vector3[lastVertexIndex + 1];
            vertices[0] = planetRadius * lightDirectionPerpendicular;
            vertices[arcEndIndex] = -vertices[0];
            vertices[lastVertexIndex - 1] = vertices[arcEndIndex] - maxShadowLength * lightDirection;
            vertices[lastVertexIndex] = vertices[0] - maxShadowLength * lightDirection;
        }

        float shadowRadius = (midPoint - vertices[0]).magnitude,
              cosRot = (vertices[arcEndIndex].x - midPoint.x) / shadowRadius,
              sinRot = (vertices[arcEndIndex].y - midPoint.y) / shadowRadius,
              starDepth = (midPoint - lightDirection * shadowArcRelativePosition * coreRadius).magnitude / shadowRadius;
        if (starDepth > 1f) starDepth = 1f;
        starDepth = Mathf.Sign(shadowArcRelativePosition - shadowLineRelativePosition) * starDepth;
        int[] triangles = new int[3 * (vertices.Length - 2)];
        float step = 180f / (arcVerticesCount + 1f);

        if (addTriangle)
        {
            int c = arcVerticesCount / 2;
            for (int i = 0; i < c; ++i)
            {
                vertices[i + 1] = PolarSystem.Position(step * (arcVerticesCount - i), shadowRadius, Vector3.zero);
                vertices[i + 1] = new Vector3(vertices[i + 1].x, vertices[i + 1].y * starDepth, 0f);
                vertices[i + 1] = midPoint +
                    new Vector3(vertices[i + 1].x * cosRot - vertices[i + 1].y * sinRot, vertices[i + 1].x * sinRot + vertices[i + 1].y * cosRot, 0f);
                triangles[3 * i] = i;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = lastVertexIndex;
            }

            triangles[3 * arcVerticesCount + 3] = c;
            triangles[3 * arcVerticesCount + 4] = lastVertexIndex - 1;
            triangles[3 * arcVerticesCount + 5] = lastVertexIndex;

            for (int i = c; i < arcVerticesCount; ++i)
            {
                vertices[i + 1] = PolarSystem.Position(step * (arcVerticesCount - i), shadowRadius, Vector3.zero);
                vertices[i + 1] = new Vector3(vertices[i + 1].x, vertices[i + 1].y * starDepth, 0f);
                vertices[i + 1] = midPoint +
                    new Vector3(vertices[i + 1].x * cosRot - vertices[i + 1].y * sinRot, vertices[i + 1].x * sinRot + vertices[i + 1].y * cosRot, 0f);
                triangles[3 * i] = i;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = lastVertexIndex - 1;
            }

            triangles[3 * arcVerticesCount] = arcEndIndex - 1;
            triangles[3 * arcVerticesCount + 1] = arcEndIndex;
            triangles[3 * arcVerticesCount + 2] = lastVertexIndex - 1;
        }
        else
        {
            for (int i = 0; i < arcVerticesCount; ++i)
            {
                vertices[i + 1] = PolarSystem.Position(step * (arcVerticesCount - i), shadowRadius, Vector3.zero);
                vertices[i + 1] = new Vector3(vertices[i + 1].x, vertices[i + 1].y * starDepth, 0f);
                vertices[i + 1] = midPoint +
                    new Vector3(vertices[i + 1].x * cosRot - vertices[i + 1].y * sinRot, vertices[i + 1].x * sinRot + vertices[i + 1].y * cosRot, 0f);
                triangles[3 * i] = i;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = lastVertexIndex;
            }

            triangles[3 * arcVerticesCount] = arcEndIndex - 1;
            triangles[3 * arcVerticesCount + 1] = arcEndIndex;
            triangles[3 * arcVerticesCount + 2] = lastVertexIndex;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        shadowMesh.mesh = mesh;
    }

    private void OnValidate()
    {
        if (maxObstaclesRatio < minObstaclesRatio) maxObstaclesRatio = minObstaclesRatio;
        if (maxBuildingHeight < minBuildingHeight) maxBuildingHeight = minBuildingHeight;

        if (maxCoreRadius < minCoreRadius) maxCoreRadius = minCoreRadius;
        if (maxGroundRadius < minGroundRadius) maxGroundRadius = minGroundRadius;
        if (maxHeight < minHeight) maxHeight = minHeight;
        if (maxDepth < minDepth) maxDepth = minDepth;

        if (maxShadowLineRelativePosition < minShadowLineRelativePosition) maxShadowLineRelativePosition = minShadowLineRelativePosition;
        if (maxShadowArcRelativePosition < minShadowArcRelativePosition) maxShadowArcRelativePosition = minShadowArcRelativePosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (segmentsType.Length == segments)
        {
            float gizmoRadius = Mathf.Sqrt(coreGroundRadius * coreGroundRadius + (blockScale * blockScale) / 4);
            float gizmoAngle = Mathf.Acos(coreGroundRadius / gizmoRadius) * 180f / Mathf.PI;
            float slotAngle = 360f / segments;

            for (int i = 0; i < segments; ++i)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, PolarSystem.Position(i * slotAngle + reliefRotation, planetRadius, transform.position));

                if (!segmentsType[i])
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawWireSphere(PolarSystem.Position(i * slotAngle + reliefRotation, coreGroundRadius + 0.5f * blockScale, transform.position), 0.5f * blockScale);
                Gizmos.DrawLine(PolarSystem.Position(i * slotAngle + gizmoAngle + reliefRotation, gizmoRadius, transform.position),
                                PolarSystem.Position(i * slotAngle - gizmoAngle + reliefRotation, gizmoRadius, transform.position));
                Gizmos.DrawLine(transform.position, PolarSystem.Position(i * slotAngle + reliefRotation, segmentsHeight[i], transform.position));
            }
        }

        if (turnOnShadow)
        {
            Gizmos.color = Color.yellow;
            Vector3 lightDirection = PolarSystem.Position(shadowAngle, 1f, Vector3.zero),
                    lightDirectionPerpendicular = new Vector3(-lightDirection.y, lightDirection.x, 0),
                    midPoint = transform.position + lightDirection * coreRadius * shadowLineRelativePosition,
                    arrowCenter = transform.position + lightDirection * (1f + planetRadius);
            Gizmos.DrawLine(arrowCenter, PolarSystem.Position(shadowAngle + 25f, 0.5f, arrowCenter));
            Gizmos.DrawLine(arrowCenter, PolarSystem.Position(shadowAngle - 25f, 0.5f, arrowCenter));
            Gizmos.DrawLine(arrowCenter + lightDirection, midPoint);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(midPoint, midPoint - lightDirection * maxShadowLength);
            Gizmos.DrawLine(midPoint - lightDirectionPerpendicular * (planetRadius + 1.5f), midPoint + lightDirectionPerpendicular * (planetRadius + 1.5f));
            Gizmos.color = Color.magenta;
            midPoint = transform.position + lightDirection * coreRadius * shadowArcRelativePosition;
            Gizmos.DrawLine(midPoint - lightDirectionPerpendicular * (planetRadius + 0.5f), midPoint + lightDirectionPerpendicular * (planetRadius + 0.5f));
        }
    }

    private static float BlendFunction(float x, in float blendingOffset)
    {
        if (x >= 1f - blendingOffset)
        {
            return 1f;
        }
        else if (x <= blendingOffset)
        {
            return 0f;
        }
        else
        {
            x = (x - blendingOffset) / (1f - blendingOffset - blendingOffset);
            return x * x * (3f - 2f * x);
        }
    }
}
