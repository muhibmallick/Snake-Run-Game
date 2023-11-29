using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BillboardScript : MonoBehaviour
{
    // this script is attached to the one texture that needs to face the camera all the time (Billboarding)
    [SerializeField]
    private float rotationSpeed;

    [Tooltip(
        "if there is any text component beneath this texture, it needs to shift its scale negative as it shift its scale again when game starts"
    )]
    [SerializeField]
    private TMP_Text requiredText;

    [Tooltip(
        "for moving the required text a bit towards z axis because when the game starts it hides behind the textContainer"
    )]
    [SerializeField]
    private float zFactor;

    private Camera mainCamera;
    private GameObject texture;

    private void Start()
    {
        // get the camera
        mainCamera = Camera.main;
        if (requiredText != null)
        {
            requiredText.rectTransform.localScale = new Vector3(
                -requiredText.rectTransform.localScale.x,
                requiredText.rectTransform.localScale.y,
                requiredText.rectTransform.localScale.z
            );
            // getTextScale = new Vector3(-getTextScale.x, getTextScale.y);

            // also move the position of the requiredText a bit towards z axis
            requiredText.rectTransform.localPosition = new Vector3(
                requiredText.rectTransform.localPosition.x,
                requiredText.rectTransform.localPosition.y,
                requiredText.rectTransform.localPosition.z + zFactor
            );
        }
    }

    private void LateUpdate()
    {
        FaceCamera();
    }

    private void FaceCamera()
    {
        texture = this.gameObject;
        Vector3 direction = mainCamera.transform.position - texture.transform.position;
        // var directionModified = new Vector3(0f, 0f, direction.z);
        Quaternion lookRoation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            lookRoation,
            rotationSpeed * Time.deltaTime
        );
    }
}
