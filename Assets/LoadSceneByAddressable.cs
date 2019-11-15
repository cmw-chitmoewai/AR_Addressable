using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;

public class LoadSceneByAddressable : MonoBehaviour
{
	//for internet
	public GameObject infoBox;

	public Text DownloadScene_Size1;
	public Text DownloadScene_Size2;
	public GameObject loadingPanel;
	public Slider ProgressSlider;
	public Text ProgressNumber;

	public AsyncOperationHandle<long> downloadSize;

	private List<IResourceLocation> AR_scene;
	public AssetLabelReference AR_Label;

	//Matching
	//private List<IResourceLocation> matching_scene;
	public AssetLabelReference Matching_Label;

	public AssetLabelReference MC_Label;

	public void Start()
	{
		InvokeRepeating("CheckInternet", 0, 1);
		loadingPanel.SetActive(false);
	}

	public void Update()
	{
		
	}

	// Check the internet connection 
	void CheckInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			infoBox.SetActive(true);
			infoBox.transform.GetChild(0).GetChild(0).GetComponent<Text>().text =
				"Connection Problem! \nPlease check your internet connection";

			Debug.Log("Internet not reachable.");
		}
		else
		{
			infoBox.SetActive(false);
			Debug.Log("Internet Available.");
		}
	}

	public void Load_ARScene()
	{
		Addressables.LoadResourceLocationsAsync(AR_Label.labelString).Completed += OnDownloadAR_scene;
	}

	public void Load_MatchingScene()
	{
		Addressables.LoadSceneAsync(Matching_Label.labelString);
	}

	public void Load_MCScene()
	{
		Addressables.LoadSceneAsync(MC_Label.labelString);
	}

	private void OnDownloadAR_scene(AsyncOperationHandle<IList<IResourceLocation>> obj)
	{
		AR_scene = new List<IResourceLocation>(obj.Result);

		Debug.Log(AR_scene[0]);

		downloadSize = Addressables.GetDownloadSizeAsync(AR_scene[0]);
		Debug.Log(downloadSize.Result);
		DownloadScene_Size1.text = downloadSize.Result.ToString();
		DownloadScene_Size2.text = downloadSize.Result.ToString();


		if (downloadSize.Result > 0)
		{
			// Delete old Bundle hahaha
			//Caching.ClearCache();

			Caching.ClearAllCachedVersions("remote_arscene_scenes_all");
			
			StartCoroutine(LoadRoutine());
		}
		else
		{
			Debug.Log("There is no Update");
			Addressables.LoadSceneAsync(AR_scene[0]);
		}	

	}

	private IEnumerator LoadRoutine()
	{
		var async = Addressables.LoadSceneAsync(AR_scene[0]);

		loadingPanel.SetActive(true);
		while (!async.IsDone)
		{

			ProgressNumber.text = (async.PercentComplete * 100f).ToString("F0") + ("%");
			//ProgressSlider.value += 0.05f;
			ProgressSlider.value = async.PercentComplete;

			Debug.Log(async.PercentComplete);
			
			yield return new WaitForSeconds(3f);
		}
		

		// At this point the scene is loaded and referenced in async.Result
		Debug.Log("LOADED!");

		//Scene myScene = async.Result;
	
	}
}
