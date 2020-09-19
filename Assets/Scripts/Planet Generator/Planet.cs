using UnityEngine;
public class Planet : MonoBehaviour
{
    [Header("Main Properties")]
    [Range(0, 65535)] public int key;
    [Range(6, 16)] public int verticesPerSegment = 10;
    private int verticesCount;

    [Header("Game")]
    [Min(0.2f)] public float blockScale = 0.5f;
    [Min(0f)] public float blockOffset = 0.02f;
    [Space(15)]
    [Range(0f, 0.9f)] public float minObstaclesRatio = 0.3f;
    [Range(0f, 0.9f)] public float maxObstaclesRatio = 0.7f;
    [Space(15)]
    [Range(1, 5)] public int minBuildingHeight = 1;
    [Range(1, 5)] public int maxBuildingHeight = 5;
    private int builtBuildings;

    public MeshFilter reliefMesh;
    public MeshRenderer reliefRenderer;
    private Material coreMaterial;

    [Header("Relief")]
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
    public float minHeight = 0.15f;
    public float maxHeight = 0.3f;
    [Space(8)]
    public float minDepth = 0.1f;
    public float maxDepth = 0.18f;
    private float[] deviations;
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
    [Space(15)]
    public bool randomizeCorePattern;
    [Space(8)]
    public float coreRChannelMaxOffset;
    public float coreGChannelMaxOffset;
    public float coreBChannelMaxOffset;
    [Space(8)]
    public Color coreColorLower;
    public Color coreColorMiddle;
    public Color coreColorHigher;

    [Header("Decorative Objects")]
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

    [Header("Shadow")]
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

    [Header("Water")]
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

    [Header("Atmosphere")]
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

    [Header("Border Ring")]
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

    [Header("Planet Properties (Read Only)")]
    [ReadOnly] public float planetRadius;
    [ReadOnly] public int buildingHeight;
    [ReadOnly] public int[] segmentsLogicalHeight; // -1 - гора, -2 - овраг, 0..buildingHeight - свободный сегмент
    [ReadOnly] public float[] segmentsPhysicalHeight;
    private int segments;
    private int freeSegments;
    private float[] segmentsCoreHeight;
    private float segmentAngle;
    private float halfSegmentAngle;
    private float reliefRotation;

    public int GetFreeSegmentsCount()
    {
        return freeSegments;
    }

    public int GetSegmentByAngle(float angle)
    {
        angle += halfSegmentAngle - reliefRotation;
        angle -= 360f * Mathf.Floor(angle / 360f);
        return (Mathf.FloorToInt(angle / segmentAngle) + segments) % segments;
    }

    public void RefreshSegments()
    {
        for (int i = 0; i < segmentsLogicalHeight.Length; ++i)
        {
            if (segmentsLogicalHeight[i] > 0) segmentsLogicalHeight[i] = 0;
        }
        builtBuildings = 0;
    }

    public int GetBuildingsCount() // Возвращает количество зданий с высотой buildingHeight
    {
        return builtBuildings;
        /*
        int count = 0;
        for (int i = 0; i < segmentsLogicalHeight.Length; ++i)
        {
            if (segmentsLogicalHeight[i] == buildingHeight) count++;
        }
        return count;
        */
    }

    private void Start()
    {
        coreMaterial = reliefRenderer.material;
        waterMaterial = waterRenderer.material;
        atmosphereMaterial = atmosphereRenderer.material;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.W)) GetNewPlanet();
        else if (Input.GetKeyUp(KeyCode.R)) GetPlanet();
    }

    public void GetNewPlanet()
    {
        key = Random.Range(0, 65535);
        GetPlanet();
    }

    public void GetPlanet()
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
        freeSegments = segments - obstaclesCount;

        buildingHeight = Random.Range(minBuildingHeight, maxBuildingHeight + 1);
        builtBuildings = 0;
        planetRadius = coreGroundRadius + buildingHeight * blockScale;

        lowestGroundRadius = coreGroundRadius;

        deviations = new float[segments];
        segmentsLogicalHeight = new int[segments];
        segmentsPhysicalHeight = new float[segments];
        segmentsCoreHeight = new float[segments];
        decorativeObjectsIndices = new int[segments];

        int obstaclePosition = Random.Range(0, segments);
        highestCoreRadius = coreRadius;
        float heightValue,
              depthMultiplier = 1f / heightDepthRatio,
              heightMultiplier = 1f / (1f - heightDepthRatio);

        for (int i = 0; i < segments; ++i)
        {
            deviations[i] = 0f;
            segmentsLogicalHeight[i] = 0;
            segmentsPhysicalHeight[i] = coreGroundRadius;
            segmentsCoreHeight[i] = coreRadius;
            decorativeObjectsIndices[i] = -1;
        }

        while (obstaclesCount > 0)
        {
            heightValue = Random.value;
            if (heightDepthRatio == 1)
            {
                heightValue = -Mathf.Lerp(minDepth, maxDepth, heightValue);
                segmentsLogicalHeight[obstaclePosition] = -2;
            }
            else
            {
                if (heightValue >= heightDepthRatio)
                {
                    heightValue = Mathf.Lerp(minHeight, maxHeight, (heightValue - heightDepthRatio) * heightMultiplier);
                    segmentsLogicalHeight[obstaclePosition] = -1;
                }
                else
                {
                    heightValue = -Mathf.Lerp(minDepth, maxDepth, heightValue * depthMultiplier);
                    segmentsLogicalHeight[obstaclePosition] = -2;
                }
            }
            deviations[obstaclePosition] = heightValue;
            segmentsPhysicalHeight[obstaclePosition] += heightValue;
            if (segmentsPhysicalHeight[obstaclePosition] < lowestGroundRadius)
                lowestGroundRadius = segmentsPhysicalHeight[obstaclePosition];
            segmentsCoreHeight[obstaclePosition] += coreOffsetMultiplier * heightValue;
            if (segmentsPhysicalHeight[obstaclePosition] - segmentsCoreHeight[obstaclePosition] < lowestGroundRadiusAllowed)
                segmentsCoreHeight[obstaclePosition] = segmentsPhysicalHeight[obstaclePosition] - lowestGroundRadiusAllowed;
            if (segmentsCoreHeight[obstaclePosition] > highestCoreRadius) highestCoreRadius = segmentsCoreHeight[obstaclePosition];
            obstaclePosition = (obstaclePosition + (7 * obstaclePosition + 23) / 11 + 37 * obstaclesCount) % segments;
            while (segmentsLogicalHeight[obstaclePosition] < 0)
            {
                obstaclePosition = (obstaclePosition + 1) % segments;
            }
            obstaclesCount--;
        }
        // Rotating the relief
        if (rotateRelief)
            reliefRotation = Random.Range(0f, 360f);
        else
            reliefRotation = 0f;
        reliefMesh.transform.rotation = Quaternion.Euler(0f, 0f, reliefRotation);
        // Randomizing pattern of planet core
        if (randomizeCorePattern)
        {
            coreMaterial.SetTextureOffset("_Noise1", new Vector2(Random.Range(0f, coreRChannelMaxOffset), Random.Range(0f, coreRChannelMaxOffset)));
            coreMaterial.SetTextureOffset("_Noise2", new Vector2(Random.Range(0f, coreGChannelMaxOffset), Random.Range(0f, coreGChannelMaxOffset)));
            coreMaterial.SetTextureOffset("_Noise3", new Vector2(Random.Range(0f, coreBChannelMaxOffset), Random.Range(0f, coreBChannelMaxOffset)));
            coreMaterial.SetColor("_Color1", coreColorLower);
            coreMaterial.SetColor("_Color2", coreColorMiddle);
            coreMaterial.SetColor("_Color3", coreColorHigher);
        }
        // Randomizing the shadow
        shadowAngle = Random.Range(minShadowAngle, minShadowAngle + shadowAngleRange);
        shadowLineRelativePosition = Random.Range(minShadowLineRelativePosition, maxShadowLineRelativePosition);
        shadowArcRelativePosition = Random.Range(minShadowArcRelativePosition, maxShadowArcRelativePosition);
        // Setting decorative objects
        obstaclesCount = Mathf.FloorToInt(segments * decorativeObjectsDensity);

        if (obstaclesCount == segments)
        {
            for (int i = 0; i < segments; ++i)
            {
                SetDecorativeObjectIndex(i);
            }
        }
        else
        {
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
        switch (segmentsLogicalHeight[segment])
        {
            case -2:
                {
                    upperBound = lowerSegmentObjects.Length;
                    break;
                }
            case -1:
                {
                    upperBound = higherSegmentObjects.Length;
                    break;
                }
            default:
                {
                    upperBound = freeSegmentObjects.Length;
                    break;
                }
        }

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
        switch (segmentsLogicalHeight[0])
        {
            case -2:
                {
                    colors[index] = reliefColorLower;
                    break;
                }
            case -1:
                {
                    colors[index] = reliefColorHigher;
                    break;
                }
            default:
                {
                    colors[index] = reliefColorFree;
                    break;
                }
        }
        uvs[index] = new Vector2(offset, 0f);
        p0 = index;

        index2 = 3 * verticesCount + 1;
        vertices[index2] = Vector3.right * segmentsPhysicalHeight[0];
        colors[index2] = colors[index];
        uvs[index2] = new Vector2(offset, 1f);
        p1 = index2;

        float verticesPerSegmentInverted = 1f / verticesPerSegment;
        int vs, vn;
        Vector3 angularOne;
        float prevHeight = segmentsPhysicalHeight[0], curHeight, adjacentHeight, blendFunctionResult,
              prevCoreHeight = segmentsCoreHeight[0], curCoreHeight, adjacentCoreHeight, adjacentPosition,
              prevDeviation = deviations[0], curDeviation;
        int prevType = segmentsLogicalHeight[0], curType, colorRule;
        Color prevColor = colors[index], curColor;
        for (int s = segments - 1; s > -1; --s)
        {
            curHeight = segmentsPhysicalHeight[s];
            curCoreHeight = segmentsCoreHeight[s];
            curType = segmentsLogicalHeight[s];
            curDeviation = deviations[s];
            switch (curType)
            {
                case -2:
                    {
                        curColor = reliefColorLower;
                        break;
                    }
                case -1:
                    {
                        curColor = reliefColorHigher;
                        break;
                    }
                default:
                    {
                        curColor = reliefColorFree;
                        break;
                    }
            }
            if (prevType == curType) colorRule = 0;
            else
            {
                if (prevType + curType == -3)
                {
                    if (curDeviation * prevDeviation > 0)
                    {
                        colorRule = 1;
                    }
                    else
                    {
                        if (curDeviation < 0) colorRule = 2;
                        else colorRule = 3;
                    }
                }
                else
                {
                    if (prevType == 0) colorRule = 4;
                    else colorRule = 5;
                }
            }
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
                switch (colorRule)
                {
                    case 0:
                        {
                            colors[index] = curColor;
                            break;
                        }
                    case 1:
                        {
                            if (adjacentPosition > 0.5f) colors[index] = curColor;
                            else colors[index] = prevColor;
                            break;
                        }
                    case 2:
                        {
                            if (adjacentHeight < coreGroundRadius) colors[index] = curColor;
                            else colors[index] = prevColor;
                            break;
                        }
                    case 3:
                        {
                            if (adjacentHeight < coreGroundRadius) colors[index] = prevColor;
                            else colors[index] = curColor;
                            break;
                        }
                    case 4:
                        {
                            if (Mathf.Abs(adjacentHeight - coreGroundRadius) > reliefColorThreshold)
                                colors[index] = curColor;
                            else colors[index] = prevColor;
                            break;
                        }
                    case 5:
                        {
                            if (Mathf.Abs(adjacentHeight - coreGroundRadius) > reliefColorThreshold)
                                colors[index] = prevColor;
                            else colors[index] = curColor;
                            break;
                        }
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
            prevDeviation = curDeviation;
            prevColor = curColor;
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
                switch (segmentsLogicalHeight[i])
                {
                    case -2:
                        {
                            newDecorativeObject = lowerSegmentObjects[decorativeObjectsIndices[i]];
                            break;
                        }
                    case -1:
                        {
                            newDecorativeObject = higherSegmentObjects[decorativeObjectsIndices[i]];
                            break;
                        }
                    default:
                        {
                            newDecorativeObject = freeSegmentObjects[decorativeObjectsIndices[i]];
                            break;
                        }
                }

                newDecorativeObject = Instantiate(newDecorativeObject, decorativeObjectsContainer);
                newDecorativeObject.transform.localPosition = PolarSystem.Position(angle, segmentsPhysicalHeight[i], Vector3.zero);
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
        Vector3[] vertices = new Vector3[4 * verticesCount + 4];
        Color[] colors = new Color[4 * verticesCount + 4];
        Vector2[] uvs = new Vector2[4 * verticesCount + 4];
        int[] triangles = new int[18 * verticesCount];
        int index = verticesCount, p0 = index, p1, p2, p3, triIndex, indexStep = verticesCount + 1;
        float angle = 0f, offset = 1f,
              verticesCountInverted = 1f / verticesCount,
              midRadius = Mathf.Lerp(coreGroundRadius, planetRadius, atmosphereColorBorder);
        Vector3 angularOne;
        vertices[index] = new Vector3(lowestGroundRadius, 0f, 0f);
        colors[index] = atmosphereColorBottom;
        uvs[index] = new Vector2(offset, 0f);

        index += indexStep;
        vertices[index] = new Vector3(coreGroundRadius, 0f, 0f);
        colors[index] = atmosphereColorBottom;
        uvs[index] = new Vector2(offset, 0f);
        p1 = index;

        index += indexStep;
        vertices[index] = new Vector3(midRadius, 0f, 0f);
        colors[index] = atmosphereColorMiddle;
        uvs[index] = new Vector2(offset, atmosphereColorBorder);
        p2 = index;

        index += indexStep;
        vertices[index] = new Vector3(planetRadius, 0f, 0f);
        colors[index] = atmosphereColorTop;
        uvs[index] = new Vector2(offset, 1f);
        p3 = index;

        for (int i = verticesCount - 1; i > -1; --i)
        {
            offset = i * verticesCountInverted;
            angle = offset * 360f;
            angularOne = PolarSystem.Position(angle, 1f, Vector3.zero);

            index = i;
            vertices[index] = angularOne * lowestGroundRadius;
            colors[index] = atmosphereColorBottom;
            uvs[index] = new Vector2(offset, 0f);

            triIndex = 18 * index;
            triangles[triIndex] = p0;
            triangles[triIndex + 1] = p1;
            triangles[triIndex + 2] = index;
            p0 = index;

            index += indexStep;
            vertices[index] = angularOne * coreGroundRadius;
            colors[index] = atmosphereColorBottom;
            uvs[index] = new Vector2(offset, 0f);

            triangles[triIndex + 3] = p0;
            triangles[triIndex + 4] = p1;
            triangles[triIndex + 5] = index;

            triangles[triIndex + 6] = p1;
            triangles[triIndex + 7] = p2;
            triangles[triIndex + 8] = index;
            p1 = index;

            index += indexStep;
            vertices[index] = angularOne * midRadius;
            colors[index] = atmosphereColorMiddle;
            uvs[index] = new Vector2(offset, atmosphereColorBorder);

            triangles[triIndex + 9] = p1;
            triangles[triIndex + 10] = p2;
            triangles[triIndex + 11] = index;

            triangles[triIndex + 12] = p2;
            triangles[triIndex + 13] = p3;
            triangles[triIndex + 14] = index;
            p2 = index;

            index += indexStep;
            vertices[index] = angularOne * planetRadius;
            colors[index] = atmosphereColorTop;
            uvs[index] = new Vector2(offset, 1f);

            triangles[triIndex + 15] = p2;
            triangles[triIndex + 16] = p3;
            triangles[triIndex + 17] = index;
            p3 = index;
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
        switch (segmentsLogicalHeight[0])
        {
            case -2:
                {
                    colors[index] = borderRingColorLower;
                    break;
                }
            case -1:
                {
                    colors[index] = borderRingColorHigher;
                    break;
                }
            default:
                {
                    colors[index] = borderRingColorFree;
                    break;
                }
        }
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
            switch (segmentsLogicalHeight[s])
            {
                case -2:
                    {
                        curColor = borderRingColorLower;
                        break;
                    }
                case -1:
                    {
                        curColor = borderRingColorHigher;
                        break;
                    }
                default:
                    {
                        curColor = borderRingColorFree;
                        break;
                    }
            }
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

        if (maxShadowLineRelativePosition < minShadowLineRelativePosition) maxShadowLineRelativePosition = minShadowLineRelativePosition;
        if (maxShadowArcRelativePosition < minShadowArcRelativePosition) maxShadowArcRelativePosition = minShadowArcRelativePosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (segmentsLogicalHeight.Length == segments)
        {
            float gizmoRadius = Mathf.Sqrt(coreGroundRadius * coreGroundRadius + (blockScale * blockScale) / 4);
            float gizmoAngle = Mathf.Acos(coreGroundRadius / gizmoRadius) * 180f / Mathf.PI;
            float slotAngle = 360f / segments;

            for (int i = 0; i < segments; ++i)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, PolarSystem.Position(i * slotAngle + reliefRotation, planetRadius, transform.position));

                if (segmentsLogicalHeight[i] > -1)
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
                Gizmos.DrawLine(transform.position, PolarSystem.Position(i * slotAngle + reliefRotation, segmentsPhysicalHeight[i], transform.position));
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
