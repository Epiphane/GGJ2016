using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ProfilePicScript : MonoBehaviour {

	private static ProfilePicScript loading;
	private ProfilePicScript nextToLoad;
	private string url;

	private Image img;

	public IEnumerator LoadProfilePic(string url) {
		this.url = url;

		WWW www = new WWW( url );

		yield return www;

		img = gameObject.AddComponent<Image> ();

		img.material.mainTexture = new Texture2D(www.texture.width, www.texture.height, TextureFormat.DXT1, false);
		www.LoadImageIntoTexture(img.mainTexture as Texture2D);
		www.Dispose();
		www = null;
	}
}
