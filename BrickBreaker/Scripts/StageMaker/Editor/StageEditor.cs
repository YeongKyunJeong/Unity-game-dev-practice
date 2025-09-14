using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageMaker))]
public class StageEditor : Editor
{
    private static int defaultLevel;

    private bool randomBrickHealth = true;
    private string[] rowBrickHealths = new string[0];
    private int loadLevel = 0;

    private int brickRowCount;
    private StringBuilder sb;

    private SerializedProperty brickRowCountProp;
    private SerializedProperty brickPerRowProp;
    private SerializedProperty brickSpaceXProp;
    private SerializedProperty brickSpaceYProp;
    private SerializedProperty rowBrickHealthsProp;
    private SerializedProperty rowElement;

    #region Cont Parameter
    private const int MAX_BRICK_ROW_COUNT = 6;
    private const int MIN_BRICK_ROW_COUNT = 1;
    private const int MAX_BRICK_PER_ROW_COUNT = 7;
    private const int MIN_BRICK_PER_ROW_COUNT = 1;
    private const float MIN_BRICK_SPACE_X = 0;
    private const float MIN_BRICK_SPACE_Y = 0;
    private const int DEFAULT_BRICK_HEALTH = 1;

    private const string BRICK_ROW_COUNT = "brickRowCount";
    private const string BRICK_PER_ROW = "brickPerRow";
    private const string BRICK_SPACE_X = "brickSpaceX";
    private const string BRICK_SPACE_Y = "brickSpaceY";
    private const string ROW_BRICK_HEALTHS = "rowBrickHealths";

    private const string brickRowCountName = "블록 행 개수";
    private const string brickPerRowName = "행 당 블록 개수";
    private const string brickSpaceYName = "블록 세로 간격";
    private const string brickSpaceXName = "블록 가로 개수";
    #endregion

    private void OnEnable()
    {
        brickRowCountProp = serializedObject.FindProperty(BRICK_ROW_COUNT);
        brickPerRowProp = serializedObject.FindProperty(BRICK_PER_ROW);
        brickSpaceXProp = serializedObject.FindProperty(BRICK_SPACE_X);
        brickSpaceYProp = serializedObject.FindProperty(BRICK_SPACE_Y);
        rowBrickHealthsProp = serializedObject.FindProperty(ROW_BRICK_HEALTHS);
    }


    public override void OnInspectorGUI()
    {
        #region Custom Inspector
        serializedObject.Update();

        DrawDefaultInspector();
        GUILayout.Space(20);

        StageMaker stageMaker = (StageMaker)target;

        EditorGUILayout.PropertyField(brickRowCountProp, new GUIContent(brickRowCountName));
        EditorGUILayout.PropertyField(brickPerRowProp, new GUIContent(brickPerRowName));
        EditorGUILayout.PropertyField(brickSpaceYProp, new GUIContent(brickSpaceYName));
        EditorGUILayout.PropertyField(brickSpaceXProp, new GUIContent(brickSpaceXName));

        AppliyMinMaxValue();

        GUILayout.Space(10);
        randomBrickHealth = EditorGUILayout.Toggle("랜덤 체력", randomBrickHealth);

        GUILayout.Space(10);
        GUILayout.Label("Stage Editor", EditorStyles.boldLabel);

        brickRowCount = brickRowCountProp.intValue;
        if (rowBrickHealthsProp.arraySize != brickRowCount)
        {
            rowBrickHealthsProp.arraySize = brickRowCount;
        }
        for (int i = 0; i < brickRowCount; i++)
        {
            rowElement = rowBrickHealthsProp.GetArrayElementAtIndex(i);
            rowElement.stringValue = EditorGUILayout.TextField($"{i + 1} 번째 줄 블록 체력", rowElement.stringValue);
        }

        serializedObject.ApplyModifiedProperties();

        #endregion
        #region ButtonAction

        if (GUILayout.Button("Generate Bricks"))
        {
            AdjustBrickHealths();
            serializedObject.ApplyModifiedProperties();
            stageMaker.GenerateBricks();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Power Clean"))
        {
            PowerClean(stageMaker.GetBrickRowParent);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Save Stage Parameter"))
        {
            string path = "";
            if (stageMaker.level < 10)
            {
                path = EditorUtility.SaveFilePanel("Save stage parameter", $"{ Application.dataPath}/StageParameter/", $"Level0{stageMaker.level}.json", "json");
                //path = EditorUtility.SaveFilePanel("Save stage parameter", "", $"Level0{stageMaker.level}.json", "json");
            }
            else
            {
                path = EditorUtility.SaveFilePanel("Save stage parameter", $"{ Application.dataPath}/StageParameter/", $"Level{stageMaker.level}.json", "json");
                //path = EditorUtility.SaveFilePanel("Save stage parameter", "", $"Level{stageMaker.level}.json", "json");
            }
            if (!string.IsNullOrEmpty(path))
            {
                stageMaker.SaveStageParameter(path);
            }
        }

        GUILayout.Space(10);

        loadLevel = EditorGUILayout.IntField("로드할 레벨", loadLevel);

        if (GUILayout.Button("Load Stage Parameter"))
        {
            string path = loadLevel.ToString();
            if (stageMaker.level < 10)
                path = EditorUtility.SaveFilePanel("Load stage parameter", "", $"Level0{path}.json", "json");
            else
                path = EditorUtility.SaveFilePanel("Load stage parameter", "", $"Level{path}.json", "json");
            if (!string.IsNullOrEmpty(path))
            {
                stageMaker.LoadStageParameter(path);
            }
        }

        #endregion
    }

    private void AdjustBrickHealths()
    {
        for (int i = 0; i < brickRowCount; i++)
        {
            sb = new StringBuilder();
            rowElement = rowBrickHealthsProp.GetArrayElementAtIndex(i);
            rowElement.stringValue = rowElement.stringValue.Replace(" ", "");

            if (randomBrickHealth)
            {
                for (int j = 0; j < brickPerRowProp.intValue; j++)
                {
                    sb.Append(Random.Range(1, 6).ToString());
                }
            }
            else
            {
                for (int j = 0; j < brickPerRowProp.intValue; j++)
                {
                    if (j < rowElement.stringValue.Length)
                    {
                        if (int.TryParse(rowElement.stringValue[j].ToString(), out int number))
                        {
                            if (number < 1)
                            {
                                number = 1;
                            }
                            if (number > 5)
                            {
                                number = 5;
                            }
                        }
                        else
                        {
                            number = DEFAULT_BRICK_HEALTH;
                        }

                        sb.Append(number.ToString());
                    }
                    else
                    {
                        sb.Append(DEFAULT_BRICK_HEALTH.ToString());
                    }
                }
            }
            rowElement.stringValue = sb.ToString();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void AppliyMinMaxValue()
    {
        if (brickRowCountProp.intValue > MAX_BRICK_ROW_COUNT)
        {
            brickRowCountProp.intValue = MAX_BRICK_ROW_COUNT;
        }
        else if (brickRowCountProp.intValue <= 0)
        {
            brickRowCountProp.intValue = MIN_BRICK_ROW_COUNT;
        }
        if (brickPerRowProp.intValue > MAX_BRICK_PER_ROW_COUNT)
        {
            brickPerRowProp.intValue = MAX_BRICK_PER_ROW_COUNT;
        }
        else if (brickPerRowProp.intValue <= 0)
        {
            brickPerRowProp.intValue = MIN_BRICK_PER_ROW_COUNT;
        }
        if (brickSpaceXProp.floatValue < MIN_BRICK_SPACE_X)
        {
            brickSpaceXProp.floatValue = MIN_BRICK_SPACE_X;
        }
        if (brickSpaceYProp.floatValue < MIN_BRICK_SPACE_Y)
        {
            brickSpaceYProp.floatValue = MIN_BRICK_SPACE_Y;
        }
    }

    private void PowerClean(Transform targetParentTransform)
    {
        int childCount = targetParentTransform.childCount;

        for (int i = childCount - 1; i > -1; i--)
        {
            DestroyImmediate(targetParentTransform.GetChild(i).gameObject);
        }

        Debug.Log($"Power Clean : {childCount} child objects deleted");
    }
}
