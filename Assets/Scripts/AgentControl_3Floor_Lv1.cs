using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControl_3Floor_Lv1 : Agent
{
    // General
    private bool onGround;
    public GameObject Floor3;
    Material Floor3Material;
    Renderer Floor3Renderer;
    Floor3_Settings Floor3_Settings;
    private float timer = 0f;

    // Agent
    Rigidbody rBody;
    private int nowFloor;
    public float AgentSpeed = 0.6f;

    // Water
    public Transform Water;

    // At Initializing
    public override void Initialize()
    {
        Floor3_Settings = FindObjectOfType<Floor3_Settings>();

        this.rBody = GetComponent<Rigidbody>();
        Floor3Renderer = Floor3.GetComponent<Renderer>();
        Floor3Material = Floor3Renderer.material;

        // Time.timeScale = 5;
    }

    // Agent Spawn
    public void SpawnAgent()
    {
        this.transform.localPosition = new Vector3(Random.Range(-19.5f, 19.5f), 4.95f, Random.Range(-14.5f, 14.5f));
    }

    // When Episode begins
    public override void OnEpisodeBegin()
    {
        onGround = false;

        // Reset Agent's status
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        nowFloor = 1;
        SpawnAgent();

        Water.localPosition = new Vector3(-25.0f, -10.0f, -25.0f);
    }

    // Get observations (size = 4)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);  // Agent(x, y, z)
        sensor.AddObservation(Water.localPosition.y);  // Water(y)
    }

    // When Agent moves (size = 3)
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var rotateDirAction = act[1];

        if (dirToGoForwardAction == 1) dirToGo = 1.0f * this.transform.forward;  // Forward
        else if (dirToGoForwardAction == 2) dirToGo = -1.0f * this.transform.forward;  // Back

        if (rotateDirAction == 1) rotateDir = 1.0f * this.transform.up;  // Rotate right
        else if (rotateDirAction == 2) rotateDir = -1.0f * this.transform.up;  // Rotate left

        this.transform.Rotate(rotateDir, Time.deltaTime * 200f);
        this.rBody.AddForce(dirToGo * AgentSpeed, ForceMode.VelocityChange);
    }

    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        Floor3Renderer.material = mat;
        yield return new WaitForSeconds(time);
        Floor3Renderer.material = Floor3Material;
    }

    // Agent's action
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (onGround)
        {
            MoveAgent(actions.DiscreteActions);
            if (Water.localPosition.y < 14.0f) Water.Translate(0f, 0.01f, 0f);

            // When Water comes
            if (this.transform.localPosition.y < Water.localPosition.y)
            {
                AddReward(-1.0f);

                EndEpisode();
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(Floor3_Settings.Failed_Floor, 0.5f));
            }

            // When Agent is on the 3rd floor
            if (nowFloor == 3) timer += 0.01f;
            else timer = 0f;

            if (timer > 5.00f)
            {
                AddReward(0.4f);
                EndEpisode();
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(Floor3_Settings.Success_Floor, 0.5f));
            }

            float DistanceToWater = this.transform.localPosition.y - Water.localPosition.y;
            AddReward(((DistanceToWater / 10f) + (0.15f * nowFloor)) / MaxStep);
        }

    }

    // Human Controling
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 1;  // Forward
        if (Input.GetKey(KeyCode.S)) discreteActionsOut[0] = 2;  // Back
        if (Input.GetKey(KeyCode.D)) discreteActionsOut[1] = 1;  // Rotate right
        if (Input.GetKey(KeyCode.A)) discreteActionsOut[1] = 2;  // Rotate left
    }

    // When touching something
    void OnCollisionEnter(Collision col)
    {
        // Touching floor?
        if (onGround)
        {
            // Wall or Obstacle
            if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Obstacle"))
            {
                AddReward(-0.8f);

                EndEpisode();
                StartCoroutine(
                GoalScoredSwapGroundMaterial(Floor3_Settings.Failed_Floor, 0.5f));
            }

            // UpStair
            else if (col.gameObject.CompareTag("UpStair"))
            {
                this.rBody.angularVelocity = Vector3.zero;
                this.rBody.velocity = Vector3.zero;
                this.transform.rotation = Quaternion.identity;

                if (this.transform.localPosition.x > 0 && this.transform.localPosition.z > 0) { this.transform.Translate(-1.0f, 7.0f, 4.0f); this.transform.Rotate(0f, -90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z > 0) { this.transform.Translate(1.0f, 7.0f, -4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z < 0) { this.transform.Translate(1.0f, 7.0f, -4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x > 0 && this.transform.localPosition.z < 0) { this.transform.Translate(-1.0f, 7.0f, 4.0f); this.transform.Rotate(0f, -90.0f, 0f); }

                nowFloor++;
                AddReward(0.3f);
            }

            // DownStair
            else if (col.gameObject.CompareTag("DownStair"))
            {
                this.rBody.angularVelocity = Vector3.zero;
                this.rBody.velocity = Vector3.zero;
                this.transform.rotation = Quaternion.identity;

                if (this.transform.localPosition.x > 0 && this.transform.localPosition.z > 0) { this.transform.Translate(-1.0f, -7.0f, -4.0f); this.transform.Rotate(0f, -90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z > 0) { this.transform.Translate(1.0f, -7.0f, 4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z < 0) { this.transform.Translate(1.0f, -7.0f, 4.0f); this.transform.Rotate(0f, 90.0f, 0f); }
                else if (this.transform.localPosition.x > 0 && this.transform.localPosition.z < 0) { this.transform.Translate(-1.0f, -7.0f, -4.0f); this.transform.Rotate(0f, -90.0f, 0f); }

                nowFloor--;
                AddReward(-0.5f);
            }
        }

        else
        {
            // Floor
            if (col.gameObject.CompareTag("Floor"))
            {
                onGround = true;
            }

            // Obstacle or Stair
            if (col.gameObject.CompareTag("Obstacle") || col.gameObject.CompareTag("UpStair") || col.gameObject.CompareTag("DownStair"))
            {
                SpawnAgent();
            }

            // Touched Water
            else if (this.transform.localPosition.y < Water.localPosition.y)
            {
                SpawnAgent();
            }
        }
    }
}