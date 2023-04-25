using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SpaceShooter.UserControl
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// Задник джойстика.
        /// </summary>
        [SerializeField] private Image _background;

        /// <summary>
        /// Рычажок джойстика.
        /// </summary>
        [SerializeField] private Image _stick;

        /// <summary>
        /// Положение джойстика.
        /// </summary>
        public Vector3 Value { get; private set; }

        /// <summary>
        /// Границы перемещения рычажка джойстика.
        /// </summary>
        private Vector2 _stickBounds;

        /// <summary>
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {
            //закэшировать границы перемещения рычажка джойстика.
            _stickBounds = new Vector2(
                (_background.rectTransform.sizeDelta.x - _stick.rectTransform.sizeDelta.x) / 2f, 
                (_background.rectTransform.sizeDelta.y - _stick.rectTransform.sizeDelta.y) / 2f
                );
        }

        /// <summary>
        /// Вызывается при перемещении указателя после нажатия на джойстике и до момента отпускания.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            //вычислить нормализованный вектор джойстика
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_background.rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 pos);
            pos.x = pos.x / _background.rectTransform.sizeDelta.x * 2;
            pos.y = pos.y / _background.rectTransform.sizeDelta.y * 2;
            Vector3 result = new Vector3(pos.x, pos.y, 0);
            Value = result.magnitude > 1 ? result.normalized : result;

            //переместить рычажок в позицию, соответствующую позиции нажатия
            _stick.rectTransform.anchoredPosition = Value * _stickBounds;
        }

        /// <summary>
        /// Вызывается при отпускании джойстика.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            Value = Vector3.zero;
            _stick.rectTransform.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Вызывается при нажатии на джойстик.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            //в момент нажатия на джойстик нужно обработать его текущую позицию
            OnDrag(eventData);
        }
    }
}
