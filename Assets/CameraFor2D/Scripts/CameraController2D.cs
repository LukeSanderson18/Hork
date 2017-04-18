using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoodStuff.NaturalLanguage;

public class CameraController2D : MonoBehaviour {
#region Internal Types
	public enum MovementAxis {
		XY,
		XZ,
		YZ
	}

	class OffsetData {
		public Vector3 StartPointRelativeToCamera { get; set; }
		public Vector3 Vector { get; set; }
		public Vector3 NormalizedVector { get; set; }
		public float DistanceFromStartPoint { get; set; }
	}
#endregion

	const string VERSION = "1.2.3";

	public IEnumerable<Transform> CurrentTarget { 
		get {
			if(targetStack.IsEmpty()) return null;
			return targetStack.Peek();
		}
		set { SetTarget(value); }
	}

	/// <summary>
	/// The position that the camera is attempting to move to. This is normally the target's position
	/// (or midpoint of the targets if there are multiple) plus the sum of all influences.  This is not
	/// the final position the camera will move to, but the position it would move to if there are no
	/// CameraBumpers to restrict movement and the camera has sufficient time to move.
	/// </summary>
	public Vector3 CameraSeekPosition { 
		get { return CameraSeekTarget.position; }
		set { 
			if(!ExclusiveModeEnabled) throw new System.InvalidOperationException("Cannot set an explicit camera seek target unless the camera is in exclusive mode");
			CameraSeekTarget.position = value;
		}
	}

	/// <summary>
	/// Modifies the "distance" to the target.  For ortho this modifies the camera's ortho-size, for
	/// perspective this modifies the actual height value.
	/// *NOTE*: After changing the DistanceMultiplier the camera bounds will be inaccurate and camera 
	/// bumpers will not work properly.  Please call CalculateScreenBounds before collision with a 
	/// CameraBumper occurs.
	/// </summary>
	/// <value>1 = normal distance, less than one zooms in, more than one zooms out</value>
	public float DistanceMultiplier {
		get { return distanceMultiplier; }
		set { 
			distanceMultiplier = value;
			if(GetComponent<Camera>().orthographic) GetComponent<Camera>().orthographicSize = originalZoom * distanceMultiplier;
			else heightFromTarget = originalZoom * distanceMultiplier;
		}
	}

	/// <summary>
	/// Exclusive mode allows you to position the camera's target manually.  You must enable exclusive mode before 
	/// attempting to set a CameraSeekPosition.  While in exclusive mode, all influences are ignored.
	/// </summary>
	public bool ExclusiveModeEnabled { get; set; }

	/// <summary>
	/// Is the camera moving to a new position as the result of a SetTarget or AddTarget?
	/// </summary>
	public bool MovingToNewTarget { get { return panningToNewTarget; } }

	public MovementAxis axis = MovementAxis.XZ;
	public LayerMask cameraBumperLayers;

	/// <summary>
	/// Where to position the camera, along the "up" vector from the target.  The "up" vector is determined
	/// based on the axis that is chosen.
	/// </summary>
	public float heightFromTarget;

	/// <summary>
	/// Eqivalent to calling AddTarget with this Transform
	/// </summary>
	public Transform initialTarget;

	/// <summary>
	/// The maximum move speed allowed when moving towards the camera's target
	/// </summary>
	public float maxMoveSpeedPerSecond = 10;

	/// <summary>
	/// Damping is a slowing or delay factor for the camera's movement.  Higher damping creates a "squishier"
	/// feeling camera that lags behind when the target moves.  To get zero lag use a value of 0.015.
	/// </summary>
	public float damping = .5f;

	/// <summary>
	/// The distance from the target where OnNewTargetReached callbacks should be sent.
	/// </summary>
	public float arrivalNotificationDistance = .01f;
	
	/// <summary>
	/// This defines a rect in screen space (so 0.5, 0.5 is the center of the screen), that when moved around in, does
	/// not cause the screen to move.  When exceeding any edge of the push box, the screen will move until the target
	/// is once again inside the push box.
	/// </summary>
	public Rect pushBox = new Rect(.5f, .5f, 0, 0);

	/// <summary>
	/// The camera to use for collision checks with CameraBumpers.  
	/// Defaults to the camera attached to the current GameObject.
	/// </summary>
	public Camera cameraToUse;

	/// <summary>
	/// Should the movement caculations be performed in FixedUpdate instead of LateUpdate
	/// </summary>
	public bool useFixedUpdate;

	// Called after the initial transition to a target set via AddTarget or SetTarget
	public System.Action OnNewTargetReached = null;

	// Called after a new target has been set via AddTarget
	public System.Action OnTargetAdded = null;

	// Called after a target has been switched via SetTarget
	public System.Action OnTargetSwitched = null;


#if UNITY_EDITOR
	/// <summary>
	///  Enable editor debug lines.  This increases the cost of many operations and will degrade the performance
	/// of the camera slightly.
	/// </summary>
	public bool drawDebugLines;
#endif

	const float CAMERA_ARRIVAL_DISTANCE = .01f;
	const float CAMERA_ARRIVAL_DISTANCE_SQUARED = CAMERA_ARRIVAL_DISTANCE * CAMERA_ARRIVAL_DISTANCE;

	Transform CameraSeekTarget { get; set; }
	
	System.Func<Vector3> HeightOffset;
	System.Func<Vector3, Vector3> GetHorizontalComponent;
	System.Func<Vector3, Vector3> GetVerticalComponent;
	System.Func<Vector3, float> GetHorizontalValue;
	System.Func<Vector3, float> GetVerticalValue;
	
	OffsetData leftRaycastPoint;
	OffsetData upperLeftRaycastPoint;
	OffsetData lowerLeftRaycastPoint;
	OffsetData rightRaycastPoint;
	OffsetData upperRightRaycastPoint;
	OffsetData lowerRightRaycastPoint;

	OffsetData upRaycastPoint;
	OffsetData downRaycastPoint;
	OffsetData leftUpRaycastPoint;
	OffsetData rightUpRaycastPoint;
	OffsetData leftDownRaycastPoint;
	OffsetData rightDownRaycastPoint;
	
	Stack<IEnumerableThatIgnoresNull<Transform>> targetStack = new Stack<IEnumerableThatIgnoresNull<Transform>>();
	Vector3 velocity;
	List<Vector3> influences = new List<Vector3>(5);
	bool panningToNewTarget;
	float panningToNewTargetSpeed;
	bool arrivalNotificationSent;
	System.Action arrivedAtNewTargetCallback;
	float arrivalNotificationDistanceSquared;
	float distanceMultiplier = 1;
	float originalZoom;
	float revertAfterDuration;
	float revertMoveSpeed;
	float acceleration;

#if UNITY_EDITOR
	readonly Color ORANGE_COLOR = new Color(1, .8f, 0);
	Vector3 lastCalculatedPosition;
	Vector3 lastCalculatedIdealPosition;
	Vector3 lastCalculatedInfluence;
	Vector3[] influencesForGizmoRendering = new Vector3[0];
#endif

	public void SetTarget(Transform target) {
		SetTarget(new [] { target });
	}

	public void SetTarget(Transform target, System.Action callback) {
		SetTarget(new [] { target }, callback);
	}

	public void SetTarget(Transform target, float moveSpeed) {
		SetTarget(new [] { target }, moveSpeed);
	}

	public void SetTarget(Transform target, float moveSpeed, System.Action callback) {
		SetTarget(new [] { target }, moveSpeed, callback);
	}

	public void SetTarget(IEnumerable<Transform> targets) {
		SetTarget(targets, null);
	}

	public void SetTarget(IEnumerable<Transform> targets, System.Action callback) {
		SetTarget(targets, maxMoveSpeedPerSecond, callback);
	}

	public void SetTarget(IEnumerable<Transform> targets, float moveSpeed) {
		SetTarget(targets, moveSpeed, null);
	}

	public void SetTarget(IEnumerable<Transform> targets, float moveSpeed, System.Action callback) {
		if(targets.Any(t => null == t)) throw new System.ArgumentException("Cannot add a target that is null");

		if(0 == moveSpeed) moveSpeed = maxMoveSpeedPerSecond;
		RemoveCurrentTarget();
		targetStack.Push(new IEnumerableThatIgnoresNull<Transform>(targets));
		panningToNewTarget = true;
		panningToNewTargetSpeed = moveSpeed;
		arrivalNotificationSent = false;
		if(OnTargetSwitched != null) OnTargetSwitched();

		if(callback != null) {
			if(arrivedAtNewTargetCallback != null) arrivedAtNewTargetCallback += callback;
			else arrivedAtNewTargetCallback = callback;
		}
	}

	public void AddTarget(Transform target) {
		AddTarget(new [] { target });
	}

	public void AddTarget(Transform target, System.Action callback) {
		AddTarget(new [] { target }, callback);
	}

	public void AddTarget(Transform target, float moveSpeed) {
		AddTarget(new [] { target }, moveSpeed);
	}

	public void AddTarget(Transform target, float moveSpeed, System.Action callback) {
		AddTarget(new [] { target }, moveSpeed, callback);
	}

	public void AddTarget(Transform target, float moveSpeed, float revertAfterDuration, float revertMoveSpeed) {
		AddTarget(new [] { target }, moveSpeed, revertAfterDuration, revertMoveSpeed);
	}

	public void AddTarget(Transform target, float moveSpeed, float revertAfterDuration, float revertMoveSpeed, System.Action callback) {
		AddTarget(new [] { target }, moveSpeed, revertAfterDuration, revertMoveSpeed, callback);
	}

	public void AddTarget(IEnumerable<Transform> targets) {
		AddTarget(targets, maxMoveSpeedPerSecond);
	}

	public void AddTarget(IEnumerable<Transform> targets, System.Action callback) {
		AddTarget(targets, maxMoveSpeedPerSecond, callback);
	}

	public void AddTarget(IEnumerable<Transform> targets, float moveSpeed) {
		AddTarget(targets, moveSpeed, null);
	}

	/// <summary>
	/// Adds one or more targets to the target stack.  Each variation of AddTarget has a version that takes a single
	/// Transform as well as an IEnumerable<Transform>.  <seealso cref="AddTarget(Transform target, float moveSpeed, System.Action callback)">
	/// 
	/// </summary>
	/// <param name="targets">An IEnumerable<Transform> of targets to be focused.</param>
	/// <param name="moveSpeed">The speed to move while transitioning to the new target.</param>
	/// <param name="callback">A System.Action to be run when arriving at the new target.</param>
	public void AddTarget(IEnumerable<Transform> targets, float moveSpeed, System.Action callback) {
		if(targets.Any(t => null == t)) throw new System.ArgumentException("Cannot add a target that is null");

		if(0 == moveSpeed) moveSpeed = maxMoveSpeedPerSecond;
		targetStack.Push(new IEnumerableThatIgnoresNull<Transform>(targets));
		panningToNewTarget = true;
		panningToNewTargetSpeed = moveSpeed;
		arrivalNotificationSent = false;
		if(OnTargetAdded != null) OnTargetAdded();
		if(callback != null) {
			if(arrivedAtNewTargetCallback != null) arrivedAtNewTargetCallback += callback;
			else arrivedAtNewTargetCallback = callback;
		}
	}

	public void AddTarget(IEnumerable<Transform> targets, float moveSpeed, float revertAfterDuration, float revertMoveSpeed) {
		AddTarget(targets, moveSpeed, revertAfterDuration, revertMoveSpeed, null);
	}

	/// <summary>
	/// Adds one or more targets to the target stack.  Each variation of AddTarget has a version that takes a single
	/// Transform as well as an IEnumerable<Transform>.  <seealso cref="AddTarget(Transform target, float moveSpeed, float revertAfterDuration, float revertMoveSpeed, System.Action callback)">
	/// 
	/// </summary>
	/// <param name="targets">An IEnumerable<Transform> of targets to be focused.</param>
	/// <param name="moveSpeed">The speed to move while transitioning to the new target.</param>
	/// <param name="revertAfterDuration">Should the newly added target be automatically removed after a duration.</param>
	/// <param name="revertMoveSpeed">The speed to move when returning to the original target.</param>
	/// <param name="callback">A System.Action to be run when arriving at the new target.</param>
	public void AddTarget(IEnumerable<Transform> targets, float moveSpeed, float revertAfterDuration, float revertMoveSpeed, System.Action callback) {
		if(targets.Any(t => null == t)) throw new System.ArgumentException("Cannot add a target that is null");

		if(0 == moveSpeed) moveSpeed = maxMoveSpeedPerSecond;
		targetStack.Push(new IEnumerableThatIgnoresNull<Transform>(targets));
		panningToNewTarget = true;
		panningToNewTargetSpeed = moveSpeed;
		arrivalNotificationSent = false;
		this.revertAfterDuration = revertAfterDuration;
		this.revertMoveSpeed = revertMoveSpeed;
		OnNewTargetReached += RevertAfterReachingTarget;
		if(OnTargetAdded != null) OnTargetAdded();

		if(callback != null) {
			if(arrivedAtNewTargetCallback != null) arrivedAtNewTargetCallback += callback;
			else arrivedAtNewTargetCallback = callback;
		}
	}

	public void RemoveCurrentTarget() {
		if(targetStack.IsEmpty()) return;

		targetStack.Pop();
		panningToNewTarget = true;
		panningToNewTargetSpeed = maxMoveSpeedPerSecond;
		arrivalNotificationSent = false;
	}

	public void AddInfluence(Vector3 influence) {
		influences.Add(influence);
	}

	public void JumpToIdealPosition() {
		if(!ExclusiveModeEnabled) throw new System.InvalidOperationException("Cannot set an explicit camera position unless the camera is in exclusive mode");
		transform.position = IdealCameraPosition();
	}

	public void JumpToTargetRespectingBumpersAndInfluences() {
		var idealPosition = IdealCameraPosition() + TotalInfluence();
		transform.position = idealPosition + CalculatePushBackOffset(idealPosition);
	}

	// This must be Awake and not start to ensure that all the delegates are assigned before scripts attempt to perform
	// any actions on the camera such as SetTarget or AddTarget.
	public void Awake() {
		if(null == cameraToUse) cameraToUse = GetComponent<Camera>();
		if(null == cameraToUse) Debug.LogError("No camera was specified and no Camera component is attached to this GameObject, CameraController2D requires a camera to function");

		switch(axis) {
		case MovementAxis.XY:
			HeightOffset = () => Vector3.forward * heightFromTarget;
			GetHorizontalComponent = (vector) => new Vector3(vector.x, 0, 0);
			GetHorizontalValue = (vector) => vector.x;
			GetVerticalComponent = (vector) => new Vector3(0, vector.y, 0);
			GetVerticalValue = (vector) => vector.y;
			break;
		case MovementAxis.XZ:
			HeightOffset = () => -Vector3.up * heightFromTarget;
			GetHorizontalComponent = (vector) => new Vector3(vector.x, 0, 0);
			GetHorizontalValue = (vector) => vector.x;
			GetVerticalComponent = (vector) => new Vector3(0, 0, vector.z);
			GetVerticalValue = (vector) => vector.z;
			break;
		case MovementAxis.YZ:
			HeightOffset = () => -Vector3.right * heightFromTarget;
			GetHorizontalComponent = (vector) => new Vector3(0, 0, vector.z);
			GetHorizontalValue = (vector) => vector.z;
			GetVerticalComponent = (vector) => new Vector3(0, vector.y, 0);
			GetVerticalValue = (vector) => vector.y;
			break;
		}

		CameraSeekTarget = new GameObject("_CameraTarget").transform;

		if(cameraToUse.orthographic) originalZoom = cameraToUse.orthographicSize;
		else originalZoom = heightFromTarget;
		CalculateScreenBounds();

		if(initialTarget != null) {
			AddTarget(initialTarget);
			JumpToTargetRespectingBumpersAndInfluences();
		}

		arrivalNotificationDistanceSquared = arrivalNotificationDistance * arrivalNotificationDistance;
	}

	public void FixedUpdate() {
		if(useFixedUpdate) ApplyMovement();
	}

	public void LateUpdate() {
		if(!useFixedUpdate) ApplyMovement();
	}

	/// <summary>
	/// This calculates the points to be used when determining collision with CameraBumper objects.
	/// Typically this only needs to be run once, but if the DistanceMultiplier is changed this
	/// will need to be run before the next collision with a CameraBumper.
	/// </summary>
	public void CalculateScreenBounds() {
		System.Func<Vector3, Vector3, OffsetData> AddRaycastOffsetPoint = (viewSpaceOrigin, viewSpacePoint) => {
			if(cameraToUse.orthographic) {
				var origin = cameraToUse.ViewportToWorldPoint(viewSpaceOrigin);
				var vectorToOffset = cameraToUse.ViewportToWorldPoint(viewSpacePoint) - origin;
				return new OffsetData { StartPointRelativeToCamera = origin - transform.position, Vector = vectorToOffset, NormalizedVector = vectorToOffset.normalized, DistanceFromStartPoint = vectorToOffset.magnitude };
			}
			else {
				var cameraPositionOnPlane = transform.position + (transform.forward * heightFromTarget);

				var originRay = cameraToUse.ViewportPointToRay(viewSpaceOrigin);
				var theta = Vector3.Angle(transform.forward, originRay.direction);
				var distanceToPlane = heightFromTarget / Mathf.Cos(theta * Mathf.Deg2Rad);
				var originPointOnPlane = originRay.origin + (originRay.direction * distanceToPlane);

				var pointRay = cameraToUse.ViewportPointToRay(viewSpacePoint);
				theta = Vector3.Angle(cameraToUse.transform.forward, pointRay.direction);
				distanceToPlane = heightFromTarget / Mathf.Cos(theta * Mathf.Deg2Rad);
				var pointOnPlane = pointRay.origin + (pointRay.direction * distanceToPlane);
				var vectorToOffset = pointOnPlane - originPointOnPlane;

				return new OffsetData { StartPointRelativeToCamera = originPointOnPlane - cameraPositionOnPlane, Vector = vectorToOffset, NormalizedVector = vectorToOffset.normalized, DistanceFromStartPoint = vectorToOffset.magnitude };
			}
		};

		leftRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 0.5f), new Vector3(0, 0.5f));
		rightRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 0.5f), new Vector3(1, 0.5f));
		lowerLeftRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 0), new Vector3(0, 0));
		lowerRightRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 0), new Vector3(1, 0));
		upperLeftRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 1), new Vector3(0, 1));
		upperRightRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 1), new Vector3(1, 1));

		downRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 0.5f), new Vector3(0.5f, 0));
		upRaycastPoint = AddRaycastOffsetPoint(new Vector3(0.5f, 0.5f), new Vector3(0.5f, 1));
		leftUpRaycastPoint = AddRaycastOffsetPoint(new Vector3(0, 0.5f), new Vector3(0, 1));
		leftDownRaycastPoint = AddRaycastOffsetPoint(new Vector3(0, 0.5f), new Vector3(0, 0));
		rightUpRaycastPoint = AddRaycastOffsetPoint(new Vector3(1, 0.5f), new Vector3(1, 1));
		rightDownRaycastPoint = AddRaycastOffsetPoint(new Vector3(1, 0.5f), new Vector3(1, 0));
	}

	void ApplyMovement() {
		var position = transform.position;

		if(!ExclusiveModeEnabled) CameraSeekTarget.position = IdealCameraPosition() + TotalInfluence();
		var idealPosition = CameraSeekPosition;
		var targetPosition = idealPosition + CalculatePushBackOffset(idealPosition);

		var targetViewportPoint = cameraToUse.WorldToViewportPoint(targetPosition);
		var targetWithinMoveBox = TargetViewportPointWithinMoveBox(targetViewportPoint);

		if(panningToNewTarget || !targetWithinMoveBox) {
			var maxSpeed = maxMoveSpeedPerSecond;
			if(panningToNewTarget) maxSpeed = panningToNewTargetSpeed;
			if(!targetWithinMoveBox && !panningToNewTarget) {
				var topLeftPoint = new Vector2(pushBox.x - (pushBox.width / 2), pushBox.y + (pushBox.height / 2));
				var bottomRightPoint = new Vector2(pushBox.x + (pushBox.width / 2), pushBox.y - (pushBox.height / 2));
				// move target to edge of move box instead of moving as far as we are able to based on deltaTime
				// to avoid having a 3 no move, 1 move update stuttering pattern

				var xDifference = 0f;
				var yDifference = 0f;
				if(targetViewportPoint.x < topLeftPoint.x || targetViewportPoint.x > bottomRightPoint.x) xDifference = Mathf.Abs(targetViewportPoint.x - pushBox.x) / (pushBox.width / 2);
				if(targetViewportPoint.y > topLeftPoint.y || targetViewportPoint.y < bottomRightPoint.y) yDifference = Mathf.Abs(targetViewportPoint.y - pushBox.y) / (pushBox.height / 2);

				float scaleFactor;

				if(xDifference > yDifference) {
					scaleFactor = 1 - (1 / xDifference);
				}
				else {
					scaleFactor = 1 - (1 / yDifference);
				}

				targetPosition = transform.position + ((targetPosition - position) * scaleFactor);
			}

			var vectorToTarget = targetPosition - position;
			var vectorToTargetAlongPlane = GetHorizontalComponent(vectorToTarget) + GetVerticalComponent(vectorToTarget);
			var interpolatedPosition = Vector3.zero;
			var xDelta = Mathf.Abs(vectorToTargetAlongPlane.x);
			var yDelta = Mathf.Abs(vectorToTargetAlongPlane.y);
			var zDelta = Mathf.Abs(vectorToTargetAlongPlane.z);
			var xyzTotal = xDelta + yDelta + zDelta;
			
			interpolatedPosition.x = Mathf.SmoothDamp(position.x, targetPosition.x, ref velocity.x, damping, maxSpeed * (xDelta / xyzTotal));
			interpolatedPosition.y = Mathf.SmoothDamp(position.y, targetPosition.y, ref velocity.y, damping, maxSpeed * (yDelta / xyzTotal));
			interpolatedPosition.z = Mathf.SmoothDamp(position.z, targetPosition.z, ref velocity.z, damping, maxSpeed * (zDelta / xyzTotal));
			transform.position = interpolatedPosition;
		}
		else {
			velocity = Vector3.zero;
		}


		#if UNITY_EDITOR
		if (drawDebugLines) {
			lastCalculatedPosition = transform.position;
			lastCalculatedIdealPosition = IdealCameraPosition();
			lastCalculatedInfluence = TotalInfluence();
			influencesForGizmoRendering = new Vector3[influences.Count];
			influences.CopyTo(influencesForGizmoRendering);
		}
		#endif

		if(!arrivalNotificationSent && panningToNewTarget && (targetPosition - position).sqrMagnitude <= arrivalNotificationDistanceSquared) {
			if(OnNewTargetReached != null) OnNewTargetReached();
			if(arrivedAtNewTargetCallback != null) {
				arrivedAtNewTargetCallback();
				arrivedAtNewTargetCallback = null;
			}
			arrivalNotificationSent = true;
			panningToNewTarget = false;
		}

		influences.Clear();
	}

	Vector3 CalculatePushBackOffset(Vector3 idealPosition) {
		var idealCenterPointAtPlayerHeight = idealPosition + HeightOffset();
		var horizontalVector = GetHorizontalComponent(Vector3.one).normalized;
		var verticalVector = GetVerticalComponent(Vector3.one).normalized;
		var horizontalFacing = 0;
		var verticalFacing = 0;
		var horizontalPushBack = 0f;
		var verticalPushBack = 0f;
		var rightHorizontalPushBack = 0f;
		var leftHorizontalPushBack = 0f;
		var upVerticalPushBack = 0f;
		var downVerticalPushBack = 0f;

		rightHorizontalPushBack = CalculatePushback(rightRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.HorizontalPositive);
		if (rightHorizontalPushBack > horizontalPushBack) {
			horizontalPushBack = rightHorizontalPushBack;
			horizontalFacing = 1;
		}
		if (0 == rightHorizontalPushBack) {
			upVerticalPushBack = CalculatePushback(rightUpRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.VerticalPositive);
			if (upVerticalPushBack > verticalPushBack) {
				verticalPushBack = upVerticalPushBack;
				verticalFacing = 1;
			}
			downVerticalPushBack = CalculatePushback(rightDownRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.VerticalNegative);
			if (downVerticalPushBack > verticalPushBack) {
				verticalPushBack = downVerticalPushBack;
				verticalFacing = -1;
			}
		}
		leftHorizontalPushBack = CalculatePushback(leftRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.HorizontalNegative);
		if (leftHorizontalPushBack > horizontalPushBack) {
			horizontalPushBack = leftHorizontalPushBack;
			horizontalFacing = -1;
		}
		if (0 == leftHorizontalPushBack) {
			upVerticalPushBack = CalculatePushback(leftUpRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.VerticalPositive);
			if (upVerticalPushBack > verticalPushBack) {
				verticalPushBack = upVerticalPushBack;
				verticalFacing = 1;
			}
			downVerticalPushBack = CalculatePushback(leftDownRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.VerticalNegative);
			if (downVerticalPushBack > verticalPushBack) {
				verticalPushBack = downVerticalPushBack;
				verticalFacing = -1;
			}
		}
		upVerticalPushBack = CalculatePushback(upRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.VerticalPositive);
		if (upVerticalPushBack > verticalPushBack) {
			verticalPushBack = upVerticalPushBack;
			verticalFacing = 1;
		}
		if (0 == upVerticalPushBack) {
			rightHorizontalPushBack = CalculatePushback(upperRightRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.HorizontalPositive);
			if (rightHorizontalPushBack > horizontalPushBack) {
				horizontalPushBack = rightHorizontalPushBack;
				horizontalFacing = 1;
			}
			leftHorizontalPushBack = CalculatePushback(upperLeftRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.HorizontalNegative);
			if (leftHorizontalPushBack > horizontalPushBack) {
				horizontalPushBack = leftHorizontalPushBack;
				horizontalFacing = -1;
			}
		}
		downVerticalPushBack = CalculatePushback(downRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.VerticalNegative);
		if (downVerticalPushBack > verticalPushBack) {
			verticalPushBack = downVerticalPushBack;
			verticalFacing = -1;
		}
		if (0 == downVerticalPushBack) {
			rightHorizontalPushBack = CalculatePushback(lowerRightRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.HorizontalPositive);
			if (rightHorizontalPushBack > horizontalPushBack) {
				horizontalPushBack = rightHorizontalPushBack;
				horizontalFacing = 1;
			}
			leftHorizontalPushBack = CalculatePushback(lowerLeftRaycastPoint, idealCenterPointAtPlayerHeight, CameraBumper.BumperDirection.HorizontalNegative);
			if (leftHorizontalPushBack > horizontalPushBack) {
				horizontalPushBack = leftHorizontalPushBack;
				horizontalFacing = -1;
			}
		}
		return (verticalVector * -verticalPushBack * verticalFacing) + (horizontalVector * -horizontalPushBack * horizontalFacing);
	}

	float CalculatePushback(OffsetData offset, Vector3 idealCenterPoint, CameraBumper.BumperDirection validDirections = CameraBumper.BumperDirection.AllDirections) {
		RaycastHit hitInfo;
		var pushbackDueToCollision = 0f;

		if(Physics.Raycast(idealCenterPoint + offset.StartPointRelativeToCamera, offset.NormalizedVector, out hitInfo, offset.DistanceFromStartPoint, cameraBumperLayers)) {
			var bumper = hitInfo.collider.GetComponent<CameraBumper>();
			if(null == bumper || (bumper != null && (bumper.blockDirection & validDirections) != CameraBumper.BumperDirection.None)) {
				pushbackDueToCollision = offset.DistanceFromStartPoint - hitInfo.distance;
#if UNITY_EDITOR
				if(drawDebugLines) Debug.DrawLine(idealCenterPoint + offset.StartPointRelativeToCamera, idealCenterPoint + offset.StartPointRelativeToCamera + (offset.NormalizedVector * hitInfo.distance), Color.red);
#endif
			}
		}
#if UNITY_EDITOR
		else if(drawDebugLines) Debug.DrawLine(idealCenterPoint + offset.StartPointRelativeToCamera, idealCenterPoint + offset.StartPointRelativeToCamera + offset.Vector, Color.green);
#endif

		return pushbackDueToCollision;
	}

	Vector3 TotalInfluence() {
		return influences.Aggregate(Vector3.zero, (offset, influence) => offset + influence);
	}

	Vector3 IdealCameraPosition() {
		if(targetStack.IsEmpty()) return Vector3.zero;
		var targets = targetStack.Peek();

		if(1 == targets.Count()) return targets.First().position - HeightOffset();

		var minHorizontal = targets.Min(t => GetHorizontalValue(t.position));
		var maxHorizontal = targets.Max(t => GetHorizontalValue(t.position));
		var horizontalOffset = (maxHorizontal - minHorizontal) * 0.5f;
		var minVertical = targets.Min(t => GetVerticalValue(t.position));
		var maxVertical = targets.Max(t => GetVerticalValue(t.position));
		var verticalOffset = (maxVertical - minVertical) * 0.5f;
		return (GetHorizontalComponent(Vector3.one) * (minHorizontal + horizontalOffset)) + (GetVerticalComponent(Vector3.one) * (minVertical + verticalOffset)) - HeightOffset();
	}

	void RevertAfterReachingTarget() {
		StartCoroutine(RemoveTargetAfterDelay(revertAfterDuration, revertMoveSpeed));
		OnNewTargetReached -= RevertAfterReachingTarget;
	}

	IEnumerator RemoveTargetAfterDelay(float delay, float revertMoveSpeed) {
		yield return new WaitForSeconds(delay);
		panningToNewTarget = true;
		panningToNewTargetSpeed = revertMoveSpeed;
		arrivalNotificationSent = false;
		targetStack.Pop();
	}

	bool TargetViewportPointWithinMoveBox(Vector3 target) {
		var topLeftPoint = new Vector2(pushBox.x - (pushBox.width / 2), pushBox.y + (pushBox.height / 2));
		var bottomRightPoint = new Vector2(pushBox.x + (pushBox.width / 2), pushBox.y - (pushBox.height / 2));
		return target.x >= topLeftPoint.x && target.x <= bottomRightPoint.x && target.y <= topLeftPoint.y && target.y >= bottomRightPoint.y;
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		if(Application.isPlaying && drawDebugLines) {
			if(!ExclusiveModeEnabled) {
				Gizmos.color = Color.magenta;
				Gizmos.DrawLine(lastCalculatedIdealPosition, lastCalculatedIdealPosition + lastCalculatedInfluence);

				Gizmos.color = ORANGE_COLOR;
				influencesForGizmoRendering.Each(influence => Gizmos.DrawLine(lastCalculatedIdealPosition, lastCalculatedIdealPosition + influence));
			}

			Gizmos.color = ORANGE_COLOR;
			Gizmos.DrawWireSphere(lastCalculatedIdealPosition, .1f);

			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(lastCalculatedPosition, .1f);

			Gizmos.color = Color.blue;
			var topLeftPoint = new Vector2(pushBox.x - (pushBox.width / 2), pushBox.y + (pushBox.height / 2));
			var bottomRightPoint = new Vector2(pushBox.x + (pushBox.width / 2), pushBox.y - (pushBox.height / 2));

			Gizmos.DrawLine(cameraToUse.ViewportToWorldPoint(new Vector3(topLeftPoint.x, topLeftPoint.y)), cameraToUse.ViewportToWorldPoint(new Vector3(bottomRightPoint.x, topLeftPoint.y)));
			Gizmos.DrawLine(cameraToUse.ViewportToWorldPoint(new Vector3(topLeftPoint.x, bottomRightPoint.y)), cameraToUse.ViewportToWorldPoint(new Vector3(bottomRightPoint.x, bottomRightPoint.y)));

			Gizmos.DrawLine(cameraToUse.ViewportToWorldPoint(new Vector3(topLeftPoint.x, topLeftPoint.y)), cameraToUse.ViewportToWorldPoint(new Vector3(topLeftPoint.x, bottomRightPoint.y)));
			Gizmos.DrawLine(cameraToUse.ViewportToWorldPoint(new Vector3(bottomRightPoint.x, topLeftPoint.y)), cameraToUse.ViewportToWorldPoint(new Vector3(bottomRightPoint.x, bottomRightPoint.y)));

		}
	}

#endif
}