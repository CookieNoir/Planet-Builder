using System.Collections.Generic;
using UnityEngine;
using System;

public class Planet : MonoBehaviour
{
    public Transform planet;
    public Transform border;

    public enum NumberOfSlots { _9 = 9, _10 = 10, _12 = 12, _15 = 15, _18 = 18 };

    [Header("Planet Properties")]
    public NumberOfSlots numberOfSlots = NumberOfSlots._9;
    [Range(0.3f, 1f)] public float planetRadius = 0.6f;

    [Range(2f, 2.5f)] public float planetAtmosphereRadius = 1f;
    private const float MIN_AR = 1.8f;
    private const float MAX_AR = 2.3f;

    [Range(1, 5)] public int height = 1;
    [Range(1, 3)] public int difficulty = 1;

    [Header("Ship Properties")]
    [Range(25f, 150f)] public float shipSpeed = 60f;
    [Range(0f, 360f)] public float startAngle = 0f;
    public bool isClockwise = false;

    [Header("Generation")]
    public int buildingsToWin;
    public int[] obstaclesTypes;
    public GameObject[] obstacles;

    private List<GameObject> _blocks;

    private int _numberOfSlots;
    private int[] _buildingsHeight;
    private int _blocksPlaced;

    private int _buildingsToWin;
    private int _buildingsCompleted;
    private int _obstaclesAmount;

    private float _blockScale;
    private float _planetRadiusAddition;

    private float _slotAngle;
    private float _slotAngleHalf;

    private NumberOfSlots randomizeNumberOfSlots()
    {
        int[] _values = (int[])Enum.GetValues(typeof(NumberOfSlots));
        int _n = UnityEngine.Random.Range(0, _values.Length);
        return (NumberOfSlots)Enum.ToObject(NumberOfSlots._9.GetType(), _values[_n]);
    }

    private bool randomizeShipRotationDirection()
    {
        int _d = UnityEngine.Random.Range(0, 2);
        if (_d > 0) return true;
        else return false;
    }

    private void setValues(bool random = false)
    {
        if (random)
        {
            numberOfSlots = randomizeNumberOfSlots();
            height = UnityEngine.Random.Range(1, 6);
            difficulty = UnityEngine.Random.Range(1, 4);
            shipSpeed = UnityEngine.Random.Range(25f, 150f);
            startAngle = UnityEngine.Random.Range(0f, 360f);
            isClockwise = randomizeShipRotationDirection();
        }
        _numberOfSlots = (int)numberOfSlots;
        _slotAngle = 360f / _numberOfSlots;
        _slotAngleHalf = _slotAngle / 2;
        float _sqrt = Mathf.Sqrt(2 - 2 * Mathf.Cos(_slotAngle * Mathf.Deg2Rad));
        if (random)
        {
            float _sum = (1 + 5 * _sqrt);
            float _minRadius = MIN_AR / _sum;
            float _maxRadius = MAX_AR / _sum;
            planetRadius = UnityEngine.Random.Range(_minRadius, _maxRadius);
            _minRadius = planetRadius * _sum;
            planetAtmosphereRadius = UnityEngine.Random.Range(_minRadius + 0.2f, MAX_AR + 0.2f);
        }
        _blockScale = planetRadius * _sqrt;
        _planetRadiusAddition = planetRadius * (1 - Mathf.Cos(_slotAngleHalf * Mathf.Deg2Rad));
        planet.localScale = new Vector3(planetRadius + _planetRadiusAddition, planetRadius + _planetRadiusAddition, 1f);
        border.localScale = new Vector3(planetRadius + height * _blockScale, planetRadius + height * _blockScale, 1f);
    }

    public void Create(bool random = false)
    {
        setValues(random);
        _buildingsHeight = new int[_numberOfSlots];
        _blocks = new List<GameObject>();
        for (int i = 0; i < _numberOfSlots; ++i)
        {
            _buildingsHeight[i] = 0;
        }
        _blocksPlaced = 0;
        _buildingsCompleted = 0;

        setObstacles(random);
        setTarget(random);
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

    private void setObstacles(bool random = false)
    {
        if (random)
        {
            _obstaclesAmount = (_numberOfSlots - 1) / 4 * difficulty + ((_numberOfSlots - 1) % 4) / 2 * (difficulty - 1);
            for (int i = 0; i < _obstaclesAmount; ++i)
            {
                int _pos = UnityEngine.Random.Range(0, _numberOfSlots);
                while (_buildingsHeight[_pos] == -1) _pos = (_pos + 1) % _numberOfSlots;
                _buildingsHeight[_pos] = -1;
                setRandomObject(obstacles, _pos);
            }
        }
        else
        {
            for (int i = 0; i < _numberOfSlots; ++i)
            {
                if (obstaclesTypes[i] > -1)
                {
                    _buildingsHeight[i] = -1;
                    setFixedObject(obstacles, i, obstaclesTypes[i]);
                }
            }
        }
    }

    private void setTarget(bool random = false)
    {
        if (random)
        {
            if (height > 1)
            {
                _buildingsToWin = (_numberOfSlots - _obstaclesAmount) / 3 + 1;
            }
            else
            {
                _buildingsToWin = _numberOfSlots - _obstaclesAmount;
            }
        }
        else _buildingsToWin = buildingsToWin;
    }

    private void setRandomObject(GameObject[] objects, int slot)
    {
        if (objects.Length > 0)
        {
            int _type = UnityEngine.Random.Range(0, objects.Length);
            GameObject obstacle = Instantiate(objects[_type], Vector3.zero, Quaternion.identity);
            obstacle.transform.localScale = new Vector3(_blockScale, _blockScale, _blockScale);
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
            obstacle.transform.localScale = new Vector3(_blockScale, _blockScale, _blockScale);
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
            if (_buildingsHeight[slot] <= -1) return new BlockInfo(false, PolarSystem.Position(slot * _slotAngle, planetRadius, transform.position), Quaternion.Euler(0, 0, slot * _slotAngle - 90f), _blockScale);
            else return new BlockInfo(false, PolarSystem.Position(slot * _slotAngle, planetRadius + height * _blockScale, transform.position), Quaternion.Euler(0, 0, slot * _slotAngle - 90f), _blockScale);
        }
        else
        {
            _buildingsHeight[slot]++;
            _blocksPlaced++;
            if (_buildingsHeight[slot] == height) _buildingsCompleted++;
            return new BlockInfo(true, PolarSystem.Position(slot * _slotAngle, planetRadius + (_buildingsHeight[slot] - 1) * _blockScale, transform.position), Quaternion.Euler(0, 0, slot * _slotAngle - 90f), _blockScale);
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
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(planetRadius + height * _blockScale, 0, 0));
        for (int i = 1; i <= height; ++i)
        {
            Gizmos.DrawLine(transform.position + new Vector3(planetRadius + i * _blockScale, -_blockScale / 2, 0), transform.position + new Vector3(planetRadius + i * _blockScale, _blockScale / 2, 0));
        }
        Gizmos.color = Color.gray;
        for (int i = height + 1; i <= 5; ++i)
        {
            Gizmos.DrawLine(transform.position + new Vector3(planetRadius + i * _blockScale, -_blockScale / 2, 0), transform.position + new Vector3(planetRadius + i * _blockScale, _blockScale / 2, 0));
        }
        Gizmos.color = Color.cyan;
        if (planetAtmosphereRadius > 5 * _blockScale + planetRadius)
        {
            Gizmos.DrawLine(transform.position + new Vector3(planetRadius + 5 * _blockScale, 0, 0), transform.position + new Vector3(planetAtmosphereRadius, 0, 0));
        }
        if (startAngle > 1f && startAngle < 359f)
        {
            Gizmos.DrawLine(PolarSystem.Position(startAngle, planetRadius, transform.position), PolarSystem.Position(startAngle, planetAtmosphereRadius, transform.position));
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, planetRadius);
    }

    private void OnDrawGizmosSelected()
    {
        float _gizmoRadius = Mathf.Sqrt(planetRadius * planetRadius + (_blockScale * _blockScale) / 4);
        float _gizmoAngle = Mathf.Acos(planetRadius / _gizmoRadius) * 180f / Mathf.PI;
        for (int i = 0; i < _numberOfSlots; ++i)
        {
            if (obstaclesTypes[i] == -1)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawWireSphere(PolarSystem.Position(i * _slotAngle, planetRadius + 0.5f * _blockScale, transform.position), 0.5f * _blockScale);
            Gizmos.DrawLine(PolarSystem.Position(i * _slotAngle + _gizmoAngle, _gizmoRadius, transform.position), PolarSystem.Position(i * _slotAngle - _gizmoAngle, _gizmoRadius, transform.position));
        }
    }

    private void OnValidate()
    {
        setValues();
        if (obstaclesTypes.Length != _numberOfSlots)
        {
            obstaclesTypes = new int[_numberOfSlots];
            for (int i = 0; i < _numberOfSlots; ++i)
            {
                obstaclesTypes[i] = -1;
            }
        }

        if (startAngle == 360f) startAngle -= 360f;
    }
}