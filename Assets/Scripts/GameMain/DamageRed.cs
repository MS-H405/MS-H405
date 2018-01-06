using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageRed : MonoBehaviour {

	/// <summary>
	/// 概要 : 被ダメージエフェクト
	/// Author : 大洞祥太
	/// </summary>

	static DamageRed instance;

	public static DamageRed Instance {
		get {
			if (instance == null) {
				instance = (DamageRed)FindObjectOfType(typeof(DamageRed));

				if (instance == null) {
					Debug.LogError("DamageRed Instance Error");
				}
			}
			return instance;
		}
	}

	private bool bRun = false;

	[SerializeField] float fMaxAlpha = 0.25f;
	[SerializeField] float fTime = 0.1f;
	private Image _image = null;
	private bool bAdd = true;
	private Color InitColor;

	void Awake() 
	{
		if (this != Instance) {
			Destroy (this.gameObject);
			return;
        }
        _image = GetComponent<Image>();
        InitColor = _image.color;
    }
	
	// Update is called once per frame
	void Update () 
	{

		#if DEBUG
		if(Input.GetKeyDown(KeyCode.A)) {
			Run();
		}
		#endif
	
		if (!bRun) 
			return;

		if (bAdd) 
		{
			_image.color += new Color (0, 0, 0, fMaxAlpha * (Time.deltaTime / fTime));

			if (_image.color.a >= fMaxAlpha) {
				bAdd = false;
				Color color = InitColor;
				color.a = fMaxAlpha;
				_image.color = color;
			}
		} 
		else 
		{
			_image.color -= new Color (0, 0, 0, fMaxAlpha * (Time.deltaTime / fTime));

			if (_image.color.a <= 0.0f) {
				bAdd = true;
				Color color = InitColor;
				color.a = 0.0f;
                _image.color = color;
				bRun = false;
			}
		}
	}

	public void Run() 
	{
		if (bRun)
			return;

		bRun = true;
		//SoundManager.Instance.PlaySE (SoundManager.eSeValue.SE_DAMAGERED);
	}
}
