using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Монетка.
    /// </summary>
    public class Coin : MonoBehaviour
    {
        private Pickup _pickup;

        private void Awake()
        {
            if (!transform.root.TryGetComponent(out Pickup pickup)) return;
            _pickup = pickup;
            pickup.Pickuped.AddListener(OnPickuped);
        }

        private void OnDestroy()
        {
            _pickup.Pickuped.RemoveListener(OnPickuped);
        }

        private void OnPickuped()
        {
            //добавить монетку к счётчику
            //Globals.Instance.AddCoin();
        }
    }
}