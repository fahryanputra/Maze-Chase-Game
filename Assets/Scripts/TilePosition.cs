using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class TilePosition : MonoBehaviour {
	
	public int[,] edges = new int[50, 50];
	public int[,] nodes = new int[50, 50];
//	public int[,] dist = new int[50, 50];

	// Use this for initialization
	void Start () {
		Tilemap tilemap = GetComponent<Tilemap>();

		BoundsInt bounds = tilemap.cellBounds;
		TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
		StreamWriter edgeswriter = new StreamWriter ("OutputEdges.txt", true);
		StreamWriter nodeswriter = new StreamWriter ("OutputNodes.txt", true);
		//StreamWriter distwriter = new StreamWriter ("OutputDistance.txt", true);

	//	int[] distnumx = new int[100]; 
	//	int[] distnumy = new int[100];
		int xcounter = 0;
		int ycounter = 0;
		int nodescounter = 0;

		for (int y = (bounds.size.y - 1); y >= 0; y--) {
			for (int x = (bounds.size.x - 1); x >= 0; x--) {
				TileBase tile = allTiles[x + y * bounds.size.x];
				if (tile != null) {
					if (tile.name == "bgPath"){
						edges [x, y] = 1;
						edgeswriter.Write (edges [x, y]);
						edgeswriter.Write (" ");
					} else {
						edges [x, y] = 0;
						edgeswriter.Write (edges [x, y]);
						edgeswriter.Write (" ");
					}
				} 
			}
			edgeswriter.WriteLine ();
		}

		for (int y = (bounds.size.y - 1); y >= 0; y--) {
			for (int x = (bounds.size.x - 1); x >= 0; x--) {
				if (edges [x, y] == 1) {
					if (edges [(x + 1), y] == 1) {
						xcounter++;
					}
					if (edges [(x - 1), y] == 1) {
						xcounter++;
					}
					if (edges [x, (y + 1)] == 1) {
						ycounter++;
					}
					if (edges [x, (y - 1)] == 1) {
						ycounter++;
					}
				}

				if (xcounter == 2 && ycounter == 0) {
					nodes [x, y] = 0;
					nodeswriter.Write (edges [x, y]);
					//nodeswriter.Write ("E");
					nodeswriter.Write ("\t");
				} else if (ycounter == 2 && xcounter == 0) {
					nodes [x, y] = 0;
					nodeswriter.Write (edges [x, y]);
					//nodeswriter.Write ("E");
					nodeswriter.Write ("\t");
				} else if (edges [x, y] == 1) {
					nodescounter++;
					nodes [x, y] = nodescounter;
					nodeswriter.Write (nodes [x, y]);
					nodeswriter.Write ("N");
					nodeswriter.Write ("\t");
				} else {
					nodes [x, y] = 0;
					nodeswriter.Write (nodes [x, y]);
					nodeswriter.Write ("\t");
				}

				xcounter = 0;
				ycounter = 0;
			}
			nodeswriter.WriteLine ();
		}

	/*
		for (int y = (bounds.size.y - 1); y >= 0; y--) {
			for (int x = (bounds.size.x - 1); x >= 0; x--) {
				if (edges [x, y] != nodes [x, y]) {
					dist [x, y] = 1;
					distwriter.Write (dist [x, y]);
					distwriter.Write (" ");
				} else {
					dist [x, y] = 0;
					distwriter.Write (dist [x, y]);
					distwriter.Write (" ");
				}
			}
			distwriter.WriteLine ();
		}
	*/
						
		edgeswriter.Close ();
		nodeswriter.Close ();
	//	distwriter.Close();
	}   
}
