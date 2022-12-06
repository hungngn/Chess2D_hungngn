using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Board
    [SerializeField] private GameObject whiteTile, blueTile;
    [SerializeField] private GameObject[,] tiles;

    //Chess prefabs
    [SerializeField] private GameObject[] _prefabs;

    //Logic
    private ChessPiece[,] chessPieces;
    private ChessPiece chosenPiece;
    private Camera currentCamera;
    private Vector2Int hover;
    void Start()
    {
        CreateBoard();
        SpawnAllPieces();
        PositionAllPieces();
    }

    void Update()
    {
        RaycastHit info;
        currentCamera = Camera.main;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
        {
            //Get tile index
            Vector2Int hitPosition = TileIndex(info.transform.gameObject);


            if(Input.GetMouseButtonDown(0))
            {
                // Debug.Log(hitPosition.x);
                // Debug.Log(hitPosition.y);
                if(chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    //what turn is it?
                    if(true)
                    {
                        chosenPiece = chessPieces[hitPosition.x, hitPosition.y];
                    }
                }
            }

            if(chosenPiece != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int prePosition = new Vector2Int(chosenPiece.currentX, chosenPiece.currentY);

                bool validMove = MoveTo(chosenPiece, hitPosition.x, hitPosition.y);
                if(!validMove)
                {
                    chosenPiece.transform.position = new Vector2(-3.5f + prePosition.x, -3.5f + prePosition.y);
                    chosenPiece = null;
                }
                else
                {
                    chosenPiece = null;
                }
            }

        }
    }

    // Board generator
    private void CreateBoard()
    {
        tiles = new GameObject[8,8];
        for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
                var isLightSquare = (x + y) % 2 != 0;
                if(isLightSquare)
                {
                    tiles[x, y] = SpawnTile(whiteTile, x, y);
                }
                else tiles[x, y] = SpawnTile(blueTile, x, y);
                // Debug.Log(x); Debug.Log(y);
            }
        }
    }

    private GameObject SpawnTile(GameObject tileType, int x, int y)
    {
        var pos = new Vector3(-3.5f + x, -3.5f + y, 0f);

        GameObject spawnTile = Instantiate(tileType) as GameObject;
        spawnTile.transform.position = pos;
        spawnTile.transform.parent = transform;
        spawnTile.transform.gameObject.layer = LayerMask.NameToLayer("Tile");
        spawnTile.name = $"x: {x}, y: {y}";
        spawnTile.AddComponent<BoxCollider>();
        return spawnTile;
    }

    
    //Spawn chesspiece
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[8,8];

        int whiteTeam = 1, blackTeam = 0;

        // White team
        chessPieces[0,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3,0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4,0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5,0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6,0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7,0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for(int i = 0; i < 8; i++)
            chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);

        //Black team
        chessPieces[0,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3,7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4,7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5,7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6,7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7,7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for(int i = 0; i < 8; i++)
            chessPieces[i,6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        
    }

    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(_prefabs[(int)type + 6 * team], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        
        return cp;
    }

    //Reposition the chesspiece
    private void PositionAllPieces()
    {
        for (int x = 0; x < 8; x++) {
			for (int y = 0; y < 8; y++) {
                if(chessPieces[x, y] != null)
                    PositionSinglePiece(x, y);
            }
        }
    }

    private void PositionSinglePiece(int x, int y)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].transform.position = new Vector2(-3.5f + x, -3.5f + y);
    }

    //Game operation
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        Vector2Int prePostion = new Vector2Int(cp.currentX, cp.currentY);

        //If there is another on the way
        if (chessPieces[x,y] != null)
        {
            ChessPiece ocp = chessPieces[x,y];
            //If it's an ally
            if (cp.team == ocp.team)
            {
                return false;
            }
            else //If it's an enemy
            {
                chessPieces[x, y].transform.position = new Vector2(6.5f, -3.5f + x);
                Destroy(chessPieces[x,y]);
            }


        }
        chessPieces[x, y] = cp;
        chessPieces[prePostion.x, prePostion.y] = null;

        PositionSinglePiece(x, y);
        return true;
    }

    private Vector2Int TileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x, y);
        return  -Vector2Int.one;
                
    }
}
