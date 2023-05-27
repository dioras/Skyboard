using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
	[Header("Components")]
	[SerializeField] private RectTransform panelContainer = default;

    private void OnEnable()
    {
		UIController.ShowUIPanelAloneAction += ReactPanel;
	}

    private void OnDisable()
    {
		UIController.ShowUIPanelAloneAction -= ReactPanel;
	}

	private void ReactPanel(UIPanelType _uIPanelType)
	{
		switch (_uIPanelType)
		{
			case UIPanelType.Main:
				Show();
				break;
			case UIPanelType.Store:
				Show();
				break;
			case UIPanelType.Game:
				Show();
				break;
			case UIPanelType.None:
				Show();
				break;
			case UIPanelType.LevelResult:
				Show();
				break;
			default:
				Hide();
				break;
		}
	}

	private void Show()
    {
		panelContainer.gameObject.SetActive(true);
    }

	private void Hide()
    {
		panelContainer.gameObject.SetActive(false);
    }
}
