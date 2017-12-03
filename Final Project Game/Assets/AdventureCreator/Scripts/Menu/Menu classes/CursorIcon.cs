﻿/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2016
 *	
 *	"CursorIcon.cs"
 * 
 *	This script is a data class for cursor icons.
 * 
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
	
	/**
	 * A data container for interaction cursor icons.
	 * These are stored in CursorManager.
	 */
	[System.Serializable]
	public class CursorIcon : CursorIconBase
	{
		
		/** If True, then the cursor will be left out of the cycle when right-clicking (only applies if SettingsManager's interactionMethod is ChooseInteractionThenHotspot) */
		public bool dontCycle = false;
		/** The display name of the icon */
		public string label;
		/** The translation ID, as used by SpeechManager */
		public int lineID = -1;
		/** A unique identifier */
		public int id;


		/**
		 * The default Constructor.
		 */
		public CursorIcon ()
		{
			dontCycle = false;
			texture = null;
			id = 0;
			lineID = -1;
			isAnimated = false;
			numFrames = 1;
			size = 0.04f;
			frameSpeeds = new float[0];
			
			label = "Icon " + (id + 1).ToString ();
		}
		
		
		/**
		 * <summary>A Constructor that generates a unique id number.</summary>
		 * <param name = "idArray">An array of previously-assigned id numbers</param>
		 */
		public CursorIcon (int[] idArray)
		{
			dontCycle = false;
			texture = null;
			id = 0;
			lineID = -1;
			isAnimated = false;
			numFrames = 1;
			frameSpeeds = new float[0];
			
			// Update id based on array
			foreach (int _id in idArray)
			{
				if (id == _id)
				{
					id ++;
				}
			}
			
			label = "Icon " + (id + 1).ToString ();
		}
		
		
		/**
		 * <summary>Gets the name of the expected input button that is used to quick-select the cursor (only applies if SettingsManager's interactionMethod is ChooseInteractionThenHotspot).</summary>
		 * <returns>The name of the expected input button, which should be defined in Unity's Input Manager</returns>
		 */
		public string GetButtonName ()
		{
			if (label != "")
			{
				return "Icon_" + label.Replace (" ", "");
			}
			return "Icon_" + id.ToString ();
		}
		
		
		/**
		 * <summary>Copies the values from another CursorIcon.</summary>
		 * <param name = "_cursorIcon">The CursorIcon to copy from</param>
		 */
		public void Copy (CursorIcon _cursorIcon)
		{
			label = _cursorIcon.label;
			lineID = _cursorIcon.lineID;
			id = _cursorIcon.id;
			dontCycle = _cursorIcon.dontCycle;
			
			base.Copy (_cursorIcon);
		}
		
	}
	
	
	/**
	 * A data container for all cursor icons, as well as animated textures used by MenuGraphic.
	 */
	[System.Serializable]
	public class CursorIconBase
	{
		
		/** The texture to use */
		public Texture texture;
		/** If True, then the texture will be considered to consist of multiple animation frames, and they will be displayed sequentially */
		public bool isAnimated = false;
		/** The number of frames in the texture, if animated */
		public int numFrames = 1;
		/** The number of rows in the texture, if animated */
		public int numRows = 1;
		/** The number of columns in the texture, if animated */
		public int numCols = 1;
		/** The size of the icon */
		public float size = 0.015f;
		/** The animation playback speed, if animated */
		public float animSpeed = 4f;
		/** If True, then animations will end on the final frame, rather than looping */
		public bool endAnimOnLastFrame = false;
		/** If True, and isAnimated = True, then the first frame will be skipped when in a looping animation */
		public bool skipFirstFrameWhenLooping = false;
		/** The offset of the "click point", when used as a cursor */
		public Vector2 clickOffset;
		/** An array of the speeds for each frame when animating, where the index of the array corresponds to the frame of the animation */
		public float[] frameSpeeds;
		
		private string uniqueIdentifier;
		private float frameIndex = 0f;
		private float frameWidth = -1f;
		private float frameHeight = -1;
		private UnityEngine.Sprite[] sprites;
		private Texture2D[] textures;
		private Texture2D texture2D;
		private Rect firstFrameRect = new Rect ();


		/**
		 * The default Constructor.
		 */
		public CursorIconBase ()
		{
			texture = null;
			isAnimated = false;
			frameSpeeds = new float[0];
			numFrames = numRows = numCols = 1;
			size = 0.015f;
			frameIndex = 0f;
			frameWidth = frameHeight = -1f;
			animSpeed = 4;
			endAnimOnLastFrame = false;
			skipFirstFrameWhenLooping = false;
			clickOffset = Vector2.zero;
		}
		
		
		/**
		 * <summary>Copies the values from another CursorIconBase.</summary>
		 * <param name = "_icon">The CursorIconBase to copy from</param>
		 */
		public void Copy (CursorIconBase _icon)
		{
			texture = _icon.texture;
			isAnimated = _icon.isAnimated;
			numFrames = _icon.numFrames;
			animSpeed = _icon.animSpeed;
			endAnimOnLastFrame = _icon.endAnimOnLastFrame;
			skipFirstFrameWhenLooping = _icon.skipFirstFrameWhenLooping;
			clickOffset = _icon.clickOffset;
			numRows = _icon.numRows;
			numCols = _icon.numCols;
			size = _icon.size;

			frameSpeeds = new float[0];
			if (_icon.frameSpeeds != null)
			{
				frameSpeeds = new float[_icon.frameSpeeds.Length];
				for (int i=0; i<frameSpeeds.Length; i++)
				{
					frameSpeeds[i] = _icon.frameSpeeds[i];
				}
			}

			Reset ();
		}
		
		
		/**
		 * <summary>Draws the graphic as part of a Menu, for when it's displayed within a MenuGraphic or MenuInteraction.</summary>
		 * <param name = "_rect">The dimensions to draw the graphic</param>
		 * <param name = "isActive">If True, then the associated MenuElement is active (e.g. the mouse is hovering over it)</param>
		 */
		public void DrawAsInteraction (Rect _rect, bool isActive)
		{
			if (texture == null)
			{
				return;
			}
			
			if (isAnimated && numFrames > 0)
			{
				if (Application.isPlaying)
				{
					if (isActive)
					{
						GUI.DrawTextureWithTexCoords (_rect, texture, GetAnimatedRect ());
					}
					else
					{
						GUI.DrawTextureWithTexCoords (_rect, texture, firstFrameRect);
						frameIndex = 0f;
					}
				}
				else
				{
					Reset ();
					GUI.DrawTextureWithTexCoords (_rect, texture, firstFrameRect);
					frameIndex = 0f;
				}
			}
			else
			{
				GUI.DrawTexture (_rect, texture, ScaleMode.StretchToFill, true, 0f);
			}
		}
		
		
		/**
		 * <summary>Gets a Sprite based on the texture supplied.
		 * The Sprite will be generated on the fly if it does not already exist.</summary>
		 * <returns>The generated Sprite</returns>
		 */
		public UnityEngine.Sprite GetSprite ()
		{
			if (Texture2D == null)
			{
				return null;
			}
			
			if (sprites == null || sprites.Length < 1)
			{
				sprites = new UnityEngine.Sprite[1];
			}
			if (sprites != null && sprites.Length > 0 && sprites[0] == null)
			{
				sprites[0] = UnityEngine.Sprite.Create (texture2D, new Rect (0f, 0f, texture2D.width, texture2D.height), new Vector2 (0.5f, 0.5f));
			}
			return sprites[0];
		}
		
		
		/**
		 * <summary>Gets a Sprite based on the animated texture supplied.
		 * The Sprite will be generated on the fly if it does not already exist.</summary>
		 * <param name = "_frameIndex">The texture frame to convert</param>
		 * <returns>The generated Sprite</returns>
		 */
		public UnityEngine.Sprite GetAnimatedSprite (int _frameIndex)
		{
			if (Texture2D == null)
			{
				return null;
			}
			
			int frameInRow = _frameIndex + 1;
			int currentRow = 1;
			while (frameInRow > numCols)
			{
				frameInRow -= numCols;
				currentRow ++;
			}
			
			if (_frameIndex >= numFrames)
			{
				frameInRow = 1;
				currentRow = 1;
			}
			
			if (sprites == null || sprites.Length <= _frameIndex)
			{
				sprites = new UnityEngine.Sprite[_frameIndex+1];
			}
			if (sprites[_frameIndex] == null)
			{
				Rect _rect = new Rect (frameWidth * (frameInRow-1) * texture.width, frameHeight * (numRows - currentRow) * texture.height, frameWidth * texture.width, frameHeight * texture.height);
				sprites[_frameIndex] = UnityEngine.Sprite.Create (texture2D, _rect, new Vector2 (0.5f, 0.5f));
			}
			
			return sprites[_frameIndex];
		}
		
		
		/**
		 * <summary>Gets a Sprite based on the animated texture supplied, when the texture is used within a MenuElement (MenuGraphic or MenuInteraction).
		 * The Sprite will be generated on the fly if it does not already exist.</summary>
		 * <param name = "isActive">If True, then the associated MenuElement is active (e.g. the mouse is hovering over it)</param>
		 * <returns>The generated Sprite</returns>
		 */
		public UnityEngine.Sprite GetAnimatedSprite (bool isActive)
		{
			if (Texture2D == null)
			{
				return null;
			}
			
			if (isAnimated && numFrames > 0)
			{
				if (Application.isPlaying)
				{
					if (sprites == null)
					{
						sprites = new UnityEngine.Sprite[numFrames];
					}
					
					if (isActive)
					{
						int i = Mathf.FloorToInt (frameIndex);
						if (sprites[i] == null)
						{
							Rect animatedRect = GetAnimatedRect ();
							animatedRect = new Rect (animatedRect.x * texture.width, animatedRect.y * texture.height, animatedRect.width * texture.width, animatedRect.height * texture.height);
							sprites[i] = UnityEngine.Sprite.Create (texture2D, animatedRect, new Vector2 (0.5f, 0.5f));
						}
						return sprites[i];
					}
					else
					{
						frameIndex = 0f;
						if (sprites[0] == null)
						{
							sprites[0] = UnityEngine.Sprite.Create (texture2D, firstFrameRect, new Vector2 (0.5f, 0.5f));
						}
						return sprites[0];
					}
				}
			}
			else
			{
				if (sprites == null || sprites.Length < 1)
				{
					sprites = new UnityEngine.Sprite[1];
				}
				if (sprites != null && sprites.Length > 0 && sprites[0] == null)
				{
					sprites[0] = UnityEngine.Sprite.Create (texture2D, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
				}
				return sprites[0];
			}
			return null;
		}
		
		
		/**
		 * Clears the generated sprite.
		 */
		public void ClearSprites ()
		{
			sprites = null;
		}
		
		
		/**
		 * <summary>Gets a slice of the texture that represents the current frame, if the texture consists of animated frames.</summary>
		 * <returns>A frame of animation</returns>
		 */
		public Texture2D GetAnimatedTexture (bool canAnimate = true)
		{
			if (Texture2D == null)
			{
				return null;
			}
			
			if (isAnimated)
			{
				Rect animatedRect = GetAnimatedRect ((canAnimate) ? -1 : 0);
				int x = Mathf.FloorToInt (animatedRect.x * Texture2D.width);
				int y = Mathf.FloorToInt (animatedRect.y * Texture2D.height);
				int width = Mathf.FloorToInt (animatedRect.width * Texture2D.width);
				int height = Mathf.FloorToInt (animatedRect.height * Texture2D.height);

				if (animatedRect.width >= 0f && animatedRect.height >= 0f)
				{
					try
					{
						Color[] pix = Texture2D.GetPixels (x, y, width, height);
						Texture2D frameTex = new Texture2D ((int) (frameWidth * Texture2D.width), (int) (frameHeight * Texture2D.height));
						frameTex.SetPixels (pix);
						frameTex.filterMode = texture.filterMode;
						frameTex.Apply ();
						return frameTex;
					}
					catch
					{
						ACDebug.LogWarning ("Cannot read texture '" + texture.name + "' - it may need to be set to type 'Advanced' and have 'Read/Write Enabled' checked.");
					}
				}
			}
			return texture2D;
		}
		
		
		/**
		 * <summary>Gets a unique identifier, based on the texture and current animation frame.</summary>
		 * <returns>A unique identifier for the instance</returns>
		 */
		public string GetName ()
		{
			return uniqueIdentifier;
		}


		/**
		 * <summary>Clears the animated Texture2D and Sprite caches.</summary>
		 */
		public void ClearCache ()
		{
			textures = null;
			texture2D = null;
			sprites = null;
		}
		
		
		/**
		 * <summary>Draws the texture as a cursor icon</summary>
		 * <param name = "centre">The cursor's centre position on-screen</param>
		 * <param name = "canAnimate">If True, and isAnimated = True, the texture can be animated. If False, the texture will never animate</param>
		 * <returns>The texture displayed, which may be only a portion of the whole texture if animated</returns>
		 */
		public Texture Draw (Vector2 centre, bool canAnimate = true)
		{
			/*if (Texture2D == null)
			{
				return null;
			}*/
			
			float _size = size;
			
			if (KickStarter.cursorManager.cursorRendering == CursorRendering.Hardware)
			{
				_size = (float) ((float) texture.width / (float) Screen.width);
			}
			Rect _rect = AdvGame.GUIBox (centre, _size);
			
			_rect.x -= clickOffset.x * _rect.width;
			_rect.y -= clickOffset.y * _rect.height;
			
			if (isAnimated && numFrames > 0 && Application.isPlaying)
			{
				if (!canAnimate)
				{
					frameIndex = 0f;
				}
				else if (skipFirstFrameWhenLooping && frameIndex < 1f)
				{
					frameIndex = 1f;
				}

				GetAnimatedRect ();
				if (textures == null)
				{
					textures = new Texture2D[numFrames];
				}
				int i = Mathf.FloorToInt (frameIndex);
				if (textures.Length < i+1)
				{
					textures = new Texture2D[i+1];
				}
				if (textures[i] == null)
				{
					if (Texture2D == null)
					{
						ACDebug.LogWarning ("Cannot animate texture " + texture + " as it is not a Texture2D.");
					}
					else
					{
						textures[i] = GetAnimatedTexture (canAnimate);
					}
				}
				GUI.DrawTexture (_rect, textures[i]);
				return textures[i];
			}
			else
			{
				GUI.DrawTexture (_rect, texture, ScaleMode.ScaleToFit, true, 0f);
				return texture;
			}
		}
		
		
		/**
		 * <summary>Gets a Rect that describes a slice of the animated texture that represents the current frame.</summary>
		 * <returns>A Rect that describes a slice of the animated texture that represents the current frame</returns>
		 */
		public Rect GetAnimatedRect ()
		{
			int currentRow = 1;
			int frameInRow = 1;

			if (frameIndex < 0f)
			{
				frameIndex = 0f;
			}
			else if (frameIndex < numFrames)
			{
				if (endAnimOnLastFrame && frameIndex >= (numFrames -1))
				{}
				else
				{
					int i = Mathf.FloorToInt (frameIndex);
					float frameSpeed = (frameSpeeds != null && i < frameSpeeds.Length) ? frameSpeeds[i] : 1f;

					float deltaTime = (Time.deltaTime == 0f) ? 0.02f : Time.deltaTime;
					frameIndex += deltaTime * animSpeed * frameSpeed;
				}
			}

			frameInRow = Mathf.FloorToInt (frameIndex)+1;
			while (frameInRow > numCols)
			{
				frameInRow -= numCols;
				currentRow ++;
			}
			
			if (frameIndex >= numFrames)
			{
				if (endAnimOnLastFrame)
				{
					frameIndex = numFrames - 1;
					frameInRow -= 1;
				}
				else
				{
					if (skipFirstFrameWhenLooping && numFrames > 1f)
					{
						frameIndex = 1f;
						frameInRow = 2;
					}
					else
					{
						frameIndex = 0f;
						frameInRow = 1;
					}

					currentRow = 1;
				}
			}
			
			if (texture != null)
			{
				uniqueIdentifier = texture.name + frameInRow.ToString () + currentRow.ToString ();
			}
			return new Rect (frameWidth * (frameInRow-1), frameHeight * (numRows - currentRow), frameWidth, frameHeight);
		}
		
		
		/**
		 * <summary>Gets a Rect that describes a slice of the animated texture that represents a specific frame.</summary>
		 * <param name = "_frameIndex">The frame in question</param>
		 * <returns>A Rect that describes a slice of the animated texture that represents a specific frame</returns>
		 */
		public Rect GetAnimatedRect (int _frameIndex)
		{
			if (_frameIndex < 0)
			{
				return GetAnimatedRect ();
			}

			int frameInRow = _frameIndex + 1;
			int currentRow = 1;
			while (frameInRow > numCols)
			{
				frameInRow -= numCols;
				currentRow ++;
			}
			
			if (_frameIndex >= numFrames)
			{
				frameInRow = 1;
				currentRow = 1;
			}
			
			return new Rect (frameWidth * (frameInRow-1), frameHeight * (numRows - currentRow), frameWidth, frameHeight);
		}
		
		
		/**
		 * Resets the animation, if the texture is animated.
		 */
		public void Reset ()
		{
			if (isAnimated)
			{
				if (numFrames > 0)
				{
					frameWidth = 1f / numCols;
					frameHeight = 1f / numRows;
					frameIndex = 0f;
				}
				else
				{
					ACDebug.LogWarning ("Cannot have an animated cursor with less than one frame!");
				}
				
				if (animSpeed < 0)
				{
					animSpeed = 0;
				}

				firstFrameRect = new Rect (0f, 1f - frameHeight, frameWidth, frameHeight);
			}
		}
		
		
		#if UNITY_EDITOR

		private bool showExtra;

		public void ShowGUI (bool includeSize, string _label = "Texture:", CursorRendering cursorRendering = CursorRendering.Software, string apiPrefix = "")
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (_label, GUILayout.Width (145));
			texture = (Texture) CustomGUILayout.ObjectField <Texture> (texture, false, GUILayout.Width (70), GUILayout.Height (70), apiPrefix + ".texture");
			EditorGUILayout.EndHorizontal ();

			if (texture == null) return;

			if (includeSize)
			{
				if (cursorRendering == CursorRendering.Software)
				{
					size = CustomGUILayout.FloatField ("Size:", size, apiPrefix + ".size");
				}

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Click offset (from " + ((cursorRendering == CursorRendering.Software) ? "centre):" : "top left):"), GUILayout.Width (150f));
				clickOffset = CustomGUILayout.Vector2Field ("", clickOffset, apiPrefix + ".clickOffset");
				EditorGUILayout.EndHorizontal ();
			}
			
			isAnimated = CustomGUILayout.Toggle ("Animate?", isAnimated, apiPrefix + ".isAnimated");
			if (isAnimated)
			{
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Frames:", GUILayout.Width (50f));
				numFrames = CustomGUILayout.IntField (numFrames, GUILayout.Width (70f), apiPrefix + ".numFrames");
				EditorGUILayout.LabelField ("Rows:", GUILayout.Width (50f));
				numRows = CustomGUILayout.IntField (numRows, GUILayout.Width (70f),apiPrefix + ".numRows");
				EditorGUILayout.LabelField ("Columns:", GUILayout.Width (50f));
				numCols = CustomGUILayout.IntField (numCols, GUILayout.Width (70f), apiPrefix + ".numCols");
				EditorGUILayout.EndHorizontal ();
				
				animSpeed = CustomGUILayout.FloatField ("Animation speed:", animSpeed, apiPrefix + ".animSpeed");

				showExtra = EditorGUILayout.Foldout (showExtra, "Additional settings:");
				if (showExtra)
				{
					EditorGUILayout.BeginVertical (CustomStyles.thinBox);
					endAnimOnLastFrame = CustomGUILayout.ToggleLeft ("End on last frame?", endAnimOnLastFrame, apiPrefix + ".endAnimOnLastFrame");
					skipFirstFrameWhenLooping = CustomGUILayout.ToggleLeft ("Skip first when animating?", skipFirstFrameWhenLooping, apiPrefix + ".skipFirstFrameWhenLooping");

					SyncFrameSpeeds ();
					for (int i=0; i<numFrames; i++)
					{
						if (i == 0 && skipFirstFrameWhenLooping) continue;
						if (i == (numFrames-1) && endAnimOnLastFrame) continue;

						frameSpeeds[i] = EditorGUILayout.Slider ("Frame #" + (i+1).ToString () + " relative speed:", frameSpeeds[i], 0.01f, 1f);
					}
					EditorGUILayout.EndVertical ();
				}
			}
		}
		
		#endif


		private Texture2D Texture2D
		{
			get
			{
				if (texture2D == null && texture != null && texture is Texture2D)
				{
					texture2D = (Texture2D) texture;
				}
				return texture2D;
			}
		}


		private void SyncFrameSpeeds ()
		{
			if (frameSpeeds == null) frameSpeeds = new float[0];

			if (frameSpeeds.Length != numFrames)
			{
				float[] backup = new float[frameSpeeds.Length];
				for (int i=0; i<frameSpeeds.Length; i++)
				{
					backup[i] = frameSpeeds[i];
				}

				frameSpeeds = new float[numFrames];
				for (int i=0; i<numFrames; i++)
				{
					if (i < backup.Length)
					{
						frameSpeeds[i] = backup[i];
					}
					else
					{
						frameSpeeds[i] = 1f;
					}
				}
			}
		}
		
	}
	
	
	/**
	 * A data container for labels that might surround a Hotspot, but aren't CursorIcons (e.g. "Use X on Y", "Give X to Y")
	 */
	[System.Serializable]
	public class HotspotPrefix
	{
		
		/** The display text */
		public string label;
		/** The translation ID, as used by SpeechManager */
		public int lineID;
		
		
		/**
		 * The default Constructor.
		 */
		public HotspotPrefix (string text)
		{
			label = text;
			lineID = -1;
		}
		
	}
	
}