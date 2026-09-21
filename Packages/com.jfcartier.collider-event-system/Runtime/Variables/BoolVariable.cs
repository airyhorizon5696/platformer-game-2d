using UnityEngine;

namespace ColliderEventSystem
{
    [CreateAssetMenu(menuName = "Collider Event System/Variables/Bool Variable", fileName = "New Bool Variable")]
    public sealed class BoolVariable : VariableBase<bool>
    {
        protected override void LoadPersisted()
        {
            RuntimeValue = PlayerPrefs.GetInt(Id, InitialValue ? 1 : 0) != 0;
        }

        protected override void SavePersisted()
        {
            PlayerPrefs.SetInt(Id, RuntimeValue ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
