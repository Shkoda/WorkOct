using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Src.Scenes
{
    /// <summary>
    /// Abstrac scene controller. Used by SceneLoader to determine how to load scene correctly.
    /// </summary>
    public abstract class ASceneController
    {
        public SceneControllerWrapper Wrapper;

        /// <summary>
        /// Destroys every GameObject and Resource used in scene. Stops every running script.
        /// </summary>
        public abstract void DestroyScene();

        /// <summary>
        /// Should return true if associated scene should load on top of others.
        /// </summary>
        public abstract bool LoadAdditively { get; }

        /// <summary>
        /// Name of scene, which should be loaded
        /// </summary>
        public abstract string SceneName { get; }

        /// <summary>
        /// Method is called right before scene starts being loaded
        /// </summary>
        /// <param name="previousScene">Previous scene name</param>
        public virtual void OnBeforeLoad(string previousScene)
        {

        }

        /// <summary>
        /// Method is called right after scene is loaded
        /// </summary>
        public virtual void OnAfterLoad()
        {

        }
        /// <summary>
        /// Called after loading scene, can be used to copy data from controller passed to SceneLoader
        /// to controller which is added to scene
        /// </summary>
        /// <param name="newController"></param>
        public virtual void CopyFrom(ASceneController newController)
        {

        }

        public virtual void Update()
        {

        }

        public virtual void LateUpdate()
        {

        }

        public virtual void Start()
        {

        }

        public virtual void Awake()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public virtual void OnGUI()
        {

        }
    }
}
