﻿using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

using Unity.Linq;
using System.Linq;
using ReversiKit;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class BoardEventsHandler : MonoBehaviour 
{
    #region Config
    private bool IS_OPPONENT_PLAYER_AI = true;
    private const float TURN_ANIMATION_DELAY = 0.5f;
    #endregion

	#region MonoBehaviour override
	
	void Start() 
	{
        this._isGameOver = false;

        var board = new MatrixBoard();
        {
            this._boardModel        = board;
            this._mutableBoardModel = board;
        }
		this._turnCalculator    = new TurnCalculator();
        this._turnSelector      = TurnSelectorBuilder.CreateCornerAndGreedyTurnSelector();

		

        this._root = GameObject.Find("root"); 


		
        populateLabels();
        populateBallsList();
		populateCellsList ();
		populateCellsMatrix ();
        populateCellColours();
		getAvailableTurns();
	}
	
	
	void Update() 
	{
		this.updateTurnLabel();
        this.updateScoreLabels();
        this.updateBallColours();
		this.highlightAvailableTurns();

		bool isMouseUpEvent = Input.GetMouseButtonUp(0);
		if (isMouseUpEvent)
		{
			this.handleMouseUpEvent();
		}
	}
	#endregion

    #region Turns Logic
    private void getAvailableTurns()
    {
        var turns = this._turnCalculator.GetValidTurnsForBoard(this._boardModel);
        this._validTurns = turns;
    }

    private void makeTurn(IReversiTurn turn)
    {
        this.unhighlightAvailableTurns();
        this.drawChangesForTurn(turn);
        this._boardModel.ApplyTurn(turn);
        this.getAvailableTurns();
        this.highlightAvailableTurns();
    }

    #region Human Player Turn
	private void handleMouseUpEvent()
	{
		Vector3 mousePosition = Input.mousePosition;
		Ray ray = Camera.main.ScreenPointToRay(mousePosition);

		RaycastHit hitInfo;
		bool hit = Physics.Raycast(ray, out hitInfo);
		if (!hit)
		{
			return;
		}

		GameObject selectedCellOrBall = hitInfo.transform.gameObject;
		bool isCell = (CELL_TAG == selectedCellOrBall.tag);
		if (isCell)
		{
			this.handleTapOnCell(selectedCellOrBall);
		}
	}

	private void handleTapOnCell(GameObject cellCube)
	{
        if (this.IsTurnOfAI)
        {
            return;
        }


		
		string cellName = cellCube.name;
        ICellCoordinates selectedCellPosition = BoardCoordinatesConverter.CellNameToCoordinates(cellName);

        if (tryPassTurnOrGameOver())
        {
            return;
        }
            

        var turnValidator = new SearchInSetTurnValidator(this._validTurns);
        bool isSelectedTurnValid = turnValidator.IsValidPositionForTurnOnBoard(selectedCellPosition, this._boardModel);
        if (!isSelectedTurnValid)
        {
           
            return;
        }
            

        IReversiTurn turn = this._validTurns.Where(t =>
        {
            string turnPositionName = BoardCoordinatesConverter.CoordinatesToCellName(t.Position);
            return cellName.Equals(turnPositionName);
        }).First();
		this.makeTurn(turn);


        if (IS_OPPONENT_PLAYER_AI)
        {
            StartCoroutine("coroutineMakeTurnByAI");
        }
	}

    private bool tryPassTurnOrGameOver()
    {
        if (0 == this._boardModel.NumberOfFreeCells)
        {
            this._turnLabel.text = "Kaybettin";
            this._isGameOver = true;

            return true;
        }

        if (null == this._validTurns || 0 == this._validTurns.Count())
        {
           
            this._boardModel.PassTurn();


           
            this.getAvailableTurns();
            if (null == this._validTurns || 0 == this._validTurns.Count())
            {
              

                this._turnLabel.text = "Kaybettin";
                this._isGameOver = true;
            }

            return true;
        }

        return false;
    }

    #endregion Human Player Turn

    #region AI Player Turn
    private IEnumerator coroutineMakeTurnByAI()
    {
        while (this.IsTurnOfAI)
        {
            yield return new WaitForSeconds(TURN_ANIMATION_DELAY);
            this.makeTurnByAI();
        }
    }

    private void makeTurnByAI()
    {
        if (tryPassTurnOrGameOver())
        {
            return;
        }

        IReversiTurn selectedTurn = 
            this._turnSelector.SelectBestTurnOnBoard(
                this._validTurns, 
                this._boardModel);

        this.makeTurn(selectedTurn);
    }

    private bool IsTurnOfAI
    { 
        get
        {
            if (this._isGameOver)
            {
                return false;
            }

            if (!this.IS_OPPONENT_PLAYER_AI)
            {
                return false;
            }

            return !this._boardModel.IsTurnOfBlackPlayer;
        }
    }
    #endregion 
    #endregion Turns Logic

    #region Turn Drawing
    private void drawChangesForTurn(IReversiTurn turn)
    {
        Material activePlayerColour = 
            this._boardModel.IsTurnOfBlackPlayer ? 
            this._blackItemMaterial : 
            this._whiteItemMaterial ;




        this.createBallWithColourAtCell(activePlayerColour, turn.Position);
        foreach (ICellCoordinates flippedCell in turn.PositionsOfFlippedItems)
        {
            this.setColourForBallAtCell(activePlayerColour, flippedCell);
        }
    }

    private void createBallWithColourAtCell(Material activePlayerColour, ICellCoordinates cell)
    {
        var cellPosition = this._cellsMatrix[cell.Row, cell.Column].transform.position;

        var sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
        {
            sphere.transform.position = new Vector3(cellPosition.x + 0.1f, cellPosition.y, cellPosition.z - 1);
            sphere.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            sphere.tag = BALL_TAG;

            var renderer = sphere.GetComponent<Renderer>();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.material = this._blackItemMaterial;
        }

        this._ballsMatrix[cell.Row, cell.Column] = sphere;
    }

    private void setColourForBallAtCell(Material activePlayerColour, ICellCoordinates cell)
    {
        this._ballsMatrix[cell.Row, cell.Column].GetComponent<Renderer>().material = activePlayerColour;
    }

	private void updateTurnLabel()
	{
        if (this._isGameOver)
        {
            return;
        }

		this._turnLabel.text = 
			this._boardModel.IsTurnOfBlackPlayer ? 
			"Sıra Siyah Taşta" :
			"Sıra Beyaz Taşta" ;
	}

    private void updateBallColours()
    {
        for (int row = 0; row != BOARD_SIZE; ++row)
            for (int col = 0; col != BOARD_SIZE; ++col)
            {
                ICellCoordinates cell = new CellCoordinates(row, col);
                if (this._boardModel.IsCellTakenByBlack(cell))
                {
                    this.setColourForBallAtCell(this._blackItemMaterial, cell);
                } 
                else if (this._boardModel.IsCellTakenByWhite(cell))
                {
                    this.setColourForBallAtCell(this._whiteItemMaterial, cell);
                }
            }
    }

    private void updateScoreLabels()
    {
        int blackScore = this._boardModel.NumberOfBlackPieces;
        int whiteScore = this._boardModel.NumberOfWhitePieces;

        this._blackScoreLabel.text = "Siyah : " + Convert.ToString(blackScore);
        this._whiteScoreLabel.text = "Beyaz : " + Convert.ToString(whiteScore);
    }
    #endregion

	#region Turn Highlight
	private void unhighlightAvailableTurns()
	{
		this.highlightAvailableTurns(false);
	}

	private void highlightAvailableTurns()
	{
		this.highlightAvailableTurns(true);
	}

	private void highlightAvailableTurns(bool shouldHighlight)
	{
		var turns = this._validTurns;
        if (null == turns)
        {
         
            return;
        }

		foreach (IReversiTurn singleTurn in turns)
		{
			ICellCoordinates turnCell = singleTurn.Position;
			GameObject cellCube = this._cellsMatrix[turnCell.Row, turnCell.Column];

			Material cellColour = null;

			if (shouldHighlight)
			{
				cellColour = this._highlightedCellMaterial;
			} 
			else
			{
				cellColour = 
					turnCell.IsBlack 		? 
					this._blackCellMaterial : 
					this._whiteCellMaterial ;
				
			}
			cellCube.GetComponent<Renderer>().material = cellColour;
		}
	}
	#endregion	

	#region Cells Initialization
	private void populateCellsList()
	{
		this._cellsList = 
            this._root.Descendants()
                      .Where(x => x.tag == "FieldCell")
                      .OrderBy(x => x.name)
                      .ToArray();
	}

	private void populateCellsMatrix()
	{
		this._cellsMatrix = new GameObject[BOARD_SIZE,BOARD_SIZE];

		for (int row = 0; row != BOARD_SIZE; ++row)
			for (int column = 0; column != BOARD_SIZE; ++column) 
			{
				int flatIndex = row * BOARD_SIZE + column;
				var cell = this._cellsList[flatIndex];
				string cellName = cell.name;

				var cellPosition = BoardCoordinatesConverter.CellNameToCoordinates(cellName);
				this._cellsMatrix [cellPosition.Row, cellPosition.Column] = cell;
			}
	}

	private void populateBallsList()
	{
		this._ballsMatrix = new GameObject[8, 8];
		{
			var d4Pos = BoardCoordinatesConverter.CellNameToCoordinates("D4");
			this._ballsMatrix[d4Pos.Row, d4Pos.Column] = this._ballD4;
			this._mutableBoardModel.TryConsumeCellByBlackPlayer(d4Pos);

			var d5Pos = BoardCoordinatesConverter.CellNameToCoordinates("D5");
			this._ballsMatrix[d5Pos.Row, d5Pos.Column] = this._ballD5;
			this._mutableBoardModel.TryConsumeCellByWhitePlayer(d5Pos);

			var e4Pos = BoardCoordinatesConverter.CellNameToCoordinates("E4");
			this._ballsMatrix[e4Pos.Row, e4Pos.Column] = this._ballE4;
			this._mutableBoardModel.TryConsumeCellByWhitePlayer(e4Pos);

			var e5Pos = BoardCoordinatesConverter.CellNameToCoordinates("E5");
			this._ballsMatrix[e5Pos.Row, e5Pos.Column] = this._ballE5;
			this._mutableBoardModel.TryConsumeCellByBlackPlayer(e5Pos);

			this._mutableBoardModel.IsTurnOfBlackPlayer = true;
		}
	}
	
    private void populateCellColours()
    {
      

        if (null == this._blackItemMaterial)
        {
            this._blackItemMaterial = this._ballsMatrix[3, 3].GetComponent<Renderer>().material;
        }
        if (null == this._whiteItemMaterial)
        {
            this._whiteItemMaterial = this._ballsMatrix[3, 4].GetComponent<Renderer>().material;
        }

        if (null == this._blackCellMaterial)
        {
            this._blackCellMaterial = this._cellsMatrix[0, 0].GetComponent<Renderer>().material;
        }

        if (null == this._whiteCellMaterial)
        {
            this._whiteCellMaterial = this._cellsMatrix[0, 1].GetComponent<Renderer>().material;
        }


    }

    private void populateLabels()
    {
        var labels = FindObjectsOfType<Text>();


        this._turnLabel       = labels.Where(l => "TurnLabel"  == l.name).First();
        this._blackScoreLabel = labels.Where(l => "BlackScore" == l.name).First();
        this._whiteScoreLabel = labels.Where(l => "WhiteScore" == l.name).First();
    }
    #endregion

	#region GUI elements
	public Text _turnLabel; 
    public Text _blackScoreLabel;
    public Text _whiteScoreLabel;

	private GameObject _root;
	private GameObject[] _cellsList;
	private GameObject[,] _cellsMatrix;
	private GameObject[,] _ballsMatrix;

	public Material _whiteItemMaterial;
	public Material _blackItemMaterial;
	public Material _highlightedCellMaterial;
	public Material _blackCellMaterial;
	public Material _whiteCellMaterial;

	public GameObject _ballD5;
	public GameObject _ballE5;
	public GameObject _ballD4;
	public GameObject _ballE4;
	#endregion

	#region Model

	private IBoardState 	_boardModel	      ;
	private ITurnCalculator _turnCalculator	  ;
    private IBoardActions 	_mutableBoardModel;
    private ITurnSelector   _turnSelector     ;
    private IEnumerable<IReversiTurn> _validTurns   ;
    bool _isGameOver;

	#endregion

    #region Constants
	private const int    BOARD_SIZE = 			8;
	private const string CELL_TAG   = "FieldCell";
	private const string BALL_TAG   = 	   "Ball";
    #endregion

}
