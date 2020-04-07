using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    public LayerMask collisionLayer;

    [HideInInspector]
    public bool colliding = false;
    [HideInInspector]
    public Vector3[] adjustedCameraClipPoints;
    [HideInInspector]
    public Vector3[] desiredCameraClipPoints;

    Camera camera;

    public void Initialize(Camera cam)
    {
        camera = cam;
        adjustedCameraClipPoints = new Vector3[5]; // 4 points for each corner of the near plane and the camera position in the center.
        desiredCameraClipPoints = new Vector3[5];// 4 points for each corner of the near plane and the camera position in the center.
    }

    // Updating the clip points of the camera since the camera is gonna be moving around in the scene.
    public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] newClipPointsArray)
    {
        if (!camera)
            return;

        // clear the content of newClipPointsArray.
        newClipPointsArray = new Vector3[5];

        float z = camera.nearClipPlane; // distance from the camera position to the new clip plane.
        float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z; // changing the 3.41 will change the size of the collision space.
        float y = x / camera.aspect;

        // find the clip points for each of the corners in the near clip plane and assign them to the newClipPointsArray.
        // top left
        newClipPointsArray[0] = (atRotation * new Vector3(-x,  y, z)) + cameraPosition; // added and rotated the point relative to camera
        // top right
        newClipPointsArray[1] = (atRotation * new Vector3( x,  y, z)) + cameraPosition; // added and rotated the point relative to camera
        // bottom left
        newClipPointsArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition; // added and rotated the point relative to camera
        // bottom right
        newClipPointsArray[3] = (atRotation * new Vector3( x, -y, z)) + cameraPosition; // added and rotated the point relative to camera
        // camera's position
        newClipPointsArray[4] = cameraPosition - camera.transform.forward; // - camera.transform.forward is to give a little bit of space behind the camera to collide with.
    }

    // Check if there is a collision within any of the clipPoints.
    bool CollisionDetectionAtClipPoints(Vector3[] clipPoints, Vector3 targetPosition)
    {
        // loop throught the clipPoints[]
        for(int i = 0; i < clipPoints.Length; i++)
        {
                                               // direction
            Ray ray = new Ray(targetPosition, clipPoints[i] - targetPosition);
            float distance = Vector3.Distance(clipPoints[i], targetPosition);
            if(Physics.Raycast(ray, distance, collisionLayer))
            {
                return true;
            }
        }
        return false;
    }

    // Get the new distance that the camera should be at from the target after the collision
    public float GetAdjustedDistanceFromTarget(Vector3 targetPosition)
    {
        // define a distance and if the value is not changed then we don't need to move the camera.
        float distance = -1;

        for(int i = 0; i < desiredCameraClipPoints.Length; i++)
        {
            // find the shortest distance between all the colliding clip points
            Ray ray = new Ray(targetPosition, desiredCameraClipPoints[i] - targetPosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if (distance == -1)
                {
                    distance = hit.distance;
                }
                else
                {
                    if (hit.distance < distance)
                        distance = hit.distance;
                }
            }
        }

        if (distance == -1)
            return 0;
        else
            return distance;
    }

    // Update colliding bool.
    public void CheckColliding(Vector3 targetPosition)
    {
        if(CollisionDetectionAtClipPoints(desiredCameraClipPoints, targetPosition))
        {
            colliding = true;
        }
        else
        {
            colliding = false;
        }
    }
}
