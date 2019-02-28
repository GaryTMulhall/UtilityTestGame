//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SpawnAgents : MonoBehaviour
//{
//    public GameObject agent;
//    List<GameObject> agents;
//    public GameObject ground;
//    int maxAgents = 4;
//    public Vector3 bounds = new Vector3();
//    private Agent agentBehaviour;
//    void Start()
//    {
//        //the safe area (bounds) is defined as the size of the ground gameobjects
//        //renderer size divided by 2, ensuring agents don't go outside
//        bounds = ground.GetComponent<SpriteRenderer>().bounds.size / 2;
//        agentBehaviour = GetComponent<Agent>();
//        agents = SpawnTheAgents();
//    }
//    private List<GameObject> SpawnTheAgents()
//    {
//        List<GameObject> agents = new List<GameObject>();
//        for (int i = 0; i < maxAgents; i++)
//        {
//            agents.Add(SpawnAgent());
//        }
//        return agents;
//    }

//    private GameObject SpawnAgent()
//    {
//        GameObject agentSpawned = Instantiate(agent);
//        agentSpawned.transform.position = new Vector3(Random.Range(-bounds.x + agentSpawned.transform.localScale.x, bounds.x - agentSpawned.transform.localScale.x),
//            (Random.Range(-bounds.y + agentSpawned.transform.localScale.y, bounds.y - agentSpawned.transform.localScale.y)), 0.0f);
//        return agentSpawned;
//    }
//}