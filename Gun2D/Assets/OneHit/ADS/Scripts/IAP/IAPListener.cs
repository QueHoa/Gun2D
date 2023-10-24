using UnityEngine;
using UnityEngine.Purchasing;

namespace OneHit
{
    public class IAPListener : MonoBehaviour
    {
        public void OnPurchased(Product product)
        {
            MasterControl.Instance.OnPurchased(product.definition.id);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            MasterControl.Instance.OnPurchaseFailed(product, reason);
        }

        public void Restore()
        {
            MasterControl.Instance.CheckRestore();
        }
        public void OpenURL(string link)
        {
            Application.OpenURL(link);
        }
    }
}