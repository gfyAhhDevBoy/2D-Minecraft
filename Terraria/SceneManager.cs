using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria
{
    internal class SceneManager
    {
        Stack<IScene> _sceneStack;

        public SceneManager()
        {
            _sceneStack = new();
        }

        public void LoadScene(IScene scene)
        {
            scene.Load();
            _sceneStack.Push(scene);
        }

        public void UnloadScene()
        {
            _sceneStack.Peek().Unload();
            _sceneStack.Pop();
        }

        public IScene GetCurrentScene() => _sceneStack.Peek();

    }
}
