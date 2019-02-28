using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public enum desires { thirst, hunger, energy, boredom, knowledge, urine }

[Serializable]
public class Desire
{
    public Transform Target;
    public desires Type;
    public float Value;
    public float Loss;
    public float Gain;
    public float WaitTimer;
    public float Importance { get { return Value * WaitTimer; } }

    //Getting the normalised versions of the Desire parameters
    [SerializeField]
    public float NormalisedValue { get { return Value / 100; } }
    public float NormalisedLoss { get { return Loss / 100; } }
    public float NormalisedGain { get { return Gain; } }
    public float NormalisedWaitTimer { get { return WaitTimer / 10; } }
    public float NormalisedImportance { get { return NormalisedValue * NormalisedWaitTimer; } }

    public Desire(desires type, float value, float loss, float gain, float waitTimer)
    {
        Type = type;
        Value = value;
        Loss = loss;
        Gain = gain;
        WaitTimer = waitTimer;
    }
}

public class Agent : MonoBehaviour
{
    /*Setting the initial values of each individual desire the agent has
     *these include the starting value/score, the rate at which the score decreases
     *how much the agent will gain from fulfilling the desire
     *and how much time the agent will take to fulfill that desire*/
    public Desire Thirst = new Desire(desires.thirst, 100, 5, 5 * 5, 3);
    public Desire Hunger = new Desire(desires.hunger, 100, 2, 2 * 5, 7);
    public Desire Energy = new Desire(desires.energy, 100, 1, 1 * 5, 10);
    public Desire Boredom = new Desire(desires.boredom, 100, 0.5f, 0.5f * 5, 5);
    public Desire Knowledge = new Desire(desires.knowledge, 100, 0.2f, 0.2f * 5, 5);
    public Desire Urine = new Desire(desires.urine, 100, 0.6f, 0.6f * 5, 6);

    private Desire[] AllDesires
    {
        get
        {
            return new Desire[] { Thirst, Hunger, Energy, Boredom, Knowledge, Urine };
        }
    }
    //an enum to hold the various emotional states available to agents
    public enum EmotionalStates
    {
        Happy,
        Sad,
        Angry,
        Scared
    }
    //creating a public instance of the EmotionalStates enum to allow manipulation
    public EmotionalStates emotionalState;
    public float targetRange = 0.1f;

    public Vector2 agentVelocity;

    public List<WeightedTargets> desiredTarget; //Creating a list of all potential targets which will be selected based on the agents requirements

    protected SeekTarget seekTarget;

    private void Start()
    {
        seekTarget = GetComponent<SeekTarget>();
        //setting the emotional state for agents to be random on start
        //emotionalState = (EmotionalStates)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EmotionalStates)).Length);
    }

    //public void OnGUI()
    //{
    //    GUI.Label(new Rect (550, 20, 160, 75), "Thirst: " + Thirst.NormalisedImportance.ToString());
    //    GUI.Label(new Rect(550, 40, 160, 75), "Hunger: " + Hunger.NormalisedImportance.ToString());
    //    GUI.Label(new Rect(550, 60, 160, 75), "Energy: " + Energy.NormalisedImportance.ToString());
    //    GUI.Label(new Rect(550, 120, 160, 100), "Urine: " + Urine.NormalisedImportance.ToString());
    //    GUI.Label(new Rect(550, 80, 160, 75), "Boredom: " + Boredom.NormalisedImportance.ToString());
    //    GUI.Label(new Rect(550, 100, 160, 100), "Knowledge: " + Knowledge.NormalisedImportance.ToString());
    //}

    private void Update()
    {
        /*Forcing all desires to be clamped between a value of 0 and 100
      *while ensuring they decrease over time multiplied by the "Loss" value attached to each desire*/
        foreach (var desire in AllDesires)
        {
            desire.Value = Mathf.Clamp(desire.Value - Time.deltaTime * desire.Loss, 0, 99);
        }
        var maxDesire = AllDesires.OrderBy(d => d.NormalisedImportance).First(); //Setting maxDesire to equal the first or highest desire
        seekTarget.SetTarget(maxDesire.Target.position, maxDesire.WaitTimer); //Setting the agents target to the position of the maxDesire

        /*Ensuring the agent drinks AND eats if it is at either the fridge or the water bottle 
         * as this makes logical sense since they are near each other */
        Vector3 atFridge = Hunger.Target.position - transform.position;
        if (atFridge.magnitude < targetRange)
        {
            Hunger.Value += Hunger.Gain;
            Thirst.Value += Thirst.Gain;
        }

        Vector3 atDrink = Thirst.Target.position - transform.position;
        if (atDrink.magnitude < targetRange)
        {
            Hunger.Value += Hunger.Gain;
            Thirst.Value += Thirst.Gain;
        }

        foreach (var desire in AllDesires)
        {
            Vector3 distance = desire.Target.position - transform.position;

            if (distance.magnitude < targetRange)
            {
                desire.Value += desire.Gain;
            }
        }
        SetColours();
        AssignValue();
        CollisionChecks();
    }

    void SetColours()
    {
        SpriteRenderer agentRenderer = GetComponent<SpriteRenderer>();
        if (emotionalState == EmotionalStates.Happy)
        {
            agentRenderer.color = Color.yellow;
        }
        if (emotionalState == EmotionalStates.Sad)
        {
            agentRenderer.color = Color.blue;
        }
        if (emotionalState == EmotionalStates.Angry)
        {
            agentRenderer.color = Color.red;
        }
        if (emotionalState == EmotionalStates.Scared)
        {
            agentRenderer.color = Color.black;
        }
    }
    void AssignValue()
    {
        if (emotionalState == EmotionalStates.Happy)
        {
            transform.tag = "Happy";
        }
        else if (emotionalState == EmotionalStates.Sad)
        {
            transform.tag = "Sad";
        }
        else if (emotionalState == EmotionalStates.Angry)
        {
            transform.tag = "Angry";
        }
        else if (emotionalState == EmotionalStates.Scared)
        {
            transform.tag = "Scared";
        }
    }
    void CollisionChecks()
    {
        if (emotionalState == EmotionalStates.Happy)
        {
            if (transform.position.x < GameObject.FindGameObjectWithTag("Sad").transform.position.x)
            {
                print("COLISSION");
                emotionalState = (EmotionalStates)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EmotionalStates)).Length);
            }
        }
    }
}
