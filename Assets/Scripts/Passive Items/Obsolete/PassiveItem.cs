using UnityEngine;

[System.Obsolete("OBSOLETE")]
public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        ApplyModifier();
    }

    protected virtual void ApplyModifier()
    {
        // Apply the boost value to the appropriate stat in the child classes
    }
}
