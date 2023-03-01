using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public TMP_Text Text;
    public Image Bar;
    public Image Border;
    public float Health = 100;
    public float MaxHealth = 100;
    private float _lerpSpeed;
    private float _healthPercent
    {
        get { return Health / MaxHealth; }
        set { Health = MaxHealth * value; }
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (_healthPercent > 1)
            _healthPercent = 1;
        if (_healthPercent < 0)
            _healthPercent = 0;
        Text.text = ((int)(_healthPercent * 100)).ToString() + "%";
        _lerpSpeed = 3f * Time.deltaTime;
        HealthBarFiller();
        ColorChanger();
    }

    void HealthBarFiller()
    {
        Bar.fillAmount = Mathf.Lerp(Bar.fillAmount, _healthPercent, _lerpSpeed);
        Border.fillAmount = Mathf.Lerp(Bar.fillAmount, _healthPercent, _lerpSpeed);
    }
    void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, _healthPercent);
        Bar.color = healthColor;
        Border.color = new Color(healthColor.r * .7f, healthColor.g * .7f, healthColor.b * .7f);
        Text.color = healthColor;
    }
}
