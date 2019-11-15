using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class AddressableScript : MonoBehaviour
{

		
	private List<IResourceLocation> remoteNums;
	public AssetLabelReference number;


    void Start()
    {

		Addressables.LoadResourceLocationsAsync(number.labelString).Completed += LocationLoaded;

	}

	


	private void LocationLoaded(AsyncOperationHandle<IList<IResourceLocation>> obj)
	{
		remoteNums = new List<IResourceLocation>(obj.Result);

		if(remoteNums.Count > 0)
		{
			Debug.Log("Load Asset Successful."); 
		}

		Addressables.InstantiateAsync(remoteNums[0], Vector3.zero, Quaternion.identity);

		Addressables.InstantiateAsync(remoteNums[1], Vector3.zero, Quaternion.identity);

		Addressables.InstantiateAsync(remoteNums[3], Vector3.zero, Quaternion.identity);

		StartCoroutine(SpawnNumbers());
	}

	

	private IEnumerator SpawnNumbers()
	{
		yield return new WaitForSeconds(1f);
		float xOff = -4.0f;

		for(int i = 0; i< remoteNums.Count; i++)
		{
			Vector3 spawnPosition = new Vector3(xOff, 3, 0);

			Addressables.InstantiateAsync(remoteNums[i], spawnPosition, Quaternion.identity);

			xOff = xOff + 3.5f;

			yield return new WaitForSeconds(1f);
		}
	}

}
