using System.Collections;
using System.Collections.Generic;
using _Scripts._Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MaxScoreDisplayer : MonoBehaviour
{
    [SerializeField] private Text _maxScoreText;
    private PlayerProfile _playerProfile;

    [Inject]
    private void Construct(PlayerProfile playerProfile)
    {
        _playerProfile = playerProfile;
    }
    
    void Start()
    {
        _maxScoreText.text = _playerProfile.MaxScore.ToString();
    }
}
