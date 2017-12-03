﻿#if UNITY_5_6_OR_NEWER

/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2016
 *	
 *	"RememberVideoPlayer.cs"
 * 
 *	This script is attached to VideoPlayer objects
 *	whose playback state we wish to save.
 * 
 */

using UnityEngine;
using System.Collections;
using UnityEngine.Video;

namespace AC
{
	
	/**
	 * Attach this to GameObjects whose VideoPlayer's playback state you wish to save.
	 * (Compatibly with Unity 5.6 and later only)
	 */
	[AddComponentMenu("Adventure Creator/Save system/Remember Video Player")]
	#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
	[HelpURL("http://www.adventurecreator.org/scripting-guide/class_a_c_1_1_remember_video_player.html")]
	#endif
	public class RememberVideoPlayer : Remember
	{

		private double loadTime;
		private bool playAfterLoad;


		/**
		 * <summary>Serialises appropriate GameObject values into a string.</summary>
		 * <returns>The data, serialised as a string</returns>
		 */
		public override string SaveData ()
		{
			VideoPlayerData videoPlayerData = new VideoPlayerData ();
			videoPlayerData.objectID = constantID;
			videoPlayerData.savePrevented = savePrevented;

			if (GetComponent <VideoPlayer>())
			{
				VideoPlayer videoPlayer = GetComponent <VideoPlayer>();
				videoPlayerData.isPlaying = videoPlayer.isPlaying;
				videoPlayerData.currentFrame = videoPlayer.frame;
				videoPlayerData.currentTime = videoPlayer.time;
			}

			return Serializer.SaveScriptData <VideoPlayerData> (videoPlayerData);
		}
		

		/**
		 * <summary>Deserialises a string of data, and restores the GameObject to its previous state.</summary>
		 * <param name = "stringData">The data, serialised as a string</param>
		 */
		public override void LoadData (string stringData)
		{
			VideoPlayerData data = Serializer.LoadScriptData <VideoPlayerData> (stringData);
			if (data == null)
			{
				return;
			}
			SavePrevented = data.savePrevented; if (savePrevented) return;

			if (GetComponent <VideoPlayer>())
			{
				VideoPlayer videoPlayer = GetComponent <VideoPlayer>();

				//videoPlayer.frame = data.currentFrame;
				videoPlayer.time = data.currentTime;

				if (data.isPlaying)
				{
					if (!videoPlayer.isPrepared)
					{
						loadTime = data.currentTime;
						playAfterLoad = true;
						videoPlayer.prepareCompleted += OnPrepareVideo;
						videoPlayer.Prepare ();
					}
					else
					{
						videoPlayer.Play ();
					}
				}
				else
				{
					if (data.currentTime > 0f)
					{
						if (!videoPlayer.isPrepared)
						{
							loadTime = data.currentTime;
							playAfterLoad = false;
							videoPlayer.prepareCompleted += OnPrepareVideo;
							videoPlayer.Prepare ();
						}
						else
						{
							videoPlayer.Pause ();
						}
					}
					else
					{
						videoPlayer.Stop ();
					}
				}
			}
		}


		private void OnPrepareVideo (VideoPlayer videoPlayer)
		{
			videoPlayer.time = loadTime;
			if (playAfterLoad)
			{
				videoPlayer.Play ();
			}
			else
			{
				videoPlayer.Pause ();
			}
		}
		
	}


	/**
	 * A data container used by the RememberVisibility script.
	 */
	[System.Serializable]
	public class VideoPlayerData : RememberData
	{

		/* True if the video is currently playing */
		public bool isPlaying;
		/* The current frame number */
		public long currentFrame;

		public double currentTime;


		/**
		 * The default Constructor.
		 */
		public VideoPlayerData () { }

	}

}

#endif