using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public enum PlayerStates {
        idle = 0,
        left,
        right,
        jump,
        landing,
        falling,
        kill,
        resurrect,
        firingWeapon,
        _stateCount
    };

    public static float[] stateDelayTimer = new float[(int)PlayerStates._stateCount];

    public delegate void playerStateHandler(PlayerStates newState);

    public static event playerStateHandler OnStateChange;

    private GameObject mobileInput;

    private PlayerStateController ps;

    private void Start()
    {
        ps = GetComponent<PlayerStateController>();
        if(SystemInfo.deviceType != DeviceType.Handheld)
        {
            mobileInput = GameObject.Find("MobileInput");
            mobileInput.SetActive(false);
        }
        
    }


    void Update()
    {
        //if (!GameStates.gameActive) return;
        // Recoger el input horizontal actual
        if(SystemInfo.deviceType == DeviceType.Desktop)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetMouseButton(0))
            {
                ps.Aim();
                //if (OnStateChange != null) OnStateChange(PlayerStates.firingWeapon);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                ps.StopAim();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (OnStateChange != null) OnStateChange(PlayerStates.jump);
            }
            if (horizontal != 0f)
            {
                // Si hay input, se cambia a left o right
                if (horizontal < 0f)
                {

                    if (OnStateChange != null) OnStateChange(PlayerStates.left);
                }
                else
                {
                    if (OnStateChange != null) OnStateChange(PlayerStates.right);
                }
            }
            else
            {
                // Si no, se cambia a idle
                if (OnStateChange != null) OnStateChange(PlayerStates.idle);
            }
        }
        else if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            if(SimpleInput.GetButton("Attack"))
            {
                ps.Aim();
                //if (OnStateChange != null) OnStateChange(PlayerStates.firingWeapon);
            }
            else if(SimpleInput.GetButtonUp("Attack"))
            {
                ps.StopAim();
            }

            if(SimpleInput.GetButtonDown("Jump"))
            {
                if (OnStateChange != null) OnStateChange(PlayerStates.jump);
            }

            if(SimpleInput.GetButton("Left"))
            {
                if (OnStateChange != null) OnStateChange(PlayerStates.left);
            }
            else if(SimpleInput.GetButton("Right"))
            {
                if (OnStateChange != null) OnStateChange(PlayerStates.right);
            }
            else
            {
                if (OnStateChange != null) OnStateChange(PlayerStates.idle);
            }
        }
        
    }
}
