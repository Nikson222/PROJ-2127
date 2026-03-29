using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace _Scripts.Game
{
    public class InputController : ITickable
    {
        public event Action Clicked;

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return; // Клик был по UI — игнорируем

                Clicked?.Invoke(); // Клик по игровому миру
            }
        }
    }
}