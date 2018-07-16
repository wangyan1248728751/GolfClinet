using System;
using GameFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

namespace Golf
{
	public class EventListener : MonoBehaviour, IEventSystemHandler, 
		IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler,
		IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, 
		IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, ISubmitHandler
	{
		public GameFrameworkAction<GameObject> onSubmit;
		public GameFrameworkAction<GameObject> onClick;
		public GameFrameworkAction<GameObject> onDown;
		public GameFrameworkAction<GameObject> onEnter;
		public GameFrameworkAction<GameObject> onUp;
		public GameFrameworkAction<GameObject> onExit;
		public GameFrameworkAction<GameObject> onSelect;
		public GameFrameworkAction<GameObject> onDoubleClick;
		public GameFrameworkAction<GameObject> onUpdateSelect;
		public GameFrameworkAction<GameObject, bool> onHover;
		public GameFrameworkAction<GameObject, bool> onPress;
		public GameFrameworkAction<GameObject, Vector2> onScroll;
		public GameFrameworkAction<GameObject> onDragStart;
		public GameFrameworkAction<GameObject, Vector2> onBeginDrag;
		public GameFrameworkAction<GameObject, Vector2> onDrag;
		public GameFrameworkAction<GameObject,Vector2> onDragEnd;
		public GameFrameworkAction<GameObject, GameObject> onDrop;
		public GameFrameworkAction<GameObject, KeyCode> onKey;

		public static EventListener Get(GameObject go)
		{
			EventListener listener = go.GetComponent<EventListener>();
			if (listener == null) listener = go.AddComponent<EventListener>();
			return listener;
		}
        
        public void OnSubmit(BaseEventData eventData)
		{
			if (onSubmit != null) onSubmit(gameObject);
		}
		public void OnPointerClick(PointerEventData eventData)
		{
			if (onClick != null) onClick(gameObject);
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			if (onDown != null) onDown(gameObject);
		}
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (onEnter != null) onEnter(gameObject);
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			if (onExit != null) onExit(gameObject);
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			if (onUp != null) onUp(gameObject);
		}
		public void OnSelect(BaseEventData eventData)
		{
			if (onSelect != null) onSelect(gameObject);
		}
		public void OnUpdateSelected(BaseEventData eventData)
		{
			if (onUpdateSelect != null) onUpdateSelect(gameObject);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (onBeginDrag != null) onBeginDrag(gameObject,eventData.position);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (onDrag != null) onDrag(gameObject, eventData.position);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (onDragEnd != null) onDragEnd(gameObject, eventData.position);
		}

		public void OnDrop(PointerEventData eventData)
		{
			if (onDrop != null) onDrop(gameObject, eventData.pointerPress);
		}

		public void OnScroll(PointerEventData eventData)
		{
			if (onScroll != null) onScroll(gameObject, eventData.scrollDelta);
		}
	}
}
