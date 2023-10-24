using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Popup : MonoBehaviour
{
    [Space]
    [SerializeField]
    protected float appearDuration = 0.4f;

    [SerializeField]
    protected float disappearDuration = 0.3f;

    [Space]
    [SerializeField]
    protected float startScale = 0.5f;

    protected CanvasGroup canvasGroup;

    protected virtual void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Hide() {
        gameObject.SetActive(false);

        transform.localScale = Vector3.one * startScale;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    public virtual void Appear() {
        //SoundManager.Instance.PlaySound(Sound.popup_appear);
        gameObject.SetActive(true);

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, appearDuration).OnComplete(() => {
            canvasGroup.interactable = true;
        });

        transform.DOKill();
        transform.DOScale(1f, appearDuration).SetEase(Ease.OutBack);
    }

    public virtual void Disappear() {
        canvasGroup.interactable = false;

        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, disappearDuration).OnComplete(() => {
            gameObject.SetActive(false);
        });

        transform.DOKill();
        transform.DOScale(startScale, disappearDuration).SetEase(Ease.InBack);
    }
}