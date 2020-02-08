// GENERATED AUTOMATICALLY FROM 'Assets/_Scripts/GAME/Runtime/Player/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""d71c33df-6462-4173-a72e-40e332fbacaf"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""1521ec31-9450-4c50-80aa-0911d6266b03"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""GunRotate"",
                    ""type"": ""Value"",
                    ""id"": ""861a1167-5d3f-43e1-aa27-29043999861c"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShootPress"",
                    ""type"": ""Button"",
                    ""id"": ""6731f24b-f1e6-4ba3-b534-25f970dc28e4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""ShootRelease"",
                    ""type"": ""Button"",
                    ""id"": ""95fe21b3-d596-4e78-8df6-fa2b2f657805"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b788bbe2-d291-434e-99a8-addc5730086c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""controller"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7f2cbf5f-7cae-4726-848a-957a137610e9"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""controller"",
                    ""action"": ""GunRotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0cd5a93-13d5-4613-97fb-fee8e928100c"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""controller"",
                    ""action"": ""ShootPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d185f045-e7c6-450d-8c7c-13d4d880481b"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""controller"",
                    ""action"": ""ShootRelease"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""controller"",
            ""bindingGroup"": ""controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_GunRotate = m_Player.FindAction("GunRotate", throwIfNotFound: true);
        m_Player_ShootPress = m_Player.FindAction("ShootPress", throwIfNotFound: true);
        m_Player_ShootRelease = m_Player.FindAction("ShootRelease", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_GunRotate;
    private readonly InputAction m_Player_ShootPress;
    private readonly InputAction m_Player_ShootRelease;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @GunRotate => m_Wrapper.m_Player_GunRotate;
        public InputAction @ShootPress => m_Wrapper.m_Player_ShootPress;
        public InputAction @ShootRelease => m_Wrapper.m_Player_ShootRelease;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                @GunRotate.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGunRotate;
                @GunRotate.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGunRotate;
                @GunRotate.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGunRotate;
                @ShootPress.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootPress;
                @ShootPress.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootPress;
                @ShootPress.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootPress;
                @ShootRelease.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootRelease;
                @ShootRelease.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootRelease;
                @ShootRelease.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShootRelease;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @GunRotate.started += instance.OnGunRotate;
                @GunRotate.performed += instance.OnGunRotate;
                @GunRotate.canceled += instance.OnGunRotate;
                @ShootPress.started += instance.OnShootPress;
                @ShootPress.performed += instance.OnShootPress;
                @ShootPress.canceled += instance.OnShootPress;
                @ShootRelease.started += instance.OnShootRelease;
                @ShootRelease.performed += instance.OnShootRelease;
                @ShootRelease.canceled += instance.OnShootRelease;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_controllerSchemeIndex = -1;
    public InputControlScheme controllerScheme
    {
        get
        {
            if (m_controllerSchemeIndex == -1) m_controllerSchemeIndex = asset.FindControlSchemeIndex("controller");
            return asset.controlSchemes[m_controllerSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnGunRotate(InputAction.CallbackContext context);
        void OnShootPress(InputAction.CallbackContext context);
        void OnShootRelease(InputAction.CallbackContext context);
    }
}
