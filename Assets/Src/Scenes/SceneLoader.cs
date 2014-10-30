using System.Collections.Generic;
using UnityEngine;
namespace Assets.Src.Scenes
{
     public static class SceneLoader
    {
        public static ASceneController CurrentController { get { return _prevControllers.Peek(); } }
        private static Stack<ASceneController> _prevControllers;
        private static ASceneController _newController;
        private static AsyncOperation _loader;

        static SceneLoader()
        {
            _prevControllers = new Stack<ASceneController>();
        }

        public static void LoadScene(ASceneController controller)
        {
            if (!LoadProgress.Equals(2))
            {
                Debugger.Log("Load scene failed: SceneLoader is busy", writeToUnityConsole: true);
                return;
            }

            controller.OnBeforeLoad(_prevControllers.Count != 0 ? _prevControllers.Peek().SceneName : "");

            if (controller.LoadAdditively)
            {
                if (controller.SceneName == null)
                {
                    _loader = new AsyncOperation();
                }
                else
                {
                    _loader = Application.LoadLevelAdditiveAsync(controller.SceneName);
                }
            }
            else
            {
                if (_prevControllers.Count > 0)
                {
                    foreach (var prevController in _prevControllers)
                    {
                        prevController.DestroyScene();

                        if (prevController.Wrapper != null)
                        {
                            Object.Destroy(prevController.Wrapper.gameObject);
                        }
                        else
                        {
                            Debug.LogWarning("prevController.Wrapper == null");
                        }
                    }
                    _prevControllers.Clear();
                }
                //Application.LoadLevel(controller.SceneName);
                if (controller.SceneName == null)
                {
                    _loader = new AsyncOperation();
                }
                else
                {
                    _loader = Application.LoadLevelAsync(controller.SceneName);
                }
            }

            _newController = controller;

        }

        public static void Update()
        {
            if (_loader != null && _loader.isDone)
            {
                _newController.OnAfterLoad();

                var go = new GameObject(_newController.SceneName + "MainObject");
                var controllerWrapper = go.AddComponent<SceneControllerWrapper>();
                controllerWrapper.SetController(_newController);
                _prevControllers.Push(_newController);

                _loader = null;
                Resources.UnloadUnusedAssets();
            }
        }

        public static float LoadProgress
        {
            get { return _loader != null ? _loader.progress : 2; }
        }

        public static void UnloadCurrent()
        {
            if (_prevControllers.Count > 1)
            {
                var current = _prevControllers.Pop();
                current.DestroyScene();
                Object.Destroy(current.Wrapper.gameObject);

                //Resources.UnloadUnusedAssets();
            }
        }

        public static ASceneController GetPreviousController()
        {
            if (_prevControllers.Count > 1)
            {
                //So lame this is...
                var current = _prevControllers.Pop();
                var aSceneController = _prevControllers.Peek();
                _prevControllers.Push(current);

                return aSceneController;
            }
            else
            {
                return null;
            }
        }

        public static void Reset()
        {
            _prevControllers = new Stack<ASceneController>();
            _newController = null;
            _loader = null;
        }
    }

}
