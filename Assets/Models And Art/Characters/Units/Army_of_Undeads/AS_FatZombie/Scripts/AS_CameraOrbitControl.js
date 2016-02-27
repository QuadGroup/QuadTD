#pragma strict

var target : Transform;
var distance = 3.0;
var distanceMinLimit = 0.5;
var distanceMaxLimit = 4.0;
var heightMinLimit = -1.0;
var heightMaxLimit = 1.0;
var heightSensitivity = 0.05;
var zoomSensitivity = 2.0;

var xSpeed = 250.0;
var ySpeed = 120.0;

var yMinLimit = -20;
var yMaxLimit = 80;

private var x = 0.0;
private var y = 0.0;
private var height = 0.0;

@script AddComponentMenu("Camera-Control/Mouse Orbit (by Arabian Studios)")

function Start () {
	// UNITY 3.5 BETA BUG WORKAROUND: have to slightly rotate the camera otherwise a weird flickering occurs on the ground plane.
	var angles = transform.eulerAngles;
	x = angles.y;
	y = angles.x + 0.1;
	
	// Make the rigid body not change rotation
 	if (GetComponent.<Rigidbody>())
		GetComponent.<Rigidbody>().freezeRotation = true;
	
	if (target)
		heightMinLimit += target.position.y;
		heightMaxLimit += target.position.y;
		height = target.position.y;
	
	UpdateCameraRotation();
	UpdateCameraPosition();
}

function LateUpdate () {
	if (target) {
		if (Input.GetButton("Fire1")) {
			UpdateCameraRotation();
		}
		if (Input.GetButton("Fire2")) {
			height += Input.GetAxis("Mouse Y") * heightSensitivity;
			height = Mathf.Clamp(height, heightMinLimit, heightMaxLimit);
		}
		UpdateCameraPosition();
	}
}

function UpdateCameraRotation() {
		x += Input.GetAxis("Mouse X") * xSpeed * 0.02;
		y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02;
 		y = ClampAngle(y, yMinLimit, yMaxLimit);
		transform.rotation = Quaternion.Euler(y, x, 0);
}

function UpdateCameraPosition() {
	distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
	distance = Mathf.Clamp(distance,distanceMinLimit,distanceMaxLimit);
	target.position.y = height;
	transform.position = transform.rotation * Vector3(0.0, 0.0, -distance) + target.position;
}
function ClampAngle (angle : float, min : float, max : float) {
	if (angle < -360)
		angle += 360;
	if (angle > 360)
		angle -= 360;
	return Mathf.Clamp (angle, min, max);
}