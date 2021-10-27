using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentControl_3Floor_Lv1_result : Agent
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

    // CSV Export
    GameObject CSVExporter;
    CSVExport CSVExport;
    private bool Export;

    private int SpawnPosCount = -1;

    private float SpawnX, SpawnZ;
    private int time = 0;
    private int ReachStair_Floor1 = 0;
    private int ReachStair_Floor2 = 0;
    private float WaterHeight = -10.0f;

    // At Initializing
    public override void Initialize()
    {
        Floor3_Settings = FindObjectOfType<Floor3_Settings>();

        this.rBody = GetComponent<Rigidbody>();
        Floor3Renderer = Floor3.GetComponent<Renderer>();
        Floor3Material = Floor3Renderer.material;

        CSVExporter = GameObject.Find("CSVExporter");
        CSVExport = CSVExporter.GetComponent<CSVExport>();
        Export = CSVExport.Export;

        Time.timeScale = 5;
    }

    // Agent Spawn
    public void SpawnAgent()
    {
        SpawnPosCount++;

        if (SpawnPosCount >= 39 * 29) CSVExport.FinishCSVExport();

        SpawnX = SpawnPosCount % 39 - 19;
        SpawnZ = SpawnPosCount / 39 - 14;

        this.transform.localPosition = new Vector3(SpawnX, 4.95f, SpawnZ);

        if (SpawnPosCount % 10 == 0) Debug.Log(SpawnPosCount);
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

        time = 0;
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

            time++;

            if (Water.localPosition.y < 14.0f) Water.Translate(0f, 0.01f, 0f);
            WaterHeight = Water.localPosition.y;

            // When Water comes
            if (this.transform.localPosition.y < WaterHeight)
            {
                AddReward(-1.0f);

                EndEpisode();
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(Floor3_Settings.Failed_Floor, 0.5f));

                if (Export)
                {
                    CSVExport.SaveData(SpawnX.ToString(),
                        SpawnZ.ToString(),
                        "0",
                        ReachStair_Floor1.ToString(),
                        ReachStair_Floor2.ToString(),
                        WaterHeight.ToString()
                    );
                }
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

                if (Export)
                {
                    CSVExport.SaveData(SpawnX.ToString(),
                        SpawnZ.ToString(),
                        "1",
                        ReachStair_Floor1.ToString(),
                        ReachStair_Floor2.ToString(),
                        WaterHeight.ToString()
                    );
                }
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

                if (Export)
                {
                    CSVExport.SaveData(SpawnX.ToString(),
                        SpawnZ.ToString(),
                        "0",
                        ReachStair_Floor1.ToString(),
                        ReachStair_Floor2.ToString(),
                        WaterHeight.ToString()
                    );
                }
            }

            // UpStair
            else if (col.gameObject.CompareTag("UpStair"))
            {
                this.rBody.angularVelocity = Vector3.zero;
                this.rBody.velocity = Vector3.zero;
                this.transform.rotation = Quaternion.identity;

                if (this.transform.localPosition.x > 0 && this.transform.localPosition.z > 0)
                {
                    this.transform.Translate(-1.0f, 7.0f, 4.0f);
                    this.transform.Rotate(0f, -90.0f, 0f);

                    if (nowFloor == 1) ReachStair_Floor1 = 1;
                    else if (nowFloor == 2) ReachStair_Floor2 = 1;
                }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z > 0)
                {
                    this.transform.Translate(1.0f, 7.0f, -4.0f);
                    this.transform.Rotate(0f, 90.0f, 0f);

                    if (nowFloor == 1) ReachStair_Floor1 = 2;
                    else if (nowFloor == 2) ReachStair_Floor2 = 2;
                }
                else if (this.transform.localPosition.x < 0 && this.transform.localPosition.z < 0)
                {
                    this.transform.Translate(1.0f, 7.0f, -4.0f);
                    this.transform.Rotate(0f, 90.0f, 0f);

                    if (nowFloor == 1) ReachStair_Floor1 = 3;
                    else if (nowFloor == 2) ReachStair_Floor2 = 3;
                }
                else if (this.transform.localPosition.x > 0 && this.transform.localPosition.z < 0)
                {
                    this.transform.Translate(-1.0f, 7.0f, 4.0f);
                    this.transform.Rotate(0f, -90.0f, 0f);

                    if (nowFloor == 1) ReachStair_Floor1 = 4;
                    else if (nowFloor == 2) ReachStair_Floor2 = 4;
                }

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
                if (Export)
                {
                    CSVExport.SaveData(SpawnX.ToString(),
                        SpawnZ.ToString(),
                        "-1",
                        ReachStair_Floor1.ToString(),
                        ReachStair_Floor2.ToString(),
                        WaterHeight.ToString()
                    );
                }
                EndEpisode();
            }

            // Touched Water
            else if (this.transform.localPosition.y < Water.localPosition.y)
            {
                if (Export)
                {
                    CSVExport.SaveData(SpawnX.ToString(),
                        SpawnZ.ToString(),
                        "-1",
                        ReachStair_Floor1.ToString(),
                        ReachStair_Floor2.ToString(),
                        WaterHeight.ToString()
                    );
                }
                EndEpisode();
            }
        }
    }
}