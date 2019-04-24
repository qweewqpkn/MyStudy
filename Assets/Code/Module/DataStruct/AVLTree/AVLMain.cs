using AVL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVLMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ////右旋
        //Debug.Log("tree ##################");
        //AVLTree<int> tree = new AVLTree<int>();
        //tree.Insert(20);
        //tree.Insert(15);
        //tree.Insert(30);
        //tree.Insert(13);
        //tree.Insert(17);
        //tree.Insert(8);
        //tree.LevelTraverse();
        ////先左旋再右旋
        //Debug.Log("tree1 ##################");
        //AVLTree<int> tree1 = new AVLTree<int>();
        //tree1.Insert(20);
        //tree1.Insert(15);
        //tree1.Insert(30);
        //tree1.Insert(12);
        //tree1.Insert(18);
        //tree1.Insert(16);
        //tree1.Insert(7);
        //tree1.Insert(5);
        //tree1.LevelTraverse();

        //先右旋再左旋
        Debug.Log("tree2 ##################");
        AVLTree<int> tree2 = new AVLTree<int>();
        tree2.Insert(20);
        tree2.Insert(15);
        tree2.Insert(30);
        tree2.Insert(25);
        tree2.Insert(36);
        tree2.Insert(37);
        tree2.LevelTraverse();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
