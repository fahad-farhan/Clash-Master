using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SpinWheel : MonoBehaviour
{
    [Header(" Events ")]
    public static Action<Color> OnColorSelected;

    [Header(" Elements ")]
    [SerializeField] private Color[] colors;
    [SerializeField] private RectTransform colorWheel;
    private bool isTurning;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Turn();
    }

    public void Turn()
    {
        if (isTurning)
            return;

        isTurning = true;

        // Reset the wheel rotation
        colorWheel.localEulerAngles = Vector3.zero;

        // Calculate the angle
        int targetAngleIndex = UnityEngine.Random.Range(0, 6);
        float targetAngle = targetAngleIndex * 60;

        // Add some turns
        targetAngle += 360 * 5;

        // Rotate
        LeanTween.rotateAround(colorWheel.gameObject, Vector3.forward, targetAngle, 2f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() => FinishedTurning(targetAngleIndex));
    }

    private void FinishedTurning(int colorIndex)
    {
        Debug.Log("Color Index : " + colorIndex);
        OnColorSelected?.Invoke(colors[colorIndex]);
        isTurning = false;
    }
}
