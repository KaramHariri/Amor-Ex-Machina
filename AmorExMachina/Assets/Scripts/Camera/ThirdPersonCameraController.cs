using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target;

    [System.Serializable]
    public class PositionSettings
    {
        public float lookSmooth = 100.0f;
        public float distaceFromTarget = -8.0f;
        public float smooth = 0.05f;

        [HideInInspector]
        public float newDistance = -8.0f;
        [HideInInspector]
        public float adjustedDistance = -8.0f;
    }

    [System.Serializable]
    public class OrbitSettings
    {
        public float xRotation = -20.0f;
        public float yRotation = -180.0f;
        public float maxXRotation = 25.0f;
        public float minXRotation = -85.0f;
        public float maxYRotation = 90.0f;
        public float minYRotation = -90.0f;
        public float verticalOrbitSmooth = 150.0f;
        public float horizontalOrbitSmooth = 15.0f;
        public bool allowOrbiting;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string ORBIT_HORIZONTAL_SNAP = "Fire1";
        public string ORBIT_HORIZONTAL = "Mouse X";
        public string ORBIT_VERTICAL = "Mouse Y";
    }

    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;
    }

    public PositionSettings positionSettings = new PositionSettings();
    public OrbitSettings orbitSettings = new OrbitSettings();
    public InputSettings inputSettings = new InputSettings();
    public DebugSettings debugSettings = new DebugSettings();
    public CameraCollisionHandler cameraCollisionHandler;

    Vector3 targetPos = Vector3.zero;
    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;
    Vector3 cameraVelocity = Vector3.zero;
    float distanceVelocity = 0.0f;
    float verticalOrbitInput, horizontalOrbitInput, horizontalOrbitSnapInput;
    public Transform fpsCameraPosition;
    public Vector3 thirdPersonCameraPosition;
    public Quaternion thirPersonCameraRotation;
    public bool fps = false;

    public float rotationSpeed = 5.0f;

    void Start()
    {
        cameraCollisionHandler = GetComponent<CameraCollisionHandler>();
        verticalOrbitInput = horizontalOrbitInput = horizontalOrbitSnapInput = 0.0f;
        MoveToTarget();
        cameraCollisionHandler.Initialize(Camera.main);
        cameraCollisionHandler.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollisionHandler.adjustedCameraClipPoints);
        cameraCollisionHandler.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollisionHandler.desiredCameraClipPoints);
    }

    void GetInput()
    {
        verticalOrbitInput = Input.GetAxisRaw(inputSettings.ORBIT_VERTICAL);
        horizontalOrbitInput = Input.GetAxisRaw(inputSettings.ORBIT_HORIZONTAL);
        horizontalOrbitSnapInput = Input.GetAxisRaw(inputSettings.ORBIT_HORIZONTAL_SNAP);
    }
    
    private void Update()
    {
        if(thirdPersonCameraPosition == Vector3.zero)
            GetInput();
        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    thirdPersonCameraPosition = transform.position;
        //    thirPersonCameraRotation = transform.rotation;
        //    fps = !fps;
        //}
        if(fps)
        {
            if (positionSettings.distaceFromTarget <= -0.01f)
            {
                //transform.position = Vector3.Lerp(transform.position, fpsCameraPosition.position, Time.deltaTime);
                //transform.position = Vector3.SmoothDamp(transform.position, fpsCameraPosition.position, ref cameraVelocity, positionSettings.smooth * 5.0f);
                positionSettings.distaceFromTarget = Mathf.SmoothDamp(positionSettings.distaceFromTarget, 0.0f, ref distanceVelocity, positionSettings.smooth * 5.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), Time.deltaTime * rotationSpeed);
                //positionSettings.distaceFromTarget = -Mathf.Abs(transform.position.z - fpsCameraPosition.position.z);
                //if (Vector3.Distance(transform.position, fpsCameraPosition.position) <= 0.1f)
                //    positionSettings.distaceFromTarget = 0.0f;
            }
        }
        else
        {
            //if (thirdPersonCameraPosition != Vector3.zero)
            //{
            //    if (Vector3.Distance(transform.position, thirdPersonCameraPosition) >= 0.1f)
            //    {
            //transform.position = Vector3.Lerp(transform.position, thirdPersonCameraPosition, Time.deltaTime);
            //transform.position = Vector3.SmoothDamp(transform.position, thirdPersonCameraPosition, ref cameraVelocity, positionSettings.smooth /** 5.0f*/);
            if (positionSettings.distaceFromTarget > -8.0f)
            {
                Debug.Log("something");
                positionSettings.distaceFromTarget = Mathf.SmoothDamp(positionSettings.distaceFromTarget, -8.0f, ref distanceVelocity, positionSettings.smooth * 5.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - thirdPersonCameraPosition), Time.deltaTime * rotationSpeed);
            }
                //positionSettings.distaceFromTarget = -Mathf.Abs(transform.position.z - thirdPersonCameraPosition.z);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, thirPersonCameraRotation/*Quaternion.LookRotation(thirdPersonCameraPosition.forward)*/, Time.deltaTime * rotationSpeed);
            //    }
            //    else
            //    {
            //        thirdPersonCameraPosition = Vector3.zero;
            //        positionSettings.distaceFromTarget = -8.0f;
            //    }
            //}
        }
    }

    private void FixedUpdate()
    {
        //if (thirdPersonCameraPosition == Vector3.zero)
        //{
       
            // Moving.
            MoveToTarget();
        if (!fps)
        {
            // Rotate
            LookAtTarget();
        }
        // player input orbit
        OrbitTarget();

            cameraCollisionHandler.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollisionHandler.adjustedCameraClipPoints);
            cameraCollisionHandler.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollisionHandler.desiredCameraClipPoints);

            DrawDebugRays();

            cameraCollisionHandler.CheckColliding(targetPos);
            positionSettings.adjustedDistance = cameraCollisionHandler.GetAdjustedDistanceFromTarget(targetPos);
        
        //}
    }

    void MoveToTarget()
    {
        targetPos = target.position;
        destination = Quaternion.Euler(orbitSettings.xRotation, orbitSettings.yRotation + target.eulerAngles.y, 0.0f) * -Vector3.forward * positionSettings.distaceFromTarget;
        destination += targetPos;

        if (cameraCollisionHandler.colliding)
        {
            adjustedDestination = Quaternion.Euler(orbitSettings.xRotation, orbitSettings.yRotation + target.eulerAngles.y, 0.0f) * Vector3.forward * positionSettings.adjustedDistance;
            adjustedDestination += targetPos;
            // smooth damp function
            transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref cameraVelocity, positionSettings.smooth);
        }
        else
        {
            // smooth damp function.
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref cameraVelocity, positionSettings.smooth);
        }
    }

    void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, positionSettings.lookSmooth * Time.deltaTime);
    }

    void OrbitTarget()
    {
        if(horizontalOrbitSnapInput > 0)
        {
            orbitSettings.yRotation = -180.0f;
        }

        orbitSettings.xRotation += -verticalOrbitInput * orbitSettings.verticalOrbitSmooth * Time.deltaTime;
        orbitSettings.yRotation += -horizontalOrbitInput * orbitSettings.verticalOrbitSmooth * Time.deltaTime;
        
        LimitOrbit();
        
    }

    void LimitOrbit()
    {
        if (orbitSettings.xRotation > orbitSettings.maxXRotation)
        {
            orbitSettings.xRotation = orbitSettings.maxXRotation;
        }
        if (orbitSettings.xRotation < orbitSettings.minXRotation)
        {
            orbitSettings.xRotation = orbitSettings.minXRotation;
        }
        if (orbitSettings.yRotation > orbitSettings.maxYRotation)
        {
            orbitSettings.yRotation = orbitSettings.maxYRotation;
        }
        if (orbitSettings.yRotation < orbitSettings.minYRotation)
        {
            orbitSettings.yRotation = orbitSettings.minYRotation;
        }
    }

    void DrawDebugRays()
    {
        // draw debug line.
        for (int i = 0; i < 5; i++)
        {
            if (debugSettings.drawDesiredCollisionLines)
            {
                Debug.DrawLine(targetPos, cameraCollisionHandler.desiredCameraClipPoints[i], Color.white);
            }
            if (debugSettings.drawAdjustedCollisionLines)
            {
                Debug.DrawLine(targetPos, cameraCollisionHandler.adjustedCameraClipPoints[i], Color.green);
            }
        }
    }
}