﻿/// <summary>
/// Yves Wang @ FISH, 2015, All rights reserved
/// Branch_ mono.
/// working in world co-ordinate
/// </summary>
using UnityEngine;
using System.Collections;
using ParticlePhysics2D;

[ExecuteInEditMode]
[AddComponentMenu("ParticlePhysics2D/Forms/BinaryTree",13)]
public class Branch_Mono : MonoBehaviour, IFormLayer {

	
	BinaryTree branch;
	[HideInInspector] [SerializeField] byte[] serializedBranch;
	public BinaryTree GetBinaryTree {get{return branch;}}
	
	[SerializeField] Simulation sim;
	public Simulation GetSimulation { get {return sim;}}

	///Binary Tree Generation
	[Range(1f,50f)]
	public float length = 20f;
	
	public bool debugSpring = true,debugAngles = false,debugParticleIndex = false,debugSpringIndex = false;
	public bool debugSpringConvID = false,debugAngleConvID = false;
	
	//branch generation params
	 public float lengthExitRatio;
	 public float angleOffsetMin,angleOffsetMax;
	 public float lengthMin1,lengthMax1,lengthMin2,lengthMax2;
	 public float lengthBranchAThreshold,lengthBranchBThreshold;
	 public int maxDepth;
	 public int leafCount;
	
	//some events
	public event System.Action OnResetForm;//called by editor to invoke form reset
	public event System.Action OnClearForm;
	
	void Awake() {
		//Debug.Log("Re construct branch from serialized bytes");
		if (serializedBranch!=null)
			branch = EasySerializer.DeserializeObjectFromBytes(serializedBranch) as BinaryTree;
	}


	
	void Start() {
		sim.Init();
		//Debug.Log("Start is called");
	}
	
	void LateUpdate(){
		sim.tick();
#if UNITY_EDITOR
		OnDrawGizmosUpdate();
#endif
	}
	
	//copy branch's topology to simulation
	void CopyBranchTopology(Particle2D p, BinaryTree b,ref Simulation s) {
		
		//if the branch has children
		if (b.branchA!=null || b.branchB!=null) {
			Particle2D temp;
			temp = (b.branchA==null) ? s.makeParticle(b.branchB.Position) : s.makeParticle(b.branchA.Position);
			temp.IsLeaf = false;
			s.makeSpring(p,temp);
			if (b.branchA!=null) CopyBranchTopology (temp,b.branchA,ref s);
			if (b.branchB!=null) CopyBranchTopology (temp,b.branchB,ref s);
			s.makeAngleConstraint(sim.getSpring(b.springIndex),sim.getSpring(b.branchA.springIndex));
			s.makeAngleConstraint(sim.getSpring(b.springIndex),sim.getSpring(b.branchB.springIndex));
			//it seems that even without angle-spring, the system is still stable after a small modification made to angle-rotation constraint
			//s.makeAngleConstraint(sim.getSpring(b.branchA.springIndex),sim.getSpring(b.branchB.springIndex),AngleConstraintTye.Spring,true,true);
		} 
		//if it's a leaf branch
		else {
			float xB = b.GetChildrenBranchPosX;
			float yB = b.GetChildrenBranchPosY;
			Particle2D temp = s.makeParticle(new Vector2(xB,yB));//temp is where the leaf is
			temp.IsLeaf = true;
			leafCount++;
			s.makeSpring(p,temp);
		}
	}
	
	//called by the editor script
	public void ResetForm(){
	
		branch = BinaryTree.GenerateBranch(length);
		//Debug.Log("Branches : " + BinaryTree.branchesCount);
		if (sim==null)
			sim = new Simulation ();
		sim.clear();
		leafCount = 0;
		Particle2D start = sim.makeParticle (branch.Position);
		start.makeFixed();
		CopyBranchTopology(start,branch,ref sim);
		sim.getParticle(1).makeFixed();
		sim.RecalculateConvergenceGroupID();
		sim.ShuffleSprings();
		sim.ShuffleAngles();
		//if (Application.isEditor) OnDrawGizmosUpdate();
		Debug.Log("Serialize branch to bytes");
		serializedBranch = EasySerializer.SerializeObjectToBytes(branch);
		
		if (OnResetForm!=null) OnResetForm();//invokde the event
		else {
			//Debug.Log("No one register OnResetForm");
		}
	}	
	
	public void ClearForm() {
		sim.clear();
		this.branch = null;
		this.leafCount = 0;
		if (OnClearForm!=null) OnClearForm();//invoke the clear form event
		else {
			//Debug.Log("No one register OnClearForm");
		}
	}
	
	public void OnDrawGizmosUpdate() {
		if (sim!=null) {
			if (debugSpring) sim.DebugSpring(transform.localToWorldMatrix,debugSpringConvID);
			if (debugAngles) sim.DebugAngles(transform.localToWorldMatrix,debugAngleConvID);
			
		}
		if (BinaryTree.debugBranch) {
			if (branch!=null) branch.DebugRender(transform.localToWorldMatrix);
			else Debug.Log("branch is null");
		}
	}
	
	//a gizmo that is handy to pick up
	 void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position,5f);
		
	}
	

}
