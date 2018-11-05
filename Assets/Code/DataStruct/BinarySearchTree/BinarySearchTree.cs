using System;
using System.Collections.Generic;
using UnityEngine;

namespace BinaryTree
{
    public class TreeNode<T> where T : IComparable<T>
    {
        public T mData;
        public TreeNode<T> mLeftNode;
        public TreeNode<T> mRightNode;      
    }

    public class BinarySearchTree<T> where T : IComparable<T>, IEquatable<T>
    {
        //存储二叉树有两种方式：
        //1.使用数组顺序存储，根节点为0，左节点下标为：2i+1,右节点小标为2i+2。缺点是：浪费储存空间，当一棵树只有右子节点时,这样只会使用pow(2,h)-1个节点，实际却占用h个节点空间。
        //2.使用链表储存，也就是我们这里使用的方式
        private TreeNode<T> mRoot;

        public void Add(T data)
        {
            Add(ref mRoot, data);
        }

        private void Add(ref TreeNode<T> node, T data)
        {
            if (node == null)
            {
                node = new TreeNode<T>();
                node.mData = data;
            }
            else
            {
                int result = data.CompareTo(node.mData);
                if (result < 0)
                {
                    Add(ref node.mLeftNode, data);

                }
                else
                {
                    Add(ref node.mRightNode, data);
                }
            }
        }

        public void Delete(T data)
        {
            Delete(mRoot, data);
        }

        private void Delete(TreeNode<T> node, T data)
        {
            if(node == null)
            {
                Debug.Log("删除数据不存在");
                return;
            }

            if(node.mData.Equals(data))
            {
                DeleteReal(ref node);
            }
            else
            {
                int result = data.CompareTo(node.mData);
                if(result < 0)
                {
                    Delete(node.mLeftNode, data);
                }
                else
                {
                    Delete(node.mRightNode, data);
                }
            }
        }

        private void DeleteReal(ref TreeNode<T> node)
        {
            if(node.mLeftNode == null && node.mRightNode == null)
            {
                node = null;
            }
            else if(node.mLeftNode != null && node.mRightNode != null)
            {
                //找到左子树下的最大节点来替换当前节点
                TreeNode<T> maxNode = node.mLeftNode;
                TreeNode<T> maxParentNode = node;
                while(maxNode.mRightNode != null)
                {
                    maxParentNode = maxNode;
                    maxNode = maxNode.mRightNode;
                }

                node.mData = maxNode.mData;
                //这里是细节
                if (node != maxParentNode)
                {
                    maxParentNode.mRightNode = maxNode.mLeftNode;
                }
                else
                {
                    maxParentNode.mLeftNode = maxNode.mLeftNode;
                }

                maxNode = null;
            }
            else if(node.mLeftNode != null && node.mRightNode == null)
            {
                TreeNode<T> newNode = node.mLeftNode;
                node.mData = newNode.mData;
                node.mLeftNode = newNode.mLeftNode;
                node.mRightNode = newNode.mRightNode;
                newNode = null;
            }
            else if (node.mLeftNode == null && node.mRightNode != null)
            {
                TreeNode<T> newNode = node.mRightNode;
                node.mData = newNode.mData;
                node.mLeftNode = newNode.mLeftNode;
                node.mRightNode = newNode.mRightNode;
                newNode = null;
            }
        }

        public void PreTraverse()
        {
            PreTraverse(mRoot);
        }

        private void PreTraverse(TreeNode<T> node)
        {
            if (node != null)
            {
                Debug.Log(node.mData.ToString());
                PreTraverse(node.mLeftNode);
                PreTraverse(node.mRightNode);
            }
        }

        public void InTraverse()
        {
            InTraverse(mRoot);
        }

        private void InTraverse(TreeNode<T> node)
        {
            if (node != null)
            {
                InTraverse(node.mLeftNode);
                Debug.Log(node.mData.ToString());
                InTraverse(node.mRightNode);
            }
        }

        public void PostTraverse()
        {
            PostTraverse(mRoot);
        }

        private void PostTraverse(TreeNode<T> node)
        {
            if (node != null)
            {
                PostTraverse(node.mLeftNode);
                PostTraverse(node.mRightNode);
                Debug.Log(node.mData.ToString());
            }
        }

        //层次遍历，借助队列辅助缓存，实现层次遍历
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

        public TreeNode<T> Search(T data)
        {
            return Search(mRoot, data);
        }

        private TreeNode<T> Search(TreeNode<T> node, T data)
        {
            if(node == null)
            {
                return null;
            }

            if(data.Equals(node.mData))
            {
                return node;
            }
            else
            {
                int result = data.CompareTo(node.mData);
                if (result < 0)
                {
                    return Search(node.mLeftNode, data);
                }
                else
                {
                    return Search(node.mRightNode, data);
                }
            }
        }
    }

}