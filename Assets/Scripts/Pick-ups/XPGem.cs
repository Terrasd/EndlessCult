using UnityEngine;

public class XPGem : Pickup
{
    public int xpGranted;

    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }

        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        player.IncreaseXP(xpGranted);
    }
}
