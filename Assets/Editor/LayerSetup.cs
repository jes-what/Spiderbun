#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WhiteJessica.PredatorPrey
{
    // Unity calls static constructors on this type of class automatically when a new package is imported
    [InitializeOnLoad]
    public class LayerSetup
    {
        static LayerSetup()
        {
            CreateLayer("Food", 3);
            CreateLayer("Obstacles", 6);
        }

        // assigns a named layer to given layer index if the slot is empty
        private static void CreateLayer(string name, int layerNum)
        {
            // load TagManager asset, where layers stored
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
                );
            SerializedProperty layers = tagManager.FindProperty("layers");
            SerializedProperty layer = layers.GetArrayElementAtIndex(layerNum);

            // only write if slot isn't occupied
            if (layer.stringValue == "")
            {
                layer.stringValue = name;
                tagManager.ApplyModifiedProperties();
                Debug.Log("Created layer: " + name);
            }
        }
    }
}
#endif