using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLibrarySceneUI : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] private TopicTileUI waterTile;
    [SerializeField] private TopicTileUI co2Tile;
    [SerializeField] private TopicTileUI housingTile;

    [Header("Images")]
    [SerializeField] private Sprite waterImage;
    [SerializeField] private Sprite co2Image;
    [SerializeField] private Sprite housingImage;

    [Header("Navigation")]
    [SerializeField] private string nextSceneName = "CardLibraryUI";

    private void Start()
    {
        waterTile.Bind(
            "WATER_LEVEL",
            "WATER-LEVEL",
            "Rising flood challenge",
            waterImage,
            OnThemeSelected
        );

        co2Tile.Bind(
            "CO2_EMISSION",
            "CO2 EMISSION",
            "Growing carbon footprint",
            co2Image,
            OnThemeSelected
        );

        housingTile.Bind(
            "HOUSING",
            "HOUSING",
            "Housing affordability crisis",
            housingImage,
            OnThemeSelected
        );
    }

    private void OnThemeSelected(string themeId)
    {
        GameSession.SelectedThemeId = themeId;
        
        if (ScreenFlowController.Instance != null)
        {
            ScreenFlowController.Instance.NavigateToNextScreen();
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
