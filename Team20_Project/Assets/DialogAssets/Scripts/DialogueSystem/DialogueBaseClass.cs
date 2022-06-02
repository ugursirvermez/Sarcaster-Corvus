using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueBaseClass : MonoBehaviour
    {
        public bool finished { get; private set; }

        protected IEnumerator WriteText(string input, Text textHolder, Color textColor, Font textFont, float delay, AudioClip sound, float delayBetweenLines)
        {
            textHolder.color = textColor;
            textHolder.font = textFont;

            for (int i = 0; i < input.Length; i++)
            {
                textHolder.text += input[i];
                //SoundManager.örnek.PlaySound(sound);
                yield return new WaitForSeconds(delay);
            }

            //bir sonraki satıra geçmeden önce fareye tıkladıktan sonra okuma seçeneği sunmak için.
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            finished = true;
        }
    }
}

