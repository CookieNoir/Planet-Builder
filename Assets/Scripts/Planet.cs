using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public enum NumberOfSlots { _6 = 6, _8 = 8, _9 = 9, _10 = 10, _12 = 12, _15 = 15, _18 = 18 };
    [Header("Planet Properties")]
    public NumberOfSlots numberOfSlots = NumberOfSlots._6;
    [Range(1, 5)] public int height = 1;
    [Range(1, 3)] public int difficulty = 1;
    [Range(15f, 180f)] public float shipSpeed = 60f;
    public float planetRadius = 1f;
    public float planetAtmosphereRadius = 1f;
    public float blockScale = 0.5f;

    [Header("Planet Components")]
    public GameObject[] props;
    public GameObject[] obstacles;

    private List<GameObject> _blocks;

    private int _numberOfSlots;
    private int[] _buildingsHeight;
    private int _blocksPlaced;

    private int _buildingsToWin;
    private int _buildingsComplete;
    private int _obstaclesAmount;

    private float _slotAngle;
    private float _slotAngleHalf;

    public void Create()
    {
        _numberOfSlots = (int)numberOfSlots;
        _buildingsHeight = new int[_numberOfSlots];
        for (int i = 0; i < _numberOfSlots; ++i)
        {
            _buildingsHeight[i] = 0;
        }
        _slotAngle = 360f / _numberOfSlots;
        _slotAngleHalf = _slotAngle / 2;
        _blocksPlaced = 0;
        _buildingsComplete = 0;

        setObstacles();
        setProps();
        setTarget();

        _blocks = new List<GameObject>();
    }

    public void Refresh()
    {
        int _count = _blocks.Count;
        for (int i = 0; i < _count; ++i)
        {
            Destroy(_blocks[i]);
        }
        _blocksPlaced = 0;
        _buildingsComplete = 0;
        for (int i = 0; i < _numberOfSlots; ++i)
        {
            if (_buildingsHeight[i] > 0) _buildingsHeight[i] = 0;
        }
    }

    public float GetSpeed()
    {
        return shipSpeed;
    }

    private void setObstacles()
    {
        _obstaclesAmount = (_numberOfSlots - 1) / 4 * difficulty + ((_numberOfSlots - 1) % 4) / 2 * (difficulty - 1);
        for (int i = 0; i < _obstaclesAmount; ++i)
        {
            int _pos = Random.Range(0, _numberOfSlots);
            while (_buildingsHeight[_pos] == -1) _pos = (_pos + 1) % _numberOfSlots;
            _buildingsHeight[_pos] = -1;
            setObject(obstacles, _pos);
        }
    }

    private void setProps()
    {
        int _propsAmount = _numberOfSlots - _obstaclesAmount - Random.Range(0, 3);
        for (int i = 0; i < _propsAmount; ++i)
        {
            int _pos = Random.Range(0, _numberOfSlots);
            while (_buildingsHeight[_pos] == -1) _pos = (_pos + 1) % _numberOfSlots;
            setObject(props, _pos);
        }
    }

    private void setTarget()
    {
        _buildingsToWin = (_numberOfSlots - _obstaclesAmount) / 3 + 1;
    }

    private void setObject(GameObject[] objects, int slot)
    {
        if (objects.Length > 0)
        {
            int _type = Random.Range(0, objects.Length);
            GameObject obstacle = Instantiate(objects[_type], Vector3.zero, Quaternion.identity);
            obstacle.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
            obstacle.transform.SetParent(gameObject.transform);
            obstacle.transform.localRotation = Quaternion.Euler(0, 0, slot * _slotAngle - 90f);
            obstacle.transform.position = PolarSystem.Position(slot * _slotAngle, planetRadius, transform.position);
        }
    }

    public BlockInfo AddBlock(float angle)
    {
        angle += _slotAngleHalf;
        while (angle >= 360f) angle -= 360f;
        int _pos = 1;
        while (angle >= _pos * _slotAngle) _pos++;
        _pos--;
        return addBlock(_pos);
    }

    private BlockInfo addBlock(int slot)
    {
        if (_buildingsHeight[slot] <= -1 || _buildingsHeight[slot] >= height)
            return new BlockInfo(false, Vector3.zero, Quaternion.identity);
        else
        {
            _buildingsHeight[slot]++;
            _blocksPlaced++;
            if (_buildingsHeight[slot] == height) _buildingsComplete++;
            return new BlockInfo(false, PolarSystem.Position(slot * _slotAngle, planetRadius + (_buildingsHeight[slot] - 1) * blockScale, transform.position), Quaternion.Euler(0, 0, slot * _slotAngle - 90f));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(planetRadius + height * blockScale, 0, 0));
        for (int i = 0; i <= height; ++i)
        {
            Gizmos.DrawLine(transform.position + new Vector3(planetRadius + i * blockScale, -blockScale / 2, 0), transform.position + new Vector3(planetRadius + i * blockScale, blockScale / 2, 0));
        }
        Gizmos.color = Color.gray;
        for (int i = height + 1; i <= 5; ++i)
        {
            Gizmos.DrawLine(transform.position + new Vector3(planetRadius + i * blockScale, -blockScale / 2, 0), transform.position + new Vector3(planetRadius + i * blockScale, blockScale / 2, 0));
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(planetRadius, 0, 0));
        if (planetAtmosphereRadius > 5 * blockScale + planetRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position + new Vector3(planetRadius + 5 * blockScale, 0, 0), transform.position + new Vector3(planetAtmosphereRadius, 0, 0));
        }
    }
}
