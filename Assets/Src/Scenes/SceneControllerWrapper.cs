using UnityEngine;

namespace Assets.Src.Scenes
{
    public class SceneControllerWrapper : MonoBehaviour
    {
        public string Name;

        private ASceneController _controller;

        public void SetController(ASceneController controller)
        {
            _controller = controller;
            controller.Wrapper = this;
            Name = controller.SceneName + "Script";
        }

        public void Update()
        {
            _controller.Update();
        }

        public void Start()
        {
            _controller.Start();
        }

        public void Awake()
        {
            if (_controller != null)
            {
                _controller.Awake();
            }
            else
            {
                Debug.LogWarning("SceneControllerWrapper: Controller not set");
            }
        }

        public void LateUpdate()
        {
            _controller.LateUpdate();
        }

        public void OnDestroy()
        {
            _controller.OnDestroy();
        }

        public void OnGUI()
        {
            _controller.OnGUI();
        }
    }
}