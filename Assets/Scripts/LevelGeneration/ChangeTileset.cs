using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChangeTileset : MonoBehaviour
{
    private Tilemap tilemap;
    private TileBase[] newEnvironmentTileset;
    private TileBase[] newHoleTileset;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        ValidateNewTilesets();
        GameManager.Instance.TileSwapper.SwapTiles(tilemap, GameManager.Instance.TileSwapper.defaultEnvironmentTileset, newEnvironmentTileset);
        GameManager.Instance.TileSwapper.SwapTiles(tilemap, GameManager.Instance.TileSwapper.defaultHoleTileset, newHoleTileset);
    }

    private void ValidateNewTilesets()
    {
        newEnvironmentTileset = GameManager.Instance.TileSwapper.environmentTilesets[LevelGenerator.Instance.floorsCleared].tileset;
        newHoleTileset = GameManager.Instance.TileSwapper.holeTilesets[LevelGenerator.Instance.floorsCleared].tileset;
    }

}