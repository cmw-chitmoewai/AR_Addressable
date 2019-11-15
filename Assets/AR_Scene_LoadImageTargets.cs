using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


public class AR_Scene_LoadImageTargets : MonoBehaviour
{

	private List<IResourceLocation> imageTargets;
	public AssetLabelReference label;

	public void Start()
	{
		
	}

	public void Update()
	{
		
	}

	public void DownloadModel()
	{
		Addressables.LoadResourceLocationsAsync(label.labelString).Completed += OnDownload_ImageTargets;
	}

	private void OnDownload_ImageTargets(AsyncOperationHandle<IList<IResourceLocation>> obj)
	{

		Vector3 spawnPosition = new Vector3(0,0, 0);

		imageTargets = new List<IResourceLocation>(obj.Result);

		Addressables.InstantiateAsync(imageTargets[0] , spawnPosition, Quaternion.identity);

		//StartCoroutine(SpawnImageTargets());

		//Addressables.LoadAssetAsync<GameObject>(imageTargets[0]).Completed += (loadedAsset) =>
		//{
		//	GameObject targets = loadedAsset.Result;
		//	Addressables.InstantiateAsync(targets);
		//};

	}

}
