using UnityEngine.EventSystems;
public class FloatingJoystick : Joystick
{
	private void Awake()
	{
		dependencyContainer.Joystick = this;
	}

	protected override void Start()
	{
		base.Start();
		background.gameObject.SetActive(false);
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
		background.gameObject.SetActive(true);
		//base.OnPointerDown(eventData);
	}


	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		background.gameObject.SetActive(false);
		base.OnPointerUp(eventData);
	}
}