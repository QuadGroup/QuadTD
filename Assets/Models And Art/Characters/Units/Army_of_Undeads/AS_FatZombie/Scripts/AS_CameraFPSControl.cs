using UnityEngine;
using System.Collections;

// I can't remember where I found this script.. just remember it being free.
// It really could use some cleaning up, though.  A lot of unnecessary variables floating around.
// Also want to make it use an orbit camera.

public class AS_CameraFPSControl : MonoBehaviour {

    public float sensitivityX = 4F;
    public float sensitivityY = 4F;
	public float force = 5;

    float mHdg = 0F;
    float mPitch = 0F;

    void Start()
    {
        mHdg = transform.localEulerAngles.y;
		mPitch = transform.localEulerAngles.x;
    }

    void Update()
    {
        if (!Input.GetMouseButton(1))
            return;

        float deltaX = Input.GetAxis("Mouse X") * sensitivityX;
        float deltaY = Input.GetAxis("Mouse Y") * sensitivityY;
		
        ChangeHeading(deltaX);
        ChangePitch(-deltaY);
    }
	void FixedUpdate ()
	{
    	GetComponent<Rigidbody>().AddForce(Input.GetAxis("Horizontal") * transform.right * force);
    	GetComponent<Rigidbody>().AddForce(Input.GetAxis("Vertical") * transform.forward * force);
	}

    void MoveForwards(float aVal)
    {
        Vector3 fwd = transform.forward;
        fwd.y = 0;
        fwd.Normalize();
        transform.position += aVal * fwd;
    }

    void Strafe(float aVal)
    {
        transform.position += aVal * transform.right;
    }

    void ChangeHeight(float aVal)
    {
        transform.position += aVal * Vector3.up;
    }

    void ChangeHeading(float aVal)
    {
        mHdg += aVal;
        WrapAngle(ref mHdg);
        transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
    }

    void ChangePitch(float aVal)
    {
        mPitch += aVal;
        WrapAngle(ref mPitch);
        transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
    }

    public static void WrapAngle(ref float angle)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
    }
}