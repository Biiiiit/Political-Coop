using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SFB; // Make sure you have the StandaloneFileBrowser plugin

public class ImageUploaderForImage : MonoBehaviour
{
    public Image targetImage; // Assign your UI Image here

    // This method can be assigned to the Button's OnClick in Inspector
    public void UploadImage()
    {
        var extensions = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg") };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Image", "", extensions, false);

        if (paths.Length > 0 && File.Exists(paths[0]))
        {
            byte[] bytes = File.ReadAllBytes(paths[0]);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);

            // Convert Texture2D to Sprite
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = sprite;
        }
    }
}
