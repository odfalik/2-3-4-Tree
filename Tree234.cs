using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace proj3 {
    public class Tree234 {
        private Node234 Root { get; set; }

        public void Insert(int value) {
            if (Root != null) {
                InsertInternal(value, Root);
                ////Debug.WriteLine(string.Format("Post-global-insert tree: {0}", Preorder(Root)));
            } else {
                Root = new Node234(value);
                ////Debug.WriteLine(string.Format("Insert Initial with {0}", value));
            }
        }

        private void InsertInternal(int value, Node234 node) {
            ////Debug.WriteLine(string.Format("Insert Internal with {0}, {1}", value, node));

            if (node.Values.Count == 3) {
                node.Children = Split(node);
                node.Values.RemoveAt(2);
                node.Values.RemoveAt(0);

                ////Debug.WriteLine(string.Format("Post-split/demotion: {0}", Preorder(node)));
            }

            int childIndexToFollow = node.GetChildIndexToFollow(value);
            ////Debug.WriteLine(string.Format("childIndexToFollow = {0}", childIndexToFollow));

            if (childIndexToFollow == -1) { // node is leaf with space
                node.AddValue(value);
            } else {
                Node234 childToFollow = node.Children[childIndexToFollow];

                if (childToFollow.Values.Count == 3) { // Child to follow is full -> must split
                    node.AddValue(childToFollow.Values[1]); // Promote middle value

                    var newChildren = Split(childToFollow);
                    node.Children.Remove(childToFollow);
                    node.AddChildren(newChildren);
                    ////Debug.WriteLine(string.Format("Post-split: {0}", node));
                }

                childIndexToFollow = node.GetChildIndexToFollow(value);
                ////Debug.WriteLine(string.Format("childIndexToFollow = {0}, {1}", childIndexToFollow, node.Children[childIndexToFollow]));
                InsertInternal(value, node.Children[childIndexToFollow]);
            }

            ////Debug.WriteLine(string.Format("Post internal insert: {0}", node));
        }

        private List<Node234> Split(Node234 node) {
            Debug.Assert(node.GetDegree() == 4 || node.GetDegree() == 0); // Node should have either 4 or 0 children(leaf)

            var newChildren = new List<Node234>();

            Node234 leftSubTree = new Node234(node.Values[0]);
            newChildren.Add(leftSubTree);

            Node234 rightSubTree = new Node234(node.Values[2]);
            newChildren.Add(rightSubTree);

            if (node.GetDegree() != 0) { // must be 4-node
                leftSubTree.AddChild(node.Children[0]);
                leftSubTree.AddChild(node.Children[1]);
                rightSubTree.AddChild(node.Children[2]);
                rightSubTree.AddChild(node.Children[3]);
            }

            ////Debug.WriteLine(string.Format("Split newChildren: {0} & {1}", newChildren[0], newChildren[1]));

            return newChildren;
        }

        public void Delete(int target) {
            if (Root != null)
                DeleteInternal(target, Root);
            else
                throw new NullReferenceException();
        }

        private void DeleteInternal(int target, Node234 node) {
            //Debug.WriteLine(string.Format("DeleteInternal {0}, {1}", target, node));

            if (node.Values.Contains(target)) {
                //Debug.WriteLine(string.Format("Target {0} is in node {1}", target, node));
                if (node.IsLeaf()) {
                    Debug.Assert(node.Values.Count != 1);
                    node.Values.Remove(target);
                    //Debug.WriteLine(string.Format("Deleted at leaf {0}, {1} ", target, node));
                    return;
                } else {
                    int predecessorIndex = node.GetChildPredecessorIndex(target);
                    Node234 predecessorNode = node.Children[predecessorIndex];
                    int predecessorValue = predecessorNode.Values[predecessorNode.Values.Count - 1]; // ??????

                    int successorIndex = node.GetChildSuccessorIndex(target);
                    Node234 successorNode = node.Children[successorIndex];
                    int successorValue = successorNode.Values[0]; //TODOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOo

                    




                    if (predecessorNode.Values.Count > 1) { // can steal from predecessor?
                        //Debug.WriteLine(string.Format("stealing from predecessor: {0}", predecessorNode));
                        node.Values[node.Values.IndexOf(target)] = predecessorValue;
                        DeleteInternal(predecessorValue, predecessorNode);


                    } else if (successorNode.Values.Count > 1) { // steal from successor?
                        //Debug.WriteLine(string.Format("stealing from successor: {0}", successorNode));
                        node.Values[node.Values.IndexOf(target)] = successorValue;
                        DeleteInternal(successorValue, successorNode);


                    } else { // cannot steal, demote/merge                  // TODO MAY BE WRONG, SEE DIGIPEN CASE 2.3
                        //Debug.WriteLine(string.Format("cannot steal, demote/merge {0} ", node));

                        // Build mergedNode, add values
                        Node234 mergedNode = new Node234(target);
                        mergedNode.AddValue(predecessorValue);
                        mergedNode.AddValue(successorValue);

                        // wire children
                        mergedNode.AddChildren(predecessorNode.Children);
                        mergedNode.AddChildren(successorNode.Children);

                        // clean up
                        node.Values.Remove(target);
                        node.Children.Remove(predecessorNode);
                        node.Children.Remove(successorNode);

                        node.AddChild(mergedNode);

                        DeleteInternal(target, mergedNode); // Recurse to delete target
                    }
                }
            } else { // still looking for node with value
                //Debug.WriteLine(string.Format("Node does not contain value"));

                int childIndexToFollow = node.GetChildIndexToFollow(target);
                Node234 childToFollow = node.Children[childIndexToFollow];

                Node234 sibling;
                int siblingValueIndex;

                if (childToFollow.Values.Count == 1) {
                    if (childIndexToFollow > 0 && node.Children[childIndexToFollow - 1].Values.Count >= 2) { // Left immediate sibling exists and has at least 2 keys
                        // Clockwise rotation
                        //Debug.WriteLine("Clockwise rotation");

                        int pivotValueIndex = childIndexToFollow - 1;
                        childToFollow.AddValue(node.Values[pivotValueIndex]); // Rot value to child

                        sibling = node.Children[childIndexToFollow - 1];
                        siblingValueIndex = sibling.Values.Count - 1; // Index of left sibling's rightmost value

                        node.Values[pivotValueIndex] = sibling.Values[siblingValueIndex]; // Rot sibling value to node
                        sibling.Values.RemoveAt(siblingValueIndex);                       //
                        if (sibling.GetDegree() != 0) {
                            childToFollow.AddChild(sibling.Children[siblingValueIndex + 1]);    // Adopt child
                            sibling.Children.RemoveAt(siblingValueIndex + 1);                   //
                        }

                    } else if (childIndexToFollow < node.GetDegree() - 1 && node.Children[childIndexToFollow + 1].Values.Count >= 2) { // Right immediate sibling exists and has at least 2 keys
                        // Counter-clockwise rotation
                        //Debug.WriteLine("Counter-clockwise rotation");

                        int pivotValueIndex = childIndexToFollow;
                        childToFollow.AddValue(node.Values[pivotValueIndex]); // Rot value to child

                        sibling = node.Children[childIndexToFollow + 1];
                        siblingValueIndex = 0; // Index of right sibling's leftmost value

                        node.Values[pivotValueIndex] = sibling.Values[siblingValueIndex]; // Rot sibling value to node
                        sibling.Values.RemoveAt(siblingValueIndex);                       //
                        if (sibling.GetDegree() != 0) {
                            childToFollow.AddChild(sibling.Children[siblingValueIndex]);    // Adopt child
                            sibling.Children.RemoveAt(siblingValueIndex);                   //
                        }

                    } else { // merge case
                        //Debug.WriteLine("Merge case");
                        
                        if (node == Root && node.GetDegree() == 2) {
                            PromoteChildren(node);
                            //Debug.WriteLine("     promote children");
                        } else {

                            if (childIndexToFollow > 0 && node.Children[childIndexToFollow - 1].Values.Count == 1) {

                                //Debug.WriteLine("     demote value and merge with left sibling");
                                DemoteValue(node, childIndexToFollow - 1, childIndexToFollow, childIndexToFollow - 1);  // demote value and merge with left sibling

                            } else if (childIndexToFollow < node.Children.Count - 1 && node.Children[childIndexToFollow + 1].Values.Count == 1) {

                                //Debug.WriteLine("     demote value and merge with right sibling");
                                DemoteValue(node, childIndexToFollow, childIndexToFollow, childIndexToFollow + 1);  // demote value and merge with right sibling

                            } else {
                                throw new Exception("Oh shit");
                            }
                        }
                    }
                }

                childIndexToFollow = node.GetChildIndexToFollow(target);
                DeleteInternal(target, node.Children[childIndexToFollow]);
            }
            //Debug.WriteLine(String.Format("Deleted {0}. => {1}", target, this));
        }

        public void PromoteChildren(Node234 node) {
            Debug.Assert(node.Values.Count == 1);
            Debug.Assert(node.Children[0].Values.Count == 1);
            Debug.Assert(node.Children[1].Values.Count == 1);

            foreach (Node234 child in node.Children)
                node.AddValue(child.Values[0]); // grab children's only value

            List<Node234> children = node.Children;
            node.Children = null;                 // Abandon children nodes
            foreach (Node234 child in children)
                node.AddChildren(child.Children); // Adopt grandchildren
        }

        public void DemoteValue(Node234 node, int parentValueIndex, int childToFollowIndex, int siblingIndex) {

            Node234 childA = node.Children[childToFollowIndex];
            Node234 childB = node.Children[siblingIndex];
            Debug.Assert(childA.Values.Count == 1);
            Debug.Assert(childB.Values.Count == 1);

            Node234 mergedNode = new Node234(node.Values[parentValueIndex]);
            mergedNode.AddValue(childA.Values[0]);
            mergedNode.AddValue(childB.Values[0]);

            node.Values.RemoveAt(parentValueIndex);

            mergedNode.AddChildren(childA.Children);
            mergedNode.AddChildren(childB.Children);

            node.Children.Remove(childA);
            node.Children.Remove(childB);
            node.AddChild(mergedNode);
        }

        public string Preorder(Node234 node) {
            string output = "";

            if (node.Children.Count > 0)
                output += Preorder(node.Children[0]) + " ";
            if (node.Values.Count > 0)
                output += node.Values[0].ToString() + " ";

            if (node.Children.Count > 1)
                output += Preorder(node.Children[1]) + " ";
            if (node.Values.Count > 1)
                output += node.Values[1].ToString() + " ";

            if (node.Children.Count > 2)
                output += Preorder(node.Children[2]) + " ";
            if (node.Values.Count > 2)
                output += node.Values[2].ToString() + " ";

            if (node.Children.Count > 3)
                output += Preorder(node.Children[3]) + " ";

            return "[ " + output + "]";
        }

        public override string ToString() {

            StringBuilder sb = new StringBuilder("");

            for (int d = 1; d <= Root.GetHeight(); d++) {
                AppendLevelString(Root, d, sb);
                sb.Append("\n\n");
            }

            return sb.ToString();
        }

        public void AppendLevelString(Node234 node, int level, StringBuilder sb) {
            if (level == 1) {
                sb.Append(node.ToPaddedString());
            } else if (level > 1) {
                foreach (Node234 child in node.Children)
                    AppendLevelString(child, level - 1, sb);
            }
        }
    }
}