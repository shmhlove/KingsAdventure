//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UITexture), true)]
public class UITextureInspector : UIBasicSpriteEditor
{
	UITexture mTex;

    void OnSelectAtlas(Object obj)
    {
        NGUISettings.atlas = obj as UIAtlas;
    }

    protected override void OnEnable ()
	{
		base.OnEnable();
		mTex = target as UITexture;
	}

	protected override bool ShouldDrawProperties ()
	{
		if (target == null) return false;

        if (Application.isPlaying == false && NGUIEditorTools.DrawHeader("Tex to Spr"))
        {
            NGUIEditorTools.BeginContents();
            GUILayout.BeginHorizontal();

            if (NGUIEditorTools.DrawPrefixButton("Atlas"))
            {
                ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
            }

            UIAtlas curAtlas = NGUISettings.atlas;

            if (curAtlas != null)
            {
                GUILayout.Label(curAtlas.name, "HelpBox", GUILayout.Height(18f));
            }
            else
            {
                GUILayout.Label("No Atlas", "HelpBox", GUILayout.Height(18f));
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("ToS", GUILayout.Width(40f)))
            {
                if (curAtlas == null)
                {
                    CreateOwnAtlas();

                    curAtlas = NGUISettings.atlas;
                }

                List<UIAtlasMaker.SpriteEntry> entries = UIAtlasMaker.CreateSprites(new List<Texture>() { mTex.mainTexture });
                UIAtlasMaker.ExtractSprites(curAtlas, entries);
                UIAtlasMaker.UpdateAtlas(curAtlas, entries);

                UISprite sprite = mTex.gameObject.AddComponent<UISprite>();
                sprite.atlas = curAtlas;
                sprite.spriteName = mTex.mainTexture.name;

                sprite.type = mTex.type;

                UISpriteData data = sprite.GetAtlasSprite();
                data.SetBorder((int)mTex.border.w, (int)mTex.border.y, (int)mTex.border.x, (int)mTex.border.z);

                sprite.depth = mTex.depth;
                sprite.width = mTex.width;
                sprite.height = mTex.height;

                UITexture.DestroyImmediate(mTex);

                EditorGUIUtility.ExitGUI();

                return false;
            }

            GUILayout.EndHorizontal();
            NGUIEditorTools.EndContents();
        }

        SerializedProperty sp = NGUIEditorTools.DrawProperty("Texture", serializedObject, "mTexture");
		NGUIEditorTools.DrawProperty("Material", serializedObject, "mMat");

		if (sp != null) NGUISettings.texture = sp.objectReferenceValue as Texture;

		if (mTex != null && (mTex.material == null || serializedObject.isEditingMultipleObjects))
		{
			NGUIEditorTools.DrawProperty("Shader", serializedObject, "mShader");
		}

		EditorGUI.BeginDisabledGroup(mTex == null || mTex.mainTexture == null || serializedObject.isEditingMultipleObjects);

		NGUIEditorTools.DrawRectProperty("UV Rect", serializedObject, "mRect");

		sp = serializedObject.FindProperty("mFixedAspect");
		bool before = sp.boolValue;
		NGUIEditorTools.DrawProperty("Fixed Aspect", sp);
		if (sp.boolValue != before) (target as UIWidget).drawRegion = new Vector4(0f, 0f, 1f, 1f);

		if (sp.boolValue)
		{
			EditorGUILayout.HelpBox("Note that Fixed Aspect mode is not compatible with Draw Region modifications done by sliders and progress bars.", MessageType.Info);
		}

		EditorGUI.EndDisabledGroup();

        return true;
	}

	/// <summary>
	/// Allow the texture to be previewed.
	/// </summary>

	public override bool HasPreviewGUI ()
	{
		return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1) &&
			(mTex != null) && (mTex.mainTexture as Texture2D != null);
	}

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		Texture2D tex = mTex.mainTexture as Texture2D;

		if (tex != null)
		{
			Rect tc = mTex.uvRect;
			tc.xMin *= tex.width;
			tc.xMax *= tex.width;
			tc.yMin *= tex.height;
			tc.yMax *= tex.height;
			NGUIEditorTools.DrawSprite(tex, rect, mTex.color, tc, mTex.border);
		}
	}

    #region Additional Custom Functions
    private void CreateOwnAtlas()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save As",
            "New Atlas.prefab", "prefab", "Save atlas as...", NGUISettings.currentPath);

        if (!string.IsNullOrEmpty(path))
        {
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

            // Select the atlas
            go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            NGUISettings.atlas = go.GetComponent<UIAtlas>();
        }
    }
    #endregion
}
