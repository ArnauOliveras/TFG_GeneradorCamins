using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACOBlackboard : MonoBehaviour 
{
    [Header("Initial Power")]
    public float initialPower;

    [Header("Power of pheromones")]
    public float powersNode1;
    public float powersNode2;
    public float powersNode3;

    [Header("Closer to the final destination")]
    public float nearestNode1;
    public float nearestNode2;
    public float nearestNode3;

    [Header("Slope favoritisms")]
    public float slopeFavoritismsREC;
    public float slopeFavoritismsMAX;

    [Header("Next Slope Similar")]
    public float similarNode1;
    public float similarNode2;
    public float similarNode3;
    public float similarNode4;
    public float similarNode5;
    public float similarNode6;
    public float similarNode7;
    public float similarNode8;

    [Header("MAX Multiplier")]
    public int MAXMultiplier;
}
