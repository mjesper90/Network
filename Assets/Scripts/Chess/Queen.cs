using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Pieces
{
    public override List<int[]> PossibleMoves();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual List<int[]> PossibleMoves()
    {
        List<int[]> moves = new List<int[]>();
        // check every tile here for possible moves, set this up so it can only move forward or attack diagonally
        return moves;
    }
}
