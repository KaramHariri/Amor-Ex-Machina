using UnityEngine;
using UnityEngine.Serialization;

namespace Cinemachine
{

	[AddComponentMenu("")] // Hide in menu
	[SaveDuringPlay]
#if UNITY_2018_3_OR_NEWER
	[ExecuteAlways]
#else
	[ExecuteInEditMode]
#endif
	public class CinemachineCustomCollider : CinemachineExtension
	{
		/// <summary>Objects on these layers will be detected.</summary>
		[Header("Obstacle Detection")]
		[Tooltip("Objects on these layers will be detected")]
		public LayerMask collideAgainst = 1;

		/// <summary>Obstacles closer to the target than this will be ignored</summary>
		[Tooltip("Obstacles closer to the target than this will be ignored")]
		public float minimumDistanceFromTarget = 0.1f;

		/// <summary>
		/// When enabled, will attempt to resolve situations where the line of sight to the 
		/// target is blocked by an obstacle
		/// </summary>
		[Space]
		[Tooltip("When enabled, will attempt to resolve situations where the line of sight to the target is blocked by an obstacle")]
		[FormerlySerializedAs("m_PreserveLineOfSight")]
		public bool avoidObstacles = true;

		/// <summary>
		/// Camera will try to maintain this distance from any obstacle.  
		/// Increase this value if you are seeing inside obstacles due to a large 
		/// FOV on the camera.
		/// </summary>
		[Tooltip("Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera.")]
		public float cameraRadius = 0.1f;

		private void OnValidate()
		{
			cameraRadius = Mathf.Max(0, cameraRadius);
			minimumDistanceFromTarget = Mathf.Max(0.01f, minimumDistanceFromTarget);
		}

		/// <summary>Callback to do the collision resolution and shot evaluation</summary>
		protected override void PostPipelineStageCallback(
			CinemachineVirtualCameraBase vcam,
			CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Finalize)
			{
				if (avoidObstacles)
				{
					Vector3 displacement = PreserveLineOfSight(ref state);
					state.PositionCorrection += displacement;
				}
			}
		}

		private Vector3 PreserveLineOfSight(ref CameraState state)
		{
			Vector3 displacement = Vector3.zero;
			if (state.HasLookAt && collideAgainst != 0)
			{
				Vector3 cameraPos = state.CorrectedPosition;
				Vector3 lookAtPos = state.ReferenceLookAt;
				Vector3 cameraForward = lookAtPos - cameraPos;
				Vector3 rayDirection = -cameraForward;

				Ray ray = new Ray(lookAtPos, rayDirection);

				float correctionDistance = 0f;
				float rayDistance = rayDirection.magnitude;


				if (Physics.SphereCast(
					ray, cameraRadius, out RaycastHit hitInfo, rayDistance, collideAgainst,
					QueryTriggerInteraction.Ignore))
				{
					if (hitInfo.distance > minimumDistanceFromTarget)
						correctionDistance = rayDistance - hitInfo.distance;

					Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
					Debug.DrawRay(ray.origin, ray.direction.normalized * hitInfo.distance, Color.yellow);
				}

				displacement = cameraForward.normalized * correctionDistance;
			}
			return displacement;
		}

	}
}
