using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallThroughManager : MonoBehaviour
{
    List<FallThrough> blueFalls;
    List<FallThrough> redFalls;

    bool fallSwitch;

    // Start is called before the first frame update
    void Start()
    {
        FallThrough[] allFalls = FindObjectsOfType<FallThrough>();

        blueFalls = new List<FallThrough>();
        redFalls = new List<FallThrough>();

        foreach(FallThrough fall in allFalls)
        {
            if(fall.thisColor == fallColor.blue)
            {
                blueFalls.Add(fall);
            }

            else if(fall.thisColor == fallColor.red)
            {
                redFalls.Add(fall);
            }
        }

        ColorStatic.Switch();
        SwitchFalls();
    }

    public void SwitchFalls()
    {
        fallSwitch = !fallSwitch;

        if (fallSwitch)
        {
            foreach (FallThrough fall in blueFalls)
            {
                fall.MakeSolid();
            }

            foreach (FallThrough fall in redFalls)
            {
                fall.MakeFallThrough();
            }
        }

        else
        {
            foreach (FallThrough fall in redFalls)
            {
                fall.MakeSolid();
            }

            foreach (FallThrough fall in blueFalls)
            {
                fall.MakeFallThrough();
            }
        }

        ColorStatic.Switch();
    }
}
