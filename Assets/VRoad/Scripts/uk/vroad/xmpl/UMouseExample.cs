using uk.vroad.api;
using uk.vroad.api.input;
using uk.vroad.api.xmpl;
using uk.vroad.ucm;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace uk.vroad.xmpl
{
    public class UMouseExample : UaMouse
    {
        private ExampleApp app;

        protected override void Awake()
        {
            app = ExampleApp.AwakeInstance();
            base.Awake();
        }
        protected override App App() { return app; }

        protected override void HandleMouse(Mouse mouse, Keyboard kb)
        {
            base.HandleMouse(mouse, kb);
            
            AppInputHandler aih = App().Aih();

            ButtonControl mouseButtonPan = mouse.leftButton;
            ButtonControl mouseButtonTilt = mouse.middleButton;

            if (mouseButtonPan.wasPressedThisFrame)
            {
                prevPosMouse = mouse.position.ReadValue();
            }
            else if (mouseButtonTilt.wasPressedThisFrame)
            {
                prevPosMouse = mouse.position.ReadValue();
            }
            else if (mouseButtonPan.isPressed)
            {
                Vector2 currentPos =  mouse.position.ReadValue();
                float panX = SCALE_MOUSE_DRAG * (currentPos.x - prevPosMouse.x);
                float panY = -SCALE_MOUSE_DRAG * (currentPos.y - prevPosMouse.y);
                
                aih.FireAnalogEvent(AppAnalogFn.PanX, panX);
                aih.FireAnalogEvent(AppAnalogFn.PanY, panY);

                prevPosMouse = currentPos;
                wasDraggingMouse = true;
            }
            else if (mouseButtonTilt.isPressed)
            {
                Vector2 currentPos =  mouse.position.ReadValue();
                float tiltY = -SCALE_MOUSE_DRAG * (currentPos.y - prevPosMouse.y);
                aih.FireAnalogEvent(AppAnalogFn.Tilt, tiltY);

                prevPosMouse = currentPos;
                wasDraggingMouse = true;
            }
            else if (wasDraggingMouse)
            {
                bool shift = kb != null && kb.shiftKey.isPressed;
                if (!shift) aih.FireAnalogEvent(AppAnalogFn.PanX, 0);
                if (!shift) aih.FireAnalogEvent(AppAnalogFn.PanY, 0);
                if (!shift) aih.FireAnalogEvent(AppAnalogFn.Tilt, 0);
                
                wasDraggingMouse = false;
            }

        }
    }
}