using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Folder;
using Unity.Collections;

public class TileSwapper : MonoBehaviour
{
    public TileBase[] defaultEnvironmentTileset { get; private set; }
    public TileBase[] defaultHoleTileset { get; private set; }

    [Serializable]
    public class Tileset
    {
        [Folder] public string folder;
        [HideInInspector] public TileBase[] tileset;
    }
    public List<Tileset> environmentTilesets;
    public List<Tileset> holeTilesets;

    private void Awake()
    {
        foreach (var tileset in environmentTilesets)
        {
            tileset.tileset = tileset.folder.LoadFolder<TileBase>();
        }
        foreach (var tileset in holeTilesets)
        {
            tileset.tileset = tileset.folder.LoadFolder<TileBase>();
        }
        defaultEnvironmentTileset = environmentTilesets[0].tileset;
        defaultHoleTileset = holeTilesets[0].tileset;
    }

    public void SwapTiles(Tilemap tilemap, TileBase[] oldTileset, TileBase[] newTileset)
    {
        if (oldTileset == newTileset) return;
        for (int i = 0; i < oldTileset.Length; i++)
        {
            tilemap.SwapTile(oldTileset[i], newTileset[i]);
        }
    }
}