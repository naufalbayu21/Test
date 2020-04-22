using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// This Base class for all inputs (User and AI). 
/// Here are all the necessary fields for managing the game.
/// </summary>
public abstract class InputBaseClass: MonoBehaviour {
	public abstract InputController Controller { get; protected set; }	//Linc to InputController.
	public abstract Vector2 MoveAxis { get; protected set; }			//Vector2 for contolled horizontal move, used only X.
	public abstract Vector2 AimPos { get; protected set; }				//Vector2 takes the value in world coordinates, for aiming.
	public abstract bool Jump { get; protected set; }					//Jump
	public abstract bool Shot { get; protected set; }					//Shot
	public abstract bool ShotPressed { get; protected set; }			//ShotPressed
	public abstract bool Reload { get; protected set; }					//Reload
	public abstract bool NextGun { get; protected set; }				//NextGun
	public abstract bool Interaction { get; protected set; }			//Interaction
	public abstract bool EnabledControl { get; protected set; }			//Enabled control

	//Method called when selecting input
	public virtual void SelectInput (InputController controller) {
		Controller = controller;
		EnabledControl = true;
	}

	//Method called when deselecting input
	public virtual void DeselectInput () {
		EnabledControl = false;
	}

	//Update only selected input
	public virtual void UpdateInput () { }
}
