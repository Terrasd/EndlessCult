using UnityEngine;

[System.Obsolete("OBSOLETE")]
[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField] private float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value; }

    [SerializeField] private int level; // Not meant to be modified in the game [Only in Editor]
    public int Level { get => level; private set => level = value; }

    [SerializeField] private GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField] private new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField] private string description; // What is the description of this passive item?
                                                 // [If this passive item is a upgrade, place the description of the upgrade]
    public string Description { get => description; private set => description = value; }

    [SerializeField] private Sprite icon; // Not meant to be modified in game [Only in Editor]
    public Sprite Icon { get => icon; private set => icon = value; }
}
