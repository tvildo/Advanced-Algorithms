﻿using System;
using System.Collections.Generic;

namespace Advanced.Algorithms.DataStructures
{
    internal enum RedBlackTreeNodeColor
    {
        Black,
        Red
    }

    //TODO implement IEnumerable & make sure duplicates are handled correctly if its not already
    //TODO support initial bulk loading 
    /// <summary>
    /// Red black tree node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class RedBlackTreeNode<T> : IBSTNode<T> where T : IComparable
    {
        internal T Value => Values[0];
        internal List<T> Values { get; set; }
        internal RedBlackTreeNode<T> Parent { get; set; }

        internal RedBlackTreeNode<T> Left { get; set; }
        internal RedBlackTreeNode<T> Right { get; set; }

        internal bool IsLeaf => Left == null && Right == null;
        internal RedBlackTreeNodeColor NodeColor { get; set; }

        internal RedBlackTreeNode<T> Sibling => this.Parent.Left == this ?
                                                this.Parent.Right : this.Parent.Left;

        internal bool IsLeftChild => this.Parent.Left == this;
        internal bool IsRightChild => this.Parent.Right == this;

        //exposed to do common tests for Binary Trees
        IBSTNode<T> IBSTNode<T>.Left => Left;
        IBSTNode<T> IBSTNode<T>.Right => Right;
        T IBSTNode<T>.Value => Value;

        internal RedBlackTreeNode(RedBlackTreeNode<T> parent, T value)
        {
            Parent = parent;
            Values = new List<T> { value };
            NodeColor = RedBlackTreeNodeColor.Red;
        }
    }

    /// <summary>
    /// Red black tree implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedBlackTree<T> where T : IComparable
    {
        internal RedBlackTreeNode<T> Root { get; private set; }
        public int Count { get; private set; }

        //O(log(n)) worst O(n) for unbalanced tree
        public int GetHeight()
        {
            return getHeight(Root);
        }


        //O(log(n)) worst O(n) for unbalanced tree
        private int getHeight(RedBlackTreeNode<T> node)
        {
            if (node == null)
            {
                return -1;
            }

            return Math.Max(getHeight(node.Left), getHeight(node.Right)) + 1;
        }
        //O(log(n)) always
        public bool HasItem(T value)
        {
            if (Root == null)
            {
                return false;
            }

            return find(Root, value) != null;
        }
        public T FindMax()
        {
            return findMax(Root).Value;
        }

        internal List<T> GetAllNodes()
        {
            var allNodes = new List<T>();

            GetAllNodes(allNodes, Root);

            return allNodes;
        }

        internal void GetAllNodes(List<T> allNodes, RedBlackTreeNode<T> currentNode)
        {
            while (true)
            {
                if (currentNode == null) return;

                allNodes.Add(currentNode.Value);

                GetAllNodes(allNodes, currentNode.Left);
                currentNode = currentNode.Right;
            }
        }

        internal void Clear()
        {
            Root = null;
            Count = 0;
        }

        private RedBlackTreeNode<T> findMax(RedBlackTreeNode<T> node)
        {
            while (true)
            {
                if (node.Right == null) return node;
                node = node.Right;
            }
        }

        public T FindMin()
        {
            return findMin(Root).Value;
        }

        private RedBlackTreeNode<T> findMin(RedBlackTreeNode<T> node)
        {
            while (true)
            {
                if (node.Left == null) return node;
                node = node.Left;
            }
        }

        //O(log(n)) worst O(n) for unbalanced tree
        internal RedBlackTreeNode<T> Find(T value)
        {
            return Root == null ? null : find(Root, value);
        }


        //find the node with the given identifier among descendants of parent and parent
        //uses pre-order traversal
        //O(log(n)) worst O(n) for unbalanced tree
        private RedBlackTreeNode<T> find(RedBlackTreeNode<T> parent, T value)
        {
            while (true)
            {
                if (parent == null)
                {
                    return null;
                }

                if (parent.Value.CompareTo(value) == 0)
                {
                    return parent;
                }

                var left = find(parent.Left, value);

                if (left != null)
                {
                    return left;
                }

                parent = parent.Right;
            }
        }

        private void rightRotate(RedBlackTreeNode<T> node)
        {
            var prevRoot = node;
            var leftRightChild = prevRoot.Left.Right;

            var newRoot = node.Left;

            //make left child as root
            prevRoot.Left.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Left;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Left;
                }
            }

            //move prev root as right child of current root
            newRoot.Right = prevRoot;
            prevRoot.Parent = newRoot;

            //move right child of left child of prev root to left child of right child of new root
            newRoot.Right.Left = leftRightChild;
            if (newRoot.Right.Left != null)
            {
                newRoot.Right.Left.Parent = newRoot.Right;
            }

            if (prevRoot == Root)
            {
                Root = newRoot;
            }
        }

        private void leftRotate(RedBlackTreeNode<T> node)
        {
            var prevRoot = node;
            var rightLeftChild = prevRoot.Right.Left;

            var newRoot = node.Right;

            //make right child as root
            prevRoot.Right.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Right;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Right;
                }
            }

            //move prev root as left child of current root
            newRoot.Left = prevRoot;
            prevRoot.Parent = newRoot;

            //move left child of right child of prev root to right child of left child of new root
            newRoot.Left.Right = rightLeftChild;
            if (newRoot.Left.Right != null)
            {
                newRoot.Left.Right.Parent = newRoot.Left;
            }

            if (prevRoot == Root)
            {
                Root = newRoot;
            }
        }


        //O(log(n)) always
        public void Insert(T value)
        {
            //empty tree
            if (Root == null)
            {
                Root = new RedBlackTreeNode<T>(null, value) { NodeColor = RedBlackTreeNodeColor.Black };
                Count++;
                return;
            }

            insert(Root, value);
            Count++;
        }

        //O(log(n)) always
        internal RedBlackTreeNode<T> InsertAndReturnNewNode(T value)
        {
            //empty tree
            if (Root == null)
            {
                Root = new RedBlackTreeNode<T>(null, value) { NodeColor = RedBlackTreeNodeColor.Black };
                Count++;
                return Root;
            }

            var newNode = insert(Root, value);
            Count++;
            return newNode;
        }

        //O(log(n)) always
        private RedBlackTreeNode<T> insert(RedBlackTreeNode<T> currentNode, T newNodeValue)
        {
            while (true)
            {
                var compareResult = currentNode.Value.CompareTo(newNodeValue);

                //current node is less than new item
                if (compareResult < 0)
                {
                    //no right child
                    if (currentNode.Right == null)
                    {
                        //insert
                        currentNode.Right = new RedBlackTreeNode<T>(currentNode, newNodeValue);
                        balanceInsertion(currentNode.Right);
                        return currentNode.Right;
                    }

                    currentNode = currentNode.Right;
                }
                //current node is greater than new node
                else if (compareResult > 0)
                {
                    if (currentNode.Left == null)
                    {
                        //insert
                        currentNode.Left = new RedBlackTreeNode<T>(currentNode, newNodeValue);
                        balanceInsertion(currentNode.Left);
                        return currentNode.Left;
                    }

                    currentNode = currentNode.Left;
                }
                else
                {
                    //duplicate
                    currentNode.Values.Add(newNodeValue);
                    return currentNode;
                }
            }
        }

        private void balanceInsertion(RedBlackTreeNode<T> nodeToBalance)
        {
            while (true)
            {
                if (nodeToBalance == Root)
                {
                    nodeToBalance.NodeColor = RedBlackTreeNodeColor.Black;
                    return;
                }

                //if node to balance is red
                if (nodeToBalance.NodeColor == RedBlackTreeNodeColor.Red)
                {
                    //red-red relation; fix it!
                    if (nodeToBalance.Parent.NodeColor == RedBlackTreeNodeColor.Red)
                    {
                        //red sibling
                        if (nodeToBalance.Parent.Sibling != null && nodeToBalance.Parent.Sibling.NodeColor == RedBlackTreeNodeColor.Red)
                        {
                            //mark both children of parent as black and move up balancing 
                            nodeToBalance.Parent.Sibling.NodeColor = RedBlackTreeNodeColor.Black;
                            nodeToBalance.Parent.NodeColor = RedBlackTreeNodeColor.Black;

                            //root is always black
                            if (nodeToBalance.Parent.Parent != Root)
                            {
                                nodeToBalance.Parent.Parent.NodeColor = RedBlackTreeNodeColor.Red;
                            }

                            nodeToBalance = nodeToBalance.Parent.Parent;
                        }
                        //absent sibling or black sibling
                        else if (nodeToBalance.Parent.Sibling == null || nodeToBalance.Parent.Sibling.NodeColor == RedBlackTreeNodeColor.Black)
                        {
                            if (nodeToBalance.IsLeftChild && nodeToBalance.Parent.IsLeftChild)
                            {
                                var newRoot = nodeToBalance.Parent;
                                swapColors(nodeToBalance.Parent, nodeToBalance.Parent.Parent);
                                rightRotate(nodeToBalance.Parent.Parent);

                                if (newRoot == Root)
                                {
                                    Root.NodeColor = RedBlackTreeNodeColor.Black;
                                }

                                nodeToBalance = newRoot;
                            }
                            else if (nodeToBalance.IsLeftChild && nodeToBalance.Parent.IsRightChild)
                            {
                                rightRotate(nodeToBalance.Parent);

                                var newRoot = nodeToBalance;

                                swapColors(nodeToBalance.Parent, nodeToBalance);
                                leftRotate(nodeToBalance.Parent);

                                if (newRoot == Root)
                                {
                                    Root.NodeColor = RedBlackTreeNodeColor.Black;
                                }

                                nodeToBalance = newRoot;
                            }
                            else if (nodeToBalance.IsRightChild && nodeToBalance.Parent.IsRightChild)
                            {
                                var newRoot = nodeToBalance.Parent;
                                swapColors(nodeToBalance.Parent, nodeToBalance.Parent.Parent);
                                leftRotate(nodeToBalance.Parent.Parent);

                                if (newRoot == Root)
                                {
                                    Root.NodeColor = RedBlackTreeNodeColor.Black;
                                }

                                nodeToBalance = newRoot;
                            }
                            else if (nodeToBalance.IsRightChild && nodeToBalance.Parent.IsLeftChild)
                            {
                                leftRotate(nodeToBalance.Parent);

                                var newRoot = nodeToBalance;

                                swapColors(nodeToBalance.Parent, nodeToBalance);
                                rightRotate(nodeToBalance.Parent);

                                if (newRoot == Root)
                                {
                                    Root.NodeColor = RedBlackTreeNodeColor.Black;
                                }

                                nodeToBalance = newRoot;
                            }
                        }
                    }
                }

                if (nodeToBalance.Parent != null)
                {
                    nodeToBalance = nodeToBalance.Parent;
                    continue;
                }

                break;
            }
        }

        private void swapColors(RedBlackTreeNode<T> node1, RedBlackTreeNode<T> node2)
        {
            var tmpColor = node2.NodeColor;
            node2.NodeColor = node1.NodeColor;
            node1.NodeColor = tmpColor;
        }

        //O(log(n)) always
        public void Delete(T value)
        {
            if (Root == null)
            {
                throw new Exception("Empty Tree");
            }

            delete(Root, value);
            Count--;
        }

        //O(log(n)) always
        private void delete(RedBlackTreeNode<T> node, T value)
        {
            RedBlackTreeNode<T> nodeToBalance = null;

            var compareResult = node.Value.CompareTo(value);

            //node is less than the search value so move right to find the deletion node
            if (compareResult < 0)
            {
                if (node.Right == null)
                {
                    throw new Exception("Item do not exist");
                }

                delete(node.Right, value);
            }
            //node is less than the search value so move left to find the deletion node
            else if (compareResult > 0)
            {
                if (node.Left == null)
                {
                    throw new Exception("Item do not exist");
                }

                delete(node.Left, value);
            }
            else
            {
                //duplicate - easy fix
                if (node.Values.Count > 1)
                {
                    node.Values.RemoveAt(node.Values.Count - 1);
                    return;
                }

                //node is a leaf node
                if (node.IsLeaf)
                {

                    //if color is red, we are good; no need to balance
                    if (node.NodeColor == RedBlackTreeNodeColor.Red)
                    {
                        deleteLeaf(node);
                        return;
                    }

                    nodeToBalance = handleDoubleBlack(node);
                    deleteLeaf(node);
                }
                else
                {
                    //case one - right tree is null (move sub tree up)
                    if (node.Left != null && node.Right == null)
                    {
                        nodeToBalance = handleDoubleBlack(node);
                        deleteLeftNode(node);

                    }
                    //case two - left tree is null  (move sub tree up)
                    else if (node.Right != null && node.Left == null)
                    {
                        nodeToBalance = handleDoubleBlack(node);
                        deleteRightNode(node);

                    }
                    //case three - two child trees 
                    //replace the node value with maximum element of left subtree (left max node)
                    //and then delete the left max node
                    else
                    {
                        var maxLeftNode = findMax(node.Left);

                        node.Values.Clear();
                        node.Values.Add(maxLeftNode.Value);

                        //delete left max node
                        delete(node.Left, maxLeftNode.Value);
                    }
                }
            }

            //handle six cases
            while (nodeToBalance != null)
            {
                nodeToBalance = handleDoubleBlack(nodeToBalance);
            }

        }

        private void deleteLeaf(RedBlackTreeNode<T> node)
        {
            //if node is root
            if (node.Parent == null)
            {
                Root = null;
            }
            //assign nodes parent.left/right to null
            else if (node.IsLeftChild)
            {
                node.Parent.Left = null;
            }
            else
            {
                node.Parent.Right = null;
            }
        }

        private void deleteRightNode(RedBlackTreeNode<T> node)
        {
            //root
            if (node.Parent == null)
            {
                Root.Right.Parent = null;
                Root = Root.Right;
                Root.NodeColor = RedBlackTreeNodeColor.Black;
                return;
            }

            //node is left child of parent
            if (node.IsLeftChild)
            {
                node.Parent.Left = node.Right;
            }
            //node is right child of parent
            else
            {
                node.Parent.Right = node.Right;
            }

            node.Right.Parent = node.Parent;

            if (node.Right.NodeColor != RedBlackTreeNodeColor.Red)
            {
                return;
            }

            //black deletion! But we can take its red child and recolor it to black
            //and we are done!
            node.Right.NodeColor = RedBlackTreeNodeColor.Black;

        }

        private void deleteLeftNode(RedBlackTreeNode<T> node)
        {
            //root
            if (node.Parent == null)
            {
                Root.Left.Parent = null;
                Root = Root.Left;
                Root.NodeColor = RedBlackTreeNodeColor.Black;
                return;
            }

            //node is left child of parent
            if (node.IsLeftChild)
            {
                node.Parent.Left = node.Left;
            }
            //node is right child of parent
            else
            {
                node.Parent.Right = node.Left;
            }

            node.Left.Parent = node.Parent;

            if (node.Left.NodeColor != RedBlackTreeNodeColor.Red)
            {
                return;
            }

            //black deletion! But we can take its red child and recolor it to black
            //and we are done!
            node.Left.NodeColor = RedBlackTreeNodeColor.Black;
        }

        private RedBlackTreeNode<T> handleDoubleBlack(RedBlackTreeNode<T> node)
        {
            //case 1
            if (node == Root)
            {
                node.NodeColor = RedBlackTreeNodeColor.Black;
                return null;
            }

            //case 2
            if (node.Parent != null
                 && node.Parent.NodeColor == RedBlackTreeNodeColor.Black
                 && node.Sibling != null
                 && node.Sibling.NodeColor == RedBlackTreeNodeColor.Red
                 && ((node.Sibling.Left == null && node.Sibling.Right == null)
                 || (node.Sibling.Left != null && node.Sibling.Right != null
                   && node.Sibling.Left.NodeColor == RedBlackTreeNodeColor.Black
                   && node.Sibling.Right.NodeColor == RedBlackTreeNodeColor.Black)))
            {
                node.Parent.NodeColor = RedBlackTreeNodeColor.Red;
                node.Sibling.NodeColor = RedBlackTreeNodeColor.Black;

                if (node.Sibling.IsRightChild)
                {
                    leftRotate(node.Parent);
                }
                else
                {
                    rightRotate(node.Parent);
                }

                return node;
            }
            //case 3
            if (node.Parent != null
             && node.Parent.NodeColor == RedBlackTreeNodeColor.Black
             && node.Sibling != null
             && node.Sibling.NodeColor == RedBlackTreeNodeColor.Black
             && (node.Sibling.Left == null && node.Sibling.Right == null
             || node.Sibling.Left != null && node.Sibling.Right != null
                                          && node.Sibling.Left.NodeColor == RedBlackTreeNodeColor.Black
                                          && node.Sibling.Right.NodeColor == RedBlackTreeNodeColor.Black))
            {
                //pushed up the double black problem up to parent
                //so now it needs to be fixed
                node.Sibling.NodeColor = RedBlackTreeNodeColor.Red;
                return node.Parent;
            }


            //case 4
            if (node.Parent != null
                 && node.Parent.NodeColor == RedBlackTreeNodeColor.Red
                 && node.Sibling != null
                 && node.Sibling.NodeColor == RedBlackTreeNodeColor.Black
                 && (node.Sibling.Left == null && node.Sibling.Right == null
                 || node.Sibling.Left != null && node.Sibling.Right != null
                                              && node.Sibling.Left.NodeColor == RedBlackTreeNodeColor.Black
                                              && node.Sibling.Right.NodeColor == RedBlackTreeNodeColor.Black))
            {
                //just swap the color of parent and sibling
                //which will compensate the loss of black count 
                node.Parent.NodeColor = RedBlackTreeNodeColor.Black;
                node.Sibling.NodeColor = RedBlackTreeNodeColor.Red;

                return null;
            }


            //case 5
            if (node.Parent != null
                && node.Parent.NodeColor == RedBlackTreeNodeColor.Black
                && node.Sibling != null
                && node.Sibling.IsRightChild
                && node.Sibling.NodeColor == RedBlackTreeNodeColor.Black
                && node.Sibling.Left != null
                && node.Sibling.Left.NodeColor == RedBlackTreeNodeColor.Red
                && node.Sibling.Right != null
                && node.Sibling.Right.NodeColor == RedBlackTreeNodeColor.Black)
            {
                node.Sibling.NodeColor = RedBlackTreeNodeColor.Red;
                node.Sibling.Left.NodeColor = RedBlackTreeNodeColor.Black;
                rightRotate(node.Sibling);

                return node;
            }

            //case 5 mirror
            if (node.Parent != null
               && node.Parent.NodeColor == RedBlackTreeNodeColor.Black
               && node.Sibling != null
               && node.Sibling.IsLeftChild
               && node.Sibling.NodeColor == RedBlackTreeNodeColor.Black
               && node.Sibling.Left != null
               && node.Sibling.Left.NodeColor == RedBlackTreeNodeColor.Black
               && node.Sibling.Right != null
               && node.Sibling.Right.NodeColor == RedBlackTreeNodeColor.Red)
            {
                node.Sibling.NodeColor = RedBlackTreeNodeColor.Red;
                node.Sibling.Right.NodeColor = RedBlackTreeNodeColor.Black;
                leftRotate(node.Sibling);

                return node;
            }

            //case 6
            if (node.Parent != null
                && node.Parent.NodeColor == RedBlackTreeNodeColor.Black
                && node.Sibling != null
                && node.Sibling.IsRightChild
                && node.Sibling.NodeColor == RedBlackTreeNodeColor.Black
                && node.Sibling.Right != null
                && node.Sibling.Right.NodeColor == RedBlackTreeNodeColor.Red)
            {
                //left rotate to increase the black count on left side by one
                //and mark the red right child of sibling to black 
                //to compensate the loss of Black on right side of parent
                node.Sibling.Right.NodeColor = RedBlackTreeNodeColor.Black;
                leftRotate(node.Parent);

                return null;
            }

            //case 6 mirror
            if (node.Parent != null
              && node.Parent.NodeColor == RedBlackTreeNodeColor.Black
              && node.Sibling != null
              && node.Sibling.IsLeftChild
              && node.Sibling.NodeColor == RedBlackTreeNodeColor.Black
              && node.Sibling.Left != null
              && node.Sibling.Left.NodeColor == RedBlackTreeNodeColor.Red)
            {
                //right rotate to increase the black count on right side by one
                //and mark the red left child of sibling to black
                //to compensate the loss of Black on right side of parent
                node.Sibling.Left.NodeColor = RedBlackTreeNodeColor.Black;
                rightRotate(node.Parent);

                return null;
            }
            return null;
        }
    }
}
