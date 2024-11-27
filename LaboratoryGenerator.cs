using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaboratoryGenerator : MonoBehaviour
{
    public bool GenerateOnStart = true;
    [Range(3, 50)]
    public int RoomCount = 9;
    public LayerMask CellLayer;

    public GameObject StartPoint;
    public GameObject EndPoint;
    public GameObject CorridorStraight;
    public GameObject CorridorTurn;
    public GameObject CorridorThreeWay;
    public GameObject RoomPrefab;
                    
    private void Start()
    {
        if (GenerateOnStart) StartCoroutine(StartGeneration());
    }

    IEnumerator StartGeneration()
    {
        List<Transform> createdExits = new List<Transform>();

        // Place the start room
        GameObject startRoom = Instantiate(StartPoint, Vector3.zero, Quaternion.identity);
        Transform startExit = startRoom.transform.Find("Exit"); // Assume Exit is a child object
        createdExits.Add(startExit);

        int roomsLeft = RoomCount - 2; // Reserve one for the end point
        int safetyLimit = 100;

        while (roomsLeft > 0 && safetyLimit > 0)
        {
            safetyLimit--;

            // Randomly pick a corridor or room type
            GameObject prefab = GetRandomPrefab();
            GameObject newSegment = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            bool collided = true;
            int alignmentAttempts = 10;

            Transform selectedExit = createdExits[Random.Range(0, createdExits.Count)];
            Transform newSegmentExit = newSegment.transform.Find("Exit"); // Assume Exit is a child object

            while (collided && alignmentAttempts > 0)
            {
                alignmentAttempts--;

                // Rotate the segment to align exits
                float shiftAngle = selectedExit.eulerAngles.y + 180 - newSegmentExit.eulerAngles.y;
                newSegment.transform.Rotate(0, shiftAngle, 0);

                // Position the new segment to align exits
                Vector3 shiftPosition = selectedExit.position - newSegmentExit.position;
                newSegment.transform.position += shiftPosition;

                // Check for overlap
                collided = Physics.CheckBox(
                    newSegment.transform.position,
                    newSegment.GetComponent<BoxCollider>().size / 2,
                    newSegment.transform.rotation,
                    CellLayer,
                    QueryTriggerInteraction.Ignore
                );

                yield return null;
            }

            if (!collided)
            {
                createdExits.Remove(selectedExit);
                DestroyImmediate(selectedExit.gameObject);

                Transform[] newExits = newSegment.GetComponentsInChildren<Transform>();
                foreach (Transform exit in newExits)
                {
                    if (exit.name == "Exit") createdExits.Add(exit);
                }

                roomsLeft--;
            }
            else
            {
                DestroyImmediate(newSegment);
            }

            yield return null;
        }

        // Place the end room
        if (createdExits.Count > 0)
        {
            Transform finalExit = createdExits[0];
            GameObject endRoom = Instantiate(EndPoint, finalExit.position, finalExit.rotation);
            DestroyImmediate(finalExit.gameObject);
        }

        Debug.Log("Generation Complete!");
    }

    private GameObject GetRandomPrefab()
    {
        // Randomly choose between corridor types and rooms
        int choice = Random.Range(0, 4);
        switch (choice)
        {
            case 0: return CorridorStraight;
            case 1: return CorridorTurn;
            case 2: return CorridorThreeWay;
            default: return RoomPrefab;
        }
    }
}
