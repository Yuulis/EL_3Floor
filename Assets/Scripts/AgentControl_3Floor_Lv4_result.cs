using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.IO;
using TMPro;

public class AgentControl_3Floor_Lv4_result : Agent
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

    // Obstacle
    public Transform StairObstacle1;
    public Transform StairObstacle2;
    public Transform StairObstacle3;

    // CSVExport
    private StreamWriter sw;
    GameObject CSVExport;
    CSVExport csvExportScript;

    private double SpawnPosX, SpawnPosZ;
    private int StairObstaclePos1, StairObstaclePos2, StairObstaclePos3;
    private string ReachStair_Floor1, ReachStair_Floor2;

    // Result
    public GameObject DataCounter;
    DataCounter DataCounterScript;

    // At Initializing
    public override void Initialize()
    {
        CSVExport = GameObject.Find("CSVExport");
        csvExportScript = CSVExport.GetComponent<CSVExport>();

        DataCounterScript = DataCounter.GetComponent<DataCounter>();

        Floor3_Settings = FindObjectOfType<Floor3_Settings>();

        this.rBody = GetComponent<Rigidbody>();
        Floor3Renderer = Floor3.GetComponent<Renderer>();
        Floor3Material = Floor3Renderer.material;

        DataCounterScript.EpisodeCounter = 0;
        DataCounterScript.SuccessCounter = 0;
        DataCounterScript.Reach1FloorCounter = 0;
        DataCounterScript.Reach2FloorCounter = 0;
        DataCounterScript.Reach3FloorCounter = 0;
        DataCounterScript.SuccessAvarageWaterHeightCounter = 0f;
        DataCounterScript.FailAvarageWaterHeightCounter = 0f;

        Time.timeScale = 10;
    }

    // Last Floor check
    public void LastFloorCheck()
    {
        if (nowFloor == 1) DataCounterScript.Reach1FloorCounter++;
        if (nowFloor == 2) DataCounterScript.Reach2FloorCounter++;
        if (nowFloor == 3) DataCounterScript.Reach3FloorCounter++;
    }

    // Agent Spawn
    public void SpawnAgent()
    {
        this.transform.localPosition = new Vector3(Random.Range(-19.5f, 19.5f), 4.95f, Random.Range(-14.5f, 14.5f));
        SpawnPosX = this.transform.localPosition.x;
        SpawnPosZ = this.transform.localPosition.z;
    }

    public void SpawnStairObstacle()
    {
        StairObstacle1.rotation = Quaternion.identity;
        StairObstacle2.rotation = Quaternion.identity;
        StairObstacle3.rotation = Quaternion.identity;

        if (csvExportScript.Export)
        {
            StairObstacle1.localPosition = new Vector3(11f, 4f, -8f);
            StairObstacle2.localPosition = new Vector3(11f, 4f, 15f);
            StairObstacle3.localPosition = new Vector3(-11f, 11f, -15f); StairObstacle3.Rotate(0f, 180f, 0f);
            StairObstaclePos1 = 4;
            StairObstaclePos2 = 1;
            StairObstaclePos3 = 7;
        }
        else
        {
            int RandomPosNum1 = Random.Range(1, 8 + 1);
            if (RandomPosNum1 == 1) StairObstacle1.localPosition = new Vector3(11f, 4f, 15f);
            if (RandomPosNum1 == 2) { StairObstacle1.localPosition = new Vector3(-11f, 4f, 8f); StairObstacle1.Rotate(0f, 180f, 0f); }
            if (RandomPosNum1 == 3) { StairObstacle1.localPosition = new Vector3(-11f, 4f, -15f); StairObstacle1.Rotate(0f, 180f, 0f); }
            if (RandomPosNum1 == 4) StairObstacle1.localPosition = new Vector3(11f, 4f, -8f);
            if (RandomPosNum1 == 5) StairObstacle1.localPosition = new Vector3(11f, 11f, 15f);
            if (RandomPosNum1 == 6) { StairObstacle1.localPosition = new Vector3(-11f, 11f, 8f); StairObstacle1.Rotate(0f, 180f, 0f); }
            if (RandomPosNum1 == 7) { StairObstacle1.localPosition = new Vector3(-11f, 11f, -15f); StairObstacle1.Rotate(0f, 180f, 0f); }
            if (RandomPosNum1 == 8) StairObstacle1.localPosition = new Vector3(11f, 11f, -8f);

            int RandomPosNum2 = Random.Range(1, 8 + 1);
            while (RandomPosNum2 == RandomPosNum1) RandomPosNum2 = Random.Range(1, 8 + 1);

            if (RandomPosNum2 == 1) StairObstacle2.localPosition = new Vector3(11f, 4f, 15f);
            if (RandomPosNum2 == 2) { StairObstacle2.localPosition = new Vector3(-11f, 4f, 8f); StairObstacle2.Rotate(0f, 180f, 0f); }
            if (RandomPosNum2 == 3) { StairObstacle2.localPosition = new Vector3(-11f, 4f, -15f); StairObstacle2.Rotate(0f, 180f, 0f); }
            if (RandomPosNum2 == 4) StairObstacle2.localPosition = new Vector3(11f, 4f, -8f);
            if (RandomPosNum2 == 5) StairObstacle2.localPosition = new Vector3(11f, 11f, 15f);
            if (RandomPosNum2 == 6) { StairObstacle2.localPosition = new Vector3(-11f, 11f, 8f); StairObstacle2.Rotate(0f, 180f, 0f); }
            if (RandomPosNum2 == 7) { StairObstacle2.localPosition = new Vector3(-11f, 11f, -15f); StairObstacle2.Rotate(0f, 180f, 0f); }
            if (RandomPosNum2 == 8) StairObstacle2.localPosition = new Vector3(11f, 11f, -8f);

            int RandomPosNum3 = Random.Range(1, 8 + 1);
            while (RandomPosNum3 == RandomPosNum1 || RandomPosNum3 == RandomPosNum2) RandomPosNum3 = Random.Range(1, 8 + 1);

            if (RandomPosNum3 == 1) StairObstacle3.localPosition = new Vector3(11f, 4f, 15f);
            if (RandomPosNum3 == 2) { StairObstacle3.localPosition = new Vector3(-11f, 4f, 8f); StairObstacle3.Rotate(0f, 180f, 0f); }
            if (RandomPosNum3 == 3) { StairObstacle3.localPosition = new Vector3(-11f, 4f, -15f); StairObstacle3.Rotate(0f, 180f, 0f); }
            if (RandomPosNum3 == 4) StairObstacle3.localPosition = new Vector3(11f, 4f, -8f);
            if (RandomPosNum3 == 5) StairObstacle3.localPosition = new Vector3(11f, 11f, 15f);
            if (RandomPosNum3 == 6) { StairObstacle3.localPosition = new Vector3(-11f, 11f, 8f); StairObstacle3.Rotate(0f, 180f, 0f); }
            if (RandomPosNum3 == 7) { StairObstacle3.localPosition = new Vector3(-11f, 11f, -15f); StairObstacle3.Rotate(0f, 180f, 0f); }
            if (RandomPosNum3 == 8) StairObstacle3.localPosition = new Vector3(11f, 11f, -8f);

            StairObstaclePos1 = RandomPosNum1;
            StairObstaclePos2 = RandomPosNum2;
            StairObstaclePos3 = RandomPosNum3;
        }

    }

    // When Episode begins
    public override void OnEpisodeBegin()
    {
        if (DataCounterScript.EpisodeCounter > 10000) csvExportScript.FinishCSVExport();

        onGround = false;

        // Reset Agent's status
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.rotation = Quaternion.identity;

        ReachStair_Floor1 = "0";
        ReachStair_Floor2 = "0";

        StairObstaclePos1 = 0;
        StairObstaclePos2 = 0;
        StairObstaclePos3 = 0;

        nowFloor = 1;
        SpawnAgent();

        SpawnStairObstacle();

        DataCounterScript.TextSet();

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

                csvExportScript.SaveData(
                    SpawnPosX.ToString(),
                    SpawnPosZ.ToString(),
                    "0",
                    ReachStair_Floor1,
                    ReachStair_Floor2,
                    Water.localPosition.y.ToString()
                );

                DataCounterScript.EpisodeCounter++;
                LastFloorCheck();
                DataCounterScript.FailAvarageWaterHeightCounter += Water.localPosition.y;

                EndEpisode();
                StartCoroutine(
                    GoalScoredSwapGroundMaterial(Floor3_Settings.Failed_Floor, 0.5f));
            }

            // When Agent is on the 3rd floor
            if (nowFloor == 3) timer += 0.01f;
            else timer = 0f;

            if (timer > 5.00f)
            {
                AddReward(0.2f);

                csvExportScript.SaveData(
                    SpawnPosX.ToString(),
                    SpawnPosZ.ToString(),
                    "1",
                    ReachStair_Floor1,
                    ReachStair_Floor2,
                    Water.localPosition.y.ToString()
                );

                DataCounterScript.EpisodeCounter++;
                DataCounterScript.SuccessCounter++;
                LastFloorCheck();
                DataCounterScript.SuccessAvarageWaterHeightCounter += Water.localPosition.y;

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
                AddReward(-0.7f);

                csvExportScript.SaveData(
                    SpawnPosX.ToString(),
                    SpawnPosZ.ToString(),
                    "0",
                    ReachStair_Floor1,
                    ReachStair_Floor2,
                    Water.localPosition.y.ToString()
                );

                DataCounterScript.EpisodeCounter++;
                LastFloorCheck();
                DataCounterScript.FailAvarageWaterHeightCounter += Water.localPosition.y;

                EndEpisode();
                StartCoroutine(
                GoalScoredSwapGroundMaterial(Floor3_Settings.Failed_Floor, 0.5f));
            }

            // UpStair
            else if (col.gameObject.CompareTag("UpStair"))
            {
                string StairName = col.gameObject.name;
                if (nowFloor == 1) ReachStair_Floor1 = StairName.Substring(9, 1);
                if (nowFloor == 2) ReachStair_Floor2 = StairName.Substring(9, 1);

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
                csvExportScript.SaveData(
                    SpawnPosX.ToString(),
                    SpawnPosZ.ToString(),
                    "-1",
                    ReachStair_Floor1,
                    ReachStair_Floor2,
                    Water.localPosition.y.ToString()
                );

                EndEpisode();
            }

            // Touched Water
            else if (this.transform.localPosition.y < Water.localPosition.y)
            {
                csvExportScript.SaveData(
                    SpawnPosX.ToString(),
                    SpawnPosZ.ToString(),
                    "-1",
                    ReachStair_Floor1,
                    ReachStair_Floor2,
                    Water.localPosition.y.ToString()
                );

                EndEpisode();
            }
        }
    }
}