using BinaryTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySearchMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
        BinarySearchTree<int> tree = new BinarySearchTree<int>();
        tree.Add(15);
        tree.Add(3);
        tree.Add(2);
        tree.Add(6);
        tree.Add(1);
        tree.Add(150);
        tree.Add(21);
        tree.Add(32);
        tree.Add(13);
        tree.LevelTraverse();
        tree.Delete(3);
        tree.LevelTraverse();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
