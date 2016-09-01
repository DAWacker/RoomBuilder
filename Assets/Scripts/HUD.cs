using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	public Camera hudCamera;
	public GameObject key;

	public float spacingX;
	public float spacingY;

	List<ImageKeyPair> imageKeyPair;

	class ImageKeyPair
	{
		public GameObject image;
		public Key key;

		public ImageKeyPair(GameObject image, Key key)
		{
			this.image = image;
			this.key = key;
		}
	}

	public void Reposition()
	{
		
	}

	public void CreateKeyImages(int num)
	{
		imageKeyPair = new List<ImageKeyPair> ();
		imageKeyPair.Add (new ImageKeyPair (key, null));

		key.GetComponent<Image> ().enabled = false;

		for (int i = 1; i < num; i++) {
			GameObject clone = GameObject.Instantiate (key);
			clone.transform.SetParent (key.transform.parent);
			clone.transform.localScale = Vector3.one;
			clone.transform.localPosition = new Vector3 (
				key.transform.localPosition.x + ((clone.GetComponent<RectTransform> ().rect.width + spacingX) * i),
				key.transform.localPosition.y,
				key.transform.localPosition.z);
			imageKeyPair.Add (new ImageKeyPair (clone, null));
		}
	}

	public void AddKey(Key key)
	{
		
	}

	void Update()
	{
		
	}
}