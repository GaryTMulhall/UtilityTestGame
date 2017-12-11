using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedTargets : MonoBehaviour
{
    public readonly Vector2 direction;
    public readonly float weight;

    public WeightedTargets(Vector2 Direction, float Weight)
    {
        direction = Direction.normalized;
        weight = Weight;
    }
    //public enum Blending {  PRIMARY, SECONDARY, TERTIARY };
    //public Blending blending = Blending.PRIMARY;
}

