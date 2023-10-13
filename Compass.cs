using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalProject
{
    public class Compass : MonoBehaviour
    {
        [SerializeField] private RawImage _compassImage;
        [SerializeField] private Transform _playerTransform;
        private void Awake()
        {
            _playerTransform = FindObjectOfType<PlayerController>().transform;
            _compassImage = GetComponent<RawImage>();
        }
        private void Update()
        {
            _compassImage.uvRect = new Rect(-_playerTransform.localEulerAngles.y / 360, 0f, 1f, 1f);
        }
    }
}
