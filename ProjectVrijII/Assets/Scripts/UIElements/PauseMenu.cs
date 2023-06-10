using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	[SerializeField]
	private CanvasGroup pauseUI;

	[SerializeField]
	private CanvasGroup pauseMenu;

	[SerializeField]
	private CanvasGroup[] allSubMenus;

	private EventSystem eventSystem;
	private CanvasGroup currentSubMenu;

	private void Start() {
		eventSystem = EventSystem.current;
		
		for (int i = 0; i < allSubMenus.Length; i++) {
			CloseSubMenu(allSubMenus[i]);
		}

		ToggleMenu(false);
	}

	private void Update() {
		//TESTING PURPOSE~~ remove later
			if(Input.GetKeyUp(KeyCode.Escape)) {
				OnPauseKey();
			}		
		//~~
	}

	//CALL when pause key is pressed
	public void OnPauseKey() {
		if(currentSubMenu == null) {
			bool isActive = pauseUI.alpha > 0;
			ToggleMenu(!isActive);
		} else {
			CloseSubMenu(currentSubMenu);
		}		
	}

	public void ToggleMenu(bool activate) {
		pauseUI.alpha = activate ? 1 : 0;
		ToggleButtons(pauseMenu, activate);

		if(activate) {
			SelectFirstButton(pauseMenu);
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}

	public void GoToScene(int scene) {
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		Resources.UnloadUnusedAssets();
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}

	public void OpenSubMenu(CanvasGroup subMenu) {
		ToggleButtons(subMenu, true);
		ToggleButtons(pauseMenu, false);
		SelectFirstButton(subMenu);
		currentSubMenu = subMenu;
		subMenu.alpha = 1;
	}

	public void CloseSubMenu(CanvasGroup subMenu) {
		ToggleButtons(subMenu, false);
		ToggleButtons(pauseMenu, true);
		SelectFirstButton(pauseMenu);
		currentSubMenu = null;
		subMenu.alpha = 0;
	}

	private void SelectFirstButton(CanvasGroup group) {
		Button button = group.GetComponentInChildren<Button>();
		eventSystem.SetSelectedGameObject(button.gameObject);
	}

	private void ToggleButtons(CanvasGroup group, bool active) {
		Button[] allButtons = group.GetComponentsInChildren<Button>();
		foreach(Button button in allButtons) {
			button.interactable = active;
		}
	}
}
