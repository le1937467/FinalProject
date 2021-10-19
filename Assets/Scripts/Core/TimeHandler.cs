using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace LDS.GameProgramming
{
    // ------------------------------------------------------------------------
    public class TimeHandler : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 144)]
        private int _TargetFrameRate = 60;

        private Coroutine _CoroutineSlowDown;
        private bool _GamePaused;
        private bool _VSyncEnabled;
        private int _DirtyDeltaTime = -1;
        private int _DirtyFixedDeltaTime = -1;
        private float _DeltaTime;
        private float _FixedDeltaTime;
        private WaitUntil _WaitUntilGameNotPaused;
        
        private Stopwatch _Stopwatch;

        private static TimeHandler _Instance;

        #region Properties

        // ------------------------------------------------------------------------
        private static TimeHandler Instance
        {
            set
            {
                if (_Instance == null)
                {
                    _Instance = value;
                }
            }
        }

        // ------------------------------------------------------------------------
        public static bool VSync
        {
            set
            {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                _Instance._VSyncEnabled = value;

                var vSyncCount = value ? 1 : 0;
                if (QualitySettings.vSyncCount != vSyncCount)
                {
                    QualitySettings.vSyncCount = value ? 1 : 0;
                }
#else
                QualitySettings.vSyncCount = 1;
#endif
            }
        }

        // ------------------------------------------------------------------------
        public static int Frame
        {
            get
            {
                return Time.frameCount;
            }
        }

        // ------------------------------------------------------------------------
        public static float DeltaTime
        {
            get
            {
                if (_Instance == null)
                {
                    return Time.deltaTime;
                }

                if (Math.Abs(Time.timeScale) < float.Epsilon)
                {
                    return 0f;
                }

                var frameCount = Time.frameCount;
                if (_Instance._DirtyDeltaTime != frameCount)
                {
                    _Instance._DirtyDeltaTime = frameCount;
                    _Instance._DeltaTime = Time.deltaTime;
                }

                return _Instance._DeltaTime;
            }
        }

        // ------------------------------------------------------------------------
        public static float UnscaledDeltaTime
        {
            get
            {
                return Time.unscaledDeltaTime;
            }
        }

        // ------------------------------------------------------------------------
        public static float FixedDeltaTime
        {
            get
            {
                if (_Instance == null)
                {
                    return Time.fixedDeltaTime;
                }

                if (Math.Abs(Time.timeScale) < float.Epsilon)
                {
                    return 0f;
                }

                var frameCount = Time.frameCount;
                if (_Instance._DirtyFixedDeltaTime != frameCount)
                {
                    _Instance._DirtyFixedDeltaTime = frameCount;
                    _Instance._FixedDeltaTime = Time.fixedDeltaTime;
                }

                return _Instance._FixedDeltaTime;
            }
        }

        // ------------------------------------------------------------------------
        public static float GameTime
        {
            get
            {
                return Time.time;
            }
        }

        // ------------------------------------------------------------------------
        public static float UnscaledGameTime
        {
            get
            {
                return Time.unscaledTime;
            }
        }

        // ------------------------------------------------------------------------
        public static bool GamePaused
        {
            get
            {
                return _Instance._GamePaused;
            }
        }

        // ------------------------------------------------------------------------
        public static CustomYieldInstruction YieldUntilNextFrame
        {
            get
            {
                if (_Instance._GamePaused)
                {
                    return _Instance._WaitUntilGameNotPaused;
                }

                return null;
            }
        }

        // ------------------------------------------------------------------------
        public static CustomYieldInstruction YieldUntilNextFrameRealtime
        {
            get
            {
                return null;
            }
        }

        // ------------------------------------------------------------------------
        public static CustomYieldInstruction YieldUntilNextFramePowerAttack
        {
            get
            {
                if (_Instance._GamePaused)
                {
                    return _Instance._WaitUntilGameNotPaused;
                }

                return null;
            }
        }

        // ------------------------------------------------------------------------
        public static Stopwatch Stopwatch
        {
            get
            {
                return _Instance._Stopwatch;
            }
            set
            {
                _Instance._Stopwatch = value;
            }
        }

        #endregion

        #region Unity Methods

        // ------------------------------------------------------------------------
        private void Awake()
        {
            Instance = this;
            _Stopwatch = new Stopwatch();

            _WaitUntilGameNotPaused = new WaitUntil(() => !_GamePaused);
            _VSyncEnabled = QualitySettings.vSyncCount != 0;

            Application.targetFrameRate = 60;
        }

        // ------------------------------------------------------------------------
        private void Update()
        {
            Application.targetFrameRate = _TargetFrameRate;
        }

        #endregion

        #region Class Methods

        // ------------------------------------------------------------------------
        public static void HitFramePause(bool pause)
        {
            if (pause)
            {
                Time.timeScale = 0.001f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        #endregion

        #region Coroutines      
        #endregion

        #region Events
        #endregion
    }
}