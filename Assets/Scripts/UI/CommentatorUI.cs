using System.Collections;
using Core;
using Data;
using Services;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CommentatorUI : MonoBehaviour
    {
        [SerializeField] private CommentatorData commentatorData;
        [SerializeField] private TextMeshProUGUI commentatorText;
        [SerializeField] private float commentDelay = 1.5f;

        private IGameplayEventService _gameplayEventService;
        private int _currentCommentIndex = -1;
        private Coroutine _hideTextCoroutine;

        private void Start()
        {
            _gameplayEventService = ServiceLocator.Get<IGameplayEventService>();
            _gameplayEventService.Subscribe(GameplayTags.Event_Score, OnScore);
            commentatorText.gameObject.SetActive(false);
        }

        private void OnScore()
        {
            if (_currentCommentIndex < commentatorData.comments.Count - 1)
            {
                _currentCommentIndex++;
            }

            string comment = commentatorData.comments[_currentCommentIndex];

            // Cancel any existing hide coroutine
            if (_hideTextCoroutine != null)
            {
                StopCoroutine(_hideTextCoroutine);
            }

            // Show the text
            commentatorText.text = comment;
            commentatorText.gameObject.SetActive(true);

            // Start coroutine to hide it after 1.5 seconds
            _hideTextCoroutine = StartCoroutine(HideTextAfterDelay(commentDelay));
        }
        
        private IEnumerator HideTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            commentatorText.gameObject.SetActive(false);
        }

        // Optional: Clean up on destroy
        private void OnDestroy()
        {
            _gameplayEventService?.Unsubscribe(GameplayTags.Event_Score, OnScore);
            if (_hideTextCoroutine != null)
            {
                StopCoroutine(_hideTextCoroutine);
            }
        }
    }
}