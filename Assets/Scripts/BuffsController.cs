using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuffsController : MonoBehaviour
{
    private MeshRenderer playerRenderer;
    public enum Buff
    {
        Shield, SlowMo, SpeedUp, Magnet
    }
    public readonly Queue<Buff> Buffs = new();

    public SliderTimer buffsSlider;
    public GameObject buffsUI;
    public Sprite shieldIcon;
    
    private bool shieldActive;
    public bool ShieldActive
    {
        get => shieldActive;
        set
        {
            shieldActive = value;
            gameObject.transform.Find("Shield").gameObject.SetActive(shieldActive);
            if(shieldActive)
                buffsUI.transform.Find("BuffsImage").GetComponent<Image>().sprite = shieldIcon;
        }
    }
    private Coroutine currentBuffCoroutine = null;
    private Coroutine hitObstacleCoroutine = null;

    private bool isInvincible;
    public bool IsInvincible
    {
        get => isInvincible;
        set
        {
            isInvincible = value;
            if(isInvincible)
                StartCoroutine(IframeBlink());
        }
    }

    public Sprite slowmoIcon;
    private bool slowmoActive;
    public bool SlowmoActive
    {
        get => slowmoActive;
        set
        {
            slowmoActive = value;
            if(slowmoActive) {
                buffsUI.transform.Find("BuffsImage").GetComponent<Image>().sprite = slowmoIcon;
            }
        }
    }

    public Sprite speedupIcon;
    private bool speedupActive;
    public bool SpeedupActive
    {
        get => speedupActive;
        set
        {
            speedupActive = value;
            if(speedupActive) {
                buffsUI.transform.Find("BuffsImage").GetComponent<Image>().sprite = speedupIcon;
            }
        }
    }

    public GameObject MagnetFieldObj;
    public Sprite magnetIcon;
    private bool magnetActive;
    public bool MagnetActive
    {
        get => magnetActive;
        set
        {
            magnetActive = value;
            if(magnetActive) {
                buffsUI.transform.Find("BuffsImage").GetComponent<Image>().sprite = magnetIcon;
            }
        }
    }

    void Start()
    {
        playerRenderer = transform.Find("Body").GetComponent<MeshRenderer>();
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Big Obstacle"))
            hitObstacleCoroutine ??= StartCoroutine(HitObstacle());

        if(other.gameObject.CompareTag("Shield")) {
            GameManager.Instance.RemoveCurrObject(other.gameObject);
            Destroy(other.gameObject);
            Buffs.Enqueue(Buff.Shield);
            ProcessNextBuff();
        }

        if(other.gameObject.CompareTag("SlowMo")) {
            GameManager.Instance.RemoveCurrObject(other.gameObject);
            Destroy(other.gameObject);
            Buffs.Enqueue(Buff.SlowMo);
            ProcessNextBuff();
        }
        
        if(other.gameObject.CompareTag("SpeedUp")) {
            GameManager.Instance.RemoveCurrObject(other.gameObject);
            Destroy(other.gameObject);
            Buffs.Enqueue(Buff.SpeedUp);
            ProcessNextBuff();
        }

        if(other.gameObject.CompareTag("Magnet")) {
            GameManager.Instance.RemoveCurrObject(other.gameObject);
            Destroy(other.gameObject);
            Buffs.Enqueue(Buff.Magnet);
            ProcessNextBuff();
        }
    }

    private IEnumerator HitObstacle()
    {
        if(ShieldActive)
        {        
            ShieldActive = false;
            IsInvincible = true;
            StopCoroutine(currentBuffCoroutine);
            currentBuffCoroutine = null;
            buffsUI.SetActive(false);
            Buffs.Dequeue();
            ProcessNextBuff();
            yield return new WaitForSeconds(3f);    // iframe period
        }
        else
            GetComponent<PlayerController>().PlayerDeath();

        hitObstacleCoroutine = null;
    }

    public void ProcessNextBuff(float buffTimer = 0f)
    {
        if(!Buffs.TryPeek(out Buff buff))
            return;

        if(currentBuffCoroutine != null)
            return;

        switch(buff)
        {
            case Buff.Shield:
                currentBuffCoroutine = StartCoroutine(ActivateShield(buffTimer));
                break;
            case Buff.SlowMo:
                currentBuffCoroutine = StartCoroutine(ActivateSlowMo(buffTimer));
                break;
            case Buff.SpeedUp:
                currentBuffCoroutine = StartCoroutine(ActivateSpeedUp(buffTimer));
                break;
            case Buff.Magnet:
                currentBuffCoroutine = StartCoroutine(ActivateMagnet(buffTimer));
                break;
        }
    }

    private IEnumerator ActivateShield(float buffTimer)
    {
        buffsUI.SetActive(true);
        ShieldActive = true;

        buffsSlider.StartTimer(10f);
        yield return new WaitForSeconds(10f - buffTimer);

        ShieldActive = false;
        buffsUI.SetActive(false);
        currentBuffCoroutine = null;
        Buffs.Dequeue();
        ProcessNextBuff();
    }

    private IEnumerator ActivateSlowMo(float buffTimer)
    {
        buffsUI.SetActive(true);
        SlowmoActive = true;
        GameManager.Instance.GameSpeed /= 2f;

        buffsSlider.StartTimer(15f);
        yield return new WaitForSeconds(15f - buffTimer);

        SlowmoActive = false;
        GameManager.Instance.GameSpeed *= 2f;
        buffsUI.SetActive(false);
        currentBuffCoroutine = null;
        Buffs.Dequeue();
        ProcessNextBuff();
    }

    private IEnumerator ActivateSpeedUp(float buffTimer)
    {
        buffsUI.SetActive(true);
        SpeedupActive = true;
        GameManager.Instance.GameSpeed *= 1.5f;

        buffsSlider.StartTimer(7f);
        yield return new WaitForSeconds(7f - buffTimer);

        SpeedupActive = false;
        GameManager.Instance.GameSpeed /= 1.5f;
        buffsUI.SetActive(false);
        currentBuffCoroutine = null;
        Buffs.Dequeue();
        ProcessNextBuff();
    }

    private IEnumerator ActivateMagnet(float buffTimer)
    {
        buffsUI.SetActive(true);
        MagnetActive = true;
        MagnetFieldObj.SetActive(true);

        buffsSlider.StartTimer(15f);
        yield return new WaitForSeconds(15f - buffTimer);

        MagnetFieldObj.SetActive(false);
        MagnetActive = false;
        buffsUI.SetActive(false);
        currentBuffCoroutine = null;
        Buffs.Dequeue();
        ProcessNextBuff();
    }

    IEnumerator IframeBlink()
    {
        float timer = 0f;

        while (timer < 3f)
        {
            playerRenderer.enabled = !playerRenderer.enabled;

            yield return new WaitForSeconds(0.15f);

            timer += 0.15f;
        }

        playerRenderer.enabled = true;
        IsInvincible = false;
    }
}
