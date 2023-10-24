using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OneHit
{
    public class NoInternetPanel : MonoBehaviour
    {
        public static NoInternetPanel Instance { get; private set; }

        [Space]
        [SerializeField] protected Image darkBG;
        [SerializeField] protected Popup popup;

        [Space]
        [SerializeField] protected float activeDuration = 0.4f;
        [SerializeField] protected float inactiveDuration = 0.3f;

        private void Awake() => Instance = this;

        private void OnValidate()
        {
            darkBG = GetComponent<Image>();
            popup = GetComponentInChildren<Popup>();
        }

        private void Start()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            gameObject.SetActive(false);
            darkBG.DOFade(0f,0);
            popup.Hide();
        }

        public void Enable()
        {
            Debug.Log(" enable no internet panel");
            gameObject.SetActive(true);
            darkBG.DOKill();
            darkBG.DOFade(1f, activeDuration).SetEase(Ease.OutCubic);
            popup.Appear();
        }

        public void Disable()
        {
            popup.Disappear();
            darkBG.DOKill();
            darkBG.DOFade(0f, inactiveDuration).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}