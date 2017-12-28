//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UISprites.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UISprite), true)]
public class UISpriteInspector : UIBasicSpriteEditor
{
	/// <summary>
	/// Atlas selection callback.
	/// </summary>

	void OnSelectAtlas (Object obj)
	{
		serializedObject.Update();
		SerializedProperty sp = serializedObject.FindProperty("mAtlas");
		sp.objectReferenceValue = obj;
		serializedObject.ApplyModifiedProperties();
		NGUITools.SetDirty(serializedObject.targetObject);
		NGUISettings.atlas = obj as UIAtlas;
	}

	/// <summary>
	/// Sprite selection callback function.
	/// </summary>

	void SelectSprite (string spriteName)
	{
		serializedObject.Update();
		SerializedProperty sp = serializedObject.FindProperty("mSpriteName");
		sp.stringValue = spriteName;
		serializedObject.ApplyModifiedProperties();
		NGUITools.SetDirty(serializedObject.targetObject);
		NGUISettings.selectedSprite = spriteName;
	}

	/// <summary>
	/// Draw the atlas and sprite selection fields.
	/// </summary>

	protected override bool ShouldDrawProperties ()
	{
        if (DrawAdvancedMenu() == false)
        {
            return false;
        }

		GUILayout.BeginHorizontal();
		if (NGUIEditorTools.DrawPrefixButton("Atlas"))
			ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
		SerializedProperty atlas = NGUIEditorTools.DrawProperty("", serializedObject, "mAtlas", GUILayout.MinWidth(20f));
		
		if (GUILayout.Button("Edit", GUILayout.Width(40f)))
		{
			if (atlas != null)
			{
				UIAtlas atl = atlas.objectReferenceValue as UIAtlas;
				NGUISettings.atlas = atl;
				NGUIEditorTools.Select(atl.gameObject);
			}
		}
		GUILayout.EndHorizontal();

		SerializedProperty sp = serializedObject.FindProperty("mSpriteName");
		NGUIEditorTools.DrawAdvancedSpriteField(atlas.objectReferenceValue as UIAtlas, sp.stringValue, SelectSprite, false);
		return true;
	}

	/// <summary>
	/// All widgets have a preview.
	/// </summary>

	public override bool HasPreviewGUI ()
	{
		return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1);
	}

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		UISprite sprite = target as UISprite;
		if (sprite == null || !sprite.isValid) return;

		Texture2D tex = sprite.mainTexture as Texture2D;
		if (tex == null) return;

		UISpriteData sd = sprite.atlas.GetSprite(sprite.spriteName);
		NGUIEditorTools.DrawSprite(tex, rect, sd, sprite.color);
	}

    #region Additional Custom Functions
    private bool DrawAdvancedMenu()
    {
        if (Application.isPlaying == true)
        {
            return true;
        }

        if (NGUIEditorTools.DrawHeader("Advanced menu") == false)
        {
            return true;
        }

        NGUIEditorTools.BeginContents();
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        _texToSprite = EditorGUILayout.ObjectField(_texToSprite, typeof(Texture), false) as Texture;

        if (GUILayout.Button("Add", GUILayout.Width(40f)))
        {
            if (_texToSprite == null)
            {
                Debug.LogError("Select a texture first.");
                return false;
            }

            UIAtlas curAtlas = GetAtlas();

            if (curAtlas == null)
            {
                if (CreateOwnAtlas() == false)
                {
                    return false;
                }

                curAtlas = GetAtlas();
            }

            List<UIAtlasMaker.SpriteEntry> entries = UIAtlasMaker.CreateSprites(new List<Texture>() { _texToSprite });
            UIAtlasMaker.ExtractSprites(curAtlas, entries);
            UIAtlasMaker.UpdateAtlas(curAtlas, entries);

            SelectSprite(_texToSprite.name);

            return false;
        }

        if (GUILayout.Button("New") == true)
        {
            if (_texToSprite == null)
            {
                Debug.LogError("Select a texture first.");
                return false;
            }

            if (CreateOwnAtlas() == false)
            {
                return false;
            }

            UIAtlas curAtlas = GetAtlas();

            List<UIAtlasMaker.SpriteEntry> entries = UIAtlasMaker.CreateSprites(new List<Texture>() { _texToSprite });
            UIAtlasMaker.ExtractSprites(curAtlas, entries);
            UIAtlasMaker.UpdateAtlas(curAtlas, entries);

            SelectSprite(_texToSprite.name);

            return false;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete sprite") == true)
        {
            UIAtlas curAtlas = GetAtlas();
            string curSprName = GetSpriteName();

            if (curAtlas == null)
            {
                Debug.LogError("No atlas in this UISprite");
                return false;
            }

            if (string.IsNullOrEmpty(curSprName) == true)
            {
                Debug.LogError("No sprite in this UISprite");
                return false;
            }

            List<UIAtlasMaker.SpriteEntry> lstSprites = new List<UIAtlasMaker.SpriteEntry>();
            UIAtlasMaker.ExtractSprites(curAtlas, lstSprites);

            for (int i = lstSprites.Count; i > 0;)
            {
                UIAtlasMaker.SpriteEntry entry = lstSprites[--i];

                if (entry.name == curSprName)
                {
                    lstSprites.RemoveAt(i);
                }
            }
            UIAtlasMaker.UpdateAtlas(curAtlas, lstSprites);
            NGUIEditorTools.RepaintSprites();

            return false;
        }

        if (GUILayout.Button("Atlas Maker") == true)
        {
            EditorWindow.GetWindow<UIAtlasMaker>(false, "Atlas Maker", true).Show();

            return false;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        NGUIEditorTools.EndContents();

        return true;
    }

    private UIAtlas GetAtlas()
    {
        SerializedProperty sp = serializedObject.FindProperty("mAtlas");

        if (sp == null)
        {
            return null;
        }

        return sp.objectReferenceValue as UIAtlas;
    }

    private string GetSpriteName()
    {
        SerializedProperty sp = serializedObject.FindProperty("mSpriteName");

        if (sp == null)
        {
            return "";
        }

        return sp.stringValue;
    }

    private bool CreateOwnAtlas()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save As",
            "New Atlas.prefab", "prefab", "Save atlas as...", NGUISettings.currentPath);

        if (string.IsNullOrEmpty(path) == true)
        {
            return false;
        }

        NGUISettings.currentPath = System.IO.Path.GetDirectoryName(path);
        GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        string matPath = path.Replace(".prefab", ".mat");

        // Try to load the material
        Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

        // If the material doesn't exist, create it
        if (mat == null)
        {
            Shader shader = Shader.Find(NGUISettings.atlasPMA ? "Unlit/Premultiplied Colored" : "Unlit/Transparent Colored");
            mat = new Material(shader);

            // Save the material
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // Load the material so it's usable
            mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
        }

        // Create a new prefab for the atlas
        Object prefab = (go != null) ? go : PrefabUtility.CreateEmptyPrefab(path);

        // Create a new game object for the atlas
        string atlasName = path.Replace(".prefab", "");
        atlasName = atlasName.Substring(path.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
        go = new GameObject(atlasName);
        go.AddComponent<UIAtlas>().spriteMaterial = mat;

        // Update the prefab
        PrefabUtility.ReplacePrefab(go, prefab);
        DestroyImmediate(go);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        // Set the atlas to current sprite
        OnSelectAtlas((Object)NGUISettings.atlas);

        return true;        
    }

    private static Texture _texToSprite = null;
#endregion
}
