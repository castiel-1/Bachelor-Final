using UnityEngine;
using UnityEditor;
using UnityEngine.VFX;

public class Settings : EditorWindow
{
    private VisualEffect visualEffect;

    // foldouts
    private bool showPathSettings = false;
    private bool showTextSettings = false;
    private bool showLLMSettings = false;

    private string[] textSizeOptions = new string[] { "One Size", "Random", "Growing", "Shrinking", "Wave" };

    // toggle
    private bool previousFaceCameraPlane = RuntimeSettingsData.faceCameraPlane;

    // scroll position
    private Vector2 scrollPos;

    [MenuItem("Window/Prototype Settings")]
    public static void ShowWindow()
    {
        GetWindow<Settings>("Prototype Settings");
    }

    private void OnEnable()
    {
        visualEffect = GameObject.Find("LetterDisplayVFX").GetComponent<VisualEffect>();
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        {
            // create space between scrollbar and content
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width - 20));
            {
                    DrawPathSettings();
                    DrawTextSettings();
                    DrawLLMSettings();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawPathSettings()
    {
        // foldout
        showPathSettings = EditorGUILayout.Foldout(showPathSettings, "Path Settings", true);

        if (!showPathSettings)
        {
            return;
        }

        EditorGUI.indentLevel++;

        DrawGravityToggle();

        EditorGUI.indentLevel--;
    }
    
    // gravity toggle
    private void DrawGravityToggle()
    {
        RuntimeSettingsData.onSurface = EditorGUILayout.Toggle("On Surface", RuntimeSettingsData.onSurface);
    }

    private void DrawTextSettings()
    {
        showTextSettings = EditorGUILayout.Foldout(showTextSettings, "Text Settings", true);

        if(!showTextSettings)
        {
            return;
        }

        EditorGUI.indentLevel++;

        DrawTextSizeDropdown();
        DrawMinMaxNumberOfWordsField();
        DrawColorPicker();
        DrawTextOrientationToggle();
        if(!RuntimeSettingsData.faceCameraPlane)
        {
            EditorGUI.indentLevel++;
            DrawTextOrientationInputVector();
            EditorGUI.indentLevel--;
        }

        EditorGUI.indentLevel--;

    }

    private void DrawTextSizeDropdown()
    {
        RuntimeSettingsData.textSizeMode = (RuntimeSettingsData.TextSizeMode) EditorGUILayout.Popup("Text Size Mode", (int) RuntimeSettingsData.textSizeMode, textSizeOptions);

        if((int) RuntimeSettingsData.textSizeMode == 0)
        {
            EditorGUI.indentLevel++;

            DrawOneSizeField();

            EditorGUI.indentLevel--;
        }
        else
        {
            EditorGUI.indentLevel++;

            DrawMinMaxSizeField();
            
            EditorGUI.indentLevel-- ;
        }
    }

    private void DrawOneSizeField()
    {
        RuntimeSettingsData.textSizeMin = EditorGUILayout.FloatField("Text Size", RuntimeSettingsData.textSizeMin);
    }

    private void DrawMinMaxSizeField()
    {
        RuntimeSettingsData.textSizeMin = EditorGUILayout.FloatField("Text Size Minimum", RuntimeSettingsData.textSizeMin);
        RuntimeSettingsData.textSizeMax = EditorGUILayout.FloatField("Text Size Maximum", RuntimeSettingsData.textSizeMax);
    }

    private void DrawMinMaxNumberOfWordsField()
    {
        EditorGUILayout.LabelField("Number of Words");

        EditorGUI.indentLevel++;

        RuntimeSettingsData.numberOfWordsMin = EditorGUILayout.IntField("Minimun", RuntimeSettingsData.numberOfWordsMin);
        RuntimeSettingsData.numberOfWordsMax = EditorGUILayout.IntField("Maximum", RuntimeSettingsData.numberOfWordsMax);

        EditorGUI.indentLevel--;
    }

    private void DrawColorPicker()
    {
        EditorGUILayout.LabelField("Text Colour Without Influence");

        EditorGUI.indentLevel++;
            RuntimeSettingsData.uninfluencedTextColor = EditorGUILayout.ColorField(GUIContent.none, RuntimeSettingsData.uninfluencedTextColor);
        EditorGUI.indentLevel--;
    }

    private void DrawTextOrientationToggle()
    {
        RuntimeSettingsData.faceCameraPlane = EditorGUILayout.Toggle("Face Camera Plane", RuntimeSettingsData.faceCameraPlane);

        if(RuntimeSettingsData.faceCameraPlane != previousFaceCameraPlane)
        {
            visualEffect.SetBool("FaceCameraPlane", RuntimeSettingsData.faceCameraPlane);
            previousFaceCameraPlane = RuntimeSettingsData.faceCameraPlane;
        }
    }

    private void DrawLLMSettings()
    {
        // foldout
        showLLMSettings = EditorGUILayout.Foldout(showLLMSettings, "LLM Settings", true);

        if (!showLLMSettings)
        {
            return;
        }

        EditorGUI.indentLevel++;

        DrawHistoryDepthField();

        EditorGUI.indentLevel--;
    }

    private void DrawHistoryDepthField()
    {
        RuntimeSettingsData.historyDepth = EditorGUILayout.IntField("History Prompt Depth", RuntimeSettingsData.historyDepth);
    }

    private void DrawTextOrientationInputVector()
    {
        RuntimeSettingsData.orientation = EditorGUILayout.Vector3Field("Orientation", RuntimeSettingsData.orientation);
    }
}

