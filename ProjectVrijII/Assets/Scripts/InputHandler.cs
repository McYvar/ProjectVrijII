using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour {
    public Action<InputHandler> OnReassignment = null;

    public Vector2 leftJoyDirection;
    public Vector2 rightJoyDirection;

    public Vector2 leftDPadDirection;
    public Action UpFirst;
    public Action DownFirst;
    public Action LeftFirst;
    public Action RightFirst;
    public Action UpLast;
    public Action DownLast;
    public Action LeftLast;
    public Action RightLast;

    public Vector2 leftDirection;

    public float leftTrigger;
    public float rightTrigger;

    public bool leftShoulder;
    public Action leftShoulderFirst;
    public Action leftShoulderLast;

    public bool rightShoulder;
    public Action rightShoulderFirst;
    public Action rightShoulderLast;

    public bool north;
    public Action northFirst;
    public Action northLast;

    public bool east;
    public Action eastFirst;
    public Action eastLast;

    public bool south;
    public Action southFirst;
    public Action southLast;

    public bool west;
    public Action westFirst;
    public Action westLast;

    public bool leftPress;
    public Action leftPressFirst;
    public Action leftPressLast;

    public bool rightPress;
    public Action rightPressFirst;
    public Action rightPressLast;

    public bool options;
    public Action optionsFirst;
    public Action optionsLast;

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void LeftJoy(InputAction.CallbackContext cc) {
        Vector2 direction = cc.ReadValue<Vector2>();
        leftJoyDirection = direction;
        leftDirection = direction;
    }

    public void RightJoy(InputAction.CallbackContext cc) {
        rightJoyDirection = cc.ReadValue<Vector2>();
    }

    public void LeftDPad(InputAction.CallbackContext cc) {
        Vector2 direction = cc.ReadValue<Vector2>();
        leftDPadDirection = direction;
        leftDirection = direction;

        if (cc.started) {
            if (direction == Vector2.up) UpFirst?.Invoke();
            else if (direction == Vector2.down) DownFirst?.Invoke();
            else if (direction == Vector2.left) LeftFirst?.Invoke();
            else if (direction == Vector2.right) RightFirst?.Invoke();
            else if (direction.x > 0 && direction.y > 0) {
                UpFirst?.Invoke();
                RightFirst?.Invoke();
            } // upper right
            else if (direction.x > 0 && direction.y < 0) {
                DownFirst?.Invoke();
                RightFirst?.Invoke();
            } // lower right
            else if (direction.x < 0 && direction.y > 0) {
                UpFirst?.Invoke();
                LeftFirst?.Invoke();
            } // upper left
            else if (direction.x < 0 && direction.y < 0) {
                DownFirst?.Invoke();
                LeftFirst?.Invoke();
            } // lower left
        } else if (cc.canceled) {
            if (direction == Vector2.up) UpLast?.Invoke();
            else if (direction == Vector2.down) DownLast?.Invoke();
            else if (direction == Vector2.left) LeftLast?.Invoke();
            else if (direction == Vector2.right) RightLast?.Invoke();
            else if (direction.x > 0 && direction.y > 0) {
                UpLast?.Invoke();
                RightLast?.Invoke();
            } // upper right
            else if (direction.x > 0 && direction.y < 0) {
                DownLast?.Invoke();
                RightLast?.Invoke();
            } // lower right
            else if (direction.x < 0 && direction.y > 0) {
                UpLast?.Invoke();
                LeftLast?.Invoke();
            } // upper left
            else if (direction.x < 0 && direction.y < 0) {
                DownLast?.Invoke();
                LeftLast?.Invoke();
            } // lower left
        }
    }

    public void LeftTrigger(InputAction.CallbackContext cc) {
        leftTrigger = cc.ReadValue<float>();
    }

    public void RightTrigger(InputAction.CallbackContext cc) {
        rightTrigger = cc.ReadValue<float>();
    }

    public void LeftShoulder(InputAction.CallbackContext cc) {
        if (cc.started) {
            leftShoulder = true;
            leftShoulderFirst?.Invoke();
        } else if (cc.canceled) {
            leftShoulder = false;
            leftShoulderLast?.Invoke();
        }
    }

    public void RightShoulder(InputAction.CallbackContext cc) {
        if (cc.started) {
            rightShoulder = true;
            rightShoulderFirst?.Invoke();
        } else if (cc.canceled) {
            rightShoulder = false;
            rightShoulderLast?.Invoke();
        }
    }

    public void North(InputAction.CallbackContext cc) {
        if (cc.started) {
            north = true;
            northFirst?.Invoke();
        } else if (cc.canceled) {
            north = false;
            northLast?.Invoke();
        }
    }

    public void East(InputAction.CallbackContext cc) {
        if (cc.started) {
            east = true;
            eastFirst?.Invoke();
        } else if (cc.canceled) {
            east = false;
            eastLast?.Invoke();
        }
    }

    public void South(InputAction.CallbackContext cc) {
        if (cc.started) {
            south = true;
            southFirst?.Invoke();
        } else if (cc.canceled) {
            south = false;
            southLast?.Invoke();
        }
    }

    public void West(InputAction.CallbackContext cc) {
        if (cc.started) {
            west = true;
            westFirst?.Invoke();
        } else if (cc.canceled) {
            west = false;
            westLast?.Invoke();
        }
    }

    public void LeftPress(InputAction.CallbackContext cc) {
        if (cc.started) {
            leftPress = true;
            leftPressFirst?.Invoke();
        } else if (cc.canceled) {
            leftPress = false;
            leftPressLast?.Invoke();
        }
    }

    public void RightPress(InputAction.CallbackContext cc) {
        if (cc.started) {
            rightPress = true;
            rightPressFirst?.Invoke();
        } else if (cc.canceled) {
            rightPress = false;
            rightPressLast?.Invoke();
        }
    }

    public void Options(InputAction.CallbackContext cc) {
        if (cc.started) {
            options = true;
            optionsFirst?.Invoke();
        } else if (cc.canceled) {
            options = false;
            optionsLast?.Invoke();
        }
    }

    public void CopyTo(InputHandler other) {
        other.OnReassignment = this.OnReassignment;
        other.UpFirst = this.UpFirst;
        other.DownFirst = this.DownFirst;
        other.LeftFirst = this.LeftFirst;
        other.RightFirst = this.RightFirst;
        other.UpLast = this.UpLast;
        other.DownLast = this.DownLast;
        other.LeftLast = this.LeftLast;
        other.RightLast = this.RightLast;

        other.leftShoulderFirst = this.leftShoulderFirst;
        other.leftShoulderLast = this.leftShoulderLast;

        other.rightShoulderFirst = this.rightShoulderFirst;
        other.rightShoulderLast = this.rightShoulderLast;

        other.northFirst = this.northFirst;
        other.northLast = this.northLast;

        other.eastFirst = this.eastFirst;
        other.eastLast = this.eastLast;

        other.southFirst = this.southFirst;
        other.southLast = this.southLast;

        other.westFirst = this.westFirst;
        other.westLast = this.westLast;

        other.leftPressFirst = this.leftPressFirst;
        other.leftPressLast = this.leftPressLast;

        other.rightPressFirst = this.rightPressFirst;
        other.rightPressLast = this.rightPressLast;

        other.optionsFirst = this.optionsFirst;
        other.optionsLast = this.optionsLast;
    }

    public void ResetBindings() {
        UpFirst = null;
        DownFirst = null;
        LeftFirst = null;
        RightFirst = null;
        UpLast = null;
        DownLast = null;
        LeftLast = null;
        RightLast = null;

        leftShoulderFirst = null;
        leftShoulderLast = null;

        rightShoulderFirst = null;
        rightShoulderLast = null;

        northFirst = null;
        northLast = null;

        eastFirst = null;
        eastLast = null;

        southFirst = null;
        southLast = null;

        westFirst = null;
        westLast = null;

        leftPressFirst = null;
        leftPressLast = null;

        rightPressFirst = null;
        rightPressLast = null;

        optionsFirst = null;
        optionsLast = null;

    }
}
