using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.IO;
using UnityEngine.Networking;


[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
	//Group
	//public List<AddressableAssetGroup> groups;
	

	//public AssetReference assetRefMember;
	public AssetLabelReference assetLabel;

	public Image image;

	public AudioSource audioSource;
	private List<IResourceLocation> remoteResList;

	private List<IResourceLocation> spriteNums;
	public AssetLabelReference spritenumber;


	private List<IResourceLocation> davidAudio;
	public AssetLabelReference david;

	// Start is called before the first frame update
	void Start()
    {
		Debug.Log(Application.persistentDataPath);

		bool cachedFileLocation = File.Exists(Application.persistentDataPath);

		//audioSource = GetComponent<AudioSource>();

		//image = GetComponent<Image>();

		//Debug.Log(assetRefMember.SubObjectName);

		//Addressables.LoadResourceLocationsAsync(assetLabel.labelString).Completed += OnDownloadComplete;

		Addressables.LoadResourceLocationsAsync(spritenumber.labelString).Completed += LocationLoaded;

		Addressables.LoadResourceLocationsAsync(david.labelString).Completed += DavidLocationLoaded;


	}

	private void LocationLoaded(AsyncOperationHandle<IList<IResourceLocation>> obj)
	{
		spriteNums = new List<IResourceLocation>(obj.Result);

		if (spriteNums.Count > 0)
		{
			Debug.Log("Load Asset Successful.");
		}

		StartCoroutine(SpawnNumbers());
	}

	private void OnDownloadComplete(AsyncOperationHandle<IList<IResourceLocation>> obj)
	{
		remoteResList = new List<IResourceLocation>(obj.Result);

		Debug.Log(remoteResList[0]);
		//Debug.Log(remoteResList[1]);

		Addressables.LoadAssetAsync<AudioClip>(remoteResList[0]).Completed += (loadedAsset) =>
		{
			audioSource.clip = loadedAsset.Result;
			audioSource.Play();
		};

		//Addressables.LoadAssetAsync<Sprite>(remoteResList[1]).Completed += (loadedAsset) =>
		//{
		//	image.sprite = loadedAsset.Result;
			
		//};

	}


	private IEnumerator SpawnNumbers()
	{
		yield return new WaitForSeconds(1f);
		float xOff = -4.0f;

		for (int i = 0; i < spriteNums.Count; i++)
		{
			Vector3 spawnPosition = new Vector3(xOff, 3, 0);

			//Addressables.InstantiateAsync(spriteNums[i], spawnPosition, Quaternion.identity);

			Addressables.LoadAssetAsync<Sprite>(spriteNums[i]).Completed += (loadedAsset) =>
			{
				image.sprite = loadedAsset.Result;
				
			};

			xOff = xOff + 3.5f;

			yield return new WaitForSeconds(1f);
		}
	}

	// Testing
	// ###################################################

	private void DavidLocationLoaded(AsyncOperationHandle<IList<IResourceLocation>> obj)
	{
		davidAudio = new List<IResourceLocation>(obj.Result);

		if (davidAudio.Count > 0)
		{
			Debug.Log("Load Asset Successful.");
		}

		for(int i =0; i< davidAudio.Count; i++)
		{
			Debug.Log(davidAudio[i].ToString());
		}

		StartCoroutine(SpawnAudio());
	}

	private IEnumerator SpawnAudio()
	{
		yield return new WaitForSeconds(1f);
		float xOff = -4.0f;

		for (int i = 0; i < davidAudio.Count; i++)
		{
			Vector3 spawnPosition = new Vector3(xOff, 3, 0);

			//Addressables.InstantiateAsync(spriteNums[i], spawnPosition, Quaternion.identity);

			Addressables.LoadAssetAsync<AudioClip>(davidAudio[i]).Completed += (loadedAsset) =>
			{
				audioSource.clip = loadedAsset.Result;
				audioSource.Play();

			};

			xOff = xOff + 3.5f;

			yield return new WaitForSeconds(1f);
		}
	}


	//########### Cache Testing
	
}


