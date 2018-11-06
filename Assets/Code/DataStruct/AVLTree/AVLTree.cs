using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVL
{
    public class TreeNode<T> where T : IComparable<T>, IEquatable<T>
    {
        public int mHeight;
        public T mData;
        public TreeNode<T> mLeftNode;
        public TreeNode<T> mRightNode;
    }

    public class AVLTree<T> where T : IComparable<T>, IEquatable<T>
    {
        private TreeNode<T>[] mPath = new TreeNode<T>[32];
        private TreeNode<T> mRoot;

        public void Insert(T data)
        {
            Array.Clear(mPath, 0, mPath.Length);
            if (mRoot == null)
            {
                mRoot = new TreeNode<T>();
                mRoot.mHeight = 0;
                mRoot.mData = data;
                return;
            }

            int index = 0;
            TreeNode<T> curNode = mRoot;
            TreeNode<T> curParentNode = null;
            while(true)
            {
                mPath[index] = curNode;
                curParentNode = curNode;
                if (data.CompareTo(curNode.mData) < 0)
                {
                    curNode = curNode.mLeftNode;
                    if (curNode == null)
                    {
                        curParentNode.mLeftNode = new TreeNode<T>();
                        curParentNode.mLeftNode.mHeight = 0;
                        curParentNode.mLeftNode.mData = data;
                        break;
                    }
                }
                else
                {
                    curNode = curNode.mRightNode;
                    if (curNode == null)
                    {
                        curParentNode.mRightNode = new TreeNode<T>();
                        curParentNode.mRightNode.mHeight = 0;
                        curParentNode.mRightNode.mData = data;
                        break;
                    }
                }
                index++;
            }

            while(index >= 0)
            {
                TreeNode<T> pathCurNode = mPath[index];
                pathCurNode.mHeight = GetHeight(pathCurNode);
                TreeNode<T> newNode = Balance(pathCurNode);
                if (newNode != null)
                {
                    if(index - 1 >= 0)
                    {
                        if (newNode.mData.CompareTo(mPath[index - 1].mData) < 0)
                        {
                            mPath[index - 1].mLeftNode = newNode;
                        }
                        else
                        {
                            mPath[index - 1].mRightNode = newNode;
                        }
                    }
                    else
                    {
                        mRoot = newNode;
                    }
                }
                index--;
            }
        }

        private TreeNode<T> Balance(TreeNode<T> curNode)
        {
            TreeNode<T> newNode = null;
            int bf = GetBF(curNode);
            if(bf == 2)
            {
                int leftBF = GetBF(curNode.mLeftNode);
                if(leftBF == 1)
                {
                    newNode = RightRotate(curNode);
                }
                else
                {
                    newNode = LeftRightRotate(curNode);
                }
            }
            else if(bf == -2)
            {
                int rightBF = GetBF(curNode.mRightNode);
                if (rightBF == 1)
                {
                    newNode = RightLeftRotate(curNode);
                }
                else
                {
                    newNode = LeftRotate(curNode);
                }
            }

            return newNode;
        }
        
        private TreeNode<T> LeftRotate(TreeNode<T> root)
        {
            TreeNode<T> newRoot = root.mRightNode;
            root.mRightNode = newRoot.mLeftNode;
            newRoot.mLeftNode = root;


            return newRoot;
        }

        private TreeNode<T> RightRotate(TreeNode<T> root)
        {
            TreeNode<T> newRoot = root.mLeftNode;
            root.mLeftNode = newRoot.mRightNode;
            newRoot.mRightNode = root;
            return newRoot;
        }

        private TreeNode<T> LeftRightRotate(TreeNode<T> root)
        {
            root.mLeftNode = LeftRotate(root.mLeftNode);
            return RightRotate(root);
        }

        private TreeNode<T> RightLeftRotate(TreeNode<T> root)
        {
            root.mRightNode = RightRotate(root.mRightNode);
            return LeftRotate(root);
        }

        private int GetBF(TreeNode<T> node)
        {
            int leftHeight = GetHeight(node.mLeftNode);
            int rightHeight = GetHeight(node.mRightNode);
            return leftHeight - rightHeight;
        }

        private int GetHeight(TreeNode<T> node)
        {
            if(node == null)
            {
                return -1;
            }
            else
            {
                int leftHeight = -1;
                if(node.mLeftNode != null)
                {
                    leftHeight = node.mLeftNode.mHeight;
                }

                int rightHeight = -1;
                if (node.mRightNode != null)
                {
                    rightHeight = node.mRightNode.mHeight;
                }

                return Mathf.Max(leftHeight, rightHeight) + 1;
            }
        }

        public void LevelTraverse()
        {
            Queue<TreeNode<T>> queue = new Queue<TreeNode<T>>();
            queue.Enqueue(mRoot);
            while(queue.Count > 0)
            {
                TreeNode<T> node = queue.Dequeue();
                if(node != null)
                {
                    Debug.Log(node.mData);
                    queue.Enqueue(node.mLeftNode);
                    queue.Enqueue(node.mRightNode);
                }
            }
        }

        public void Delete()
        {

        }
    }

}