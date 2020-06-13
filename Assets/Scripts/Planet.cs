using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public enum NumberOfSlots { _6 = 6, _8 = 8, _9 = 9, _10 = 10, _12 = 12, _15 = 15, _18 = 18 };
    [Header("Planet Properties")]
    public NumberOfSlots numberOfSlots = NumberOfSlots._6;
    [Range(1, 5)] public int height = 1;
    [Range(1, 3)] public int difficulty = 1;
    public float planetRadius = 1f;
    public float planetAtmosphereRadius = 1f;
    public float blockScale = 0.5f;

    [Header("Ship Properties")]
    [Range(15f, 180f)] public float shipSpeed = 60f;
    [Range(0f, 360f)] public float startAngle = 0f;
    public bool isClockwise = false;

    [Header("Generation")]
    public bool randomObjects;
    public int buildingsToWin;

    public int[] objectsTypes;
    public bool[] isObstacle;

    public GameObject[] props;
    public GameObject[] obstacles;

    private List<GameObject> _blocks;

    private int _numberOfSlots;
    private int[] _buildingsHeight;
    private int _blocksPlaced;

    private int _buildingsToWin;
    private int _buildingsCompleted;
    private int _obstaclesAmount;

    private float _slotAngle;
    private float _slotAngleHalf;

    public void Create(bool random = false)
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
        _buildingsCompleted = 0;

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
        _buildingsCompleted = 0;
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
        if (randomObjects)
        {
            _obstaclesAmount = (_numberOfSlots - 1) / 4 * difficulty + ((_numberOfSlots - 1) % 4) / 2 * (difficulty - 1);
            for (int i = 0; i < _obstaclesAmount; ++i)
            {
                int _pos = Random.Range(0, _numberOfSlots);
                while (_buildingsHeight[_pos] == -1) _pos = (_pos + 1) % _numberOfSlots;
                _buildingsHeight[_pos] = -1;
                setRandomObject(obstacles, _pos);
            }
        }
        else
        {
            for (int i = 0; i < _numberOfSlots; ++i)
            {
                if (objectsTypes[i] != -1 && isObstacle[i])
                {
                    _buildingsHeight[i] = -1;
                    setFixedObject(obstacles, i, objectsTypes[i]);
                }
            }
        }
    }

    private void setProps()
    {
        if (randomObjects)
        {
            int _propsAmount = _numberOfSlots - _obstaclesAmount - Random.Range(0, 3);
            for (int i = 0; i < _propsAmount; ++i)
            {
                int _pos = Random.Range(0, _numberOfSlots);
                while (_buildingsHeight[_pos] == -1) _pos = (_pos + 1) % _numberOfSlots;
                setRandomObject(props, _pos);
            }
        }
        else
        {
            for (int i = 0; i < _numberOfSlots; ++i)
            {
                if (objectsTypes[i] != -1 && !isObstacle[i]) setFixedObject(props, i, objectsTypes[i]);
            }
        }
    }

    private void setTarget()
    {
        if (randomObjects) _buildingsToWin = (_numberOfSlots - _obstaclesAmount) / 3 + 1;
        else _buildingsToWin = buildingsToWin;
    }

    private void setRandomObject(GameObject[] objects, int slot)
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

    private void setFixedObject(GameObject[] objects, int slot, int type)
    {
        if (objects.Length > 0)
        {
            GameObject obstacle = Instantiate(objects[type], Vector3.zero, Quaternion.identity);
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
        {
            if (_buildingsHeight[slot] <= -1) return new BlockInfo(false, PolarSystem.Position(slot * _slotAngle, planetRadius, transform.position), Quaternion.Euler(0, 0, slot * _slotAngle - 90f), blockScale);
            else return new BlockInfo(false, PolarSystem.Position(slot * _slotAngle, planetRadius + height * blockScale, transform.position), Quaternion.Euler(0, 0, slot * _slotAngle - 90f), blockScale);
        }
        else
        {
            _buildingsHeight[slot]++;
            _blocksPlaced++;
            if (_buildingsHeight[slot] == height) _buildingsCompleted++;
            return new BlockInfo(true, PolarSystem.Position(slot * _slotAngle, planetRadius + (_buildingsHeight[slot] - 1) * blockScale, transform.position), Quaternion.Euler(0, 0, slot * _slotAngle - 90f), blockScale);
        }
    }

    public void addBlockToBlocksList(GameObject block)
    {
        _blocks.Add(block);
    }

    public bool IsCompleted()
    {
        if (_buildingsCompleted >= _buildingsToWin)
        {
            return true;
        }
        else
        {
            return false;
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
        Gizmos.color = Color.cyan;
        if (planetAtmosphereRadius > 5 * blockScale + planetRadius)
        {
            Gizmos.DrawLine(transform.position + new Vector3(planetRadius + 5 * blockScale, 0, 0), transform.position + new Vector3(planetAtmosphereRadius, 0, 0));
        }
        if (startAngle > 1f && startAngle < 359f)
        {
            Gizmos.DrawLine(PolarSystem.Position(startAngle, planetRadius, transform.position), PolarSystem.Position(startAngle, planetAtmosphereRadius, transform.position));
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!randomObjects)
            for (int i = 0; i < _numberOfSlots; ++i)
            {
                if (objectsTypes[i] == -1) Gizmos.color = Color.green;
                else
                {
                    if (isObstacle[i]) Gizmos.color = Color.red;
                    else Gizmos.color = Color.yellow;
                }
                Gizmos.DrawWireSphere(PolarSystem.Position(i * _slotAngle, planetRadius + 0.5f * blockScale, transform.position), 0.5f * blockScale);
            }
        else
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _numberOfSlots; ++i)
            {
                Gizmos.DrawWireSphere(PolarSystem.Position(i * _slotAngle, planetRadius + 0.5f * blockScale, transform.position), 0.5f * blockScale);
            }
        }
    }

    private void OnValidate()
    {
        _numberOfSlots = (int)numberOfSlots;
        _slotAngle = 360f / _numberOfSlots;
        _slotAngleHalf = _slotAngle / 2;
        if (!randomObjects)
        {
            if (objectsTypes.Length != _numberOfSlots)
            {
                objectsTypes = new int[_numberOfSlots];
                for (int i = 0; i < _numberOfSlots; ++i)
                {
                    objectsTypes[i] = -1;
                }
            }
            if (isObstacle.Length != _numberOfSlots) isObstacle = new bool[_numberOfSlots];
        }
        else
        {
            objectsTypes = null;
            isObstacle = null;
        }
    }
}
