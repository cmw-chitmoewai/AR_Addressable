using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LPManager : MonoBehaviour {

	public static LPManager Instance = null; 

	public enum QType
	{
		MODEL,
		QLETTER,
		QSOUND
	}
	public QType curQType = QType.QLETTER;

	public GameObject objPos;
	public AudioClip[] correctSounds;
	public AudioClip wrongSound;
	public AudioClip starSound;
	public GameObject banMusic;
	public GameObject summonParticle; 
	public GameObject trophyParticle;
	public AudioClip trophySound;

	public Text trophyText;
	public Sprite starNormal;
	public Sprite starSpecial; 

	public GameObject qPanel;
	public GameObject qSound; 
	public Text qPanelText;

	public GAui trophy;
	public GAui continuePanel;
	public GAui readyPanel; 
	public GameObject blackBG;
	public GAui exitPanel;

	public TextAsset ansTexts;
	public GameObject[] prefabs;
	public GameObject[] ansButtons;
	public Image[] stars;
	public GameObject[] starParticles;

	private GameObject curObject;
	private List<string> ansList = new List<string>();
	private List<GameObject> prefabList = new List<GameObject>();
	private List<AudioClip> audioList = new List<AudioClip>();
	private string[] ansArray;
	private AudioSource audioSource;

	private int[] arr = new int[60];
	private int qIndex = 0;
	private int starCount;
	private int trophyCount;


	private void Awake()
	{
		Instance = this; 
	}

	// Use this for initialization
	void Start ()
	{
		trophyCount = PlayerPrefs.GetInt("TrophyCount", 0);
		trophyText.text = trophyCount.ToString();

		audioSource = GetComponent<AudioSource>();
		audioSource.volume = PlayerPrefs.GetFloat("SOUND_VOLUME", 0.5f);

		ansArray = ansTexts.ToString().Split('\n');

		// initialize to arr array 
		for (int i = 0; i < 60; i++)
		{
			arr[i] = i; 
		}

		// Shuffle array 
		for (int i = 0; i < arr.Length; i++)
		{
			int rand = Random.Range(0, arr.Length);

			int temp = arr[rand];
			arr[rand] = arr[i];
			arr[i] = temp;
		}

		// Shuffel the correct sound. 
		for (int i = 0; i < correctSounds.Length; i++)
		{
			int ran = Random.Range(0, correctSounds.Length);


			var temp = correctSounds[ran];
			correctSounds[ran] = correctSounds[i];
			correctSounds[i] = temp;
		}

		// Entry point for genereating questions.
		StartCoroutine(ReadyPanel());
		

		// Check Music
		if (MusicManager.Instance != null)
		{
			if (MusicManager.Instance.audioSource.isPlaying)
			{
				banMusic.SetActive(false);
				// MusicManager.Instance.StopMusic();
			}
			else
			{
				banMusic.SetActive(true);
				// MusicManager.Instance.PlayMusic();
			}
		}

		//Debug.Log(ansList);
		//Debug.Log(arr);
	}

	IEnumerator ReadyPanel ()
	{
		if(!readyPanel.gameObject.activeInHierarchy)
		{
			readyPanel.gameObject.SetActive(true);
		}

		yield return new WaitForSeconds(0.2f);

		readyPanel.MoveIn();

		yield return new WaitForSeconds(1f);

		readyPanel.MoveOut();

		// Start generating questions
		StartCoroutine(PrepareQuestion());
	}

	//
	IEnumerator PrepareQuestion ()
	{
		// Waiting for Ready panel
		yield return new WaitForSeconds(1);

		StartQuestion();
	}

	// Generate random answer array 
	List<string> GetAnsList ()
	{
		List<string> ans = new List<string>();

		// Get answer list from objects name.
		for (int i = 0; i < prefabs.Length; i++)
		{
			ans.Add(prefabs[i].name);
		}

		return ans; 
	}

	// Generate the prefabs
	List<GameObject> GetPrefabList ()
	{
		List<GameObject> pbs = new List<GameObject>();

		// Create the object list 
		for (int i = 0; i < prefabs.Length; i++)
		{
			pbs.Add(prefabs[i]);
		}

		return pbs; 
	}

	// Get audio list 
	List<AudioClip> GetAudioList ()
	{
		List<AudioClip> audioList = new List<AudioClip>();

		for (int i = 0; i < prefabs.Length; i++)
		{
			string n = prefabs[i].name;

			// Make the first letter to Upper Case.
			string pathName = n.First().ToString().ToUpper() + n.Substring(1);

			AudioClip clip = Resources.Load("Pronunciation/ENUS/" + pathName) as AudioClip; 

			audioList.Add(clip);
		}

		return audioList;
	}

	// Get Random Questions type 
	QType GetQuestionType ()
	{
		int ran = Random.Range(0, 3);

		QType type = QType.QLETTER; 

		switch (ran)
		{
			case 0:
				type = QType.QLETTER;
				break;

			case 1:
				type = QType.MODEL;
				break;

			case 2:
				type = QType.QSOUND;
				break; 

			default:
				break;
		}
		// For release
		return type;
	}

	// Start show questions 
	void StartQuestion ()
	{
		qIndex++;
		if (qIndex >= arr.Length)
		{
			qIndex = 0;
		}

		// Clear the answer list first. 
		ansList.Clear();

		// Generate the answer list
		ansList = GetAnsList();

		// Clear Prefab List 
		prefabList.Clear();

		// Get Prefab List 
		prefabList = GetPrefabList();

		// Clear Audio List 
		audioList.Clear();

		// Get Audio List
		audioList = GetAudioList();

		// Get a random Question Type 
		curQType = GetQuestionType();

		// Destroy the current showing object 
		if (curObject != null)
		{
			Destroy(curObject);
		}

		// Random button id for correct answer.
		int id = Random.Range(0, 3);

		switch (curQType)
		{
			case QType.MODEL:

				qPanel.SetActive(false);
				qSound.SetActive(false);

				qPanelText.enabled = true;

				// Show question 
				curObject = Instantiate(prefabs[arr[qIndex]], objPos.transform.position, Quaternion.identity);
				curObject.GetComponent<ObjectScript>().curState = ObjectScript.State.ON_LP;
				curObject.GetComponent<ObjectScript>().title = prefabs[arr[qIndex]].name;
				curObject.GetComponent<ObjectScript>().PlayAnimation();

				// Show particles 
				summonParticle.SetActive(false);
				summonParticle.SetActive(true);

				// Generate the correct answer.
				ansButtons[id].GetComponent<AnsButton>().OnSetAnswer(ansList[arr[qIndex]], false);
				ansList.RemoveAt(arr[qIndex]);
				break;

			case QType.QLETTER:
				

				qPanel.SetActive(true);
				qSound.SetActive(false);

				// qPanelText.enabled = true; 

				// Set the name of the questions from object list.
				qPanelText.text = prefabs[arr[qIndex]].name;

				// Set the correct answer 
				// GameObject obj = Instantiate(prefabs[arr[qIndex]], transform.position, Quaternion.identity);
				// obj.transform.SetParent(ansButtons[id].transform, true);
				// obj.GetComponent<ObjectScript>().title = prefabs[arr[qIndex]].name;

				ansButtons[id].GetComponent<AnsButton>().OnSetAnswer(ansList[arr[qIndex]], true);
				ansList.RemoveAt(arr[qIndex]);

				qPanel.GetComponent<GAui>().MoveIn();

				break;

			case QType.QSOUND:
				qSound.SetActive(true);
				qPanel.SetActive(false);

				// qPanelText.enabled = false; 

				qSound.GetComponent<AudioSource>().clip = audioList[arr[qIndex]];

				qSound.GetComponentInChildren<Text>(true).text = ansList[arr[qIndex]];

				// Generate the correct answer.
				ansButtons[id].GetComponent<AnsButton>().OnSetAnswer(ansList[arr[qIndex]], true);
				ansList.RemoveAt(arr[qIndex]);

				qSound.GetComponent<GAui>().MoveIn();

				break;

			default:
				break;
		}

		// Fill the remaining two buttons 
		// Generate for other two button.
		for (int i = 0; i < 3; i++)
		{
			if (i == id)
				continue;

			int ran = Random.Range(0, ansList.Count);		

			switch (curQType)
			{
				case QType.MODEL:
					ansButtons[i].GetComponent<AnsButton>().OnSetAnswer(ansList[ran], false);
					ansList.RemoveAt(ran);
					break;
					
				case QType.QLETTER:
					ansButtons[i].GetComponent<AnsButton>().OnSetAnswer(ansList[ran], true);
					ansList.RemoveAt(ran);

					break;
				
				case QType.QSOUND:
					ansButtons[i].GetComponent<AnsButton>().OnSetAnswer(ansList[ran], true);
					ansList.RemoveAt(ran);
					break;
				default:
					break;
			}
		}
	}

	private bool isCorrect = false;
	private int correctSoundIndex = 0;

	// On click answer button 
	public void OnClickAnswer (Text ansText)
	{
		// Answer Correct.
		if (ansText.text.ToLower() == prefabs[arr[qIndex]].name)
		{
			// Random each sound (Not perfect).
			// int ran = Random.Range(0, correctSounds.Length);

			if(correctSoundIndex >= correctSounds.Length)
			{
				correctSoundIndex = 0;
			}

			Debug.Log("You're right.");
			audioSource.clip = correctSounds[correctSoundIndex];
			correctSoundIndex++;

			isCorrect = true;

			starCount++;

			StartCoroutine(IncStar());

			//if(starCount == 3)
			//{
			//	trophyCount++;
			//	trophyText.text = trophyCount.ToString();
			//}
		}
		// Answer wrong.
		else
		{
			Debug.Log("You're Wrong.");
			audioSource.clip = wrongSound;

			if (starCount > 0)
			{
				starCount--;
				stars[starCount].sprite = starNormal;
				starParticles[starCount].SetActive(false);
			}
		}

		// Disable all answer button to prevent double click 
		foreach (var item in ansButtons)
		{
			item.GetComponent<Button>().interactable = false; 
		}

		audioSource.Play();

		StartCoroutine(CloseQuestion());
	}

	IEnumerator CloseQuestion ()
	{
		yield return new WaitForSeconds(1);

		switch (curQType)
		{
			case QType.MODEL:
				break;
			case QType.QLETTER:
				qPanel.GetComponent<GAui>().MoveOut();
				break;
			case QType.QSOUND:
				qSound.GetComponent<GAui>().MoveOut();
				break;
			default:
				break;
		}

		foreach (var item in ansButtons)
		{
			item.GetComponent<GAui>().MoveOut();
		}

		yield return new WaitForSeconds(1);

		if (starCount < stars.Length)
		{
			StartCoroutine(PrepareQuestion());
		}
	}

	IEnumerator IncStar ()
	{
		yield return new WaitForSeconds(1.2f);

		if (isCorrect)
		{
			isCorrect = false;

			// Increase star count 
			
			stars[starCount - 1].sprite = starSpecial;
			starParticles[starCount - 1].SetActive(true);

			audioSource.clip = starSound;
			audioSource.Play();

			// Check and reset stars.
			if (starCount == stars.Length)
			{
				trophyParticle.SetActive(false);
				trophyParticle.SetActive(true);

				audioSource.clip = trophySound;
				audioSource.Play();

				trophy.transform.localPosition = new Vector2(-226, 0);

				if(!trophy.gameObject.activeInHierarchy)
				{
					trophy.gameObject.SetActive(true);
				}

				// Show Trophy
				trophy.MoveIn();

				Destroy(curObject);

				yield return new WaitForSeconds(1.5f);

				trophy.MoveOut();

				for (int i = 0; i < stars.Length; i++)
				{
					stars[i].sprite = starNormal;
					starParticles[i].SetActive(false);
				}

				starCount = 0;

				yield return new WaitForSeconds(1);

				trophyCount++;
				PlayerPrefs.SetInt("TrophyCount", trophyCount);
				trophyText.text = trophyCount.ToString();

				blackBG.SetActive(true);

				if(!continuePanel.gameObject.activeInHierarchy)
				{
					continuePanel.gameObject.SetActive(true);
				}

				continuePanel.MoveIn();
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// StartQuestion();
		}
	}

	#region Button Click Region
	// On Click Home 
	public void OnClickHome ()
	{
		// blackBG.SetActive(true);

		// exitPanel.MoveIn();

		SceneManager.LoadScene("LPMenu");
	}

	// On Click Sound 
	public void OnClickSound ()
	{
		if (MusicManager.Instance != null)
		{
			if (MusicManager.Instance.audioSource.isPlaying)
			{
				banMusic.SetActive(true);
				MusicManager.Instance.StopMusic();
			}
			else
			{
				banMusic.SetActive(false);
				MusicManager.Instance.PlayMusic();
			}
		}
	}

	public void OnClickContinue ()
	{
		StartCoroutine(PrepareQuestion());

		blackBG.SetActive(false);

		continuePanel.MoveOut();
	}

	public void OnClickStop ()
	{
		SceneManager.LoadScene("LPMenu"); 
	}

	#endregion
}
