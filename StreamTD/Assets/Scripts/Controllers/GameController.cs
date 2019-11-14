﻿using System.Collections;
using System.Linq;
using Assets.Scripts.Attacks;
using Assets.Scripts.Units;
using Assets.Scripts.Units.Enemy;
using Assets.Scripts.Units.Soldier;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        public static System.Random RandomGenerator = new System.Random();
        public MapGrid Map { get; private set; }
        public UIController UIController { get; private set; }
        public ProjectilesController ProjectilesController { get; private set; }
        public PlayerBase PlayerBase { get; private set; }
        public ScoreController ScoreController { get; private set; } = new ScoreController();
        public HQManager Hqm { get; private set; }

        private static GameMode _mode = GameMode.Play;

        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private GameObject _mapFieldObjectPrefab;
        [SerializeField] private GameObject _projectilePrefab;

        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private Canvas _mapCanvas;
        [SerializeField] private Canvas _choiceModalCanvas;
        [SerializeField] private Text _choiceText;

        [SerializeField] private Button _choiceLeft;
        [SerializeField] private Button _choiceMid;
        [SerializeField] private Button _choiceRight;


        [SerializeField] private Text _nameText;
        [SerializeField] private Text _describText;
        [SerializeField] private Button _hqSoldiersTilesButton;
        [SerializeField] private SpriteRenderer[] _levelPoints;
        [SerializeField] private Slider _expSlider;

        public EnemiesController EnemiesController { get; set; }
        public SoldiersController SoldiersController { get; set; }

        public static GameMode Mode
        {
            get => _mode;
            set => SetMode(value);
        }

        private static void SetMode(GameMode value)
        {
            _mode = value;
        }

        private void Start()
        {
            EnemiesController = new EnemiesController(this);
            SoldiersController = new SoldiersController(this);
            Hqm = new HQManager(SoldiersController);

            UIController = new UIController(this, _mainCanvas, _choiceModalCanvas, _choiceText, _choiceLeft, _choiceMid, _choiceRight, _hqSoldiersTilesButton, _nameText, _describText, _levelPoints, _expSlider);
            UIController.Instantiate();

            Map = new MapGrid(_mapFieldObjectPrefab, _mapCanvas, this);
            PlayerBase = new PlayerBase(SoldiersController);
            PlayerBase.Tile = Map.Path.Last();

            ProjectilesController = new ProjectilesController(_projectilePrefab, _mainCanvas);

            EnemiesController.EnemyPrefab = _enemyPrefab;
            EnemiesController.ParentCanvas = _mainCanvas;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (Mode)
                {
                    case GameMode.Play:
                        Mode = GameMode.Pause;
                        break;
                    case GameMode.Pause:
                        Mode = GameMode.Play;
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) _gameSpeed = 1;
            if (Input.GetKeyDown(KeyCode.Alpha2)) _gameSpeed = 2;
            if (Input.GetKeyDown(KeyCode.Alpha3)) _gameSpeed = 3;
            if (Input.GetKeyDown(KeyCode.Alpha4)) _gameSpeed = 4;
            if (Input.GetKeyDown(KeyCode.Alpha5)) _gameSpeed = 5;

            ProcessGameLoop();
        }

        private int _gameSpeed = 1;

        private void ProcessGameLoop()
        {
            if (Mode == GameMode.Play)
            {
                for (int i = 0; i < _gameSpeed; i++)
                {
                    ProjectilesController.ProcessProjectiles();
                    SoldiersController.ProcessActions();
                    EnemiesController.ProcessActions();
                    CooldownController.UpdateCooldowns();
                    GenerateWave();
                }
            }

            UIController.UpgradeManager.Render();
        }

        private float _enemySpawnCooldown = 3f;
        private float _currentEnemySpawnCooldown;

        private void GenerateWave()
        {

            if (_currentEnemySpawnCooldown > 0)
            {
                _currentEnemySpawnCooldown -= Time.deltaTime;
                return;
            }

            SpawnEnemy();
            _currentEnemySpawnCooldown = _enemySpawnCooldown;
        }

        private float _difficulty = 0.001f;
        private void SpawnEnemy()
        {
            _difficulty += 0.005f;

            UnitParameters defaultEnemySettings;
            if (RandomGenerator.Next(0, 101) < 25)
            {
                defaultEnemySettings = new UnitParameters { MovementSpeed = 0.0012f, Damage = 1, AttackRange = 1, MaxHealth = 400, Health = 400, AttacksPerSecond = 0.5f };
                _enemySpawnCooldown = 3f;
            }
            else 
            {
                defaultEnemySettings = new UnitParameters { MovementSpeed = 0.010f, Damage = 1, AttackRange = 1, MaxHealth = 40, Health = 40, AttacksPerSecond = 2f };
                _enemySpawnCooldown = 0.75f;
            }

            defaultEnemySettings = defaultEnemySettings.IncreaseAllByFactor(_difficulty);
            EnemiesController.SpawnEnemy(new DummyEnemy(Map.Path.ToArray(), SoldiersController, EnemiesController, PlayerBase, (DamageType)RandomGenerator.Next(1,4), (HealthType)RandomGenerator.Next(1, 4), defaultEnemySettings));
        }
    }

    public enum GameMode
    {
        Play = 0,
        Building = 1,
        Upgrading = 2,
        Pause = 3
    }
}