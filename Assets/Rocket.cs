using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
	[SerializeField] float rcsThrust = 100f;
	[SerializeField] private float mainThrust = 100f;
	[SerializeField] float levelLoadDelay = 2f;
	
	[SerializeField] private AudioClip mainEngine;
	[SerializeField] private AudioClip success;
	[SerializeField] private AudioClip death;
	
	[SerializeField] private ParticleSystem mainEngineParticles;
	[SerializeField] private ParticleSystem successParticles;
	[SerializeField] private ParticleSystem deathParticles;

	private Rigidbody rigidBody;
	private AudioSource audioSource;
	
	enum State { Alive, Dying, Transcending }

	private State state = State.Alive;

	private bool collisionsDisabled = false;

	// Use this for initialization
	void Start ()
	{
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (state == State.Alive)
		{
			RespondToThrustInput();
			RespondToRotateInput();
		}

		if (Debug.isDebugBuild)
		{
			RespondToDebugKeys();
		}
	}

	private void RespondToDebugKeys()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			LoadNextLevel();
		}
		else if(Input.GetKeyDown(KeyCode.C))
		{
			collisionsDisabled = !collisionsDisabled;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (state != State.Alive || !collisionsDisabled)
		{
			return;
		}
		
		switch (collision.gameObject.tag)
		{
			case "Friendly":
				// do nothing
				break;
			case "Finish":
				StartSuccessSequence();
				break;
			default:
				StartDeathSequence();
				break;
		}
	}

	private void StartSuccessSequence()
	{
		state = State.Transcending;
		audioSource.Stop();
		audioSource.PlayOneShot(success);
		successParticles.Play();
		Invoke("LoadNextLevel", levelLoadDelay); // parameterize time
	}
	
	private void StartDeathSequence()
	{
		state = State.Dying;
		audioSource.Stop();
		audioSource.PlayOneShot(death);
		deathParticles.Play();
		Invoke("LoadFirstLevel", levelLoadDelay); // parameterize time
	}

	private void LoadNextLevel()
	{
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		int nextSceneIndex = currentSceneIndex++;
		int numberOfScenes = SceneManager.sceneCountInBuildSettings;
		if (nextSceneIndex > numberOfScenes)
		{
			nextSceneIndex = 0;
		}
		SceneManager.LoadScene(nextSceneIndex);
	}
	
	private void LoadFirstLevel()
	{
		SceneManager.LoadScene(0);
	}

	private void RespondToThrustInput()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			ApplyThrust();
		}
		else
		{
			audioSource.Stop();
			mainEngineParticles.Stop();
		}
	}

	private void ApplyThrust()
	{
		rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
		if (!audioSource.isPlaying)
		{
			audioSource.PlayOneShot(mainEngine);
			mainEngineParticles.Play();
		}
	}

	private void RespondToRotateInput()
	{		
		float rotationThisFrame = rcsThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.A))
		{
			rigidBody.freezeRotation = true; // take manual control of rotation
			transform.Rotate(Vector3.forward * rotationThisFrame);
			rigidBody.freezeRotation = false; // resume physics control of rotation
		} else if (Input.GetKey(KeyCode.D))
		{
			rigidBody.freezeRotation = true; // take manual control of rotation
			transform.Rotate(-Vector3.forward * rotationThisFrame);
			rigidBody.freezeRotation = false; // resume physics control of rotation
		}
	}
}
