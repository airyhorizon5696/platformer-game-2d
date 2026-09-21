using UnityEngine;

namespace ColliderEventSystem
{
    [CreateAssetMenu(menuName = "Collider Event System/Variables/String Variable", fileName = "New String Variable")]
    public sealed class StringVariable : VariableBase<string>
    {
        protected override void LoadPersisted()
        {
            RuntimeValue = PlayerPrefs.GetString(Id, InitialValue);
        }

        protected override void SavePersisted()
        {
            PlayerPrefs.SetString(Id, RuntimeValue);
            PlayerPrefs.Save();
        }
    }
}
