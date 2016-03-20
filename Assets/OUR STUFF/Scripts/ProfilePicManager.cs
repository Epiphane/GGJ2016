using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfilePicManager : MonoBehaviour {

	static Dictionary<string, Texture2D> profilePictures = new Dictionary<string, Texture2D>();

	private static Request loading, last;
	private class Request {
		public Texture2D image;
		public string url;
		public Request next;

		public Request(Texture2D i, string u) { image = i; url = u; }
	}

	void Awake() {
		if (GameObject.FindGameObjectsWithTag ("ProfilePicManager").Length > 1)
			Destroy (gameObject);
		else
			DontDestroyOnLoad (gameObject);
	}

	public Texture2D Load(string url) {
		if (profilePictures.ContainsKey (url)) {
			return profilePictures [url];
		}

		Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);

		profilePictures [url] = tex;

		if (loading != null) {
			last.next = new Request (tex, url);
		} else {
			loading = last = new Request (tex, url);

			Load ();
		}

		Debug.Log ("Returning");
		return tex;
	}

	private void Load() {
		Request request = loading;

		while (request != null) {
			WWW www = new WWW (request.url);

			while (!www.isDone)
				;
//			yield return www = new WWW (request.url);

			Debug.Log ("Woah " + request.url);
			www.LoadImageIntoTexture (request.image);
//			www.Dispose ();
//			www = null;

			request = request.next;
		}
	}
}
