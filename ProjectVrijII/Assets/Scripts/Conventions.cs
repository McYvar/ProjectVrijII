using UnityEngine;
using UnityEngine.Events;

namespace conventions {
    // interfaces = IUpperCamelCasing
    public interface IAmInterface {

    }

    public class Conventions : MonoBehaviour {
        // Unity game loop functions at the top; Awake, OnEnable, Start, Update, FixedUpdate.
        
        // always add private / public
        // properties / global variables = lowerCamelCasing
        private GameObject globalVaraible;
        float f1, f2, f3; // clariy, otherwise under each other

        // actions/events
        public UnityEvent OnEventTrigger; // UpperCamelCasing + On

        // local enums
        public enum LocalGameEnum {
            ENUM_ONE = 0,
            ENUM_TWO = 1,
            ENUM_THREE = 3
        }

        // type parameters = lowerCamelCasing
        // methods = UpperCamelCasing
        public void DoThis(bool doSomething) {
            // if/else statements, always write out, minor exceptions
            if (doSomething) {
                // do something...
            } else {
                // else do something
            }

            if (doSomething) return; // inline if if clear enough


            // local variables / constants
            int thisInt = 0; // lowerCamelCasing

            LocalGameEnum localGameEnum = LocalGameEnum.ENUM_ONE;
            switch (localGameEnum) {
                case LocalGameEnum.ENUM_ONE:
                    // option 1
                    break;
                case LocalGameEnum.ENUM_TWO:
                    // option 2
                    break;
                case LocalGameEnum.ENUM_THREE:
                    // option 1
                    break;
            }

            float something = 0;
            for (int i = 0; i < something; i++) {
                for (int j = 0; j < something; j++) {
                    // Use i and j for normal for loops.
                    transform.position += Vector3.forward * thisInt;
                }
            }
        }
    }

    public class SomeThing {
        float exampleFloat;

        public SomeThing(float _exampleFloat) {
            exampleFloat = _exampleFloat;
        }
    }

    // global enum
    public enum GlobalGameEnum { 
        ENUM_ONE = 0,
        ENUM_TWO = 1,
        ENUM_THREE = 3
    }
}
