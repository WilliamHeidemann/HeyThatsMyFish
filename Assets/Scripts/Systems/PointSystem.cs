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
        private Dictionary<Team, int> _scoreBoard = new();
        [SerializeField] private TextMeshProUGUI redPointsText;
        [SerializeField] private TextMeshProUGUI bluePointsText;
        [SerializeField] private TextMeshProUGUI greenPointsText;
        [SerializeField] private TextMeshProUGUI yellowPointsText;
        [SerializeField] private Sprite oneFish;
        [SerializeField] private Sprite twoFish;
        [SerializeField] private Sprite threeFish;
        [SerializeField] private GameObject award;
        [SerializeField] private LeanTweenType easeType;
        [SerializeField] private GameObject endGameScreen;
        [SerializeField] private TextMeshProUGUI endGameText;
        
        public void InitializeScoreBoard(int playerCount)
        {
            if (playerCount < 2) throw new Exception("Player count less than 2");
            _scoreBoard = new Dictionary<Team, int>
            {
                { Team.Red, 0 },
                { Team.Blue, 0 }
            };
            UpdatePointsGUI(Team.Red);
            UpdatePointsGUI(Team.Blue);
            
            if (playerCount > 2)
            {
                _scoreBoard.Add(Team.Green, 0);
                UpdatePointsGUI(Team.Green);
            }
            else greenPointsText.gameObject.SetActive(false);
            
            if (playerCount > 3)
            {
                _scoreBoard.Add(Team.Yellow, 0);
                UpdatePointsGUI(Team.Yellow);
            }
            else yellowPointsText.gameObject.SetActive(false);
            
            endGameScreen.SetActive(false);
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
                Team.Green => greenPointsText,
                Team.Yellow => yellowPointsText,
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
        
        public void EndGame()
        {
            endGameScreen.SetActive(true);
            if (_scoreBoard[Team.Red] == _scoreBoard[Team.Blue])
            {
                endGameText.color = Color.black;
                endGameText.text = "TIE";
            } 
            else if (_scoreBoard[Team.Red] > _scoreBoard[Team.Blue])
            {
                endGameText.color = Color.red;
                endGameText.text = "RED WINS";
            }
            else
            {
                endGameText.color = Color.blue;
                endGameText.text = "BLUE WINS";
            }
        }
    }
}