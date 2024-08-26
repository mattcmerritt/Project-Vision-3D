using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Suspicion
{
    public InteractionTypes interactionType;
    public SuspicionType suspicionType;

    // information regarding the current levels of suspicion for this type of action
    public float currentLevel;
    public float threshold;
    public float rateOfIncrease;
}
