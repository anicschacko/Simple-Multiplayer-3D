using UnityEngine;

[CreateAssetMenu(menuName = "Game/Config")]
public sealed class GameConfig : ScriptableObject
{
    public Vector2 gridSize;

    public float pickUpItemLifeSpan = 5f;

    public float gameTimer = 180f;

    public Vector3 RandomVec3InsideGrid(float spawnHeight = 0)
    {
        float x = (gridSize.x / 2) - 1;
        float y = (gridSize.y / 2) - 1;

        return new Vector3(Random.Range(-x, x), spawnHeight, Random.Range(-y, y));

    }
}
