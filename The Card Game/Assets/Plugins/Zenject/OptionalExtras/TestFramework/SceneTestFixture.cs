#if UNITY_EDITOR

using Zenject.Internal;
using ModestTree;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using System.Collections;
using UnityEngine.TestTools;
using Assert = ModestTree.Assert;
using System.Linq;

// Ignore warning about using SceneManager.UnloadScene instead of SceneManager.UnloadSceneAsync
#pragma warning disable 618

namespace Zenject
{
    public abstract class SceneTestFixture
    {
        bool _hasLoadedScene;

        protected DiContainer SceneContainer
        {
            get; private set;
        }

        public IEnumerator LoadScene(string sceneName)
        {
            Assert.That(!_hasLoadedScene, "Attempted to load scene '{0}' twice", sceneName);
            _hasLoadedScene = true;

            // Clean up any leftovers from previous test
            ZenjectTestUtil.DestroyEverythingExceptTestRunner(false);

            Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                "Cannot load scene '{0}' for test '{1}'.  The scenes used by SceneTestFixture derived classes must be added to the build settings for the test to work",
                sceneName, GetType());

            var loader = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!loader.isDone)
            {
                yield return null;
            }

            if (ProjectContext.HasInstance)
            {
                var sceneContext = ProjectContext.Instance.Container.Resolve<SceneContextRegistry>()
                    .TryGetSceneContextForScene(SceneManager.GetSceneByName(sceneName));

                if (sceneContext != null)
                {
                    SceneContainer = sceneContext.Container;
                    SceneContainer.Inject(this);
                }
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
            StaticContext.Clear();
            _hasLoadedScene = false;
        }

        [TearDown]
        public virtual void Teardown()
        {
            ZenjectTestUtil.DestroyEverythingExceptTestRunner(true);
            StaticContext.Clear();
        }
    }
}

#endif
