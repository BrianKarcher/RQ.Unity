using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Serialization;

namespace Pathfinding {
	public delegate void GraphNodeDelegate (GraphNode node);
	public delegate bool GraphNodeDelegateCancelable (GraphNode node);

	public abstract class GraphNode {
		/** Internal unique index */
		private int nodeIndex;

		/** Bitpacked field holding several pieces of data.
		 * \see Walkable
		 * \see Area
		 * \see GraphIndex
		 * \see Tag
		 */
		protected uint flags;

#if !ASTAR_NO_PENALTY
		/** Penalty cost for walking on this node.
		 * This can be used to make it harder/slower to walk over certain nodes.
		 *
		 * A penalty of 1000 (Int3.Precision) corresponds to the cost of walking one world unit.
		 */
		private uint penalty;
#endif

		/** Constructor for a graph node. */
		protected GraphNode (AstarPath astar) {
			if (!System.Object.ReferenceEquals(astar, null)) {
				this.nodeIndex = astar.GetNewNodeIndex();
				astar.InitializeNode(this);
			} else {
				throw new System.Exception("No active AstarPath object to bind to");
			}
		}

		/** Destroys the node.
		 * Cleans up any temporary pathfinding data used for this node.
		 * The graph is responsible for calling this method on nodes when they are destroyed, including when the whole graph is destoyed.
		 * Otherwise memory leaks might present themselves.
		 *
		 * Once called the #Destroyed property will return true and subsequent calls to this method will not do anything.
		 *
		 * \note Assumes the current active AstarPath instance is the same one that created this node.
		 *
		 * \warning Should only be called by graph classes on their own nodes
		 */
		public void Destroy () {
			//Already destroyed
			if (Destroyed) return;

			ClearConnections(true);

			if (AstarPath.active != null) {
				AstarPath.active.DestroyNode(this);
			}
			nodeIndex = -1;
		}

		public bool Destroyed {
			get {
				return nodeIndex == -1;
			}
		}

		/** Internal unique index.
		 * Every node will get a unique index.
		 * This index is not necessarily correlated with e.g the position of the node in the graph.
		 */
		public int NodeIndex { get { return nodeIndex; } }

		/** Position of the node in world space.
		 * \note The position is stored as an Int3, not a Vector3.
		 * You can convert an Int3 to a Vector3 using an explicit conversion.
		 * \code var v3 = (Vector3)node.position; \endcode
		 */
		public Int3 position;

		#region Constants
		/** Position of the walkable bit. \see Walkable */
		const int FlagsWalkableOffset = 0;
		/** Mask of the walkable bit. \see Walkable */
		const uint FlagsWalkableMask = 1 << FlagsWalkableOffset;

		/** Start of area bits. \see Area */
		const int FlagsAreaOffset = 1;
		/** Mask of area bits. \see Area */
		const uint FlagsAreaMask = (131072-1) << FlagsAreaOffset;

		/** Start of graph index bits. \see GraphIndex */
		const int FlagsGraphOffset = 24;
		/** Mask of graph index bits. \see GraphIndex */
		const uint FlagsGraphMask = (256u-1) << FlagsGraphOffset;

		public const uint MaxAreaIndex = FlagsAreaMask >> FlagsAreaOffset;
		/** Max number of graphs-1 */
		public const uint MaxGraphIndex = FlagsGraphMask >> FlagsGraphOffset;

		/** Start of tag bits. \see Tag */
		const int FlagsTagOffset = 19;
		/** Mask of tag bits. \see Tag */
		const uint FlagsTagMask = (32-1) << FlagsTagOffset;

		#endregion


		#region Properties

		/** Holds various bitpacked variables.
		 */
		public uint Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}

		/** Penalty cost for walking on this node. This can be used to make it harder/slower to walk over certain areas. */
		public uint Penalty {
#if !ASTAR_NO_PENALTY
			get {
				return penalty;
			}
			set {
				if (value > 0xFFFFFF)
					Debug.LogWarning("Very high penalty applied. Are you sure negative values haven't underflowed?\n" +
						"Penalty values this high could with long paths cause overflows and in some cases infinity loops because of that.\n" +
						"Penalty value applied: "+value);
				penalty = value;
			}
#else
			get { return 0U; }
			set {}
#endif
		}

		/** True if the node is traversable */
		public bool Walkable {
			get {
				return (flags & FlagsWalkableMask) != 0;
			}
			set {
				flags = flags & ~FlagsWalkableMask | (value ? 1U : 0U) << FlagsWalkableOffset;
			}
		}

		public uint Area {
			get {
				return (flags & FlagsAreaMask) >> FlagsAreaOffset;
			}
			set {
				flags = (flags & ~FlagsAreaMask) | (value << FlagsAreaOffset);
			}
		}

		public uint GraphIndex {
			get {
				return (flags & FlagsGraphMask) >> FlagsGraphOffset;
			}
			set {
				flags = flags & ~FlagsGraphMask | value << FlagsGraphOffset;
			}
		}

		public uint Tag {
			get {
				return (flags & FlagsTagMask) >> FlagsTagOffset;
			}
			set {
				flags = flags & ~FlagsTagMask | value << FlagsTagOffset;
			}
		}

		#endregion

		public virtual void UpdateRecursiveG (Path path, PathNode pathNode, PathHandler handler) {
			//Simple but slow default implementation
			pathNode.UpdateG(path);

			handler.PushNode(pathNode);

			GetConnections(delegate(GraphNode other) {
				PathNode otherPN = handler.GetPathNode(other);
				if (otherPN.parent == pathNode && otherPN.pathID == handler.PathID) other.UpdateRecursiveG(path, otherPN, handler);
			});
		}

		public virtual void FloodFill (Stack<GraphNode> stack, uint region) {
			//Simple but slow default implementation

			GetConnections(delegate(GraphNode other) {
				if (other.Area != region) {
					other.Area = region;
					stack.Push(other);
				}
			});
		}

		/** Calls the delegate with all connections from this node */
		public abstract void GetConnections (GraphNodeDelegate del);

		public abstract void AddConnection (GraphNode node, uint cost);
		public abstract void RemoveConnection (GraphNode node);

		/** Remove all connections from this node.
		 * \param alsoReverse if true, neighbours will be requested to remove connections to this node.
		 */
		public abstract void ClearConnections (bool alsoReverse);

		/** Checks if this node has a connection to the specified node */
		public virtual bool ContainsConnection (GraphNode node) {
			// Simple but slow default implementation
			bool contains = false;

			GetConnections(neighbour => {
				contains |= neighbour == node;
			});
			return contains;
		}

		/** Recalculates all connection costs from this node.
		 * Depending on the node type, this may or may not be supported.
		 * Nothing will be done if the operation is not supported
		 * \todo Use interface?
		 */
		public virtual void RecalculateConnectionCosts () {
		}

		/** Add a portal from this node to the specified node.
		 * This function should add a portal to the left and right lists which is connecting the two nodes (\a this and \a other).
		 *
		 * \param other The node which is on the other side of the portal (strictly speaking it does not actually have to be on the other side of the portal though).
		 * \param left List of portal points on the left side of the funnel
		 * \param right List of portal points on the right side of the funnel
		 * \param backwards If this is true, the call was made on a node with the \a other node as the node before this one in the path.
		 * In this case you may choose to do nothing since a similar call will be made to the \a other node with this node referenced as \a other (but then with backwards = true).
		 * You do not have to care about switching the left and right lists, that is done for you already.
		 *
		 * \returns True if the call was deemed successful. False if some unknown case was encountered and no portal could be added.
		 * If both calls to node1.GetPortal (node2,...) and node2.GetPortal (node1,...) return false, the funnel modifier will fall back to adding to the path
		 * the positions of the node.
		 *
		 * The default implementation simply returns false.
		 *
		 * This function may add more than one portal if necessary.
		 *
		 * \see http://digestingduck.blogspot.se/2010/03/simple-stupid-funnel-algorithm.html
		 */
		public virtual bool GetPortal (GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards) {
			return false;
		}

		/** Open the node */
		public abstract void Open (Path path, PathNode pathNode, PathHandler handler);



		public virtual void SerializeNode (GraphSerializationContext ctx) {
			//Write basic node data.
			ctx.writer.Write(Penalty);
			ctx.writer.Write(Flags);
		}

		public virtual void DeserializeNode (GraphSerializationContext ctx) {
			Penalty = ctx.reader.ReadUInt32();
			Flags = ctx.reader.ReadUInt32();

			// Set the correct graph index (which might have changed, e.g if loading additively)
			GraphIndex = ctx.graphIndex;
		}

		/** Used to serialize references to other nodes e.g connections.
		 * Use the GraphSerializationContext.GetNodeIdentifier and
		 * GraphSerializationContext.GetNodeFromIdentifier methods
		 * for serialization and deserialization respectively.
		 *
		 * Nodes must override this method and serialize their connections.
		 * Graph generators do not need to call this method, it will be called automatically on all
		 * nodes at the correct time by the serializer.
		 */
		public virtual void SerializeReferences (GraphSerializationContext ctx) {
		}

		/** Used to deserialize references to other nodes e.g connections.
		 * Use the GraphSerializationContext.GetNodeIdentifier and
		 * GraphSerializationContext.GetNodeFromIdentifier methods
		 * for serialization and deserialization respectively.
		 *
		 * Nodes must override this method and serialize their connections.
		 * Graph generators do not need to call this method, it will be called automatically on all
		 * nodes at the correct time by the serializer.
		 */
		public virtual void DeserializeReferences (GraphSerializationContext ctx) {
		}
	}

	public abstract class MeshNode : GraphNode {
		protected MeshNode (AstarPath astar) : base(astar) {
		}

		public GraphNode[] connections;
		public uint[] connectionCosts;

		public abstract Int3 GetVertex (int i);
		public abstract int GetVertexCount ();
		public abstract Vector3 ClosestPointOnNode (Vector3 p);
		public abstract Vector3 ClosestPointOnNodeXZ (Vector3 p);

		public override void ClearConnections (bool alsoReverse) {
			// Remove all connections to this node from our neighbours
			if (alsoReverse && connections != null) {
				for (int i = 0; i < connections.Length; i++) {
					connections[i].RemoveConnection(this);
				}
			}

			connections = null;
			connectionCosts = null;
		}

		public override void GetConnections (GraphNodeDelegate del) {
			if (connections == null) return;
			for (int i = 0; i < connections.Length; i++) del(connections[i]);
		}

		public override void FloodFill (Stack<GraphNode> stack, uint region) {
			//Faster, more specialized implementation to override the slow default implementation
			if (connections == null) return;

			// Iterate through all connections, set the area and push the neighbour to the stack
			// This is a simple DFS (https://en.wikipedia.org/wiki/Depth-first_search)
			for (int i = 0; i < connections.Length; i++) {
				GraphNode other = connections[i];
				if (other.Area != region) {
					other.Area = region;
					stack.Push(other);
				}
			}
		}

		public override bool ContainsConnection (GraphNode node) {
			for (int i = 0; i < connections.Length; i++) if (connections[i] == node) return true;
			return false;
		}

		public override void UpdateRecursiveG (Path path, PathNode pathNode, PathHandler handler) {
			pathNode.UpdateG(path);

			handler.PushNode(pathNode);

			for (int i = 0; i < connections.Length; i++) {
				GraphNode other = connections[i];
				PathNode otherPN = handler.GetPathNode(other);
				if (otherPN.parent == pathNode && otherPN.pathID == handler.PathID) {
					other.UpdateRecursiveG(path, otherPN, handler);
				}
			}
		}

		/** Add a connection from this node to the specified node.
		 * If the connection already exists, the cost will simply be updated and
		 * no extra connection added.
		 *
		 * \note Only adds a one-way connection. Consider calling the same function on the other node
		 * to get a two-way connection.
		 */
		public override void AddConnection (GraphNode node, uint cost) {
			if (node == null) throw new System.ArgumentNullException();

			// Check if we already have a connection to the node
			if (connections != null) {
				for (int i = 0; i < connections.Length; i++) {
					if (connections[i] == node) {
						// Just update the cost for the existing connection
						connectionCosts[i] = cost;
						return;
					}
				}
			}

			// Create new arrays which include the new connection
			int connLength = connections != null ? connections.Length : 0;

			var newconns = new GraphNode[connLength+1];
			var newconncosts = new uint[connLength+1];
			for (int i = 0; i < connLength; i++) {
				newconns[i] = connections[i];
				newconncosts[i] = connectionCosts[i];
			}

			newconns[connLength] = node;
			newconncosts[connLength] = cost;

			connections = newconns;
			connectionCosts = newconncosts;
		}

		/** Removes any connection from this node to the specified node.
		 * If no such connection exists, nothing will be done.
		 *
		 * \note This only removes the connection from this node to the other node.
		 * You may want to call the same function on the other node to remove its eventual connection
		 * to this node.
		 */
		public override void RemoveConnection (GraphNode node) {
			if (connections == null) return;

			// Iterate through all connections and check if there are any to the node
			for (int i = 0; i < connections.Length; i++) {
				if (connections[i] == node) {
					// Create new arrays which have the specified node removed
					int connLength = connections.Length;

					var newconns = new GraphNode[connLength-1];
					var newconncosts = new uint[connLength-1];
					for (int j = 0; j < i; j++) {
						newconns[j] = connections[j];
						newconncosts[j] = connectionCosts[j];
					}
					for (int j = i+1; j < connLength; j++) {
						newconns[j-1] = connections[j];
						newconncosts[j-1] = connectionCosts[j];
					}

					connections = newconns;
					connectionCosts = newconncosts;
					return;
				}
			}
		}

		/** Checks if \a p is inside the node in XZ space
		 *
		 * The default implementation uses XZ space and is in large part got from the website linked below
		 * \author http://unifycommunity.com/wiki/index.php?title=PolyContainsPoint (Eric5h5)
		 *
		 * The TriangleMeshNode overrides this and implements faster code for that case.
		 */
		public virtual bool ContainsPoint (Int3 p) {
			bool inside = false;

			int count = GetVertexCount();

			for (int i = 0, j = count-1; i < count; j = i++) {
				if (((GetVertex(i).z <= p.z && p.z < GetVertex(j).z) || (GetVertex(j).z <= p.z && p.z < GetVertex(i).z)) &&
					(p.x < (GetVertex(j).x - GetVertex(i).x) * (p.z - GetVertex(i).z) / (GetVertex(j).z - GetVertex(i).z) + GetVertex(i).x))
					inside = !inside;
			}
			return inside;
		}

		public override void SerializeReferences (GraphSerializationContext ctx) {
			if (connections == null) {
				ctx.writer.Write(-1);
			} else {
				ctx.writer.Write(connections.Length);
				for (int i = 0; i < connections.Length; i++) {
					ctx.SerializeNodeReference(connections[i]);
					ctx.writer.Write(connectionCosts[i]);
				}
			}
		}

		public override void DeserializeReferences (GraphSerializationContext ctx) {
			int count = ctx.reader.ReadInt32();

			if (count == -1) {
				connections = null;
				connectionCosts = null;
			} else {
				connections = new GraphNode[count];
				connectionCosts = new uint[count];

				for (int i = 0; i < count; i++) {
					connections[i] = ctx.DeserializeNodeReference();
					connectionCosts[i] = ctx.reader.ReadUInt32();
				}
			}
		}
	}
}
