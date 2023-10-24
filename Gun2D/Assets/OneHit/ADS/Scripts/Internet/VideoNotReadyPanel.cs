using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OneHit
{
    public class VideoNotReadyPanel : MonoBehaviour
    {
        public static VideoNotReadyPanel Instance { get; private set; }

        [Space]
        [SerializeField] protected Image darkBG;
        [SerializeField] protected Popup popup;
        [SerializeField] protected GameObject closeButton;

        [Space]
        [SerializeField] protected float activeDuration = 0.4f;
        [SerializeField] protected float inactiveDuration = 0.3f;

        private void Awake() => Instance = this;

        private void OnValidate()
        {
            darkBG = GetComponent<Image>();
            popup = GetComponentInChildren<Popup>();
            closeButton = transform.GetChild(0).gameObject;
        }

        private void Start()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            gameObject.SetActive(false);
            closeButton.SetActive(false);
            darkBG.DOFade(0f, 0);
            popup.Hide();
        }

        public void Enable()
        {
            Debug.Log("enable video not ready panel");
            gameObject.SetActive(true);
            closeButton.SetActive(true);
            darkBG.DOKill();
            darkBG.DOFade(1f, activeDuration).SetEase(Ease.OutCubic);
            popup.Appear();
        }

        public virtual void Disable()
        {
            closeButton.SetActive(false);
            popup.Disappear();
            darkBG.DOKill();
            darkBG.DOFade(0f, inactiveDuration).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}