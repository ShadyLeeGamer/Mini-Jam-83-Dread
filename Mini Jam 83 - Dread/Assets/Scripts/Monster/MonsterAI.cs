using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    static int MonstersMoving = 0;

    List<Node> Path;
    Room CurrentRoom;
    Node CurrentNode;

    float Speed = 10f;
    int StepsTaken = 0;
    
    public void SetCurrentRoom(Room room)
    {
        CurrentRoom = room;
        CurrentNode = RoomManager.Instance.GetClosestNode(CurrentRoom, transform.position);
    }

    public void TakeTurn()
    {
        MonstersMoving++;
        StepsTaken = 0;

        SetPath(RoomManager.Instance.Pathfind(CurrentNode, RoomManager.Instance.GetClosestWalkableNode(CurrentRoom, new Vector3(0, 0, 0))));
    }

    public void SetPath(List<Node> path)
    {
        Path = path;

        for(int x = 0; x < path.Count - 1; x++)
        {
            var lineGo = new GameObject();
            var line = lineGo.AddComponent<LineRenderer>();
            line.transform.parent = transform;
            line.positionCount = 2;
            line.SetPosition(0, new Vector3(path[x].Position.x, 1, path[x].Position.y));
            line.SetPosition(1, new Vector3(path[x + 1].Position.x, 1, path[x + 1].Position.y));
        }
    }

    void Update()
    {
        if(Path != null)
        {
            var distanceLeft = new Vector2(transform.position.x - Path[0].Position.x, transform.position.z - Path[0].Position.y);

            transform.position += ((new Vector3(Path[0].Position.x, transform.position.y, Path[0].Position.y) - transform.position).normalized * Time.deltaTime * Speed);

            var newDistanceLeft = new Vector2(transform.position.x - Path[0].Position.x, transform.position.z - Path[0].Position.y);

            // Check to make sure we didn't overshoot the goal
            if(distanceLeft.x < 0 && newDistanceLeft.x > 0)
            {
                ProgressPath();
            }
            else if (distanceLeft.x > 0 && newDistanceLeft.x < 0)
            {
                ProgressPath();
            }
            else if (newDistanceLeft.y > 0 && distanceLeft.y < 0)
            {
                ProgressPath();
            }
            else if (newDistanceLeft.y < 0 && distanceLeft.y > 0)
            {
                ProgressPath();
            }
        }
    }

    void ProgressPath()
    {
        print(StepsTaken);
        StepsTaken++;

        CurrentNode = Path[0];
        CurrentRoom = CurrentNode.Room;

        transform.position = new Vector3(Path[0].Position.x, transform.position.y, Path[0].Position.y);
        Path.RemoveAt(0);

        if (StepsTaken >= 2 || Path.Count <= 0)
        {
            MonstersMoving--;
            Path = null;

            if (MonstersMoving <= 0)
            {
                MonstersMoving = 0;
                GameManager.Instance.MonstersMoved();
            }
        }
    }
}
