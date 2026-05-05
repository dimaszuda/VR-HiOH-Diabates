using UnityEngine;
using UnityEditor;

public class ConvertTexturesToSprites : MonoBehaviour
{
    [MenuItem("Tools/Convert Textures to Sprites")]
    static void Convert()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Image-Icon/insulin" }); // Ganti path foldermu

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.SaveAndReimport();

                Debug.Log("Converted to Sprite: " + path);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Conversion finished!");
    }
}
