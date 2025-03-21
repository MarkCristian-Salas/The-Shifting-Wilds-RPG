﻿using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Broccoli.Pipe;
using Broccoli.Utils;

namespace Broccoli.Manager
{
	/// <summary>
	/// Texture manager.
	/// Manages textures used on the pipeline and creates atlases.
	/// </summary>
	public class TextureManager {
		#region Texture Settings
		/// <summary>
		/// Texture export settings.
		/// </summary>
		public struct TextureSettings {
			public bool exportToNormal;
			public bool alphaIsTransparency;
			public Color bgColor;
			public bool applyGammaCorrection;
			public TextureSettings (bool alphaIsTransparency, bool exportToNormal, Color bgColor, bool applyGammaCorrection = false) {
				this.alphaIsTransparency = alphaIsTransparency;
				this.exportToNormal = exportToNormal;
				this.bgColor = bgColor;
				this.applyGammaCorrection = applyGammaCorrection;
			}
		}
		#endregion

		#region Vars
		/// <summary>
		/// The last folder created or set on the manager.
		/// </summary>
		public string lastFolder = "";
		/// <summary>
		/// Relationship between id and texture.
		/// </summary>
		private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D> ();
		/// <summary>
		/// Textures settings.
		/// </summary>
		private Dictionary<string, TextureSettings> textureToSettings = new Dictionary<string, TextureSettings> ();
		/// <summary>
		/// Atlas names.
		/// </summary>
		private List<string> atlases = new List<string> ();
		/// <summary>
		/// Atlas name to atlas texture settings.
		/// </summary>
		private Dictionary<string, TextureSettings> atlasToTextureSettings = new Dictionary<string, TextureSettings> ();
		/// <summary>
		/// Assignation of texture id to atlas index.
		/// </summary>
		private Dictionary<string, int> textureToAtlases = new Dictionary<string, int> ();
		/// <summary>
		/// Assignation of rects areas after creation of atlases.
		/// </summary>
		private Dictionary<string, Rect[]> atlasesRects = new Dictionary<string, Rect[]> ();
		/// <summary>
		/// Path to texture asset after saving textures.
		/// </summary>
		private Dictionary<string, string> textureAssetPath = new Dictionary<string, string> ();
		/// <summary>
		/// Path to atlas asset after saving atlases.
		/// </summary>
		private Dictionary<string, string> atlasAssetPath = new Dictionary<string, string> ();
		/// <summary>
		/// List of textures instantiated on this manager.
		/// </summary>
		private List<Texture2D> instantiatedTextures = new List<Texture2D> ();
		/// <summary>
		/// The textures for atlas.
		/// </summary>
		List<Texture2D> texturesForAtlas = new List<Texture2D> ();
		#endregion

		#region Singleton
		/// <summary>
		/// Singleton object for the manager.
		/// </summary>
		static TextureManager _textureManager = null;
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static TextureManager GetInstance () {
			if (_textureManager == null) {
				_textureManager = new TextureManager ();
			}
			return _textureManager;
		}
		#endregion

		#region Textures
		public Texture2D GetTexture (string textureId) {
			if (textures.ContainsKey (textureId)) {
				return textures [textureId];
			}
			return null;
		}
		public bool HasTexture (string textureId) {
			return textures.ContainsKey (textureId);
		}
		/// <summary>
		/// Add a texture to the manager.
		/// </summary>
		/// <returns>Texture added to the manager or null if no file was found.</returns>
		/// <param name="textureId">Texture identifier.</param>
		/// <param name="texturePath">Path to texture file.</param>
		public Texture2D AddTexture (string textureId, string texturePath) {
			Texture2D texture = null;
			byte[] fileData;
			if (File.Exists (texturePath)) {
				texture = new Texture2D (2, 2);
				fileData = File.ReadAllBytes (texturePath);
				texture.LoadImage (fileData);
				AddTexture (textureId, texture);
			}
			return texture;
		}
		/// <summary>
		/// Adds a texture to the manager.
		/// </summary>
		/// <param name="textureId">Texture identifier.</param>
		/// <param name="texture">Texture to add to the manager.</param>
		public void AddTexture (string textureId, Texture2D texture) {
			if (! textures.ContainsKey (textureId)) {
				textures.Add (textureId, texture);
			}
		}
		/// <summary>
		/// Adds or replaces a texture on the manager.
		/// </summary>
		/// <param name="textureId">Texture identifier.</param>
		/// <param name="texture">Texture to add or to replace with.</param>
		/// <param name="destroyExisting">True if a former texture should be free from memory.</param>
		/// <returns>True if the texture was added, false if was replaced.</returns>
		public bool AddOrReplaceTexture (string textureId, Texture2D texture, bool destroyExisting = false) {
			bool textureExists = textures.ContainsKey (textureId);
			if (textureExists) {
				RemoveTexture (textureId, destroyExisting);
			}
			AddTexture (textureId, texture);
			return !textureExists;
		}
		/// <summary>
		/// Removes a texture.
		/// </summary>
		/// <returns><c>true</c>, if texture was removed, <c>false</c> otherwise.</returns>
		/// <param name="textureId">Texture identifier.</param>
		/// <param name="destroy">Destroys the texture.</param>
		public bool RemoveTexture (string textureId, bool destroy = false) {
			if (textures.ContainsKey (textureId)) {
				if (destroy) {
					#if UNITY_EDITOR
					Object.DestroyImmediate (textures [textureId], false);
					#else
					Object.Destroy (textures [textureId]);
					#endif
				}
				textures.Remove (textureId);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Number of textures in the manager.
		/// </summary>
		/// <returns>The texture count.</returns>
		public int GetTextureCount () {
			return textures.Count;
		}
		/// <summary>
		/// Checks if an atlas has been registered.
		/// </summary>
		/// <param name="atlasId">Atlas identifier.</param>
		/// <returns><c>True</c> if the atlas is registered.</returns>
		public bool HasAtlas (string atlasId = "atlas") {
			return atlases.Contains (atlasId);
		}
		/// <summary>
		/// Registers an added texture to be taken as part of an atlas.
		/// </summary>
		/// <returns><c>true</c>, if texture to atlas was added, <c>false</c> otherwise.</returns>
		/// <param name="textureId">Texture identifier.</param>
		/// <param name="atlasId">Atlas identifier.</param>
		public bool RegisterTextureToAtlas (string textureId, string atlasId = "atlas") {
			if (textures.ContainsKey (textureId)) {
				if (!atlases.Contains (atlasId)) {
					atlases.Add (atlasId);
				}
				textureToAtlases.Add (textureId, atlases.IndexOf (atlasId));
				return true;
			}
			return false;
		}
		/// <summary>
		/// Deregisters an atlas.
		/// </summary>
		/// <param name="atlasId">Atlas identifier.</param>
		/// <returns><c>True</c> if the atlas was registered.</returns>
		public bool DeregisterAtlas (string atlasId = "atlas") {
			if (atlases.Contains (atlasId)) {
				int atlasIndex = atlases.IndexOf (atlasId);
				List<string> texturesRel = new List<string>();
				var texToAtlasEnum = textureToAtlases.GetEnumerator ();
				while (texToAtlasEnum.MoveNext ()) {
					if (texToAtlasEnum.Current.Value == atlasIndex) {
						texturesRel.Add (texToAtlasEnum.Current.Key);
					}
				}
				for (int i = 0; i < texturesRel.Count; i++) {
					textureToAtlases.Remove (texturesRel[i]);
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// Sets a registered texture settings.
		/// </summary>
		/// <param name="textureId">Texture identifier.</param>
		/// <param name="textureSettings">Texture export settings.</param>
		/// <returns><c>True</c>, if the texture settings were set, <c>false</c> otherwise.</returns>
		public bool SetTextureSettings (string textureId, TextureSettings textureSettings) {
			if (textures.ContainsKey(textureId)) {
				if (textureToSettings.ContainsKey (textureId)) {
					textureToSettings.Remove (textureId);
				}
				textureToSettings.Add (textureId, textureSettings);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Sets the atlas texture settings.
		/// </summary>
		/// <returns><c>true</c>, if atlas to normal map was set, <c>false</c> otherwise.</returns>
		/// <param name="atlasId">Atlas identifier.</param>
		/// <param name="textureSettings">Atlas texture export settings.</param>
		public bool SetAtlasSettings (string atlasId, TextureSettings textureSettings) {
			if (atlases.Contains (atlasId)) {
				if (atlasToTextureSettings.ContainsKey (atlasId)) {
					atlasToTextureSettings.Remove (atlasId);
				}
				atlasToTextureSettings.Add (atlasId, textureSettings);
				return true;
			}
			return false;
		}
		/// <summary>
		/// Tries to get the main texture assigned to a material.
		/// </summary>
		/// <returns>The main texture assigned to the material, if the method could not find one null is returned.</returns>
		/// <param name="material">Material to inspect.</param>
		/// <param name="makeCopy">If set to <c>true</c> make a copy of the texture.</param>
		public Texture2D GetMainTexture (Material material, bool makeCopy = true) {
			Texture2D tex = null;
			if (material.HasProperty ("_MainTex")) {
				tex = (Texture2D)material.GetTexture ("_MainTex");
			}
			Texture2D dest = null;
			if (tex != null && makeCopy) {
				dest = GetCopy (tex);
				instantiatedTextures.Add (dest);
			} else {
				dest = tex;
			}
			return dest;
		}
		/// <summary>
		/// Tries to get the normal texture assigned to a material.
		/// </summary>
		/// <returns>The normal texture assigned to the material, if the method could not find one null is returned.</returns>
		/// <param name="material">Material to inspect.</param>
		/// <param name="makeCopy">If set to <c>true</c> make a copy of the texture.</param>
		public Texture2D GetNormalTexture (Material material, bool makeCopy = true) {
			Texture2D normalTexture= null;
			if (material.HasProperty ("_BumpSpecMap")) {
				normalTexture = (Texture2D)material.GetTexture ("_BumpSpecMap");
			}
			Texture2D dest = null;
			if (normalTexture == null && material.HasProperty ("_BumpMap")) {
				normalTexture = (Texture2D)material.GetTexture ("_BumpMap");
			}
			if (normalTexture != null && makeCopy) {
				dest = GetCopy (normalTexture);
				if (!instantiatedTextures.Contains (dest)) {
					instantiatedTextures.Add (dest);
				}
			} else {
				dest = normalTexture;
			}
			return dest;
		}
		/// <summary>
		/// Tries to get the extras texture assigned to a material.
		/// </summary>
		/// <returns>The extras texture assigned to the material, if the method could not find one null is returned.</returns>
		/// <param name="material">Material to inspect.</param>
		/// <param name="makeCopy">If set to <c>true</c> make a copy of the texture.</param>
		public Texture2D GetExtrasTexture (Material material, bool makeCopy = true) {
			Texture2D tex = null;
			if (material.HasProperty ("_ExtraTex")) {
				tex = (Texture2D)material.GetTexture ("_ExtraTex");
			}
			Texture2D dest = null;
			if (tex != null && makeCopy) {
				dest = GetCopy (tex);
				instantiatedTextures.Add (dest);
			} else {
				dest = tex;
			}
			return dest;
		}
		/// <summary>
		/// Tries to get the subsurface texture assigned to a material.
		/// </summary>
		/// <returns>The subsurface texture assigned to the material, if the method could not find one null is returned.</returns>
		/// <param name="material">Material to inspect.</param>
		/// <param name="makeCopy">If set to <c>true</c> make a copy of the texture.</param>
		public Texture2D GetSubsurfaceTexture (Material material, bool makeCopy = true) {
			Texture2D tex = null;
			if (material.HasProperty ("_SubsrfaceTex")) {
				tex = (Texture2D)material.GetTexture ("_SubsurfaceTex");
			}
			Texture2D dest = null;
			if (tex != null && makeCopy) {
				dest = GetCopy (tex);
				instantiatedTextures.Add (dest);
			} else {
				dest = tex;
			}
			return dest;
		}
		// https://forum.unity.com/threads/unpacknormal-fixed4-packednormal-role.101163/
		// https://forum.unity.com/threads/normal-map-looks-red-in-shader-graph.1071962/
		
		/// <summary>
		/// Gets a readable copy of a texture.
		/// </summary>
		/// <returns>The readable texture.</returns>
		/// <param name="baseTexture">Base texture.</param>
		public Texture2D GetCopy (Texture2D baseTexture, float alphaFactor = 1f) {
			Texture2D texCopy = null;
			if (baseTexture != null) {
				// Create a temporary RenderTexture of the same size as the texture
				RenderTexture tmpRenderTexture = RenderTexture.GetTemporary (
					baseTexture.width,
					baseTexture.height,
					0,
					RenderTextureFormat.ARGB32,
					RenderTextureReadWrite.sRGB);
				// Blit the pixels on texture to the RenderTexture
				Graphics.Blit(baseTexture, tmpRenderTexture);
				// Backup the currently set RenderTexture
				RenderTexture previousRenderTexture = RenderTexture.active;
				// Set the current RenderTexture to the temporary one we created
				RenderTexture.active = tmpRenderTexture;
				// Create a new readable Texture2D to copy the pixels to it
				texCopy = new Texture2D (baseTexture.width, baseTexture.height);
				// Copy the pixels from the RenderTexture to the new Texture
				texCopy.ReadPixels(new Rect(0, 0, tmpRenderTexture.width, tmpRenderTexture.height), 0, 0);
				if (alphaFactor != 1f) {
					Color[] pixels = texCopy.GetPixels ();
					for (int i = 0; i < pixels.Length; ++i)
					{
						pixels[i].a = pixels[i].a * alphaFactor;
					}
					texCopy.SetPixels (pixels);
				}
				texCopy.Apply();
				// Reset the active RenderTexture
				RenderTexture.active = previousRenderTexture;
				// Release the temporary RenderTexture
				RenderTexture.ReleaseTemporary (tmpRenderTexture);
				if (!instantiatedTextures.Contains (texCopy)) {
					instantiatedTextures.Add (texCopy);
				}
			}
			return texCopy;
		}

		/// <summary>
		/// Gets a readable copy of a texture.
		/// </summary>
		/// <returns>The readable texture.</returns>
		/// <param name="baseTexture">Base texture.</param>
		public Texture2D GetNormalToRGBA (Texture2D baseTexture) {
			Texture2D texCopy = null;
			if (baseTexture != null) {
				// Create a temporary RenderTexture of the same size as the texture
				RenderTexture tmpRenderTexture = RenderTexture.GetTemporary (
					baseTexture.width,
					baseTexture.height,
					0,
					RenderTextureFormat.ARGB32,
					RenderTextureReadWrite.sRGB);
				Material unpackNormalMat = new Material (Shader.Find("Hidden/BroccoUnpackNormal"));
				// Blit the pixels on texture to the RenderTexture
				Graphics.Blit(baseTexture, tmpRenderTexture, unpackNormalMat);
				// Backup the currently set RenderTexture
				RenderTexture previousRenderTexture = RenderTexture.active;
				// Set the current RenderTexture to the temporary one we created
				RenderTexture.active = tmpRenderTexture;
				// Create a new readable Texture2D to copy the pixels to it
				texCopy = new Texture2D (baseTexture.width, baseTexture.height);
				// Copy the pixels from the RenderTexture to the new Texture
				texCopy.ReadPixels(new Rect(0, 0, tmpRenderTexture.width, tmpRenderTexture.height), 0, 0);
				texCopy.Apply();
				// Reset the active RenderTexture
				RenderTexture.active = previousRenderTexture;
				// Release the temporary RenderTexture
				RenderTexture.ReleaseTemporary (tmpRenderTexture);
				if (!instantiatedTextures.Contains (texCopy)) {
					instantiatedTextures.Add (texCopy);
				}
			}
			return texCopy;
		}

		/// <summary>
		/// Gets the texture asset path.
		/// </summary>
		/// <returns>The texture asset path.</returns>
		/// <param name="textureId">Texture identifier.</param>
		public string GetTextureAssetPath (string textureId) {
			if (textureAssetPath.ContainsKey (textureId)) {
				return textureAssetPath [textureId];
			}
			return string.Empty;
		}
		/// <summary>
		/// Gets the atlas asset path.
		/// </summary>
		/// <returns>The atlas asset path.</returns>
		/// <param name="atlasId">Atlas identifier.</param>
		public string GetAtlasAssetPath (string atlasId = "atlas") {
			if (atlasAssetPath.ContainsKey (atlasId)) {
				return atlasAssetPath [atlasId];
			}
			return string.Empty;
		}
		/// <summary>
		/// Gets the atlas rects.
		/// </summary>
		/// <returns>The atlas rects.</returns>
		/// <param name="atlasId">Atlas identifier.</param>
		public Rect[] GetAtlasRects (string atlasId = "atlas") {
			if (atlasesRects.ContainsKey (atlasId)) {
				return atlasesRects [atlasId];
			}
			return new Rect[0];
		}
		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear () {
			#if UNITY_EDITOR
			var enumerator = textures.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				string assetPath = AssetDatabase.GetAssetPath (enumerator.Current.Value);
				if (string.IsNullOrEmpty (assetPath)) {
					UnityEngine.Object.DestroyImmediate (enumerator.Current.Value, false);
				}
			}
			#endif
			textures.Clear ();
			textureToSettings.Clear ();
			atlases.Clear ();
			atlasToTextureSettings.Clear ();
			textureToAtlases.Clear ();
			atlasesRects.Clear ();
			textureAssetPath.Clear ();
			atlasAssetPath.Clear ();
			for (int i = 0; i < instantiatedTextures.Count; i++) {
				Object.DestroyImmediate (instantiatedTextures[i]);
			}
			instantiatedTextures.Clear ();
			texturesForAtlas.Clear ();
		}
		#endregion

		#region Texture IO
		/// <summary>
		/// Gets or creates a folder.
		/// </summary>
		/// <returns>The or create folder.</returns>
		/// <param name="parentFolder">Parent folder.</param>
		/// <param name="newFolderName">New folder name.</param>
		public string GetOrCreateFolder (string parentFolder, string newFolderName) {
			lastFolder = "";
			#if UNITY_EDITOR
			if (!AssetDatabase.IsValidFolder (parentFolder)) {
				throw new UnityException ("Broccoli Tree Creator: Parent folder does not exist (" + parentFolder + ").");
			} else if (AssetDatabase.IsValidFolder (parentFolder + "/" + newFolderName)) {
				lastFolder = parentFolder + "/" + newFolderName;
				return lastFolder;
			}
			AssetDatabase.CreateFolder (parentFolder, newFolderName);
			#endif
			lastFolder = parentFolder + "/" + newFolderName;
			return lastFolder;
		}
		/// <summary>
		/// Saves a texture to a file.
		/// </summary>
		/// <param name="texture">Texture.</param>
		/// <param name="filename">Filename.</param>
		public void SaveTextureToFile (Texture2D texture, string filename) {
			#if UNITY_EDITOR
			System.IO.File.WriteAllBytes (filename, texture.EncodeToPNG());
			AssetDatabase.ImportAsset (filename);
			#endif
		}
		/// <summary>
		/// Saves textures to assets.
		/// </summary>
		/// <param name="includeAtlases">If set to <c>true</c> include atlases.</param>
		/// <param name="folderPath">Folder path.</param>
		public void SaveTexturesToAssets (bool includeAtlases = false, string folderPath = "") {
			#if UNITY_EDITOR
			if (string.IsNullOrEmpty (folderPath))
				folderPath = lastFolder;
			if (!AssetDatabase.IsValidFolder(folderPath))
				throw new UnityException ("Broccoli Tree Creator: Path specified to create the mipmap file is not valid (" + 
					folderPath + ").");
			var enumerator = textures.GetEnumerator ();
			string textureId;
			while (enumerator.MoveNext ()) {
				textureId = enumerator.Current.Key;
				if (!textureToAtlases.ContainsKey (textureId)) {
					string texturePath = folderPath + "/" + textureId + ".png";
					SaveTextureToFile (textures[textureId], texturePath);
					textureAssetPath.Add (textureId, texturePath);
					if (textureToSettings.ContainsKey (textureId)) {
						TextureSettings settings = textureToSettings[textureId];
						TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath (texturePath);
						if (settings.exportToNormal)
							importer.textureType = TextureImporterType.NormalMap;
						if (settings.alphaIsTransparency)
							importer.alphaIsTransparency = true;
						importer.SaveAndReimport ();
					}
				}
			}
			if (includeAtlases) {
				SaveAtlasesToAssets (folderPath);
			}
			#endif
		}
		/// <summary>
		/// Saves atlases to assets.
		/// </summary>
		/// <param name="folderPath">Folder path.</param>
		/// <param name="maximumAtlasSize">Maximum atlas size.</param>
		public void SaveAtlasesToAssets (string folderPath = "", int maximumAtlasSize = 512) {
			#if UNITY_EDITOR
			if (string.IsNullOrEmpty (folderPath))
				folderPath = lastFolder;
			if (!AssetDatabase.IsValidFolder(folderPath))
				throw new UnityException ("Broccoli Tree Creator: Path specified to create the mipmap file is not valid (" + 
					folderPath + ").");
			for (int i = 0; i < atlases.Count; i++) {
				texturesForAtlas.Clear ();
				var enumerator = textureToAtlases.GetEnumerator ();
				string textureId;
				while (enumerator.MoveNext ()) {
					textureId = enumerator.Current.Key;
					if (textureToAtlases [textureId] == i) {
						texturesForAtlas.Add (textures [textureId]);
					}
				}
				if (texturesForAtlas.Count > 0) {
					Texture2D atlas = new Texture2D (2, 2);
					//Rect[] rects = atlas.PackTextures (texturesForAtlas.ToArray (), 2, maximumAtlasSize, false);
					TexturePacker.TexturePackParams texPackParams = 
						new TexturePacker.TexturePackParams (2, TexturePacker.GetPixelSize (maximumAtlasSize), Color.clear, false);
					if (atlasToTextureSettings.ContainsKey (atlases[i])) {
						TextureSettings texSettings = atlasToTextureSettings[atlases[i]];
						texPackParams.bgColor = texSettings.bgColor;
						texPackParams.applyGammaCorrection = texSettings.applyGammaCorrection;
					}
					Rect[] rects = TexturePacker.PackTextures (out atlas, texturesForAtlas.ToArray (), TexturePacker.PackMode.ScaleEven, texPackParams);
					atlasesRects.Add (atlases[i], rects);
					atlas.alphaIsTransparency = true;
					string texturePath = folderPath + "/" + atlases [i] + ".png";
					SaveTextureToFile (atlas, texturePath);
					atlasAssetPath.Add (atlases [i], texturePath);
					if (atlasToTextureSettings.ContainsKey (atlases[i])) {
						TextureSettings settings = atlasToTextureSettings[atlases[i]];
						TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath (texturePath);
						if (settings.exportToNormal)
							importer.textureType = TextureImporterType.NormalMap;
						if (settings.alphaIsTransparency)
							importer.alphaIsTransparency = true;
						importer.SaveAndReimport ();
					}
					instantiatedTextures.Add (atlas);
				}
				texturesForAtlas.Clear ();
			}
			#endif
		}
		#endregion
	}
}