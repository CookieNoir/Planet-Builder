using UnityEngine;

public class BlockInfo
{
    public bool canPlace;
    public Vector3 position;
    public Quaternion rotation;
    public float scale;

    public BlockInfo(bool _canPlace, Vector3 _position, Quaternion _rotation, float _scale)
    {
        canPlace = _canPlace;
        position = _position;
        rotation = _rotation;
        scale = _scale;
    }
}
