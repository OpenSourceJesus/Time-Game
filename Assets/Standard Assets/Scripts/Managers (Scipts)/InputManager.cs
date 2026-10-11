using System;
using Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
	public InputSettings settings;
	public static InputSettings Settings
	{
		get
		{
			return Instance.settings;
		}
	}
	public InputActionReference moveInputActionRef;
	public InputActionReference jumpInputActionRef;
	public InputActionReference interactInputActionRef;
	public InputActionReference aimInputActionRef;
	public InputActionReference throwInputActionRef;
	public InputActionReference resetInputActionRef;
	public static bool UsingGamepad
	{
		get
		{
			return Gamepad.current != null;
		}
	}
	public static bool UsingMouse
	{
		get
		{
			return Mouse.current != null;
		}
	}
	public static bool UsingKeyboard
	{
		get
		{
			return Keyboard.current != null;
		}
	}
	public static bool UsingPhone
	{
		get
		{
#if UNITY_ANDROID || UNITY_IOS
			return true;
#else
			return false;
#endif
		}
	}
	public static bool LeftClickInput
	{
		get
		{
			return UsingMouse && Mouse.current.leftButton.isPressed;
		}
	}
	public bool _LeftClickInput
	{
		get
		{
			return LeftClickInput;
		}
	}
	public static bool RightClickInput
	{
		get
		{
			return UsingMouse && Mouse.current.rightButton.isPressed;
		}
	}
	public bool _RightClickInput
	{
		get
		{
			return RightClickInput;
		}
	}
	public static Vector2? MousePosition
	{
		get
		{
			if (UsingMouse)
				return Mouse.current.position.ReadValue();
			else
				return null;
		}
	}
	public Vector2? _MousePosition
	{
		get
		{
			return MousePosition;
		}
	}

	public override void Awake ()
	{
		base.Awake ();
		moveInputActionRef.action.Enable();
		jumpInputActionRef.action.Enable();
		interactInputActionRef.action.Enable();
		aimInputActionRef.action.Enable();
		throwInputActionRef.action.Enable();
		resetInputActionRef.action.Enable();
	}

	void OnDestroy ()
	{
		moveInputActionRef.action.Disable();
		jumpInputActionRef.action.Disable();
		interactInputActionRef.action.Disable();
		aimInputActionRef.action.Disable();
		throwInputActionRef.action.Disable();
		resetInputActionRef.action.Disable();
	}
}