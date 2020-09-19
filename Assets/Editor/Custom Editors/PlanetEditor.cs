using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    // Main Values
    SerializedProperty key;
    SerializedProperty verticesPerSegment;

    // Game
    SerializedProperty blockScale;
    SerializedProperty blockOffset;
    SerializedProperty minObstaclesRatio;
    SerializedProperty maxObstaclesRatio;
    SerializedProperty minBuildingHeight;
    SerializedProperty maxBuildingHeight;

    // Relief
    SerializedProperty reliefMesh;
    SerializedProperty reliefRenderer;

    SerializedProperty minCoreRadius;
    SerializedProperty maxCoreRadius;
    SerializedProperty minGroundRadius;
    SerializedProperty maxGroundRadius;

    SerializedProperty lowestGroundRadiusAllowed;
    SerializedProperty heightDepthRatio;

    SerializedProperty minHeight;
    SerializedProperty maxHeight;
    SerializedProperty minDepth;
    SerializedProperty maxDepth;

    SerializedProperty reliefBlendingOffset;
    SerializedProperty coreOffsetMultiplier;
    SerializedProperty rotateRelief;

    SerializedProperty reliefColorFree;
    SerializedProperty reliefColorHigher;
    SerializedProperty reliefColorLower;

    SerializedProperty reliefColorThreshold;
    SerializedProperty randomizeCorePattern;

    SerializedProperty coreRChannelMaxOffset;
    SerializedProperty coreGChannelMaxOffset;
    SerializedProperty coreBChannelMaxOffset;

    SerializedProperty coreColorLower;
    SerializedProperty coreColorMiddle;
    SerializedProperty coreColorHigher;

    // Decorative Objects
    SerializedProperty turnOnDecorativeObjects;
    SerializedProperty decorativeObjectsContainer;
    SerializedProperty decorativeObjectsDensity;
    SerializedProperty freeSegmentObjects;
    SerializedProperty higherSegmentObjects;
    SerializedProperty lowerSegmentObjects;

    // Shadow
    SerializedProperty turnOnShadow;
    SerializedProperty shadowMesh;
    SerializedProperty minShadowAngle;
    SerializedProperty shadowAngleRange;
    SerializedProperty minShadowLineRelativePosition;
    SerializedProperty maxShadowLineRelativePosition;
    SerializedProperty minShadowArcRelativePosition;
    SerializedProperty maxShadowArcRelativePosition;
    SerializedProperty maxShadowLength;
    SerializedProperty arcVerticesCount;

    // Water
    SerializedProperty turnOnWater;
    SerializedProperty waterMesh;
    SerializedProperty waterRenderer;
    SerializedProperty fillWholePlanet;
    SerializedProperty waterOffset;
    SerializedProperty waterColor;
    SerializedProperty waterDepthColor;
    SerializedProperty waterWavesAmplitude;
    SerializedProperty waterWavesFrequency;
    SerializedProperty waterWavesSpeed;

    // Atmosphere
    SerializedProperty turnOnAtmosphere;
    SerializedProperty atmosphereMesh;
    SerializedProperty atmosphereRenderer;
    SerializedProperty atmosphereColorBottom;
    SerializedProperty atmosphereColorMiddle;
    SerializedProperty atmosphereColorTop;
    SerializedProperty atmosphereColorBorder;
    SerializedProperty atmosphereColorNight;

    // Border Ring
    SerializedProperty turnOnBorderRing;
    SerializedProperty borderRingMesh;
    SerializedProperty bottomBorderOffset;
    SerializedProperty topBorderOffset;
    SerializedProperty borderRingColorFree;
    SerializedProperty borderRingColorHigher;
    SerializedProperty borderRingColorLower;
    SerializedProperty borderRingColorBlendingOffset;

    // Planet Properties
    SerializedProperty planetRadius;
    SerializedProperty buildingHeight;
    SerializedProperty segmentsLogicalHeight;
    SerializedProperty segmentsPhysicalHeight;

    private bool showPointers = false;
    private bool showPropertiesOfSwitchableParts = true;
    private bool toggleAll = false;
    private Rect rect;

    private void OnEnable()
    {
        // Main Values
        key = serializedObject.FindProperty("key");
        verticesPerSegment = serializedObject.FindProperty("verticesPerSegment");

        // Game
        blockScale = serializedObject.FindProperty("blockScale");
        blockOffset = serializedObject.FindProperty("blockOffset");
        minObstaclesRatio = serializedObject.FindProperty("minObstaclesRatio");
        maxObstaclesRatio = serializedObject.FindProperty("maxObstaclesRatio");
        minBuildingHeight = serializedObject.FindProperty("minBuildingHeight");
        maxBuildingHeight = serializedObject.FindProperty("maxBuildingHeight");

        // Relief
        reliefMesh = serializedObject.FindProperty("reliefMesh");
        reliefRenderer = serializedObject.FindProperty("reliefRenderer");
        minCoreRadius = serializedObject.FindProperty("minCoreRadius");
        maxCoreRadius = serializedObject.FindProperty("maxCoreRadius");
        minGroundRadius = serializedObject.FindProperty("minGroundRadius");
        maxGroundRadius = serializedObject.FindProperty("maxGroundRadius");
        lowestGroundRadiusAllowed = serializedObject.FindProperty("lowestGroundRadiusAllowed");
        heightDepthRatio = serializedObject.FindProperty("heightDepthRatio");
        minHeight = serializedObject.FindProperty("minHeight");
        maxHeight = serializedObject.FindProperty("maxHeight");
        minDepth = serializedObject.FindProperty("minDepth");
        maxDepth = serializedObject.FindProperty("maxDepth");
        reliefBlendingOffset = serializedObject.FindProperty("reliefBlendingOffset");
        coreOffsetMultiplier = serializedObject.FindProperty("coreOffsetMultiplier");
        rotateRelief = serializedObject.FindProperty("rotateRelief");
        reliefColorFree = serializedObject.FindProperty("reliefColorFree");
        reliefColorHigher = serializedObject.FindProperty("reliefColorHigher");
        reliefColorLower = serializedObject.FindProperty("reliefColorLower");
        reliefColorThreshold = serializedObject.FindProperty("reliefColorThreshold");
        randomizeCorePattern = serializedObject.FindProperty("randomizeCorePattern");
        coreRChannelMaxOffset = serializedObject.FindProperty("coreRChannelMaxOffset");
        coreGChannelMaxOffset = serializedObject.FindProperty("coreGChannelMaxOffset");
        coreBChannelMaxOffset = serializedObject.FindProperty("coreBChannelMaxOffset");
        coreColorLower = serializedObject.FindProperty("coreColorLower");
        coreColorMiddle = serializedObject.FindProperty("coreColorMiddle");
        coreColorHigher = serializedObject.FindProperty("coreColorHigher");

        // Decorative Objects
        turnOnDecorativeObjects = serializedObject.FindProperty("turnOnDecorativeObjects");
        decorativeObjectsContainer = serializedObject.FindProperty("decorativeObjectsContainer");
        decorativeObjectsDensity = serializedObject.FindProperty("decorativeObjectsDensity");
        freeSegmentObjects = serializedObject.FindProperty("freeSegmentObjects");
        higherSegmentObjects = serializedObject.FindProperty("higherSegmentObjects");
        lowerSegmentObjects = serializedObject.FindProperty("lowerSegmentObjects");

        // Shadow
        turnOnShadow = serializedObject.FindProperty("turnOnShadow");
        shadowMesh = serializedObject.FindProperty("shadowMesh");
        minShadowAngle = serializedObject.FindProperty("minShadowAngle");
        shadowAngleRange = serializedObject.FindProperty("shadowAngleRange");
        minShadowLineRelativePosition = serializedObject.FindProperty("minShadowLineRelativePosition");
        maxShadowLineRelativePosition = serializedObject.FindProperty("maxShadowLineRelativePosition");
        minShadowArcRelativePosition = serializedObject.FindProperty("minShadowArcRelativePosition");
        maxShadowArcRelativePosition = serializedObject.FindProperty("maxShadowArcRelativePosition");
        maxShadowLength = serializedObject.FindProperty("maxShadowLength");
        arcVerticesCount = serializedObject.FindProperty("arcVerticesCount");

        // Water
        turnOnWater = serializedObject.FindProperty("turnOnWater");
        waterMesh = serializedObject.FindProperty("waterMesh");
        waterRenderer = serializedObject.FindProperty("waterRenderer");
        fillWholePlanet = serializedObject.FindProperty("fillWholePlanet");
        waterOffset = serializedObject.FindProperty("waterOffset");
        waterColor = serializedObject.FindProperty("waterColor");
        waterDepthColor = serializedObject.FindProperty("waterDepthColor");
        waterWavesAmplitude = serializedObject.FindProperty("waterWavesAmplitude");
        waterWavesFrequency = serializedObject.FindProperty("waterWavesFrequency");
        waterWavesSpeed = serializedObject.FindProperty("waterWavesSpeed");

        // Atmosphere
        turnOnAtmosphere = serializedObject.FindProperty("turnOnAtmosphere");
        atmosphereMesh = serializedObject.FindProperty("atmosphereMesh");
        atmosphereRenderer = serializedObject.FindProperty("atmosphereRenderer");
        atmosphereColorBottom = serializedObject.FindProperty("atmosphereColorBottom");
        atmosphereColorMiddle = serializedObject.FindProperty("atmosphereColorMiddle");
        atmosphereColorTop = serializedObject.FindProperty("atmosphereColorTop");
        atmosphereColorBorder = serializedObject.FindProperty("atmosphereColorBorder");
        atmosphereColorNight = serializedObject.FindProperty("atmosphereColorNight");

        // Border Ring
        turnOnBorderRing = serializedObject.FindProperty("turnOnBorderRing");
        borderRingMesh = serializedObject.FindProperty("borderRingMesh");
        bottomBorderOffset = serializedObject.FindProperty("bottomBorderOffset");
        topBorderOffset = serializedObject.FindProperty("topBorderOffset");
        borderRingColorFree = serializedObject.FindProperty("borderRingColorFree");
        borderRingColorHigher = serializedObject.FindProperty("borderRingColorHigher");
        borderRingColorLower = serializedObject.FindProperty("borderRingColorLower");
        borderRingColorBlendingOffset = serializedObject.FindProperty("borderRingColorBlendingOffset");

        // Planet Properties
        planetRadius = serializedObject.FindProperty("planetRadius");
        buildingHeight = serializedObject.FindProperty("buildingHeight");
        segmentsLogicalHeight = serializedObject.FindProperty("segmentsLogicalHeight");
        segmentsPhysicalHeight = serializedObject.FindProperty("segmentsPhysicalHeight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUIUtility.labelWidth = 190;
        EditorGUIUtility.fieldWidth = 60;

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(30);
                showPointers = EditorGUILayout.Toggle("Show Pointers", showPointers);
                GUILayout.Space(520);
                showPropertiesOfSwitchableParts = EditorGUILayout.Toggle("Show Properties of Toggleable", showPropertiesOfSwitchableParts);
                GUILayout.Space(20);
                EditorGUI.BeginChangeCheck();
                toggleAll = EditorGUILayout.Toggle("Toggle All Additional Parts", toggleAll);
                if (EditorGUI.EndChangeCheck())
                {
                    turnOnDecorativeObjects.boolValue = toggleAll;
                    turnOnShadow.boolValue = toggleAll;
                    turnOnWater.boolValue = toggleAll;
                    turnOnAtmosphere.boolValue = toggleAll;
                    turnOnBorderRing.boolValue = toggleAll;
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(key);
                    EditorGUILayout.PropertyField(verticesPerSegment);
                    if (showPointers)
                    {
                        EditorGUILayout.PropertyField(reliefMesh);
                        EditorGUILayout.PropertyField(reliefRenderer);
                        EditorGUILayout.PropertyField(decorativeObjectsContainer);
                        EditorGUILayout.PropertyField(shadowMesh);
                        EditorGUILayout.PropertyField(waterMesh);
                        EditorGUILayout.PropertyField(waterRenderer);
                        EditorGUILayout.PropertyField(atmosphereMesh);
                        EditorGUILayout.PropertyField(atmosphereRenderer);
                        EditorGUILayout.PropertyField(borderRingMesh);
                    }

                    GUILayout.Space(15);
                    rect = EditorGUILayout.BeginHorizontal();
                    Handles.color = Color.gray;
                    Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(blockScale);
                    EditorGUILayout.PropertyField(blockOffset);
                    EditorGUILayout.PropertyField(minObstaclesRatio);
                    EditorGUILayout.PropertyField(maxObstaclesRatio);
                    EditorGUILayout.PropertyField(minBuildingHeight);
                    EditorGUILayout.PropertyField(maxBuildingHeight);

                    GUILayout.Space(15);
                    rect = EditorGUILayout.BeginHorizontal();
                    Handles.color = Color.gray;
                    Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(planetRadius);
                    EditorGUILayout.PropertyField(buildingHeight);
                    EditorGUILayout.PropertyField(segmentsLogicalHeight);
                    EditorGUILayout.PropertyField(segmentsPhysicalHeight);
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(minCoreRadius);
                    EditorGUILayout.PropertyField(maxCoreRadius);
                    EditorGUILayout.PropertyField(minGroundRadius);
                    EditorGUILayout.PropertyField(maxGroundRadius);
                    EditorGUILayout.PropertyField(lowestGroundRadiusAllowed);
                    EditorGUILayout.PropertyField(heightDepthRatio);
                    EditorGUILayout.PropertyField(minHeight);
                    EditorGUILayout.PropertyField(maxHeight);
                    EditorGUILayout.PropertyField(minDepth);
                    EditorGUILayout.PropertyField(maxDepth);
                    EditorGUILayout.PropertyField(reliefBlendingOffset);
                    EditorGUILayout.PropertyField(coreOffsetMultiplier);
                    EditorGUILayout.PropertyField(rotateRelief);
                    EditorGUILayout.PropertyField(reliefColorFree);
                    EditorGUILayout.PropertyField(reliefColorHigher);
                    EditorGUILayout.PropertyField(reliefColorLower);
                    EditorGUILayout.PropertyField(reliefColorThreshold);

                    GUILayout.Space(15);
                    {
                        rect = EditorGUILayout.BeginHorizontal();
                        Handles.color = Color.gray;
                        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(randomizeCorePattern);
                    if (randomizeCorePattern.boolValue)
                    {
                        EditorGUILayout.PropertyField(coreRChannelMaxOffset);
                        EditorGUILayout.PropertyField(coreGChannelMaxOffset);
                        EditorGUILayout.PropertyField(coreBChannelMaxOffset);
                        EditorGUILayout.PropertyField(coreColorLower);
                        EditorGUILayout.PropertyField(coreColorMiddle);
                        EditorGUILayout.PropertyField(coreColorHigher);
                    }
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(turnOnDecorativeObjects);
                    if (turnOnDecorativeObjects.boolValue && showPropertiesOfSwitchableParts)
                    {
                        EditorGUILayout.PropertyField(decorativeObjectsDensity);
                        EditorGUILayout.PropertyField(freeSegmentObjects);
                        EditorGUILayout.PropertyField(higherSegmentObjects);
                        EditorGUILayout.PropertyField(lowerSegmentObjects);
                    }

                    GUILayout.Space(15);
                    {
                        rect = EditorGUILayout.BeginHorizontal();
                        Handles.color = Color.gray;
                        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(turnOnShadow);
                    if (turnOnShadow.boolValue && showPropertiesOfSwitchableParts)
                    {
                        EditorGUILayout.PropertyField(minShadowAngle);
                        EditorGUILayout.PropertyField(shadowAngleRange);
                        EditorGUILayout.PropertyField(minShadowLineRelativePosition);
                        EditorGUILayout.PropertyField(maxShadowLineRelativePosition);
                        EditorGUILayout.PropertyField(minShadowArcRelativePosition);
                        EditorGUILayout.PropertyField(maxShadowArcRelativePosition);
                        EditorGUILayout.PropertyField(maxShadowLength);
                        EditorGUILayout.PropertyField(arcVerticesCount);
                    }

                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(turnOnWater);
                    if (turnOnWater.boolValue && showPropertiesOfSwitchableParts)
                    {
                        EditorGUILayout.PropertyField(fillWholePlanet);
                        EditorGUILayout.PropertyField(waterOffset);
                        EditorGUILayout.PropertyField(waterColor);
                        EditorGUILayout.PropertyField(waterDepthColor);
                        EditorGUILayout.PropertyField(waterWavesAmplitude);
                        EditorGUILayout.PropertyField(waterWavesFrequency);
                        EditorGUILayout.PropertyField(waterWavesSpeed);
                    }

                    GUILayout.Space(15);
                    {
                        rect = EditorGUILayout.BeginHorizontal();
                        Handles.color = Color.gray;
                        Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(turnOnAtmosphere);
                    if (turnOnAtmosphere.boolValue && showPropertiesOfSwitchableParts)
                    {
                        EditorGUILayout.PropertyField(atmosphereColorBottom);
                        EditorGUILayout.PropertyField(atmosphereColorMiddle);
                        EditorGUILayout.PropertyField(atmosphereColorTop);
                        EditorGUILayout.PropertyField(atmosphereColorBorder);
                        EditorGUILayout.PropertyField(atmosphereColorNight);
                    }
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(turnOnBorderRing);
                    if (turnOnBorderRing.boolValue && showPropertiesOfSwitchableParts)
                    {
                        EditorGUILayout.PropertyField(bottomBorderOffset);
                        EditorGUILayout.PropertyField(topBorderOffset);
                        EditorGUILayout.PropertyField(borderRingColorFree);
                        EditorGUILayout.PropertyField(borderRingColorHigher);
                        EditorGUILayout.PropertyField(borderRingColorLower);
                        EditorGUILayout.PropertyField(borderRingColorBlendingOffset);
                    }
                }
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}
