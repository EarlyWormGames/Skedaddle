using UnityEngine;
using System;
using System.Reflection;
using XInputDotNetPure;

public enum ControllerButtons
{
    A,
    B,
    Back,
    Guide,
    LeftShoulder,
    LeftStick,
    RightShoulder,
    RightStick,
    Start,
    X,
    Y
}

public enum ControllerDpad
{
    Down,
    Left,
    Right,
    Up
}

public class Controller : MonoBehaviour
{
    public const float TRIGGER_DEADZONE = 0.2f;
    public const float STICK_DEADZONE = 0.6f;

    public static Controller Instance
    {
        get
        {
            if (m_Instance != null)
                return m_Instance;

            GameObject obj = new GameObject();
            m_Instance = obj.AddComponent<Controller>();
            return m_Instance;
        }
    }

    static Controller m_Instance;
    GamePadState m_PrevState;
    GamePadState m_State;

    void Awake()
    {
        m_State = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
    }

    void Update()
    {
        m_PrevState = m_State;
        m_State = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
    }

    public static bool AnyButtonDown()
    {
        if (!Instance.m_State.IsConnected)
            return false;


        //BUTTONS
        ControllerButtons[] buttons = Enum.GetValues(typeof(ControllerButtons)) as ControllerButtons[];
        for (int i = 0; i < buttons.Length; ++i)
        {
            if (GetButton(buttons[i]))
                return true;
        }

        //DPAD
        ControllerDpad[] dpad = Enum.GetValues(typeof(ControllerDpad)) as ControllerDpad[];
        for (int i = 0; i < dpad.Length; ++i)
        {
            if (GetDpad(dpad[i]))
                return true;
        }



        //THUMBSTICKS
        if (GetStick(true) != Vector2.zero)
            return true;

        if (GetStick(false) != Vector2.zero)
            return true;


        //TRIGGERS
        if (GetTrigger(true) >= TRIGGER_DEADZONE)
            return true;

        if (GetTrigger(false) >= TRIGGER_DEADZONE)
            return true;

        return false;
    }

    //=====================================================
    #region Buttons
    public static bool GetButton(ControllerButtons a_button)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        return ((ButtonState)Instance.m_State.Buttons.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_State.Buttons, null)) == ButtonState.Pressed;
    }

    public static bool GetButtonDown(ControllerButtons a_button)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        ButtonState prevState = (ButtonState)Instance.m_PrevState.Buttons.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_PrevState.Buttons, null);
        ButtonState nowState = (ButtonState)Instance.m_State.Buttons.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_State.Buttons, null);

        return prevState == ButtonState.Released && nowState == ButtonState.Pressed;
    }

    public static bool GetButtonUp(ControllerButtons a_button)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        ButtonState prevState = (ButtonState)Instance.m_PrevState.Buttons.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_PrevState.Buttons, null);
        ButtonState nowState = (ButtonState)Instance.m_State.Buttons.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_State.Buttons, null);

        return prevState == ButtonState.Pressed && nowState == ButtonState.Released;
    }
    #endregion
    //=====================================================

    //=====================================================
    #region DPAD
    public static bool GetDpad(ControllerDpad a_button)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        return ((ButtonState)Instance.m_State.DPad.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_State.DPad, null)) == ButtonState.Pressed;
    }

    public static bool GetDpadDown(ControllerDpad a_button)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        ButtonState prevState = (ButtonState)Instance.m_PrevState.DPad.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_PrevState.DPad, null);
        ButtonState nowState = (ButtonState)Instance.m_State.DPad.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_State.DPad, null);

        return prevState == ButtonState.Released && nowState == ButtonState.Pressed;
    }

    public static bool GetDpadUp(ControllerDpad a_button)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        ButtonState prevState = (ButtonState)Instance.m_PrevState.DPad.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_PrevState.DPad, null);
        ButtonState nowState = (ButtonState)Instance.m_State.DPad.GetType().GetProperty(a_button.ToString()).GetValue(Instance.m_State.DPad, null);

        return prevState == ButtonState.Pressed && nowState == ButtonState.Released;
    }
    #endregion
    //=====================================================

    //=====================================================
    #region Sticks
    public static Vector2 GetStick(bool a_isLeftStick)
    {
        if (!Instance.m_State.IsConnected)
            return Vector2.zero;

        Vector2 vec = Vector2.zero;
        vec.x = a_isLeftStick ? Instance.m_State.ThumbSticks.Left.X : Instance.m_State.ThumbSticks.Right.X;
        vec.y = a_isLeftStick ? Instance.m_State.ThumbSticks.Left.Y : Instance.m_State.ThumbSticks.Right.Y;
        return vec;
    }

    public static bool GetStickPositionDown(bool a_isLeftStick, ControllerDpad a_direction)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        switch (a_direction)
        {
            case ControllerDpad.Down:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.Y : Instance.m_PrevState.ThumbSticks.Right.Y) >= -STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.Y : Instance.m_State.ThumbSticks.Right.Y) < -STICK_DEADZONE)
                        return true;
                    break;
                }
            case ControllerDpad.Left:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.X : Instance.m_PrevState.ThumbSticks.Right.X) >= -STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.X : Instance.m_State.ThumbSticks.Right.X) < -STICK_DEADZONE)
                        return true;
                    break;
                }
            case ControllerDpad.Right:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.X : Instance.m_PrevState.ThumbSticks.Right.X) <= STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.X : Instance.m_State.ThumbSticks.Right.X) > STICK_DEADZONE)
                        return true;
                    break;
                }
            case ControllerDpad.Up:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.Y : Instance.m_PrevState.ThumbSticks.Right.Y) <= STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.Y : Instance.m_State.ThumbSticks.Right.Y) > STICK_DEADZONE)
                        return true;
                    break;
                }
        }
        return false;
    }

    public static bool GetStickPositionUp(bool a_isLeftStick, ControllerDpad a_direction)
    {
        switch (a_direction)
        {
            case ControllerDpad.Down:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.Y : Instance.m_PrevState.ThumbSticks.Right.Y) < -STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.Y : Instance.m_State.ThumbSticks.Right.Y) >= -STICK_DEADZONE)
                        return true;
                    break;
                }
            case ControllerDpad.Left:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.X : Instance.m_PrevState.ThumbSticks.Right.X) < -STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.X : Instance.m_State.ThumbSticks.Right.X) >= -STICK_DEADZONE)
                        return true;
                    break;
                }
            case ControllerDpad.Right:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.X : Instance.m_PrevState.ThumbSticks.Right.X) > STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.X : Instance.m_State.ThumbSticks.Right.X) <= STICK_DEADZONE)
                        return true;
                    break;
                }
            case ControllerDpad.Up:
                {
                    if ((a_isLeftStick ? Instance.m_PrevState.ThumbSticks.Left.Y : Instance.m_PrevState.ThumbSticks.Right.Y) > STICK_DEADZONE &&
                        (a_isLeftStick ? Instance.m_State.ThumbSticks.Left.Y : Instance.m_State.ThumbSticks.Right.Y) <= STICK_DEADZONE)
                        return true;
                    break;
                }
        }
        return false;
    }
    #endregion
    //=====================================================

    //=====================================================
    #region Triggers
    public static float GetTrigger(bool a_isLeft)
    {
        if (!Instance.m_State.IsConnected)
            return 0;
        return a_isLeft ? Instance.m_State.Triggers.Left : Instance.m_State.Triggers.Right;
    }

    public static bool GetTriggerDown(bool a_isLeft)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        float prev = a_isLeft ? Instance.m_PrevState.Triggers.Left : Instance.m_PrevState.Triggers.Right;
        float now = a_isLeft ? Instance.m_State.Triggers.Left : Instance.m_State.Triggers.Right;

        return prev < TRIGGER_DEADZONE && now >= TRIGGER_DEADZONE;
    }

    public static bool GetTriggerUp(bool a_isLeft)
    {
        if (!Instance.m_State.IsConnected)
            return false;

        float prev = a_isLeft ? Instance.m_PrevState.Triggers.Left : Instance.m_PrevState.Triggers.Right;
        float now = a_isLeft ? Instance.m_State.Triggers.Left : Instance.m_State.Triggers.Right;

        return prev >= TRIGGER_DEADZONE && now < TRIGGER_DEADZONE;
    }
    #endregion
    //=====================================================

    public static bool Connected
    {
        get
        {
            return Instance.m_State.IsConnected;
        }
    }
}
