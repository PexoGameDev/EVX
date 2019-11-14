﻿using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class UIController
    {
        private readonly Canvas _canvas;

        private readonly Button _choiceLeft;
        private readonly Text _choiceLeftText;
        private readonly Canvas _choiceMenu;
        private readonly Button _choiceMid;
        private readonly Text _choiceMidText;
        private readonly Button _choiceRight;
        private readonly Text _choiceRightText;

        private readonly Text _choiceText;
        private readonly GameController _gc;

        public readonly HQUIManager HQUIManager;
        private GameObject _panelPrefab;
        public UpgradeUIManager UpgradeManager;

        public UIController(GameController gc, Canvas mainCanvas, Canvas choiceMenu, Text choiceText, Button choiceLeft,
            Button choiceMid, Button choiceRight, Button hqSoldiersTile, Text nameText, Text describText, SpriteRenderer[] levelPoints,
            Slider expSlider)
        {
            _gc = gc;
            _canvas = mainCanvas;
            _choiceMenu = choiceMenu;
            _choiceText = choiceText;

            _choiceLeft = choiceLeft;
            _choiceLeftText = choiceLeft.GetComponentInChildren<Text>();

            _choiceMid = choiceMid;
            _choiceMidText = choiceMid.GetComponentInChildren<Text>();

            _choiceRight = choiceRight;
            _choiceRightText = choiceRight.GetComponentInChildren<Text>();

            HQUIManager = new HQUIManager(hqSoldiersTile);
            UpgradeManager = new UpgradeUIManager(choiceMenu, choiceLeft, choiceMid, choiceRight, nameText, describText, levelPoints,
                expSlider);
        }

        public void Instantiate()
        {
            HQUIManager.Instantiate(_canvas, SetButtonsBehaviour, _gc.Hqm, _gc.EnemiesController,
                _gc.SoldiersController);
        }

        public async Task SetButtonsBehaviour(string title, string leftButtonName, string midButtonName,
            string rightButtonName, Action leftButtonBehaviour, Action middleButtonBehaviour,
            Action rightButtonBehaviour)
        {
            var chosen = false;

            _choiceText.text = title;
            _choiceLeftText.text = leftButtonName;
            _choiceMidText.text = midButtonName;
            _choiceRightText.text = rightButtonName;

            _choiceMenu.gameObject.SetActive(true);
            _choiceLeft.gameObject.SetActive(true);
            _choiceMid.gameObject.SetActive(true);
            _choiceRight.gameObject.SetActive(true);

            if (leftButtonBehaviour == null) _choiceLeft.gameObject.SetActive(false);
            if (middleButtonBehaviour == null) _choiceMid.gameObject.SetActive(false);
            if (rightButtonBehaviour == null) _choiceRight.gameObject.SetActive(false);

            _choiceLeft.onClick.RemoveAllListeners();
            _choiceMid.onClick.RemoveAllListeners();
            _choiceRight.onClick.RemoveAllListeners();

            _choiceLeft.onClick.AddListener(() =>
            {
                leftButtonBehaviour?.Invoke();
                chosen = true;
            });

            _choiceMid.onClick.AddListener(() =>
            {
                middleButtonBehaviour?.Invoke();
                chosen = true;
            });

            _choiceRight.onClick.AddListener(() =>
            {
                rightButtonBehaviour?.Invoke();
                chosen = true;
            });

            await Task.Run(() =>
            {
                while (!chosen)
                {
                }
            });

            _choiceMenu.gameObject.SetActive(false);
        }
    }
}