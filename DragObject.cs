using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{


    private Vector3 offset;                     // used for translating screen coords to world coords
    private Vector3 screenPoint;

    public enum Direction { x_axis, z_axis };       // creates a drop-down menu in the object inspector to switch between x and z dragging; sets z to default.
    public Direction dropdown = Direction.z_axis;

    const float MIN_X = -1.0f;                      // outer limits of the board in my game world
    const float MAX_X = 1.0f;

    const float MIN_Z = -1.5f;
    const float MAX_Z = 0.5f;

    private bool dragging = true;       // enables dragging and sets solved to false
    private bool solved = false;






    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);   // converts screen coords to world coords
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        dragging = true;
    }






    void OnMouseDrag()
    {
        if (dragging)
        {

            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

            if (dropdown == Direction.z_axis)       // for some reason z and x are reversed but it works
            {
                curPosition.x = transform.position.x;

                if (curPosition.z < MIN_Z)          // keeps cubes within board area
                    curPosition.z = MIN_Z;

                if (curPosition.z > MAX_Z)
                    curPosition.z = MAX_Z;
            }



            if (dropdown == Direction.x_axis)
            {
                curPosition.z = transform.position.z;

               

                if (curPosition.x < MIN_X)          // keeps cubes within board area
                    curPosition.x = MIN_X;

                if (!solved)
                {
                    if (curPosition.x > MAX_X)      // if the puzzle is solved, let the target cube slide out
                        curPosition.x = MAX_X;
                }
            }

            transform.position = curPosition;

        }
    }        
    








    void OnMouseUp()                    // since this is on a grid, the center of a 2x1 vertical cube ends up half a space off, so we need to correct this by rounding.
    {

        if (dropdown == Direction.z_axis)
        {
            float curZ = this.transform.position.z;    // get current z position, round that to nearest int, check if its closer to +0.5 or -0.5 
            float roundedZ = Mathf.Round(curZ);
            float upper = roundedZ + 0.5f;
            float lower = roundedZ - 0.5f;

            if (Mathf.Abs(upper - curZ) < Mathf.Abs(lower - curZ))
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, upper);
            }

            else if (Mathf.Abs(upper - curZ) >= Mathf.Abs(lower - curZ))
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, lower);
            }

        }

        if (dropdown == Direction.x_axis)
            this.transform.position = new Vector3(Mathf.Round(this.transform.position.x), this.transform.position.y, this.transform.position.z);

        dragging = true;
    }









    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Cube")   // if your cube hits another cube, disable the ability to drag.
        {
            dragging = false;
        }

        if (col.gameObject.tag == "Exit")   // if the target cube hits the exit square, set solved to true so the cube can slide out of the puzzle area.
        {
            solved = true;
        }
    }
}
