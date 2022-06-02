using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
	[System.Serializable]
    public class DialogueLine : DialogueBaseClass
    {
        private Text textHolder;

        [Header ("Metin Seçenekleri")]
        [SerializeField] private string input;
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;

        [Header("Zaman Parametresi")]
        [SerializeField] private float erteleme;
        [SerializeField] private float delayBetweenLines;

        [Header("Ses")]
        [SerializeField] private AudioClip ses;

        [Header("Karakter Resmi")]
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private Image imageHolder;

        private void Awake()
        {
            textHolder = GetComponent<Text>();
            textHolder.text = "";

            imageHolder.sprite = characterSprite;
            imageHolder.preserveAspect = true;
        }

        private void Start()
        {
            StartCoroutine(WriteText(input, textHolder, textColor, textFont, erteleme, ses, delayBetweenLines));
        }
    }
}