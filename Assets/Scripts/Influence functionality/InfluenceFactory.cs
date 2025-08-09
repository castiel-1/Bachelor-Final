using UnityEngine;
using UnityEngine.UIElements;

public static class InfluenceFactory
{
    public static SemanticInfluence CreateSemanticInfluence(string name, Vector3 position, float radius, GameObject prefab, string promptModifier)
    {
        // use standard sphere if no gameObject chosen
        if(prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/SemanticInfluenceP");
        }

        // use beginning of prompt if no name is chosen
        if(name == "" && promptModifier != "")
        {
            string[] promptWords = promptModifier.Split(' ');

            Debug.Log(promptWords[0]);

            name = promptWords[0];
        }

        SemanticInfluence influence = new SemanticInfluence(name, position, radius, prefab, promptModifier);

        return influence;
    }

    public static ColorInfluence CreateColorInfluence(string name, Vector3 position, float radius, GameObject prefab, Color color)
    {
        // use standard sphere if no gameObject chosen
        if (prefab == null)
        {
            prefab = Resources.Load<GameObject>("Prefabs/colorInfluenceP");
        }

        ColorInfluence colorInfluence = new ColorInfluence(name, position, radius, prefab, color);

        return colorInfluence;
    }
}
