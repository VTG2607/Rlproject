using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents; //To change the base class from MonoBehaviour to Agent
using Unity.MLAgents.Actuators; 
using Unity.MLAgents.Sensors; 

public class AgentCtrl : Agent
{
    [SerializeField] private Transform target; //It's a reference which stores a GameObject's position, orientation and scale in the 3D world
    [SerializeField] private float speed = 8f;


    public override void OnEpisodeBegin() 
    {
        // Monster position 
        transform.localPosition = new Vector3(Random.Range(-13f, 13f), -11.7f, Random.Range(-13f, 12f));

        //Slime position 
        target.localPosition = new Vector3(Random.Range(-13f, 13f), -12.08f, Random.Range(-13f, 12f));
    }
    public override void CollectObservations(VectorSensor sensor) 
    {
        // Slime and Monster positions
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions) //This method receives actions
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // the speed of the Agent
        Vector3 velocity = new Vector3(moveX, 0f, moveZ).normalized * Time.deltaTime * speed;

        transform.localPosition += velocity;

        AddReward(-0.01f); // penalising agent
    }

    public override void Heuristic(in ActionBuffers actionsOut) //Heuristic movement for bug fixing(Player Control)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }

    private void OnTriggerEnter(Collider other) //This method is used to detect the collision of the Monster
    {
        if (other.gameObject.tag == "Reward") //Tag for the target and condition to add reward
        {
            AddReward(2f);
            EndEpisode();
        }

        if (other.gameObject.tag == "Wall") // Tag for the wall and condition to penalty 
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}