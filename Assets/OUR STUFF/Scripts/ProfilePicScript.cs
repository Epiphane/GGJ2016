using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProfilePicScript : MonoBehaviour {

	private static ProfilePicScript loading;
	private ProfilePicScript nextToLoad;
	private string url;

	private Image img;

	private class Request {
		public Texture2D image;
		public string url;

		public Request(Texture2D i, string u) { image = i; url = u; }
	}

	private static List<Request> requests = new List<Request> ();

	private IEnumerator Load(Request request) {
		WWW www = new WWW( request.url );

		yield return www = new WWW (request.url);

		www.LoadImageIntoTexture(request.image);
		www.Dispose();
		www = null;
	}

	public void LoadProfilePic(string url) {
		img = gameObject.AddComponent<Image> ();
		img.material.mainTexture = GameObject.FindGameObjectWithTag("ProfilePicManager").GetComponent<ProfilePicManager>().Load (url);

		return;
		

		this.url = url;

		img = gameObject.AddComponent<Image> ();
		img.material.mainTexture = new Texture2D(64, 64, TextureFormat.DXT1, false);
		Request myReq = new Request (img.material.mainTexture as Texture2D, url);

//		requests.Add (myReq);

//		if (requests.Count == 1) {
			StartCoroutine(Load (myReq));
//		}
	}
}
