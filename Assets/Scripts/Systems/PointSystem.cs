using System;
using System.Collections.Generic;
using System.Linq;
using Logic;
using TMPro;
using UnityEngine;

namespace Systems
{
    public class PointSystem : MonoBehaviour
    {
        private readonly Dictionary<Team, int> _scoreBoard = new();
        [SerializeField] private TextMeshProUGUI redPointsText;
        [SerializeField] private TextMeshProUGUI bluePointsText;
        [SerializeField] private Sprite oneFish;
        [SerializeField] private Sprite twoFish;
        [SerializeField] private Sprite threeFish;
        [SerializeField] private GameObject award;
        [SerializeField] private LeanTweenType easeType;
        
        private void Start()
        {
            _scoreBoard.Add(Team.Red, 0);
            _scoreBoard.Add(Team.Blue, 0);
            UpdatePointsGUI(Team.Red);
            UpdatePointsGUI(Team.Blue);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) AnimateAward(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) AnimateAward(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) AnimateAward(3);
        }

        public void SetFishSprites(IEnumerable<Tile> tiles)
        {
            var locationsAndSpriteRenderers = FindObjectsByType<InteractableTile>(FindObjectsSortMode.None)
                .Select(tile => (tile.location, tile.GetComponent<SpriteRenderer>())).ToList();
            
            locationsAndSpriteRenderers.ForEach(tuple =>
            {
                var tile = tiles.First(tile => tile.Location == tuple.location);
                tuple.Item2.sprite = tile.FishCount switch
                {
                    1 => oneFish,
                    2 => twoFish,
                    3 => threeFish,
                    _ => throw new ArgumentOutOfRangeException()
                };
            });

        }

        public void AwardPoints(int amount, Team team)
        {
            _scoreBoard[team] += amount;
            UpdatePointsGUI(team);
            AnimateAward(amount);
        }
        
        private void UpdatePointsGUI(Team team)
        {
            var textGUI = team switch
            {
                Team.Red => redPointsText,
                Team.Blue => bluePointsText,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
            textGUI.text = $"{team.ToString()}: {_scoreBoard[team].ToString()}";
        }

        private void AnimateAward(int amount)
        {
            if (amount <= 0) return;
            var awardPopUp = Instantiate(award, award.transform.parent);
            awardPopUp.GetComponent<TextMeshProUGUI>().text = $"+{amount}";
            var size = amount switch
            {
                1 => 2f,
                2 => 3f,
                3 => 5f,
                _ => 5f
            };
            
            awardPopUp.SetActive(true);
            LeanTween.scale(awardPopUp, Vector3.one * size, 1).setEase(easeType).setDestroyOnComplete(true);
        }
    }
}