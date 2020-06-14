using UnityEngine;
using System.Collections;

public class ShipUIMover : UiMovement
{
    private Vector2 _defaultPosition;
    private Vector2 _newPosition;
    private Vector2 _newPosition2;
    private bool _moved = false;
    private IEnumerator _waiter;

    private void Start()
    {
        _defaultPosition = defaultPosition;
        _newPosition = newPosition;
        _newPosition2 = newPosition + bias;
        _waiter = waiter();
    }

    public override void Translate()
    {
        base.Translate();
        StopCoroutine(_waiter);
        _waiter = waiter();
        StartCoroutine(_waiter);
    }

    private IEnumerator waiter()
    {
        while (!done)
        {
            yield return null;
        }
        ChangeMoved();
        if (!_moved)
        {
            defaultPosition = _newPosition;
            newPosition = _newPosition2;
        }
        else
        {
            defaultPosition = _defaultPosition;
            newPosition = _newPosition;
            GetComponent<RectTransform>().anchoredPosition = _defaultPosition;
        }
        _moved = !_moved;
    }
}
