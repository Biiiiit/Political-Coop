using UnityEngine;
using UnityEngine.UI;

public class ImageMaskController : MonoBehaviour
{
    [Header("The Image component inside the mask")]
    public Image maskedImage;

    public void SetImage(Sprite newSprite)
    {
        if (maskedImage == null)
        {
            Debug.LogError("Masked image reference missing!");
            return;
        }

        maskedImage.sprite = newSprite;

        // Makes sure the sprite is visible
        maskedImage.enabled = newSprite != null;
        maskedImage.preserveAspect = true;
    }
}
