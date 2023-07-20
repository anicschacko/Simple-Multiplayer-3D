using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    private GameConfig _gameConfig;
    
    private void Start()
    {
        _gameConfig = GameController.GameConfig;
        CreateLevelGrid();
    }
    
    private void CreateLevelGrid()
    {
        var scale = new Vector3(_gameConfig.gridSize.x/10, 1f, _gameConfig.gridSize.y/10);
        this.transform.localScale = scale;
    }
}
