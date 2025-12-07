using UnityEngine;

public class TransformData
{
    public float[] position;
    public float[] rotation;
    public float[] scale;

    public TransformData() { }

    public TransformData(Transform transform)
    {
        position = new float[] { transform.position.x, transform.position.y, transform.position.z };
        rotation = new float[] { transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w };
        scale = new float[] { transform.localScale.x, transform.localScale.y, transform.localScale.z };
    }

    public void ApplyTo(Transform transform)
    {
        transform.position = new Vector3(position[0], position[1], position[2]);
        transform.rotation = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
        transform.localScale = new Vector3(scale[0], scale[1], scale[2]);
    }
}
