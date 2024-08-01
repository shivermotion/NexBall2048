using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorTileController : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int xCount = 5;
    [SerializeField] private int yCount = 10;

    [SerializeField] private List<GameObject> activeTiles = new();
    
    // game board is 20x40
    // tile is 4x4
    private void Start()
    {
        Vector3 TilePos(int x, int y) => new Vector3((x * 2)+1, 0,(y * 2)+1);
        
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var newTile = Instantiate(tilePrefab, transform);
                newTile.transform.localPosition = TilePos(x, y);
                activeTiles.Add(newTile);
            }
        }
    }
}
